using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using log4net.Appender;
using log4net.Core;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{
    public class ElasticSearchAppender : BufferingAppenderSkeleton
    {
        static readonly string AppenderType = typeof (ElasticSearchAppender).Name;

        const int DefaultOnCloseTimeout = 30000;
        readonly ManualResetEvent workQueueEmptyEvent;

        int queuedCallbackCount;
        IRepository repository;

        public ElasticSearchAppender()
        {
            workQueueEmptyEvent = new ManualResetEvent(true);
            OnCloseTimeout = DefaultOnCloseTimeout;
        }

        public string ConnectionString { get; set; }
        public int OnCloseTimeout { get; set; }

        public override void ActivateOptions()
        {
            base.ActivateOptions();

            ServicePointManager.Expect100Continue = false;

            try
            {
                Validate(ConnectionString);
            }
            catch (Exception ex)
            {
                HandleError("Failed to validate ConnectionString in ActivateOptions", ex);
                return;
            }

            // Artificially add the buffer size to the connection string so it can be parsed
            // later to decide if we should send a _bulk API call
            ConnectionString += string.Format(";BufferSize={0}", BufferSize);
            repository = CreateRepository(ConnectionString);            
        }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            BeginAsyncSend();
            if (TryAsyncSend(events)) return;
            EndAsyncSend();
            HandleError("Failed to async send logging events in SendBuffer");
        }

        protected override void OnClose()
        {
            base.OnClose();

            if (TryWaitAsyncSendFinish()) return;
            HandleError("Failed to send all queued events in OnClose");
        }

        protected virtual IRepository CreateRepository(string connectionString)
        {
            return Repository.Create(connectionString);
        }

        protected virtual bool TryAsyncSend(IEnumerable<LoggingEvent> events)
        {
            return ThreadPool.QueueUserWorkItem(SendBufferCallback, logEvent.CreateMany(events));
        }

        protected virtual bool TryWaitAsyncSendFinish()
        {
            return workQueueEmptyEvent.WaitOne(OnCloseTimeout, false);
        }

        private void BeginAsyncSend()
        {
            workQueueEmptyEvent.Reset();
            Interlocked.Increment(ref queuedCallbackCount);
        }

        private void SendBufferCallback(object state)
        {
            try
            {
                repository.Add((IEnumerable<logEvent>) state, BufferSize);
            }
            catch (Exception ex)
            {
                HandleError("Failed to add logEvents to {0} in SendBufferCallback".With(repository.GetType().Name), ex);
            }
            finally
            {
                EndAsyncSend();
            }
        }

        private void EndAsyncSend()
        {
            if (Interlocked.Decrement(ref queuedCallbackCount) > 0)
                return;
            workQueueEmptyEvent.Set();
        }

        void HandleError(string message)
        {
            ErrorHandler.Error("{0} [{1}]: {2}.".With(AppenderType, Name, message));
        }

        void HandleError(string message, Exception ex)
        {
            ErrorHandler.Error("{0} [{1}]: {2}.".With(AppenderType, Name, message), ex, ErrorCode.GenericFailure);
        }

        static void Validate(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            if (connectionString.Length == 0)
            {
                throw new ArgumentException("connectionString is empty", "connectionString");
            }
        }
    }    
}

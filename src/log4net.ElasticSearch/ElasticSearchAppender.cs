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
        const int DefaultOnCloseTimeout = 30000;
        static readonly string AppenderType = typeof (ElasticSearchAppender).Name;
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
                HandleError("ActivateOptions", "Failed to validate ConnectionString", ex);
                return;
            }

            repository = CreateRepository(ConnectionString);
        }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            BeginAsyncSend();
            if (TryAsyncSend(events)) return;
            EndAsyncSend();
            HandleError("SendBuffer", "Failed to async send logging events");
        }

        protected override void OnClose()
        {
            base.OnClose();

            if (TryWaitAsyncSendFinish()) return;
            HandleError("OnClose", "Failed to send all queued events");
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

        void BeginAsyncSend()
        {
            workQueueEmptyEvent.Reset();
            Interlocked.Increment(ref queuedCallbackCount);
        }

        void SendBufferCallback(object state)
        {
            try
            {
                repository.Add((IEnumerable<logEvent>) state);
            }
            catch (Exception ex)
            {
                HandleError("SendBufferCallback", "Failed to addd logEvents to {0}".With(repository.GetType().Name), ex);
            }
            finally
            {
                EndAsyncSend();
            }
        }

        void EndAsyncSend()
        {
            if (Interlocked.Decrement(ref queuedCallbackCount) > 0)
                return;
            workQueueEmptyEvent.Set();
        }

        void HandleError(string method, string message)
        {
            ErrorHandler.Error("{0}.{1} [{2}]: {3}.".With(AppenderType, method, Name, message));
        }

        void HandleError(string method, string message, Exception ex)
        {
            ErrorHandler.Error("{0}.{1} [{2}]: {3}.".With(AppenderType, method, Name, message), ex, ErrorCode.GenericFailure);
        }

        static void Validate(string connectionString)
        {
            if (connectionString.IsNull())
            {
                throw new ArgumentNullException("connectionString");
            }

            if (connectionString.IsEmpty())
            {
                throw new ArgumentException("connectionString is empty", "connectionString");
            }
        }
    }
}
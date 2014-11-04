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
        readonly ManualResetEvent workQueueEmptyEvent;

        int queuedCallbackCount;
        IRepository repository;

        public ElasticSearchAppender()
        {
            workQueueEmptyEvent = new ManualResetEvent(true);
        }

        public string ConnectionString { get; set; }

        public override void ActivateOptions()
        {
            ServicePointManager.Expect100Continue = false;

            try
            {
                Validate(ConnectionString);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Valid ConnectionString must be provided", ex, ErrorCode.GenericFailure);
                return;
            }

            repository = Repository.Create(ConnectionString);
        }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            BeginAsyncSend();
            if (ThreadPool.QueueUserWorkItem(SendBufferCallback, events))
                return;
            EndAsyncSend();
            ErrorHandler.Error("ElasticSearchAppender [{0}] failed to ThreadPool.QueueUserWorkItem logging events in SendBuffer.".With(Name));
        }

        protected override void OnClose()
        {
            if (workQueueEmptyEvent.WaitOne(30000, false))
                return;
            ErrorHandler.Error("ElasticSearchAppender [{0}] failed to send all queued events before close, in OnClose.".With(Name));
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
                repository.Add(LogEvent.CreateMany((IEnumerable<LoggingEvent>) state));
            }
            catch (Exception ex)
            {
                ErrorHandler.Error("Failed in SendBufferCallback", ex);
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

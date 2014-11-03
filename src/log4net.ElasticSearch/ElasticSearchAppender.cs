using System;
using System.Diagnostics;
using System.Net;
using log4net.Appender;
using log4net.Core;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{
    public class ElasticSearchAppender : AppenderSkeleton
    {
        IRepository repository;

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

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                Validate(loggingEvent);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(string.Format("{0} must not be null", typeof(LoggingEvent).Name), ex, ErrorCode.GenericFailure);
                return;
            }

            try
            {
                repository.Add(LogEvent.Create(loggingEvent));
            }
            catch (Exception ex)
            {                
                ErrorHandler.Error(string.Format("Failed to add {0} to ElasticSearch", typeof(LogEvent).Name), ex, ErrorCode.GenericFailure);
            }
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

        static void Validate(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }
        }
    }
}

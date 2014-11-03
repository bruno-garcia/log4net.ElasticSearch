using System;
using System.Net;
using log4net.Appender;
using log4net.Core;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{
    public class ElasticSearchAppender : AppenderSkeleton
    {
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
            }
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
                var client = Repository.Create(ConnectionString);

                var logEvent = LogEventFactory.Create(loggingEvent);

                client.Add(logEvent);
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

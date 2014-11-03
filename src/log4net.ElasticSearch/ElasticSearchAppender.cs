using System;
using log4net.Appender;
using log4net.Core;

namespace log4net.ElasticSearch
{
    public class ElasticSearchAppender : AppenderSkeleton
    {
        public string ConnectionString { get; set; }
        
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                var exception = new InvalidOperationException("Connection string not present.");
                ErrorHandler.Error("Connection string not included in appender.", exception, ErrorCode.GenericFailure);

                return;
            }
            var client = Repository.Create(ConnectionString);

            var logEvent = LogEventFactory.Create(loggingEvent);
            try
            {
                client.Add(logEvent);
            }
            catch (InvalidOperationException ex)
            {
                ErrorHandler.Error("Invalid connection to ElasticSearch", ex, ErrorCode.GenericFailure);
            }
        }
    }
}

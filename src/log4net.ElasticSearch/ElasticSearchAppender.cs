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
            try
            {
                var logEvent = LogEventFactory.Create(loggingEvent);

                var client = Repository.Create(ConnectionString);
                client.Add(logEvent);
            }
            catch (ArgumentNullException ex)
            {
                ErrorHandler.Error("Unable to create LogEvent from LoggingEvent", ex, ErrorCode.GenericFailure);
            }
            catch (ArgumentException ex)
            {
                ErrorHandler.Error("ConnectionString not provided", ex, ErrorCode.GenericFailure);
            }
            catch (InvalidOperationException ex)
            {
                ErrorHandler.Error("ConnectionString is invalid", ex, ErrorCode.GenericFailure);
            }
        }
    }
}

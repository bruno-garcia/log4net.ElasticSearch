using System;
using log4net.ElasticSearch.Models;
using log4net.Appender;
using log4net.Core;

namespace log4net.ElasticSearch
{
    public class ElasticSearchAppender : AppenderSkeleton
    {
        public string ConnectionString { get; set; }

        /// <summary>
        /// Add a log event to the ElasticSearch Repo
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(Core.LoggingEvent loggingEvent)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                var exception = new InvalidOperationException("Connection string not present.");
                ErrorHandler.Error("Connection string not included in appender.", exception, ErrorCode.GenericFailure);

                return;
            }
            var settings = ConnectionBuilder.BuildElsticSearchConnection(ConnectionString);
            var client = new LogClient(settings);

            var logEvent = CreateLogEvent(loggingEvent);
            try
            {
                client.CreateEvent(logEvent);
            }
            catch (InvalidOperationException ex)
            {
                ErrorHandler.Error("Invalid connection to ElasticSearch", ex, ErrorCode.GenericFailure);
            }
        }

        private static LogEvent CreateLogEvent(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }

            var logEvent = new LogEvent();
            logEvent.Id = new UniqueIdGenerator().GenerateUniqueId();
            logEvent.LoggerName = loggingEvent.LoggerName;
            logEvent.Domain = loggingEvent.Domain;
            logEvent.Identity = loggingEvent.Identity;
            logEvent.ThreadName = loggingEvent.ThreadName;
            logEvent.UserName = loggingEvent.UserName;
            logEvent.MessageObject = loggingEvent.MessageObject == null ? "" : loggingEvent.MessageObject.ToString();
            logEvent.TimeStamp = loggingEvent.TimeStamp;
            logEvent.Exception = loggingEvent.ExceptionObject == null ? "" : loggingEvent.ExceptionObject.ToString();
            logEvent.Message = loggingEvent.RenderedMessage;
            logEvent.Fix = loggingEvent.Fix.ToString();
            logEvent.HostName = Environment.MachineName;

            if (loggingEvent.Level != null)
            {
                logEvent.Level = loggingEvent.Level.DisplayName;
            }

            if (loggingEvent.LocationInformation != null)
            {
                logEvent.ClassName = loggingEvent.LocationInformation.ClassName;
                logEvent.FileName = loggingEvent.LocationInformation.FileName;
                logEvent.LineNumber = loggingEvent.LocationInformation.LineNumber;
                logEvent.FullInfo = loggingEvent.LocationInformation.FullInfo;
                logEvent.MethodName = loggingEvent.LocationInformation.MethodName;
            }

            var properties = loggingEvent.GetProperties();
           
            foreach (var propertyKey in properties.GetKeys())
            {
                logEvent.Properties.Add(propertyKey, properties[propertyKey].ToString());
            }

            return logEvent;
        }
    }
}

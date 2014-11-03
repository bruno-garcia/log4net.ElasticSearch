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

        private static logEvent CreateLogEvent(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }

            var logEvent = new logEvent();
            logEvent.loggerName = loggingEvent.LoggerName;
            logEvent.domain = loggingEvent.Domain;
            logEvent.identity = loggingEvent.Identity;
            logEvent.threadName = loggingEvent.ThreadName;
            logEvent.userName = loggingEvent.UserName;
            logEvent.messageObject = loggingEvent.MessageObject == null ? "" : loggingEvent.MessageObject.ToString();
            logEvent.timeStamp = loggingEvent.TimeStamp.ToUniversalTime().ToString("O");
            logEvent.exception = loggingEvent.ExceptionObject == null ? "" : loggingEvent.ExceptionObject.ToString();
            logEvent.message = loggingEvent.RenderedMessage;
            logEvent.fix = loggingEvent.Fix.ToString();
            logEvent.hostName = Environment.MachineName;

            if (loggingEvent.Level != null)
            {
                logEvent.level = loggingEvent.Level.DisplayName;
            }

            if (loggingEvent.LocationInformation != null)
            {
                logEvent.className = loggingEvent.LocationInformation.ClassName;
                logEvent.fileName = loggingEvent.LocationInformation.FileName;
                logEvent.lineNumber = loggingEvent.LocationInformation.LineNumber;
                logEvent.fullInfo = loggingEvent.LocationInformation.FullInfo;
                logEvent.methodName = loggingEvent.LocationInformation.MethodName;
            }

            var properties = loggingEvent.GetProperties();
           
            foreach (var propertyKey in properties.GetKeys())
            {
                logEvent.properties.Add(propertyKey, properties[propertyKey].ToString());
            }

            // Add a "@timestamp" field to match the logstash format
            logEvent.properties.Add("@timestamp", loggingEvent.TimeStamp.ToUniversalTime().ToString("O")); 

            return logEvent;
        }
    }
}

using System;
using log4net.Core;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{
    public static class LogEventFactory
    {
        public static LogEvent Create(LoggingEvent loggingEvent)
        {
            var logEvent = new LogEvent
            {
                Id = new UniqueIdGenerator().GenerateUniqueId(),
                LoggerName = loggingEvent.LoggerName,
                Domain = loggingEvent.Domain,
                Identity = loggingEvent.Identity,
                ThreadName = loggingEvent.ThreadName,
                UserName = loggingEvent.UserName,
                MessageObject = loggingEvent.MessageObject == null ? "" : loggingEvent.MessageObject.ToString(),
                TimeStamp = loggingEvent.TimeStamp.ToUniversalTime().ToString("O"),
                Exception = loggingEvent.ExceptionObject == null ? "" : loggingEvent.ExceptionObject.ToString(),
                Message = loggingEvent.RenderedMessage,
                Fix = loggingEvent.Fix.ToString(),
                HostName = Environment.MachineName
            };

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

            // Add a "@timestamp" field to match the logstash format
            logEvent.Properties.Add("@timestamp", loggingEvent.TimeStamp.ToUniversalTime().ToString("O"));

            return logEvent;
        }         
    }
}
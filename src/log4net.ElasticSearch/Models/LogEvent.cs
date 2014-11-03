using System;
using System.Collections.Generic;
using log4net.Core;

namespace log4net.ElasticSearch.Models
{
    public class LogEvent
    {
        public LogEvent()
        {
            Properties = new Dictionary<string, string>();
        }

        public string Id { get; set; }

        public string TimeStamp { get; set; }
  
        public string Message { get; set; }
    
        public string MessageObject { get; set; }
      
        public string Exception { get; set; }
        
        public string LoggerName { get; set; }

        public string Domain { get; set; }

        public string Identity { get; set; }

        public string Level { get; set; }

        public string ClassName { get; set; }

        public string FileName { get; set; }

        public string LineNumber { get; set; }

        public string FullInfo { get; set; }

        public string MethodName { get; set; }

        public string Fix { get; set; }

        public IDictionary<string, string> Properties { get; set; }

        public string UserName { get; set; }

        public string ThreadName { get; set; }

        public string HostName { get; set; }

        public static LogEvent Create(LoggingEvent loggingEvent)
        {
            var logEvent = new LogEvent
            {
                Id = UniqueIdGenerator.GenerateUniqueId(),
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
                HostName = Environment.MachineName,
                Level = loggingEvent.Level == null ? null : loggingEvent.Level.DisplayName
            };
            
            if (loggingEvent.LocationInformation != null)
            {
                logEvent.ClassName = loggingEvent.LocationInformation.ClassName;
                logEvent.FileName = loggingEvent.LocationInformation.FileName;
                logEvent.LineNumber = loggingEvent.LocationInformation.LineNumber;
                logEvent.FullInfo = loggingEvent.LocationInformation.FullInfo;
                logEvent.MethodName = loggingEvent.LocationInformation.MethodName;
            }

            AddProperties(loggingEvent, logEvent);

            return logEvent;
        }

        static void AddProperties(LoggingEvent loggingEvent, LogEvent logEvent)
        {
            var properties = loggingEvent.GetProperties();

            foreach (var propertyKey in properties.GetKeys())
            {
                logEvent.Properties.Add(propertyKey, properties[propertyKey].ToString());
            }

            // Add a "@timestamp" field to match the logstash format
            logEvent.Properties.Add("@timestamp", loggingEvent.TimeStamp.ToUniversalTime().ToString("O"));
        }
    }
}
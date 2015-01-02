using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Core;

namespace log4net.ElasticSearch.Models
{
    public class logEvent
    {
        public logEvent()
        {
            properties = new Dictionary<string, string>();
        }

        public string timeStamp { get; set; }
  
        public string message { get; set; }
    
        public object messageObject { get; set; }
      
        public object exception { get; set; }
        
        public string loggerName { get; set; }

        public string domain { get; set; }

        public string identity { get; set; }

        public string level { get; set; }

        public string className { get; set; }

        public string fileName { get; set; }

        public string lineNumber { get; set; }

        public string fullInfo { get; set; }

        public string methodName { get; set; }

        public string fix { get; set; }

        public IDictionary<string, string> properties { get; set; }

        public string userName { get; set; }

        public string threadName { get; set; }

        public string hostName { get; set; }
                
        public static IEnumerable<logEvent> CreateMany(IEnumerable<LoggingEvent> loggingEvents)
        {
            return loggingEvents.Select(@event => Create(@event)).ToArray();
        }

        static logEvent Create(LoggingEvent loggingEvent)
        {
            var logEvent = new logEvent
            {
                loggerName = loggingEvent.LoggerName,
                domain = loggingEvent.Domain,
                identity = loggingEvent.Identity,
                threadName = loggingEvent.ThreadName,
                userName = loggingEvent.UserName,
                messageObject = loggingEvent.MessageObject ?? new object(),
                timeStamp = loggingEvent.TimeStamp.ToUniversalTime().ToString("O"),
                exception = loggingEvent.ExceptionObject ?? new object(),
                message = loggingEvent.RenderedMessage,
                fix = loggingEvent.Fix.ToString(),
                hostName = Environment.MachineName,
                level = loggingEvent.Level == null ? null : loggingEvent.Level.DisplayName
            };

            if (loggingEvent.LocationInformation != null)
            {
                logEvent.className = loggingEvent.LocationInformation.ClassName;
                logEvent.fileName = loggingEvent.LocationInformation.FileName;
                logEvent.lineNumber = loggingEvent.LocationInformation.LineNumber;
                logEvent.fullInfo = loggingEvent.LocationInformation.FullInfo;
                logEvent.methodName = loggingEvent.LocationInformation.MethodName;
            }

            AddProperties(loggingEvent, logEvent);

            return logEvent;
        }
        
        static void AddProperties(LoggingEvent loggingEvent, logEvent logEvent)
        {
            loggingEvent.Properties().Union(AppenderPropertiesFor(loggingEvent)).
                         Do(pair => logEvent.properties.Add(pair));
        }

        static IEnumerable<KeyValuePair<string, string>> AppenderPropertiesFor(LoggingEvent loggingEvent)
        {
            yield return Pair.For("@timestamp", loggingEvent.TimeStamp.ToUniversalTime().ToString("O"));
        }
    }
}
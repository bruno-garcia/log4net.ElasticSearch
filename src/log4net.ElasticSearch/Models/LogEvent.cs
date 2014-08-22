using System;
using System.Collections.Generic;

namespace log4net.ElasticSearch.Models
{
    /// <summary>
    /// Base log event type that we will send to Elasticsearch (serialized)
    /// </summary>
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

       
    }
}
using System.Collections.Generic;

namespace log4net.ElasticSearch.Models
{
    /// <summary>
    /// Base log event type that we will send to Elasticsearch (serialized)
    /// </summary>
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

       
    }
}
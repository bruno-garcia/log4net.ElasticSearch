using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using log4net.Appender;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{
    public class ElasticSearchAppender : AppenderSkeleton
    {
        private readonly ConnectionSettings elasticSettings;
        private readonly ElasticClient client;

        public string ElasticSearchServer { get; set; }
        public string ElasticSearchIndex { get; set; }

        public ElasticSearchAppender()
        {
            ElasticSearchServer = "localhost";
            ElasticSearchIndex = "log";

            elasticSettings = new ConnectionSettings(ElasticSearchServer, 9200)
                          .SetDefaultIndex(ElasticSearchIndex);

            client = new ElasticClient(elasticSettings);
        }
        /// <summary>
        /// Add a log event to the ElasticSearch Repo
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(Core.LoggingEvent loggingEvent)
        {
            LogEvent logEvent = new LogEvent
                {
                    Date = loggingEvent.TimeStamp,
                    Exception = loggingEvent.RenderedMessage,
                    Level = loggingEvent.Level.ToString(),
                    Logger = loggingEvent.LoggerName,
                    Message = loggingEvent.RenderedMessage,
                    Thread = loggingEvent.ThreadName
                };

            var results = client.Index(logEvent);
            
        }
    }
}

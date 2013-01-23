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

        public string ConnectionString { get; set; }

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
            string conn = ConnectionString;
            var builder = new System.Data.Common.DbConnectionStringBuilder();
            builder.ConnectionString = conn.Replace("{", "\"").Replace("}", "\"");
            var keys = builder.Keys;

            LogEvent logEvent = new LogEvent();
            
            if (logEvent == null)
            {
                throw new ArgumentNullException("logEvent");
            }

            logEvent.LoggerName = loggingEvent.LoggerName;
            logEvent.Domain = loggingEvent.Domain;
            logEvent.Identity = loggingEvent.Identity;
            logEvent.ThreadName = loggingEvent.ThreadName;
            logEvent.UserName = loggingEvent.UserName;
            logEvent.MessageObject = loggingEvent.MessageObject;
            logEvent.TimeStamp = loggingEvent.TimeStamp;
            logEvent.Exception = loggingEvent.ExceptionObject;
            logEvent.Message = loggingEvent.RenderedMessage;
            logEvent.Fix = loggingEvent.Fix.ToString();

            if (logEvent.Level != null)
            {
                logEvent.Level = loggingEvent.Level.ToString();
            }

            if (loggingEvent.LocationInformation != null)
            {
                logEvent.ClassName = loggingEvent.LocationInformation.ClassName;
                logEvent.FileName = loggingEvent.LocationInformation.FileName;
                logEvent.LineNumber = loggingEvent.LocationInformation.LineNumber;
                logEvent.FullInfo = loggingEvent.LocationInformation.FullInfo;
                logEvent.MethodName = loggingEvent.LocationInformation.MethodName;
            }

            logEvent.Properties = loggingEvent.Properties.GetKeys().ToDictionary(key => key, key => logEvent.Properties[key].ToString());

            var results = client.Index(logEvent);
            
        }
    }
}

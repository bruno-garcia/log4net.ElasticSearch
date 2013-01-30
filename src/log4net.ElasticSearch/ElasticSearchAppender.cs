using System;
using System.Linq;
using Nest;
using log4net.Appender;
using log4net.Core;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{
    public class ElasticSearchAppender : AppenderSkeleton
    {
        //private readonly ConnectionSettings elasticSettings;
        private  ElasticClient client;

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

            client = new ElasticClient(ConnectionBuilder.BuildElsticSearchConnection(ConnectionString));
            
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
            logEvent.MessageObject = loggingEvent.MessageObject == null ? "" : loggingEvent.MessageObject.ToString();
            logEvent.TimeStamp = loggingEvent.TimeStamp;
            logEvent.Exception = loggingEvent.ExceptionObject == null ? "" : loggingEvent.MessageObject.ToString();
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

            if (client.IsValid)
            {
                var results = client.Index(logEvent);    
            }
            else
            {
                var exception = new InvalidOperationException("Connection to ElasticSearch is invalid.");
                ErrorHandler.Error("Invalid connection to ElasticSearch", exception, ErrorCode.GenericFailure);

                return;
            }
        }
    }
}

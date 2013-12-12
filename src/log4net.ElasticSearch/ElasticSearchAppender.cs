using System;
using System.Collections.Generic;
using System.Dynamic;
using log4net.ElasticSearch.Extensions;
using log4net.ElasticSearch.Models;
using Nest;
using log4net.Appender;
using log4net.Core;

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
            var settings = ConnectionBuilder.BuildElsticSearchConnection(ConnectionString);
            client = new ElasticClient(settings);
            var logEvent = CreateLogEvent(loggingEvent);
            try
            {
                client.IndexAsync(logEvent);
            }
            catch (InvalidOperationException ex)
            {
                ErrorHandler.Error("Invalid connection to ElasticSearch", ex, ErrorCode.GenericFailure);
            }
        }

        private static dynamic CreateLogEvent(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }
            dynamic logEvent = new ExpandoObject();
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
            var expandoDict = logEvent as IDictionary<string, Object>;
            foreach (var propertyKey in properties.GetKeys())
            {
                expandoDict.Add(propertyKey, properties[propertyKey].ToString());
            }
            return logEvent;
        }
    }
}

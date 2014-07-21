using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using log4net.ElasticSearch.Models;
using Nest;
using log4net.Appender;
using log4net.Core;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch
{
    public class ElasticSearchAppender : AppenderSkeleton
    {
        private ElasticClient _client;
        private SmartFormatter _indexName;
        private SmartFormatter _indexType;
        public static readonly string TagsKeyName = "@Tags";

        public string Server { get; set; }
        public string Port { get; set; }
        public bool IndexAsync { get; set; }
        TemplateInfo Template { get; set; }
        public ElasticAppenderFilters ElasticFilters { get; set; }

        public string IndexName
        {
            set { _indexName = value.ToLower(); }
        }

        public string IndexType
        {
            set { _indexType = value; }
        }

        public ElasticSearchAppender()
        {
            Server = "localhost";
            Port = "9200";
            IndexName = "LogEvent-%{+yyyy-MM-dd}";
            IndexType = "LogEvent";
            IndexAsync = false;
            Template = null;

            ElasticFilters = new ElasticAppenderFilters();
        }

        public override void ActivateOptions()
        {
            var connectionSettings = new ConnectionSettings(new Uri(string.Format("http://{0}:{1}", Server, Port)));
            _client = new ElasticClient(connectionSettings);

            if (Template != null && Template.IsValid)
            {
                _client.PutTemplateRaw(Template.Name, File.ReadAllText(Template.FileName));
            }

            ElasticFilters.PrepareConfiguration(_client);
        }

        protected override void OnClose()
        {
            if (_client == null) return;

            _client.Flush();
            _client = null;
        }
        
        /// <summary>
        /// Add a log event to the ElasticSearch Repo
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(Core.LoggingEvent loggingEvent)
        {
            if (_client == null || loggingEvent == null)
            {
                return;
            }

            var logEvent = CreateLogEvent(loggingEvent);
            ElasticFilters.PrepareEvent(logEvent, _client);

            try
            {
                DoIndex(logEvent);
            }
            catch (InvalidOperationException ex)
            {
                ErrorHandler.Error("Invalid connection to ElasticSearch", ex, ErrorCode.GenericFailure);
            }
        }

        private void DoIndex(JObject logEvent)
        {
            var indexName = _indexName.Format(logEvent);
            var indexType = _indexType.Format(logEvent);

            if (IndexAsync)
            {
                _client.IndexAsync(logEvent, indexName, indexType);
            }
            else
            {
                _client.Index(logEvent, indexName, indexType);
            }
        }

        private static JObject CreateLogEvent(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }

            var logEvent = new JObject();
            
            logEvent["Id"] = UniqueIdGenerator.Instance.GenerateUniqueId();
            logEvent["LoggerName"] = loggingEvent.LoggerName;
            logEvent["Domain"] = loggingEvent.Domain;
            logEvent["Identity"] = loggingEvent.Identity;
            logEvent["ThreadName"] = loggingEvent.ThreadName;
            logEvent["UserName"] = loggingEvent.UserName;
            logEvent["MessageObject"] = loggingEvent.MessageObject == null ? "" : loggingEvent.MessageObject.ToString();
            logEvent["TimeStamp"] = loggingEvent.TimeStamp;
            logEvent["Exception"] = loggingEvent.ExceptionObject == null ? "" : loggingEvent.ExceptionObject.ToString();
            logEvent["Message"] = loggingEvent.RenderedMessage;
            logEvent["Fix"] = loggingEvent.Fix.ToString();
            logEvent["HostName"] = Environment.MachineName;

            if (loggingEvent.Level != null)
            {
                logEvent["Level"] = loggingEvent.Level.DisplayName;
            }

            if (loggingEvent.LocationInformation != null)
            {
                logEvent["ClassName"] = loggingEvent.LocationInformation.ClassName;
                logEvent["FileName"] = loggingEvent.LocationInformation.FileName;
                logEvent["LineNumber"] = loggingEvent.LocationInformation.LineNumber;
                logEvent["FullInfo"] = loggingEvent.LocationInformation.FullInfo;
                logEvent["MethodName"] = loggingEvent.LocationInformation.MethodName;
            }

            var properties = loggingEvent.GetProperties();
            foreach (var propertyKey in properties.GetKeys())
            {
                logEvent.Add(propertyKey, properties[propertyKey].ToString());
            }
            return logEvent;
        }
    }
}

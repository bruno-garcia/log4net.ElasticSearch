using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
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

        public string Server { get; set; }
        public string Ip { get; set; }
        public string IndexName { get; set; }
        public string IndexType { get; set; }
        public bool IsIndexAsync { get; set; }
        TemplateInfo Template { get; set; }

        public List<IElasticOption> ElasticOptions { get; set; }

        public ElasticSearchAppender()
        {
            Server = "localhost";
            Ip = "9200";
            IndexName = "log_test";
            IndexType = "LogEvent";
            IsIndexAsync = false;
            Template = null;

            ElasticOptions = new List<IElasticOption>();
        }

        public override void ActivateOptions()
        {
            var connectionSettings = new ConnectionSettings(new Uri(string.Format("http://{0}:{1}", Server, Ip)));
            _client = new ElasticClient(connectionSettings);

            if (Template != null && Template.IsValid)
            {
                _client.PutTemplateRaw(Template.Name, File.ReadAllText(Template.FileName));
            }

            foreach (var option in ElasticOptions)
            {
                option.PrepareConfiguration(_client);
            }
        }

        public void AddElasticOption(IElasticOption newOption)
        {
            ElasticOptions.Add(newOption);
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
            foreach (var option in ElasticOptions)
            {
                option.PrepareEvent(logEvent);
            }

            try
            {
                DoIndex(logEvent);
            }
            catch (InvalidOperationException ex)
            {
                ErrorHandler.Error("Invalid connection to ElasticSearch", ex, ErrorCode.GenericFailure);
            }
        }

        private void DoIndex<T>(T logEvent) where T : class
        {
            _client.Index(logEvent, IndexName, IndexType);
        }

        private JObject CreateLogEvent(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }
            JObject logEvent = new JObject();
            
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
            var expandoDict = logEvent as IDictionary<string, Object>;
            foreach (var propertyKey in properties.GetKeys())
            {
                expandoDict.Add(propertyKey, properties[propertyKey].ToString());
            }
            return logEvent;
        }
    }
}

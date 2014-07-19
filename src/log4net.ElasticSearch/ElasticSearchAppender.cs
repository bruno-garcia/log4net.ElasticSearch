using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using log4net.ElasticSearch.Models;
using log4net.Layout;
using log4net.Util;
using Nest;
using log4net.Appender;
using log4net.Core;

namespace log4net.ElasticSearch
{
    public class TemplateInfo : IOptionHandler
    {
        private readonly IErrorHandler _errorHandler;

        public string Name { get; set; }
        public string FileName { get; set; }
        public bool IsValid { get; private set; }

        public TemplateInfo()
        {
            IsValid = false;
            _errorHandler = new OnlyOnceErrorHandler(this.GetType().Name);
        }

        public void ActivateOptions()
        {
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(FileName))
            {
                _errorHandler.Error("Template name or fileName is empty!");
            }

            if (!File.Exists(FileName))
            {
                _errorHandler.Error(string.Format("Could not find template file: {0}", FileName));
            }

            IsValid = true;
        }
    }
    public class ElasticSearchAppender : AppenderSkeleton
    {
        private ElasticClient _client;
        private ConnectionSettings _settings;
        private List<IElasticOption> _elasticOptions = new List<IElasticOption>();

        public string ConnectionString { get; set; }
        public string IndexName { get; set; }
        public string IndexType { get; set; }
        public bool IndexAsync { get; set; }
        TemplateInfo Template { get; set; }

        public List<IElasticOption> ElasticOptions
        {
            get { return _elasticOptions; }
            set { _elasticOptions = value; }
        }

        public override void ActivateOptions()
        {

            if (string.IsNullOrEmpty(ConnectionString))
            {
                var exception = new InvalidOperationException("Connection string not present.");
                ErrorHandler.Error("Connection string not included in appender.", exception, ErrorCode.GenericFailure);

                _client = null;
                return;
            }
            
            _settings = ConnectionBuilder.BuildElsticSearchConnection(ConnectionString);
            _client = new ElasticClient(_settings);

            if (Template != null && Template.IsValid)
            {
                _client.PutTemplateRaw(Template.Name, File.ReadAllText(Template.FileName));
            }


        }

        public void AddElasticOption(IElasticOption newOption)
        {
            _elasticOptions.Add(newOption);
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
            _client.Index(logEvent, _settings.DefaultIndex, "LogEvent");
        }

        private dynamic CreateLogEvent(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }
            dynamic logEvent = new ExpandoObject();
            
            logEvent.Id = UniqueIdGenerator.Instance.GenerateUniqueId();
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

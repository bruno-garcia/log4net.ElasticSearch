using System;
using System.IO;
using System.Threading;
using log4net.ElasticSearch.Models;
using log4net.Util;
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

        private readonly object _bulkSync;
        private int _currentBulkSize;
        private BulkDescriptor _bulkDescriptor;
        private readonly Timer _timer;

        public string Server { get; set; }
        public string Port { get; set; }
        public bool IndexAsync { get; set; }
        public int BulkSize { get; set; }
        public int BulkIdleTimeout { get; set; }
        public TemplateInfo Template { get; set; }
        public ElasticAppenderFilters ElasticFilters { get; set; }

        public string IndexName
        {
            set { _indexName = value; }
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
            BulkSize = 100;
            BulkIdleTimeout = 2;
            Template = null;

            _bulkSync = new object();
            _currentBulkSize = 0;
            _bulkDescriptor = new BulkDescriptor();
            _timer = new Timer(TimerElapsed, "timer", -1, -1);

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

            StartTimer();
        }

        private void StartTimer()
        {
            _timer.Change(TimeSpan.FromSeconds(BulkIdleTimeout), TimeSpan.FromMilliseconds(-1));
        }

        /// <summary>
        /// On case of error or when the appender is closed on configuration change.
        /// </summary>
        protected override void OnClose()
        {
            _timer.Change(-1, -1);
            DoIndexNow();
        }
        
        /// <summary>
        /// Add a log event to the ElasticSearch Repo
        /// </summary>
        /// <param name="loggingEvent"></param>
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (_client == null || loggingEvent == null)
            {
                return;
            }

            var logEvent = CreateLogEvent(loggingEvent);
            PrepareAndAddToBulk(logEvent);

            if (Interlocked.Increment(ref _currentBulkSize) >= BulkSize)
            {
                DoIndexNow();
            }
        }

        /// <summary>
        /// Prepare the event and add it to the BulkDescriptor.
        /// </summary>
        /// <param name="logEvent"></param>
        private void PrepareAndAddToBulk(JObject logEvent)
        {
            ElasticFilters.PrepareEvent(logEvent, _client);

            var indexName = _indexName.Format(logEvent).ToLower();
            var indexType = _indexType.Format(logEvent);

            lock (_bulkSync)
            {
                _bulkDescriptor.Index<JObject>(descriptor =>
                {
                    descriptor.Object(logEvent);
                    descriptor.Index(indexName);
                    descriptor.Type(indexType);
                    return descriptor;
                });
            }
        }

        public void TimerElapsed(object state)
        {
            DoIndexNow();
            StartTimer();
        }
        private void DoIndexNow()
        {
            if (_currentBulkSize == 0)
                return;

            BulkDescriptor bulk;
            lock (_bulkSync)
            {
                if (_currentBulkSize == 0)
                    return;

                bulk = _bulkDescriptor;
                _bulkDescriptor = new BulkDescriptor();
                _currentBulkSize = 0;
            }

            try
            {
                if (IndexAsync)
                {
                    _client.BulkAsync(bulk);
                }
                else
                {
                    _client.Bulk(bulk);
                }
            }
            catch (Exception ex)
            {
                LogLog.Error(GetType(), "Invalid connection to ElasticSearch", ex);
                throw;
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

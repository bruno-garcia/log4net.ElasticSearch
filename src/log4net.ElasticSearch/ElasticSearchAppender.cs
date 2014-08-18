using System;
using System.IO;
using System.Threading;
using Elasticsearch.Net;
using log4net.ElasticSearch.InnerExceptions;
using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.SmartFormatters;
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
        private LogEventSmartFormatter _indexName;
        private LogEventSmartFormatter _indexType;

        private BulkDescriptorProxy _bulkDescriptor;
        private readonly Timer _timer;

        public FixFlags FixedFields { get; set; }

        public int BulkSize { get; set; }
        public int BulkIdleTimeout { get; set; }
        public int TimeoutToWaitForTimer { get; set; }

        // elastic configuration
        public string Server { get; set; }
        public string Port { get; set; }
        public bool IndexAsync { get; set; }
        public int MaxAsyncConnections { get; set; }
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
            FixedFields = FixFlags.Partial;

            BulkSize = 2000;
            BulkIdleTimeout = 5000;
            TimeoutToWaitForTimer = 5000;
            _bulkDescriptor = new BulkDescriptorProxy();
            _timer = new Timer(TimerElapsed, "timer", -1, -1);

            Server = "localhost";
            Port = "9200";
            IndexName = "LogEvent-%{+yyyy-MM-dd}";
            IndexType = "LogEvent";
            IndexAsync = true;
            MaxAsyncConnections = 10;
            Template = null;

            ElasticFilters = new ElasticAppenderFilters();
        }

        public override void ActivateOptions()
        {
            var connectionSettings = new ConnectionSettings(new Uri(string.Format("http://{0}:{1}", Server, Port)));
            connectionSettings.SetMaximumAsyncConnections(MaxAsyncConnections);
            _client = new ElasticClient(connectionSettings);
            
            if (Template != null && Template.IsValid)
            {
                var res = _client.Raw.IndicesPutTemplateForAll(Template.Name, File.ReadAllText(Template.FileName));
                if (!res.Success)
                {
                    throw new ErrorSettingTemplateException(res);
                }
            }

            ElasticFilters.PrepareConfiguration(_client);

            RestartTimer();
        }

        private void RestartTimer()
        {
            var timeout = TimeSpan.FromMilliseconds(BulkIdleTimeout);
            _timer.Change(timeout, timeout);
        }

        /// <summary>
        /// On case of error or when the appender is closed on configuration change.
        /// </summary>
        protected override void OnClose()
        {
            DoIndexNow();

            // let the timer finish its job
            WaitHandle notifyObj = new AutoResetEvent(false);
            _timer.Dispose(notifyObj);
            notifyObj.WaitOne(TimeoutToWaitForTimer);
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
            PrepareAndAddToBulk(logEvent, loggingEvent);

            if (_bulkDescriptor.Size >= BulkSize && BulkSize > 0)
            {
                DoIndexNow();
            }
        }

        /// <summary>
        /// Prepare the event and add it to the BulkDescriptor.
        /// </summary>
        /// <param name="logEvent"></param>
        /// <param name="loggingEvent"></param>
        private void PrepareAndAddToBulk(JObject logEvent, LoggingEvent loggingEvent)
        {
            ElasticFilters.PrepareEvent(logEvent, _client);

            var indexName = _indexName.Format(logEvent).ToLower();
            var indexType = _indexType.Format(logEvent);

            _bulkDescriptor.AddIndexOperation<JObject>(descriptor =>
            {
                descriptor.Document(logEvent);
                descriptor.Index(indexName);
                descriptor.Type(indexType);
                return descriptor;
            });
        }

        public void TimerElapsed(object state)
        {
            DoIndexNow();
        }

        private void DoIndexNow()
        {
            // ref-swap its atomic so we dont need to lock 
            BulkDescriptorProxy bulk = _bulkDescriptor;
            _bulkDescriptor = new BulkDescriptorProxy();

            try
            {
                bulk.DoIndex(_client, IndexAsync);
            }
            catch (Exception ex)
            {
                LogLog.Error(GetType(), "Invalid connection to ElasticSearch", ex);
            }
        }

        private JObject CreateLogEvent(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }

            var logEvent = new JObject();

            //logEvent["Id"] = UniqueIdGenerator.Instance.GenerateUniqueId();
            logEvent["TimeStamp"] = loggingEvent.TimeStamp;
            logEvent["LoggerName"] = loggingEvent.LoggerName;
            logEvent["ThreadName"] = loggingEvent.ThreadName;

            logEvent["Message"] = loggingEvent.MessageObject == null ? "" : loggingEvent.MessageObject.ToString();
            logEvent["Exception"] = loggingEvent.ExceptionObject == null ? "" : loggingEvent.ExceptionObject.ToString();
            //logEvent["Message"] = loggingEvent.RenderedMessage;
            //logEvent["Fix"] = loggingEvent.Fix.ToString(); // We need this?
            logEvent["AppDomain"] = loggingEvent.Domain;
            logEvent["HostName"] = Environment.MachineName;

            if (loggingEvent.Level != null)
            {
                logEvent["Level"] = loggingEvent.Level.DisplayName;
            }

            if (FixedFields.HasFlag(FixFlags.Identity))
            {
                logEvent["Identity"] = loggingEvent.Identity;
            }

            if (FixedFields.HasFlag(FixFlags.UserName))
            {
                logEvent["UserName"] = loggingEvent.UserName;
            }

            if (FixedFields.HasFlag(FixFlags.LocationInfo) && loggingEvent.LocationInformation != null)
            {
                var locationInfo = logEvent["LocationInformation"] = new JObject();
                locationInfo["ClassName"] = loggingEvent.LocationInformation.ClassName;
                locationInfo["FileName"] = loggingEvent.LocationInformation.FileName;
                locationInfo["LineNumber"] = loggingEvent.LocationInformation.LineNumber;
                locationInfo["FullInfo"] = loggingEvent.LocationInformation.FullInfo;
                locationInfo["MethodName"] = loggingEvent.LocationInformation.MethodName;
            }

            if (FixedFields.HasFlag(FixFlags.Properties))
            {
                var properties = loggingEvent.GetProperties();
                foreach (var propertyKey in properties.GetKeys())
                {
                    logEvent.Add(propertyKey, properties[propertyKey].ToString());
                }
            }
            return logEvent;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

            _currentBulkSize = 0;
            BulkSize = 2000;
            BulkIdleTimeout = 10;
            TimeoutToWaitForTimer = 5000;
            _bulkSync = new object();
            _bulkDescriptor = new BulkDescriptor();
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
                _client.PutTemplateRaw(Template.Name, File.ReadAllText(Template.FileName));
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
            PrepareAndAddToBulk(logEvent);

            if (Interlocked.Increment(ref _currentBulkSize) >= BulkSize && BulkSize > 0)
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

        private JObject CreateLogEvent(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }

            var logEvent = new JObject();
            
            logEvent["Id"] = UniqueIdGenerator.Instance.GenerateUniqueId();
            logEvent["LoggerName"] = loggingEvent.LoggerName;
            logEvent["ThreadName"] = loggingEvent.ThreadName;
            logEvent["Domain"] = loggingEvent.Domain;

            logEvent["MessageObject"] = loggingEvent.MessageObject == null ? "" : loggingEvent.MessageObject.ToString();
            logEvent["TimeStamp"] = loggingEvent.TimeStamp;
            logEvent["Exception"] = loggingEvent.ExceptionObject == null ? "" : loggingEvent.ExceptionObject.ToString();
            logEvent["Message"] = loggingEvent.RenderedMessage;
            //logEvent["Fix"] = loggingEvent.Fix.ToString(); // We need this?
            logEvent["HostName"] = Environment.MachineName;

            if (loggingEvent.Level != null)
            {
                logEvent["Level"] = loggingEvent.Level.DisplayName;
            }

            if (FixedFields.IsSwitched(FixFlags.Identity))
            {
                logEvent["Identity"] = loggingEvent.Identity;
            }

            if (FixedFields.IsSwitched(FixFlags.UserName))
            {
                logEvent["UserName"] = loggingEvent.UserName;
            }

            if (FixedFields.IsSwitched(FixFlags.LocationInfo) && loggingEvent.LocationInformation != null)
            {
                var locationInfo = logEvent["LocationInformation"] = new JObject();
                locationInfo["ClassName"] = loggingEvent.LocationInformation.ClassName;
                locationInfo["FileName"] = loggingEvent.LocationInformation.FileName;
                locationInfo["LineNumber"] = loggingEvent.LocationInformation.LineNumber;
                locationInfo["FullInfo"] = loggingEvent.LocationInformation.FullInfo;
                locationInfo["MethodName"] = loggingEvent.LocationInformation.MethodName;
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

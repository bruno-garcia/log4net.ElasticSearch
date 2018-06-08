using System;
using System.Collections.Generic;
using Nest;

namespace log4net.ElasticSearch.Tests
{
    [ElasticsearchType(Name = "LogEvent")]
    public class LogEvent
    {
        public LogEvent()
        {
            Properties = new Dictionary<string, string>();
        }

        [Nest.Text]
        public string Id { get; set; }

        [Nest.Date]
        public DateTime TimeStamp { get; set; }

        [Nest.Text]
        public string Message { get; set; }

        [Nest.Nested]
        public object MessageObject { get; set; }

        [Nest.Text]
        public string Exception { get; set; }

        [Nest.Text]
        public string LoggerName { get; set; }

        [Nest.Text]
        public string Domain { get; set; }

        [Nest.Text]
        public string Identity { get; set; }

        [Nest.Text]
        public string Level { get; set; }

        [Nest.Text]
        public string ClassName { get; set; }

        [Nest.Text]
        public string FileName { get; set; }

        [Nest.Text]
        public string LineNumber { get; set; }

        [Nest.Text]
        public string FullInfo { get; set; }

        [Nest.Text]
        public string MethodName { get; set; }

        [Nest.Text]
        public string Fix { get; set; }

        [Nest.Nested]
        public IDictionary<string, string> Properties { get; set; }

        [Nest.Text]
        public string UserName { get; set; }

        [Nest.Text]
        public string ThreadName { get; set; }

        [Nest.Text]
        public string HostName { get; set; }


    }
}
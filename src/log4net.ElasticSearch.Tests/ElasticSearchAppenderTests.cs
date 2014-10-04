using System;
using System.Linq;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchAppenderTests : ElasticSearchTestSetup
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ElasticSearchAppenderTests));

        [Fact]
        public void Can_insert_record()
        {
            var logEvent = new LogEvent
                {
                    ClassName = "IntegrationTestClass",
                    Domain = "TestDomain",
                    Exception = "This is a test exception",
                    FileName = "c:\test\file.txt",
                    Fix = "none",
                    FullInfo = "A whole bunch of error info dump",
                    Identity = "localhost\\user",
                    Level = "9",
                    LineNumber = "99",
                    TimeStamp = DateTime.Now
                };

            var results = client.Index(logEvent);

            results.Id.Should().NotBeNull();
        }

        [Fact(Skip = "xunit weirdness")]
        public void Can_read_properties()
        {
            GlobalContext.Properties["globalDynamicProperty"] = "global";
            ThreadContext.Properties["threadDynamicProperty"] = "thread";
            LogicalThreadContext.Properties["logicalThreadDynamicProperty"] = "local thread";
            _log.Info("loggingtest");

            Thread.Sleep(1500);

            var searchResults = client.Search<dynamic>(s => s.Query(q => q.Term("message", "loggingtest")));

            searchResults.Total.Should().Be(1);

            var firstEntry = searchResults.Documents.First();

            ((string) firstEntry.globalDynamicProperty.ToString()).Should().Be("global");
            ((string)firstEntry.threadDynamicProperty.ToString()).Should().Be("thread");
            ((string)firstEntry.logicalThreadDynamicProperty.ToString()).Should().Be("local thread");
        }

        [Fact]
        public void Can_read_inserted_record()
        {
            var logEvent = new LogEvent {Message = "testmessage", ClassName = "thisclass"};

            client.Index(logEvent);
            Thread.Sleep(2000);

            var searchResults = client.Search<LogEvent>(s => s.Query(q => q.Term("ClassName", "thisclass")));

            searchResults.Total.Should().Be(1);
        }

        [Fact]
        public void Can_create_an_event_from_log4net()
        {
            _log.Info("loggingtest");
            Thread.Sleep(2000);

            var searchResults = client.Search<LogEvent>(s => s.Query(q => q.Term("Message", "loggingtest")));

            searchResults.Total.Should().Be(1);            
        }
    }
}

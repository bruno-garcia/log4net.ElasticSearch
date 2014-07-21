using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using Nest;
using Xunit;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchAppenderTests : ElasticSearchTestSetup, IDisposable
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ElasticSearchAppenderTests));

        public ElasticSearchAppenderTests()
        {
            
        }

        [Fact]
        public void Can_insert_record()
        {
            var logEvent = new 
                {
                    ClassName = "IntegrationTestClass",
                    Domain = "TestDomain",
                    Exception = "This is a test exception",
                    FileName = "c:\\test\\file.txt",
                    Fix = "none",
                    FullInfo = "A whole bunch of error info dump",
                    Identity = "localhost\\user",
                    Level = "9",
                    LineNumber = "99",
                    TimeStamp = DateTime.Now
                };

            var results = Client.Index(logEvent, _testIndex, "type1");

            Assert.NotNull(results.Id);
        }

        [Fact(Skip = "xunit weirdness")]
        public void Can_read_properties()
        {
            GlobalContext.Properties["globalDynamicProperty"] = "global";
            ThreadContext.Properties["threadDynamicProperty"] = "thread";
            LogicalThreadContext.Properties["logicalThreadDynamicProperty"] = "local thread";
            _log.Info("loggingtest");

            Thread.Sleep(1500);

            var searchResults = Client.Search(s => s.Query(q => q.Term("message", "loggingtest")));

            Assert.Equal(1, Convert.ToInt32(searchResults.Hits.Total));
            var firstEntry = searchResults.Documents.First();
            Assert.Equal("global", firstEntry.globalDynamicProperty.ToString());
            Assert.Equal("thread", firstEntry.threadDynamicProperty.ToString());
            Assert.Equal("local thread", firstEntry.logicalThreadDynamicProperty.ToString());
        }

        [Fact]
        public void Can_read_inserted_record()
        {
            var logEvent = new 
            {
                ClassName = "IntegrationTestClass",
                Exception = "ReadingTest"
            };

            Client.Index(logEvent);
            Client.Refresh();

            var searchResults = Client.Search(s => s.Query(q => q.Term("exception", "readingtest")));

            Assert.Equal(1, Convert.ToInt32(searchResults.Hits.Total));
        }

        [Fact]
        public void Can_create_an_event_from_log4net()
        {
            _log.Info("loggingtest");
            Thread.Sleep(2000);

            var searchResults = Client.Search(s => s.Query(q => q.Term("message", "loggingtest")));

            Assert.Equal(1, Convert.ToInt32(searchResults.Hits.Total));
            
        }

        //[Fact]
        //public void Can_update_logger_configuration()
        //{
        //    while (true)
        //    {
        //        _log.Info("log");
        //        Thread.Sleep(2000);
        //    }
        //}

        public void Dispose()
        {
            DeleteTestIndex();
        }
    }
}

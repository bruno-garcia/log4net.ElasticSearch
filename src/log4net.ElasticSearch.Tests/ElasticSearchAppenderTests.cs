using System;
using Xunit;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchAppenderTests : ElasticSearchTestSetup, IDisposable
    {
        public ElasticSearchAppenderTests()
        {
            SetupTestIndex();
        }

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

            Assert.Equal(results.OK, true);
        }

        [Fact]
        public void Can_read_inserted_record()
        {
            var logEvent = new LogEvent
            {
                ClassName = "IntegrationTestClass",
                Exception = "ReadingTest"
            };

            client.Index(logEvent);
            client.Refresh();

            var searchResults = client.Search<LogEvent>(s => s.Query(q => q.Term(x => x.Exception, "readingtest")));

            Assert.Equal(1, Convert.ToInt32(searchResults.Hits.Total));
        }

        public void Dispose()
        {
            DeleteTestIndex();
        }
    }
}

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using Nest;
using NUnit.Framework;

namespace log4net.ElasticSearch.Tests
{
    [TestFixture]
    public class ElasticSearchAppenderTests : ElasticSearchTestSetup
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ElasticSearchAppenderTests));

        [SetUp]
        public void TestsSetup()
        {
            DeleteTestIndex();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            DeleteTestIndex();
        }

        [Test]
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

            var results = Client.Index(logEvent);

            Assert.NotNull(results.Id);
        }

        [Test]
        public void Can_read_inserted_record()
        {
            var logEvent = new
            {
                ClassName = "IntegrationTestClass",
                Exception = "ReadingTest"
            };

            Client.Index(logEvent);
            Client.Refresh();

            var searchResults = Client.Search(s => s.Query(q => q.Term("Exception", "readingtest")));

            Assert.AreEqual(1, Convert.ToInt32(searchResults.Hits.Total));
        }

        [Test]
        public void Can_create_an_event_from_log4net()
        {
            _log.Info("loggingtest");

            var searchResults = Client.Search(s => s.Query(q => q.Term("Message", "loggingtest")));

            Assert.AreEqual(1, Convert.ToInt32(searchResults.Hits.Total));

        }

        [Test]
        public void Can_read_properties()
        {
            GlobalContext.Properties["globalDynamicProperty"] = "global";
            ThreadContext.Properties["threadDynamicProperty"] = "thread";
            LogicalThreadContext.Properties["logicalThreadDynamicProperty"] = "local thread";
            _log.Info("loggingtest");

            var searchResults = Client.Search(s => s.Query(q => q.Term("Message", "loggingtest")));

            Assert.AreEqual(1, Convert.ToInt32(searchResults.Hits.Total));
            var firstEntry = searchResults.Documents.First();
            Assert.AreEqual("global", firstEntry.globalDynamicProperty.ToString());
            Assert.AreEqual("thread", firstEntry.threadDynamicProperty.ToString());
            Assert.AreEqual("local thread", firstEntry.logicalThreadDynamicProperty.ToString());
        }

        [Test]
        public void Can_read_KvFilter_properties()
        {
            _log.Info("this is message key=value, another = 'another' object:[id=1] anotherObj:[id=2,content=blue]");

            var searchResults = Client.Search(s => s.Take(1));
            var entry = searchResults.Documents.First();
            Assert.AreEqual("value", entry.key.ToString());
            Assert.AreEqual("another", entry.another.ToString());
            Assert.AreEqual("1", entry["object"].id.ToString());
            Assert.AreEqual("blue", entry.anotherObj.content.ToString());
        }

    }
}

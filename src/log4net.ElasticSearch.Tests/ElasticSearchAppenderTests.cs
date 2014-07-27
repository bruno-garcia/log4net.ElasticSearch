using System;
using System.Linq;
using System.Threading;
using Nest;
using NUnit.Framework;

namespace log4net.ElasticSearch.Tests
{
    [TestFixture]
    public class ElasticSearchAppenderTests 
    {
        public ElasticClient Client;
        public readonly string TestIndex = "log_test_" + DateTime.Now.ToString("yyyy-MM-dd");
        
        private static readonly ILog _log = LogManager.GetLogger(typeof(ElasticSearchAppenderTests));

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            ConnectionSettings elasticSettings =
               new ConnectionSettings(new Uri("http://127.0.0.1:9200"))
                   .SetDefaultIndex(TestIndex);

            Client = new ElasticClient(elasticSettings);
            FixtureTearDown();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Client.DeleteIndex(TestIndex);
            Client.DeleteIndex("log-tests");
        }

        [SetUp]
        public void TestSetup()
        {
            FixtureTearDown();
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

            var results = Client.Index(logEvent, "log-tests", "anonymous");

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

            Client.Index(logEvent, "log-tests", "anonymous");
            Client.Refresh();
            var searchResults = Client.Search(s => s.Take(1));
            Assert.AreEqual(1, searchResults.Hits.Total);
            Assert.AreEqual("ReadingTest", searchResults.Documents.First().Exception);
        }

        [Test]
        public void Can_create_an_event_from_log4net()
        {
            _log.Info("loggingtest");

            Client.Refresh();
            var searchResults = Client.Search(s => s.Query(q => q.Term("Message", "loggingtest")));
            
            Assert.AreEqual(1, searchResults.Hits.Total);
            var doc = searchResults.Documents.First();
            Assert.IsNull(doc["@type"]);
            Assert.IsNotNull(doc["SmartValue2"]);
        }

        [Test]
        public void Can_read_properties()
        {
            GlobalContext.Properties["globalDynamicProperty"] = "global";
            ThreadContext.Properties["threadDynamicProperty"] = "thread";
            LogicalThreadContext.Properties["logicalThreadDynamicProperty"] = "local thread";
            _log.Info("loggingtest");

            Client.Refresh();
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
            _log.Info("this is message key=value, another = 'another' object:[id=1] anotherObj:[another id=2,content=blue]");

            Client.Refresh();
            var searchResults = Client.Search(s => s.Take(1));
            
            var entry = searchResults.Documents.First();
            Assert.AreEqual("value", entry.key.ToString());
            Assert.AreEqual("another", entry.another.ToString());
            Assert.AreEqual("1", entry["object"].id.ToString());
            Assert.AreEqual("blue", entry.anotherObj.content.ToString());
            Assert.AreEqual("another id=2,content=blue", entry.anotherObj._raw.ToString());
        }

        [Test]
        public void Can_read_grok_propertis()
        {
            _log.Error("error! name is UnknownError");

            Client.Refresh();
            var res = Client.Search(s => s.Take((1)));
            var doc = res.Documents.First();
            Assert.AreEqual("UnknownError", doc.name.ToString());
        }

    }
}

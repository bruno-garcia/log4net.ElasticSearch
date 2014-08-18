using System;
using System.Linq;
using System.Threading;
using Elasticsearch.Net;
using log4net.Appender;
using log4net.ElasticSearch.Filters;
using log4net.Repository.Hierarchy;
using Nest;
using Newtonsoft.Json.Linq;
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
            string host = null, port = null;
            QueryConfiguration(appender =>
            {
                appender.IndexName = TestIndex;

                host = appender.Server;
                port = appender.Port;
            });

            ConnectionSettings elasticSettings =
                new ConnectionSettings(new Uri(string.Format("http://{0}:{1}", host, port)))
                    .SetDefaultIndex(TestIndex);

            Client = new ElasticClient(elasticSettings);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            Client.DeleteIndex(descriptor => descriptor.Index(TestIndex));
        }

        [SetUp]
        public void TestSetup()
        {
            FixtureTearDown();
            QueryConfiguration(appender =>
            {
                appender.BulkSize = 1;
                appender.BulkIdleTimeout = -1;
            });
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

            var results = Client.Index(logEvent, descriptor => descriptor.Type("anonymous"));

            Assert.IsNotNullOrEmpty(results.Id);
        }

        [Test]
        public void Can_read_inserted_record()
        {
            var logEvent = new
            {
                ClassName = "IntegrationTestClass",
                Exception = "ReadingTest"
            };

            Client.Index(logEvent, descriptor => descriptor.Type("anonymous"));
            Client.Refresh();

            var searchResults = Client.Search<dynamic>(s => s.AllTypes().MatchAll());
            Assert.AreEqual(1, searchResults.Total);
            Assert.AreEqual("ReadingTest", searchResults.Documents.First().exception.ToString());
        }

        [Test]
        public void Can_create_an_event_from_log4net()
        {
            _log.Info("loggingtest");

            Client.Refresh();

            var searchResults = Client.Search<JObject>(s => s.AllTypes().Query(q => q.Term("Message", "loggingtest")));

            Assert.AreEqual(1, searchResults.Total);
        }

        [Test]
        public void Can_add_and_remove_smart_values()
        {
            _log.Info("loggingtest");

            Client.Refresh();

            var searchResults = Client.Search<JObject>(s => s.AllTypes().Query(q => q.Term("Message", "loggingtest")));

            Assert.AreEqual(1, searchResults.Total);
            var doc = searchResults.Documents.First();
            Assert.IsNull(doc["@type"]);
            Assert.IsNotNull(doc["SmartValue2"]);
            Assert.AreEqual("the type is Special", doc["SmartValue2"].ToString());
        }

        [Test]
        public void Can_read_properties()
        {
            GlobalContext.Properties["globalDynamicProperty"] = "global";
            ThreadContext.Properties["threadDynamicProperty"] = "thread";
            LogicalThreadContext.Properties["logicalThreadDynamicProperty"] = "local thread";
            _log.Info("loggingtest");

            Client.Refresh();
            var searchResults = Client.Search<dynamic>(s => s.AllTypes().Query(q => q.Term("Message", "loggingtest")));

            Assert.AreEqual(1, searchResults.Total);
            var firstEntry = searchResults.Documents.First();
            Assert.AreEqual("global", firstEntry.globalDynamicProperty.ToString());
            Assert.AreEqual("thread", firstEntry.threadDynamicProperty.ToString());
            Assert.AreEqual("local thread", firstEntry.logicalThreadDynamicProperty.ToString());
        }

        [Test]
        [TestCase(1, new[] { ",", " " }, new[] { ":", "=" })]
        [TestCase(2, new[] { ";", " " }, new[] { "~" })]
        [TestCase(3, new[] { ";" },      new[] { "~" }, ExpectedException = typeof(Exception), ExpectedMessage = "spaces issue")]
        [TestCase(4, new[] { "\\|"," "}, new[] { "\\>" })]
        public void Can_read_KvFilter_properties(int num, string[] fieldSplit, string[] valueSplit)
        {
            ElasticAppenderFilters oldFilters = null;
            QueryConfiguration(appender =>
            {
                oldFilters = appender.ElasticFilters;
                appender.ElasticFilters = new ElasticAppenderFilters();
                appender.ElasticFilters.AddFilter(new KvFilter()
                {
                    FieldSplit = string.Join("", fieldSplit),
                    ValueSplit = string.Join("", valueSplit)
                });
                appender.ActivateOptions();
            });

            _log.InfoFormat(
                "this is message{1}key{0}value{1}another {0} 'another'{1}object{0}[this is object :)]",
                valueSplit[0].TrimStart('\\'), fieldSplit[0].TrimStart('\\'));

            Client.Refresh();
            var searchResults = Client.Search<dynamic>(s => s.AllTypes().Take(1));

            var entry = searchResults.Documents.First();

            QueryConfiguration(appender =>
            {
                appender.ElasticFilters = oldFilters;
                appender.ActivateOptions();
            });

            Assert.IsNotNull(entry.key);
            Assert.IsNotNull(entry["object"]);
            if (entry.another == null)
            {
                throw new Exception("spaces issue");
            }

            Assert.AreEqual("value", entry.key.ToString());
            Assert.AreEqual("another", entry.another.ToString());
            Assert.AreEqual("this is object :)", entry["object"].ToString());
        }

        [Test]
        public void Can_read_grok_propertis()
        {
            var newGuid = Guid.NewGuid();
            _log.Error("error! name is UnknownError and guid " + newGuid);

            Client.Refresh();
            var res = Client.Search<dynamic>(s => s.AllTypes().Take((1)));
            var doc = res.Documents.First();
            Assert.AreEqual("UnknownError", doc.name.ToString());
            Assert.AreEqual(newGuid.ToString(), doc.the_guid.ToString());
            Assert.IsNullOrEmpty(doc["0"]);
        }

        [Test]
        public void Can_convert_to_Array_filter()
        {
            _log.Info("someIds=[123, 124 ,125 , 007] anotherIds=[33]");

            Client.Refresh();

            var res = Client.Search<JObject>(s => s.AllTypes().Take((1)));
            var doc = res.Documents.First();
            Assert.AreEqual(true, doc["someIds"].HasValues);
            Assert.Contains("123", doc["someIds"].Values<string>().ToArray());
            Assert.AreEqual(true, doc["anotherIds"].HasValues);
            Assert.AreEqual("33", doc["anotherIds"].Values<string>().First());
        }

        [Test]
        public void test_ttl()
        {
            Client.PutTemplate("ttltemplate",
                descriptor =>
                    descriptor.Template("*").Settings(settings => settings.Add("indices.ttl.interval", "1s").Add("indices.ttl.bulk_size", "1"))
                        .AddMapping<dynamic>(mapping => mapping.Type("_default_").TtlField(ttl => ttl.Enable().Default("1ms"))));

            _log.Info("test");
            Thread.Sleep(2000);
            Client.Refresh();
            Client.Optimize();

            var res = Client.Search<dynamic>(s => s.AllTypes().AllIndices());
            Client.DeleteTemplate("ttltemplate");

            Assert.AreEqual(0, res.Total);
        }

        [Test]
        [Ignore("the build agent have problems on running performance")]
        public void Performance()
        {
            QueryConfiguration(appender =>
            {
                appender.BulkSize = 250;
                appender.BulkIdleTimeout = -1;
            });
            Program.Main(1, 1500);
        }

        private static void QueryConfiguration(Action<ElasticSearchAppender> action)
        {
            var hierarchy = LogManager.GetRepository() as Hierarchy;
            if (hierarchy != null)
            {
                IAppender[] appenders = hierarchy.GetAppenders();
                foreach (IAppender appender in appenders)
                {
                    var elsAppender = appender as ElasticSearchAppender;
                    if (elsAppender != null && action != null)
                    {
                        action(elsAppender);
                    }
                }
            }
        }
    }
}

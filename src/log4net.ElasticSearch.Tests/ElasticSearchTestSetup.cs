using System;
using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.SmartFormatter;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchTestSetup
    {
        public readonly ElasticClient Client;
        public readonly string TestIndex = "log_test_" + DateTime.Now.ToString("yyyy-MM-dd");

        public ElasticSearchTestSetup()
        {
            ConnectionSettings elasticSettings =
                new ConnectionSettings(new Uri("http://127.0.0.1:9200"))
                    .SetDefaultIndex(TestIndex);

            Client = new ElasticClient(elasticSettings);
        }

        public void DeleteTestIndex()
        {
            Client.DeleteIndex(TestIndex);
        }
    }
}

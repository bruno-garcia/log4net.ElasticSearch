using System;
using Nest;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchTestSetup
    {
        public readonly ElasticClient Client;
        protected readonly string _testIndex;

        public ElasticSearchTestSetup()
        {
            _testIndex = string.Format("{0}-{1}", "log_test", DateTime.Now.ToString("yyyy-MM-dd"));

            ConnectionSettings elasticSettings = new ConnectionSettings(new Uri("http://127.0.0.1:9200"))
                .SetDefaultIndex(_testIndex);
            
            Client = new ElasticClient(elasticSettings);

            Client.DeleteIndex(_testIndex);
        }

        public void DeleteTestIndex()
        {
            Client.DeleteIndex(_testIndex);
        }
    }
}

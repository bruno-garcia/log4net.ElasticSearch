using System;
using Nest;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchTestSetup
    {
        private readonly ConnectionSettings elasticSettings;
        public readonly ElasticClient client;

        public ElasticSearchTestSetup()
        {
            elasticSettings = new ConnectionSettings(new Uri("http://127.0.0.1:9200"))
                .SetDefaultIndex("log_test");
            
            client = new ElasticClient(elasticSettings);
        }

        public void SetupTestIndex()
        {
            client.DeleteIndex("log_test");
            client.CreateIndex("log_test", c => c
                                                    .NumberOfReplicas(0)
                                                    .NumberOfShards(1));
        }

        public void DeleteTestIndex()
        {
            client.DeleteIndex("log_test");
        }
    }
}

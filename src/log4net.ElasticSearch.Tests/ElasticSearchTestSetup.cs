using System;
using Nest;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchTestSetup
    {
        private readonly ConnectionSettings elasticSettings;
        public readonly ElasticClient client;
        private string testIndex;

        public ElasticSearchTestSetup()
        {
            testIndex = string.Format("{0}-{1}", "log_test", DateTime.Now.ToString("yyyy.MM.dd"));

            elasticSettings = new ConnectionSettings(new Uri("http://127.0.0.1:9200"))
                .SetDefaultIndex(testIndex);
            
            client = new ElasticClient(elasticSettings);

            client.DeleteIndex(new DeleteIndexRequest(testIndex));
        }

        public void DeleteTestIndex()
        {
            client.DeleteIndex(new DeleteIndexRequest(testIndex));
        }
    }
}

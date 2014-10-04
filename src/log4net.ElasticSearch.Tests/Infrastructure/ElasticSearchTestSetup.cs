using System;
using Nest;

namespace log4net.ElasticSearch.Tests.Infrastructure
{
    public abstract class ElasticSearchTestSetup : IDisposable
    {
        protected readonly ElasticClient Client;
        readonly string defaultIndex;

        protected ElasticSearchTestSetup()
        {
            defaultIndex = GetDefaultIndex();

            Client = new ElasticClient(GetConnectionSettings(defaultIndex));

            DeleteDefaultIndex();
        }

        public void Dispose()
        {
            DeleteDefaultIndex();
        }

        static string GetDefaultIndex()
        {
            return string.Format("{0}-{1}", "log_test", DateTime.Now.ToString("yyyy.MM.dd"));
        }

        static ConnectionSettings GetConnectionSettings(string index)
        {
            var elasticSearchUri = new Uri(string.Format("http://{0}:9200", Environment.MachineName));

            return !AppSettings.Instance.UseFiddler()
                       ? new ConnectionSettings(elasticSearchUri).SetDefaultIndex(index)
                       : new ConnectionSettings(elasticSearchUri).SetDefaultIndex(index).
                                                                  DisableAutomaticProxyDetection(false).
                                                                  SetProxy(new Uri("http://localhost:8888"), "", "");
        }

        void DeleteDefaultIndex()
        {
            Client.DeleteIndex(new DeleteIndexRequest(defaultIndex));
        }
    }
}
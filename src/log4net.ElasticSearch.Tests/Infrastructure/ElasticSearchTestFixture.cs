using System;
using Nest;

namespace log4net.ElasticSearch.Tests.Infrastructure
{
    public class ElasticSearchTestFixture : IDisposable
    {
        readonly string defaultIndex;

        public ElasticSearchTestFixture()
        {
            defaultIndex = GetDefaultIndex();

            Client = new ElasticClient(GetConnectionSettings(defaultIndex));

            DeleteDefaultIndex();
        }

        public ElasticClient Client { get; private set; }

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
            var defaultConnectionSettings = new ConnectionSettings(GetElasticSearchUri()).
                SetDefaultIndex(index).                
                SetDefaultTypeNameInferrer(t => t.Name).
                SetDefaultPropertyNameInferrer(p => p);

            return !AppSettings.Instance.UseFiddler()
                       ? defaultConnectionSettings
                       : defaultConnectionSettings.
                             DisableAutomaticProxyDetection(false).
                             SetProxy(new Uri("http://localhost:8888"), "", "");
        }

        static Uri GetElasticSearchUri()
        {
            return new Uri(string.Format("http://{0}:9200", Environment.MachineName));
        }

        void DeleteDefaultIndex()
        {
            Client.DeleteIndex(new DeleteIndexRequest(defaultIndex));
        }
    }
}
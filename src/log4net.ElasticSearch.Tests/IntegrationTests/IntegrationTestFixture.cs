using System;
using System.IO;
using System.Reflection;
using log4net.Config;
using Nest;
using Xunit;

namespace log4net.ElasticSearch.Tests.IntegrationTests
{
    public class IntegrationTestFixture : IDisposable
    {
        readonly string defaultIndex;

        public IntegrationTestFixture()
        {
            //for some reason log4net initialization using the AssemblyInfo attribute
            //is not always working as expected, so let's initialize it here
            var assembly = Assembly.GetExecutingAssembly();
            var logRepository = LogManager.GetRepository(assembly);
            var fi = new FileInfo("logConfig.xml");
            if (!fi.Exists)
            {
                throw new FileNotFoundException("Cannot find log4net configuration.");
            }
            XmlConfigurator.Configure(logRepository, fi);

            defaultIndex = GetDefaultIndex();

            Client = new ElasticClient(ConnectionSettings(defaultIndex));

            DeleteDefaultIndex();
        }

        public ElasticClient Client { get; private set; }

        public void Dispose()
        {
            DeleteDefaultIndex();            
        }

        static string GetDefaultIndex()
        {
            return string.Format("{0}", "log_test");
        }

        static ConnectionSettings ConnectionSettings(string index)
        {
            var defaultConnectionSettings = new ConnectionSettings(ElasticSearchUri()).
                DefaultIndex(index).                
                DefaultTypeNameInferrer(t => t.Name).
                DefaultFieldNameInferrer(p => p);

            return !AppSettings.Instance.UseFiddler()
                       ? defaultConnectionSettings
                       : defaultConnectionSettings.
                             DisableAutomaticProxyDetection(false).
                             Proxy(new Uri("http://localhost:8888"), "", "");
        }

        static Uri ElasticSearchUri()
        {
            return new Uri(string.Format("http://{0}:9200", "127.0.0.1".ToString()));
        }

        void DeleteDefaultIndex()
        {
            Client.DeleteIndex(new DeleteIndexRequest(defaultIndex));
        }
    }

    [CollectionDefinition("IndexCollection")]
    public class DatabaseCollection : ICollectionFixture<IntegrationTestFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
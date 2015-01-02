﻿using System;
using Nest;

namespace log4net.ElasticSearch.Tests.IntegrationTests
{
    public class IntegrationTestFixture : IDisposable
    {
        readonly string defaultIndex;

        public IntegrationTestFixture()
        {
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
            return string.Format("{0}-{1}", "log_test", DateTime.Now.ToString("yyyy.MM.dd"));
        }

        static ConnectionSettings ConnectionSettings(string index)
        {
            var defaultConnectionSettings = new ConnectionSettings(ElasticSearchUri()).
                SetDefaultIndex(index).                
                SetDefaultTypeNameInferrer(t => t.Name).
                SetDefaultPropertyNameInferrer(p => p);

            return !AppSettings.Instance.UseFiddler()
                       ? defaultConnectionSettings
                       : defaultConnectionSettings.
                             DisableAutomaticProxyDetection(false).
                             SetProxy(new Uri("http://localhost:8888"), "", "");
        }

        static Uri ElasticSearchUri()
        {
            return new Uri(string.Format("http://{0}:9200", Environment.MachineName));
        }

        void DeleteDefaultIndex()
        {
            Client.DeleteIndex(new DeleteIndexRequest(defaultIndex));
        }
    }
}
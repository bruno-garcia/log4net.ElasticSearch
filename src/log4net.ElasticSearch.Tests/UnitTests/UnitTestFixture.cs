using System;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class UnitTestFixture : IDisposable
    {
        const string ConnectionString = "Server=localhost;Index=log_test;Port=9200;rolling=true";
        const int BufferSize = 100;

        public void Initialise()
        {
            RepositoryStub = new RepositoryStub();

            Appender = new ElasticSearchAppender(s => RepositoryStub)
                {
                    Lossy = false,
                    BufferSize = BufferSize,
                    ConnectionString = ConnectionString
                };

            Appender.ActivateOptions();
        }

        public RepositoryStub RepositoryStub { get; set; }

        public ElasticSearchAppender Appender { get; set; }

        public void Dispose() {}
    }
}
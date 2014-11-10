using System;
using log4net.ElasticSearch.Tests.UnitTests.Stubs;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class UnitTestFixture : IDisposable
    {
        const string ConnectionString = "Server=localhost;Index=log_test;Port=9200;rolling=true";
        const int BufferSize = 100;

        public void SetUp(bool? failSend = null, bool? failClose = null)
        {
            Repository = new RepositoryStub();
            ErrorHandler = new ErrorHandlerStub();

            Appender = new TestableAppender(Repository)
                {
                    Lossy = false,
                    BufferSize = BufferSize,
                    ConnectionString = ConnectionString,
                    ErrorHandler = ErrorHandler,
                    FailSend = failSend,
                    FailClose = failClose
                };

            Appender.ActivateOptions();
        }

        public ElasticSearchAppender Appender { get; private set; }

        public RepositoryStub Repository { get; private set; }

        public ErrorHandlerStub ErrorHandler { get; private set; }

        public void Dispose() {}
    }
}
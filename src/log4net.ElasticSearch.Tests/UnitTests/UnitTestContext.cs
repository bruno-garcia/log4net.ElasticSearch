using System;
using log4net.ElasticSearch.Tests.UnitTests.Stubs;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class UnitTestContext : IDisposable
    {
        const string ConnectionString = "Server=localhost;Index=log_test;Port=9200;rolling=true";
        const int BufferSize = 100;

        public ElasticSearchAppender Appender { get; private set; }

        public RepositoryStub Repository { get; private set; }

        public ErrorHandlerStub ErrorHandler { get; private set; }

        public void Dispose()
        {
            if (Appender == null) return;

            try
            {
                Appender.Flush();
            }
            catch{}
        }

        public static UnitTestContext Create(int bufferSize = BufferSize, bool? failSend = null, bool? failClose = null)
        {
            var repository = new RepositoryStub();
            var errorHandler = new ErrorHandlerStub();

            var appender = new TestableAppender(repository)
            {
                Lossy = false,
                BufferSize = bufferSize,
                ConnectionString = ConnectionString,
                ErrorHandler = errorHandler,
                FailSend = failSend,
                FailClose = failClose,
                RollingIndexNameDateFormat = "yyyy-MM-dd"
            };

            appender.AddFieldNameOverride(new FieldNameOverride
                {
                    Original = "timetamp",
                    Replacement = "timestamp"
                }
            );

            appender.AddFieldValueReplica(new FieldValueReplica
                {
                    Original = "timeStamp",
                    Replica = "@timestamp"
                }
            );

            appender.ActivateOptions();

            return new UnitTestContext
                {
                    Repository = repository,
                    ErrorHandler = errorHandler,
                    Appender = appender
                };
        }
    }
}
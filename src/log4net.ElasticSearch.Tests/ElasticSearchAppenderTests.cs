using System.Linq;
using FluentAssertions;
using Nest;
using Xunit;
using Xunit.Sdk;
using log4net.ElasticSearch.Tests.Infrastructure;
using LogEvent = log4net.ElasticSearch.Models.LogEvent;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchAppenderTests : IUseFixture<ElasticSearchFixture>
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ElasticSearchAppenderTests));

        private ElasticClient elasticClient;

        public void SetFixture(ElasticSearchFixture fixture)
        {
            elasticClient = fixture.Client;
        }

        [Fact]
        public void Can_create_an_event_from_log4net()
        {
            var message = Faker.Lorem.Words(1).Single();

            _log.Info(message);

            Retry.Ignoring<AssertException>(() =>
            {
                var logEntries =
                    elasticClient.Search<LogEvent>(s => s.Query(qd => qd.Term(le => le.Message, message)));

                logEntries.Total.Should().Be(1);
            });
        }

        [Fact]
        public void Global_context_properties_are_logged()
        {
            const string globalPropertyName = "globalProperty";

            var globalProperty = Faker.Lorem.Sentence(2);
            var message = Faker.Lorem.Words(1).Single();

            GlobalContext.Properties[globalPropertyName] = globalProperty;

            _log.Info(message);

            Retry.Ignoring<AssertException>(() =>
                {
                    var logEntries =
                        elasticClient.Search<LogEvent>(sd => sd.Query(qd => qd.Term(le => le.Message, message)));

                    logEntries.Total.Should().Be(1);

                    var actualLogEntry = logEntries.Documents.First();

                    actualLogEntry.Properties[globalPropertyName].Should().Be(globalProperty);
                });
        }

        [Fact]
        public void Thread_context_properties_are_logged()
        {
            const string threadPropertyName = "threadProperty";

            var threadProperty = Faker.Lorem.Sentence(2);
            var message = Faker.Lorem.Words(1).Single();

            ThreadContext.Properties[threadPropertyName] = threadProperty;

            _log.Info(message);

            Retry.Ignoring<AssertException>(() =>
                {
                    var logEntries =
                        elasticClient.Search<LogEvent>(sd => sd.Query(qd => qd.Term(le => le.Message, message)));

                    logEntries.Total.Should().Be(1);

                    var actualLogEntry = logEntries.Documents.First();

                    actualLogEntry.Properties[threadPropertyName].Should().Be(threadProperty);
                });
        }

        [Fact(Skip = "LogicalThreadContext properties cause SerializationException")]
        public void Local_thread_context_properties_cause_error()
        {
            const string localThreadPropertyName = "logicalThreadProperty";

            var localTreadProperty = Faker.Lorem.Sentence(2);
            var message = Faker.Lorem.Words(1).Single();

            LogicalThreadContext.Properties[localThreadPropertyName] = localTreadProperty;

            _log.Info(message);

            Retry.Ignoring<AssertException>(() =>
                {
                    var logEntries =
                        elasticClient.Search<LogEvent>(sd => sd.Query(qd => qd.Term(le => le.Message, message)));

                logEntries.Total.Should().Be(1);

                var actualLogEntry = logEntries.Documents.First();

                actualLogEntry.Properties[localThreadPropertyName].Should().Be(localTreadProperty);
            });
        }

    }
}

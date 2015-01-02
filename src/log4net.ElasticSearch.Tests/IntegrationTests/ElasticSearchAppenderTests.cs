using System;
using System.Linq;
using FluentAssertions;
using Nest;
using Xunit;
using Xunit.Sdk;
using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.Tests.Infrastructure;

namespace log4net.ElasticSearch.Tests.IntegrationTests
{
    public class ElasticSearchAppenderTests : IUseFixture<IntegrationTestFixture>
    {
        static readonly ILog _log = LogManager.GetLogger(typeof(ElasticSearchAppenderTests));

        ElasticClient elasticClient;

        public void SetFixture(IntegrationTestFixture fixture)
        {
            elasticClient = fixture.Client;
        }

        [Fact]
        public void Can_create_an_event_from_log4net()
        {
            var message = Faker.Lorem.Words(1).Single();

            _log.Info(message, new ApplicationException(Faker.Lorem.Words(1).Single()));

            Retry.Ignoring<AssertException>(() =>
            {
                var logEntries =
                    elasticClient.Search<logEvent>(s => s.Query(qd => qd.Term(le => le.message, message)));

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
                        elasticClient.Search<logEvent>(sd => sd.Query(qd => qd.Term(le => le.message, message)));

                    logEntries.Total.Should().Be(1);

                    var actualLogEntry = logEntries.Documents.First();

                    actualLogEntry.properties[globalPropertyName].Should().Be(globalProperty);
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
                        elasticClient.Search<logEvent>(sd => sd.Query(qd => qd.Term(le => le.message, message)));

                    logEntries.Total.Should().Be(1);

                    var actualLogEntry = logEntries.Documents.First();

                    actualLogEntry.properties[threadPropertyName].Should().Be(threadProperty);
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
                        elasticClient.Search<logEvent>(sd => sd.Query(qd => qd.Term(le => le.message, message)));

                logEntries.Total.Should().Be(1);

                var actualLogEntry = logEntries.Documents.First();

                actualLogEntry.properties[localThreadPropertyName].Should().Be(localTreadProperty);
            });
        }

    }
}

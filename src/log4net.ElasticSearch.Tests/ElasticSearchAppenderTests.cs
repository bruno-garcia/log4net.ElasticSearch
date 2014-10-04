using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchAppenderTests : ElasticSearchTestSetup
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ElasticSearchAppenderTests));

        [Fact]
        public void Global_context_properties_are_logged()
        {
            const string globalPropertyName = "globalProperty";

            var globalProperty = Faker.Lorem.Sentence(2);
            var message = Faker.Lorem.Words(1).First();

            GlobalContext.Properties[globalPropertyName] = globalProperty;

            _log.Info(message);

            Retry.Ignoring<AssertException>(() =>
                {
                    var searchResults = Client.Search<LogEvent>(s => s.Query(q => q.Term(@event => @event.Message, message)));

                    searchResults.Total.Should().Be(1);

                    var firstEntry = searchResults.Documents.First();

                    firstEntry.Properties[globalPropertyName].Should().Be(globalProperty);
                });
        }

        [Fact]
        public void Thread_context_properties_are_logged()
        {
            const string threadPropertyName = "threadProperty";

            var threadProperty = Faker.Lorem.Sentence(2);
            var message = Faker.Lorem.Words(1).First();

            ThreadContext.Properties[threadPropertyName] = threadProperty;

            _log.Info(message);

            Retry.Ignoring<AssertException>(() =>
                {
                    var searchResults = Client.Search<LogEvent>(s => s.Query(q => q.Term(@event => @event.Message, message)));

                    searchResults.Total.Should().Be(1);

                    var firstEntry = searchResults.Documents.First();

                    firstEntry.Properties[threadPropertyName].Should().Be(threadProperty);
                });
        }

        [Fact(Skip = "LogicalThreadContext properties cause SerializationException")]
        public void Local_thread_context_properties_cause_error()
        {
            const string localThreadPropertyName = "logicalThreadProperty";
            var localTreadProperty = Faker.Lorem.Sentence(2);
            var message = Faker.Lorem.Words(1).First();

            LogicalThreadContext.Properties[localThreadPropertyName] = localTreadProperty;

            _log.Info(message);
        }

        [Fact]
        public void Can_create_an_event_from_log4net()
        {
            var message = Faker.Lorem.Words(1).First();
            _log.Info(message);

            Retry.Ignoring<AssertException>(() =>
                {
                    var searchResults =
                        Client.Search<LogEvent>(s => s.Query(q => q.Term(@event => @event.Message, message)));

                    searchResults.Total.Should().Be(1);                                
                });
        }
    }
}

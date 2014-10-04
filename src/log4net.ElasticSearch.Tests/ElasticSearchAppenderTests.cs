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
        public void Can_read_properties()
        {
            const string globalPropertyName = "globalProperty";
            const string threadPropertyName = "threadProperty";
            const string localThreadPropertyName = "logicalThreadProperty";

            var globalProperty = Faker.Lorem.Sentence(2);
            var threadProperty = Faker.Lorem.Sentence(2);
            var localTreadProperty = Faker.Lorem.Sentence(2);
            var message = Faker.Lorem.Words(1).First();

            GlobalContext.Properties[globalPropertyName] = globalProperty;
            ThreadContext.Properties[threadPropertyName] = threadProperty;
//            LogicalThreadContext.Properties[localThreadPropertyName] = localTreadProperty;

            _log.Info(message);

            Retry.Ignoring<AssertException>(() =>
                {
                    var searchResults = client.Search<LogEvent>(s => s.Query(q => q.Term(@event => @event.Message, message)));

                    searchResults.Total.Should().Be(1);

                    var firstEntry = searchResults.Documents.First();

                    firstEntry.Properties[globalPropertyName].Should().Be(globalProperty);
                    firstEntry.Properties[threadPropertyName].Should().Be(threadProperty);
//                    firstEntry.Properties["logicalThreadDynamicProperty"].Should().Be(localTreadProperty);
                });
        }

        [Fact]
        public void Can_create_an_event_from_log4net()
        {
            var message = Faker.Lorem.Words(1).First();
            _log.Info(message);

            Retry.Ignoring<AssertException>(() =>
                {
                    var searchResults =
                        client.Search<LogEvent>(s => s.Query(q => q.Term(@event => @event.Message, message)));

                    searchResults.Total.Should().Be(1);                                
                });
        }
    }
}

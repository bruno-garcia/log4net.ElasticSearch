using FluentAssertions;
using Xunit;
using Xunit.Sdk;
using log4net.ElasticSearch.Tests.Infrastructure;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchTests : ElasticSearchTestSetup
    {
        [Fact]
        public void Can_insert_record()
        {
            var results = Client.Index(LogEventBuilder.Default.LogEvent);

            results.Id.Should().NotBeNull();
        }

        [Fact]
        public void Can_read_inserted_record()
        {
            var logEvent = LogEventBuilder.Default.LogEvent;

            Client.Index(logEvent);            

            Retry.Ignoring<AssertException>(() =>
                {
                    var searchResults =
                        Client.Search<LogEvent>(
                            s => s.Query(q => q.Term(@event => @event.ClassName, logEvent.ClassName)));

                    searchResults.Total.Should().Be(1);                    
                });
        }

    }
}
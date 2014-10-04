using FluentAssertions;
using Nest;
using Xunit;
using Xunit.Sdk;
using log4net.ElasticSearch.Tests.Infrastructure;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchTests : IUseFixture<ElasticSearchTestSetup>
    {
        private ElasticClient elasticClient;

        public void SetFixture(ElasticSearchTestSetup fixture)
        {
            elasticClient = fixture.Client;
        }

        [Fact]
        public void Can_insert_record()
        {
            var results = elasticClient.Index(LogEventBuilder.Default.LogEvent);

            results.Id.Should().NotBeNull();
        }

        [Fact]
        public void Can_read_inserted_record()
        {
            var logEvent = LogEventBuilder.Default.LogEvent;

            elasticClient.Index(logEvent);            

            Retry.Ignoring<AssertException>(() =>
                {
                    var searchResults =
                        elasticClient.Search<LogEvent>(
                            s => s.Query(q => q.Term(@event => @event.ClassName, logEvent.ClassName)));

                    searchResults.Total.Should().Be(1);                    
                });
        }

    }
}
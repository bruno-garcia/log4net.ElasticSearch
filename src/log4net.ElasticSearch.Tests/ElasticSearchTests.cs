using FluentAssertions;
using Nest;
using Xunit;
using Xunit.Sdk;
using log4net.ElasticSearch.Tests.Infrastructure;
using logEvent = log4net.ElasticSearch.Models.logEvent;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchTests : IUseFixture<ElasticSearchFixture>
    {
        private ElasticClient elasticClient;

        public void SetFixture(ElasticSearchFixture fixture)
        {
            elasticClient = fixture.Client;
        }

        [Fact]
        public void Can_insert_record()
        {
            var indexResponse = elasticClient.Index(Infrastructure.LogEventBuilder.Default.LogEvent);

            indexResponse.Id.Should().NotBeNull();
        }

        [Fact]
        public void Can_read_indexed_document()
        {
            var logEvent = Infrastructure.LogEventBuilder.Default.LogEvent;

            elasticClient.Index(logEvent);            

            Retry.Ignoring<AssertException>(() =>
                {
                    var logEntries =
                        elasticClient.Search<logEvent>(
                            sd => sd.Query(qd => qd.Term(le => le.className, logEvent.className)));

                    logEntries.Total.Should().Be(1);                    
                });
        }

    }
}
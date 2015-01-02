using FluentAssertions;
using Nest;
using Xunit;
using Xunit.Sdk;
using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.Tests.Infrastructure;
using log4net.ElasticSearch.Tests.Infrastructure.Builders;

namespace log4net.ElasticSearch.Tests.IntegrationTests
{
    public class ElasticSearchTests : IUseFixture<IntegrationTestFixture>
    {
        ElasticClient elasticClient;

        public void SetFixture(IntegrationTestFixture fixture)
        {
            elasticClient = fixture.Client;
        }

        [Fact]
        public void Can_insert_record()
        {
            var indexResponse = elasticClient.Index(LogEventBuilder.Default.LogEvent);

            indexResponse.Id.Should().NotBeNull();
        }

        [Fact]
        public void Can_read_indexed_document()
        {
            var logEvent = LogEventBuilder.Default.LogEvent;

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
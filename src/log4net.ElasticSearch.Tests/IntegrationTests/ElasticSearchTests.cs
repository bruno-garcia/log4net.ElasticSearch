using FluentAssertions;
using Nest;
using Xunit;
using Xunit.Sdk;
using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.Tests.Infrastructure;
using log4net.ElasticSearch.Tests.Infrastructure.Builders;

namespace log4net.ElasticSearch.Tests.IntegrationTests
{
    [Collection("IndexCollection")]
    public class ElasticSearchTests
    {
        private ElasticClient elasticClient;
        private IntegrationTestFixture testFixture;

        public ElasticSearchTests(IntegrationTestFixture testFixture)
        {
            this.testFixture = testFixture;
            elasticClient = testFixture.Client;
        }

        [Fact]
        public void Can_insert_record()
        {
            var indexRequest = new IndexRequest<logEvent>(LogEventBuilder.Default.LogEvent);
            var indexResponse = elasticClient.Index(indexRequest);

            indexResponse.Id.Should().NotBeNull();
        }

        [Fact]
        public void Can_read_indexed_document()
        {
            var logEvent = LogEventBuilder.Default.LogEvent;
            var indexRequest = new IndexRequest<logEvent>(logEvent);
            elasticClient.Index(indexRequest);    

            Retry.Ignoring<XunitException>(() =>
                {
                    var logEntries =
                        elasticClient.Search<logEvent>(
                            sd => sd.Query(qd => qd.Term(le => le.className, logEvent.className)));

                    logEntries.Total.Should().Be(1);                    
                });
        }

    }
}
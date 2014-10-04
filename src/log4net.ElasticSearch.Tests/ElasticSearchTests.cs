using FluentAssertions;
using Nest;
using Xunit;
using Xunit.Sdk;
using log4net.ElasticSearch.Tests.Infrastructure;
using LogEvent = log4net.ElasticSearch.Models.LogEvent;

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
                        elasticClient.Search<LogEvent>(
                            sd => sd.Query(qd => qd.Term(le => le.ClassName, logEvent.ClassName)));

                    logEntries.Total.Should().Be(1);                    
                });
        }

    }
}
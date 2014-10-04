using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchTests : ElasticSearchTestSetup
    {
        [Fact]
        public void Can_insert_record()
        {
            var results = client.Index(LogEventBuilder.Default.LogEvent);

            results.Id.Should().NotBeNull();
        }

        [Fact]
        public void Can_read_inserted_record()
        {
            var logEvent = LogEventBuilder.Default.LogEvent;

            client.Index(logEvent);            

            Retry.Ignoring<AssertException>(() =>
                {
                    var searchResults = client.Search<LogEvent>(s => s.Query(q => q.Term("ClassName", logEvent.ClassName)));

                    searchResults.Total.Should().Be(1);                    
                });
        }

    }
}
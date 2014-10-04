using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;

namespace log4net.ElasticSearch.Tests
{
    public class ElasticSearchAppenderTests : ElasticSearchTestSetup
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ElasticSearchAppenderTests));

        [Fact(Skip = "xunit weirdness")]
        public void Can_read_properties()
        {
            const string globalProperty = "global";
            const string threadProperty = "thread";
            const string localTreadProperty = "local thread";

            GlobalContext.Properties["globalDynamicProperty"] = globalProperty;
            ThreadContext.Properties["threadDynamicProperty"] = threadProperty;
            LogicalThreadContext.Properties["logicalThreadDynamicProperty"] = localTreadProperty;

            _log.Info("loggingtest");

            Retry.Ignoring<AssertException>(() =>
                {
                    var searchResults = client.Search<dynamic>(s => s.Query(q => q.Term("message", "loggingtest")));

                    searchResults.Total.Should().Be(1);

                    var firstEntry = searchResults.Documents.First();

                    ((string)firstEntry.globalDynamicProperty.ToString()).Should().Be(globalProperty);
                    ((string)firstEntry.threadDynamicProperty.ToString()).Should().Be(threadProperty);
                    ((string)firstEntry.logicalThreadDynamicProperty.ToString()).Should().Be(localTreadProperty);                    
                });
        }

        [Fact]
        public void Can_create_an_event_from_log4net()
        {
            const string message = "loggingtest";
            _log.Info(message);

            Retry.Ignoring<AssertException>(() =>
                {
                    var searchResults = client.Search<LogEvent>(s => s.Query(q => q.Term("Message", message)));

                    searchResults.Total.Should().Be(1);                                
                });
        }
    }
}

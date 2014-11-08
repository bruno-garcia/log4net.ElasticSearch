using System.Diagnostics;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;
using log4net.ElasticSearch.Tests.Infrastructure;
using log4net.ElasticSearch.Tests.Infrastructure.Builders;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class ElasticSearchAppenderTests : IUseFixture<UnitTestFixture>
    {
        UnitTestFixture fixture;

        [Fact]
        public void When_number_of_LogEvents_is_less_than_Buffer_nothing_is_sent_to_ElasticSearch()
        {
            fixture.Initialise();

            fixture.Appender.DoAppend(LoggingEventsBuilder.LessThan(fixture.Appender.BufferSize).ToArray());

            fixture.RepositoryStub.LogEntries.TotalCount()
                          .Should()
                          .Be(0, "nothing should be logged when the buffer limit hasn't been reached");
        }

        [Fact]
        public void When_number_of_LogEvents_equals_Buffer_nothing_is_sent_to_ElasticSearch()
        {
            fixture.Initialise();

            fixture.Appender.DoAppend(LoggingEventsBuilder.OfSize(fixture.Appender.BufferSize).ToArray());

            Retry.Ignoring<AssertException>(() => fixture.RepositoryStub.LogEntries.TotalCount()
                                                                .Should()
                                                                .Be(0, "nothing should be logged when the buffer limit hasn't been exceeded"));
        } 

        [Fact]
        public void When_number_of_LogEvents_exceeds_Buffer_by_1_then_Buffer_is_sent_to_ElasticSearch()
        {
            fixture.Initialise();
            
            var loggingEvents = LoggingEventsBuilder.OfSize(fixture.Appender.BufferSize + 1).ToArray();         

            fixture.Appender.DoAppend(loggingEvents);

            Retry.Ignoring<AssertException>(() => fixture.RepositoryStub.LogEntries.TotalCount()
                                                                .Should()
                                                                .Be(loggingEvents.Count(), "buffer should be sent to ElasticSearch"));
        }

        [Fact]
        public void When_number_of_LogEvents_greatly_exceeds_Buffer_then_Buffer_is_sent_to_ElasticSearch()
        {
            fixture.Initialise();

            var loggingEvents = LoggingEventsBuilder.GreaterThan(fixture.Appender.BufferSize + 1).ToArray();         

            fixture.Appender.DoAppend(loggingEvents);

            Retry.Ignoring<AssertException>(() => fixture.RepositoryStub.LogEntries.TotalCount()
                                                                .Should()
                                                                .Be(fixture.Appender.BufferSize + 1, "buffer should be sent to ElasticSearch"));
        }

        [Fact]
        public void When_number_of_LogEvents_greatly_exceeds_Buffer_then_remaining_entries_are_sent_to_ElasticSearch_when_Appender_closes()
        {
            fixture.Initialise();

            var loggingEvents = LoggingEventsBuilder.GreaterThan(fixture.Appender.BufferSize + 1).ToArray();
            
            fixture.Appender.DoAppend(loggingEvents);
            fixture.Appender.Close();

            Retry.Ignoring<AssertException>(() => fixture.RepositoryStub.LogEntries.TotalCount()
                                                                .Should()
                                                                .Be(loggingEvents.Count(), "all events should be logged by the time the buffer closes"));
        }

        public void SetFixture(UnitTestFixture data)
        {
            fixture = data;
        }
    }
}
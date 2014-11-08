using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.Tests.Infrastructure.Builders;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class ElasticSearchAppenderTests
    {
        [Fact]
        public void When_number_of_LogEvents_is_less_than_Buffer_nothing_is_sent_to_ElasticSearch()
        {
            const int bufferSize = 10;

            var repositoryStub = new RepositoryStub();

            var elasticSearchAppender = new ElasticSearchAppender(s => repositoryStub)
                {
                    BufferSize = bufferSize
                };

            LoggingEventsBuilder.LessThan(bufferSize).Do(@event => elasticSearchAppender.DoAppend(@event));

            repositoryStub.LogEntries.Any()
                          .Should()
                          .BeFalse("nothing should be logged when the buffer limit hasn't been reached");
        } 
    }

    public class RepositoryStub : IRepository
    {
        readonly ConcurrentBag<IEnumerable<logEvent>> logEntries;

        public RepositoryStub()
        {
            logEntries = new ConcurrentBag<IEnumerable<logEvent>>();
        }

        public void Add(IEnumerable<logEvent> logEvents)
        {
            logEntries.Add(logEvents);
        }

        public IEnumerable<IEnumerable<logEvent>> LogEntries { get{ return logEntries; } }        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Core;

namespace log4net.ElasticSearch.Tests.Infrastructure.Builders
{
    public static class LoggingEventsBuilder
    {
        private static readonly Random Random = new Random();

        public static IEnumerable<LoggingEvent> LessThan(int buffer)
        {
            return OfSize(Random.Next(buffer));
        }

        static IEnumerable<LoggingEvent> OfSize(int size)
        {
            return Enumerable.Range(0, size - 1).Select(i => NewEvent);
        }

        static LoggingEvent NewEvent
        {
            get { return new LoggingEvent(new LoggingEventData()); }
        }

    }
}
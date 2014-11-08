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
            return OfSize(Random.Next(1, buffer));
        }

        public static IEnumerable<LoggingEvent> OfSize(int size)
        {
            return Enumerable.Range(0, size).Select(i => NewEvent);
        }

        public static IEnumerable<LoggingEvent> GreaterThan(int buffer)
        {
            return OfSize(Random.Next(buffer, buffer + 10));
        }

        public static IEnumerable<LoggingEvent> MultiplesOf(int size)
        {
            return OfSize(size * Random.Next(2, 10));
        }

        static LoggingEvent NewEvent
        {
            get { return new LoggingEvent(new LoggingEventData()); }
        }

    }
}
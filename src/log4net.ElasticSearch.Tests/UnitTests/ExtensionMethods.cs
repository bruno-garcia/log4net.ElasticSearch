using System.Collections.Generic;
using System.Linq;
using log4net.Appender;
using log4net.Core;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public static class ExtensionMethods
    {
        public static int TotalCount<T>(this IEnumerable<IEnumerable<T>> self)
        {
            return self.Sum(inner => inner.Count());
        }

        public static void AppendAndClose(this IBulkAppender self, IEnumerable<LoggingEvent> events)
        {
            self.DoAppend(events.ToArray());
            self.Close();
        }

        public static T As<T>(this object self)
        {
            return (T) self;
        }

        public static T Second<T>(this IEnumerable<T> self)
        {
            return self.Skip(1).First();
        }
    }
}
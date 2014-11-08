using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class RepositoryStub : IRepository
    {
        readonly ConcurrentBag<IEnumerable<logEvent>> logEntries;
        readonly ConcurrentDictionary<int, IEnumerable<logEvent>> logEntriesByThread;

        public RepositoryStub()
        {
            logEntries = new ConcurrentBag<IEnumerable<logEvent>>();
            logEntriesByThread = new ConcurrentDictionary<int, IEnumerable<logEvent>>();
        }

        public void Add(IEnumerable<logEvent> logEvents)
        {
            var entries = logEvents.ToArray();
            logEntries.Add(entries);
            logEntriesByThread.AddOrUpdate(Thread.CurrentThread.ManagedThreadId, i => entries,
                                           (i, events) => events.Union(entries));
        }

        public IEnumerable<IEnumerable<logEvent>> LogEntries { get{ return logEntries; } }
        public IDictionary<int, IEnumerable<logEvent>> LogEntriesByThread { get { return logEntriesByThread; } }        
    }
}
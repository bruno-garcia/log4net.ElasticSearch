using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch.Tests.UnitTests
{
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

            Console.WriteLine("Added {0} entries", logEvents.Count());
        }

        public IEnumerable<IEnumerable<logEvent>> LogEntries { get{ return logEntries; } }        
    }
}
using System;
using System.Collections.Generic;
using log4net.ElasticSearch.Infrastructure;

namespace log4net.ElasticSearch.Tests.UnitTests.Stubs
{
    public class HttpClientStub : IHttpClient
    {
        readonly Action action;
        readonly IDictionary<Uri, IList<object>> items;

        public HttpClientStub(Action action)
        {
            this.action = action;

            items = new Dictionary<Uri, IList<object>>();
        }

        public void Post<T>(Uri uri, T item)
        {
            if (!items.ContainsKey(uri))
            {
                items[uri] = new List<object>();
            }
            items[uri].Add(item);

            action();
        }

        public void PostBulk<T>(Uri uri, T items)
        {

        }

        public IEnumerable<KeyValuePair<Uri, IList<object>>> Items { get { return items; } }
    }
}
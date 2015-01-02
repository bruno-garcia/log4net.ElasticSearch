using System.Collections.Generic;

namespace log4net.ElasticSearch
{
    public static class Pair
    {
        public static KeyValuePair<TKey, TValue> For<TKey, TValue>(TKey key, TValue value)
        {
            return new KeyValuePair<TKey, TValue>(key, value);
        }
    }
}
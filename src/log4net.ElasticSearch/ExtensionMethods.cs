using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using log4net.Core;
using log4net.ElasticSearch.Infrastructure;
using log4net.Util;
using Newtonsoft.Json;

namespace log4net.ElasticSearch
{
    public static class ExtensionMethods
    {
        public static void Do<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self)
            {
                action(item);
            }
        }

        public static string With(this string self, params object[] args)
        {
            return string.Format(self, args);
        }

        public static IEnumerable<KeyValuePair<string, string>> Properties(this LoggingEvent self)
        {
            return self.GetProperties().AsPairs();
        }

        public static string ToJson<T>(this T self, CustomDataContractResolver resolver)
        {
            var json = JsonConvert.SerializeObject(self, new JsonSerializerSettings { ContractResolver = resolver });

            if (resolver.FieldValueReplica != null && resolver.FieldValueReplica.Count > 0)
            {
                resolver.FieldValueReplica.ForEach(x =>
                {
                    var originalValue = self.GetType().GetProperty(x.Original)?.GetValue(self, null);
                    var obj = JsonConvert.DeserializeObject<ExpandoObject>(json) as IDictionary<string, Object>;
                    obj.Add(x.Replica, originalValue);
                    json = JsonConvert.SerializeObject(obj);
                });
            }
            
            return json;
        }

        public static bool Contains(this StringDictionary self, string key)
        {
            return self.ContainsKey(key) && !self[key].IsNullOrEmpty();
        }

        public static bool ToBool(this string self)
        {
            return bool.Parse(self);
        }

        /// <summary>
        /// Take the full connection string and break it into is constituent parts
        /// </summary>
        /// <param name="self">The connection string itself</param>
        /// <returns>A dictionary of all the parts</returns>
        public static StringDictionary ConnectionStringParts(this string self)
        {
            var builder = new DbConnectionStringBuilder
            {
                ConnectionString = self//.Replace("{", "\"").Replace("}", "\"")
            };

            var parts = new StringDictionary();
            foreach (string key in builder.Keys)
            {
                parts[key] = Convert.ToString(builder[key]);
            }
            return parts;
        }

        static IEnumerable<KeyValuePair<string, string>> AsPairs(this ReadOnlyPropertiesDictionary self)
        {
            return self.GetKeys().Select(key => Pair.For(key, self[key].ToStringOrNull()));
        }

        static string ToStringOrNull(this object self)
        {
            return self != null ? self.ToString() : null;
        }

        static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;
using log4net.Core;
using log4net.Util;

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

        public static string ToJson<T>(this T self)
        {
            return new JavaScriptSerializer().Serialize(self);
        }

        static IEnumerable<KeyValuePair<string, string>> AsPairs(this ReadOnlyPropertiesDictionary self)
        {
            return self.GetKeys().Select(key => Pair.For(key, self[key].ToStringOrNull()));
        }

        static string ToStringOrNull(this object self)
        {
            return self != null ? self.ToString() : null;
        }
    }
}
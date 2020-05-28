using System;
using System.Collections.Specialized;
using log4net.ElasticSearch.Infrastructure;

#if NETFRAMEWORK
using System.Web;
#else
    using System.Net;
#endif          

namespace log4net.ElasticSearch.Models
{
    public class Uri
    {
        private static string typeName;
        private static string indexNameDateFormat;

        readonly StringDictionary connectionStringParts;

        static Uri()
        {
            typeName = "logEvent";
            indexNameDateFormat = "yyyy.MM.dd";
        }

        public static void Init(string rollingIndexNameDateFormat, string indexTypeName)
        {
            typeName = string.IsNullOrWhiteSpace(indexTypeName)
                ? string.Empty
                : $"/{indexTypeName.Trim()}";

            indexNameDateFormat = rollingIndexNameDateFormat;
        }

        Uri(StringDictionary connectionStringParts)
        {
            this.connectionStringParts = connectionStringParts;
        }

        public static implicit operator System.Uri(Uri uri)
        {
            if (!string.IsNullOrWhiteSpace(uri.User()) && !string.IsNullOrWhiteSpace(uri.Password()))
            {
#if NETFRAMEWORK
                var user = HttpUtility.UrlEncode(uri.User());
                var password = HttpUtility.UrlEncode(uri.Password());
#else
                    var user = WebUtility.UrlEncode(uri.User());
                    var password = WebUtility.UrlEncode(uri.Password());
#endif
                return
                    new System.Uri($"{uri.Scheme()}://{user}:{password}@{uri.Server()}:{uri.Port()}/{uri.Index()}{typeName}{uri.Routing()}{uri.Bulk()}");
            }

            return string.IsNullOrEmpty(uri.Port())
                ? new System.Uri($"{uri.Scheme()}://{uri.Server()}/{uri.Index()}{typeName}{uri.Routing()}{uri.Bulk()}")
                : new System.Uri($"{uri.Scheme()}://{uri.Server()}:{uri.Port()}/{uri.Index()}{typeName}{uri.Routing()}{uri.Bulk()}");
        }

        public static Uri For(string connectionString)
        {
            return new Uri(connectionString.ConnectionStringParts());
        }

        string User()
        {
            return connectionStringParts[Keys.User];
        }

        string Password()
        {
            return connectionStringParts[Keys.Password];
        }

        string Scheme()
        {
            return connectionStringParts[Keys.Scheme] ?? "http";
        }

        string Server()
        {
            return connectionStringParts[Keys.Server];
        }

        string Port()
        {
            return connectionStringParts[Keys.Port];
        }

        string Routing()
        {
            var routing = connectionStringParts[Keys.Routing];
            if (!string.IsNullOrWhiteSpace(routing))
            {
                return string.Format("?routing={0}", routing);
            }

            return string.Empty;
        }

        string Bulk()
        {
            var bufferSize = connectionStringParts[Keys.BufferSize];
            if (Convert.ToInt32(bufferSize) > 1)
            {
                return "/_bulk";
            }

            return string.Empty;
        }

        string Index()
        {
            var index = connectionStringParts[Keys.Index];

            return IsRollingIndex(connectionStringParts)
                       ? "{0}-{1}".With(index, Clock.Date.ToString(indexNameDateFormat))
                       : index;
        }

        static bool IsRollingIndex(StringDictionary parts)
        {
            return parts.Contains(Keys.Rolling) && parts[Keys.Rolling].ToBool();
        }

        private static class Keys
        {
            public const string Scheme = "Scheme";
            public const string User = "User";
            public const string Password = "Pwd";
            public const string Server = "Server";
            public const string Port = "Port";
            public const string Index = "Index";
            public const string Rolling = "Rolling";
            public const string BufferSize = "BufferSize";
            public const string Routing = "Routing";
        }
    }
}

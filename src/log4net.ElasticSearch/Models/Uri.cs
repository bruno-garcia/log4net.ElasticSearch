using System;
using System.Collections.Specialized;
using System.Data.Common;

namespace log4net.ElasticSearch.Models
{
    public class Uri
    {
        readonly string index;
        readonly string port;
        readonly string server;

        readonly string scheme;
        readonly string user;
        readonly string password;

        Uri(string server, string port, string index, string scheme, string user, string password)
        {
            this.server = server;
            this.port = port;
            this.index = index;

            this.scheme = scheme;
            this.user = user;
            this.password = password;
        }

        public static implicit operator System.Uri(Uri uri)
        {
            if (string.IsNullOrWhiteSpace(uri.user) || string.IsNullOrWhiteSpace(uri.password))
            {
                return new System.Uri(string.Format("{0}://{1}:{2}/{3}/logEvent", uri.scheme, uri.server, uri.port, uri.index));    
            }

            return new System.Uri(string.Format("{0}://{1}:{2}@{3}:{4}/{5}/logEvent", uri.scheme, uri.user, uri.password, uri.server, uri.port, uri.index));
        }

        public static Uri Create(string connectionString)
        {
            try
            {
                var builder = new DbConnectionStringBuilder
                {
                    ConnectionString = connectionString.Replace("{", "\"").Replace("}", "\"")
                };

                var lookup = new StringDictionary();
                foreach (string key in builder.Keys)
                {
                    lookup[key] = Convert.ToString(builder[key]);
                }

                var index = lookup["Index"];

                if (!string.IsNullOrEmpty(lookup["rolling"]))
                {
                    if (lookup["rolling"] == "true")
                    {
                        index = string.Format("{0}-{1}", index, DateTime.Now.ToString("yyyy.MM.dd"));
                    }
                }

                var scheme = lookup["Scheme"] ?? "http";
                var user = lookup["User"] ?? string.Empty;
                var password = lookup["Pwd"] ?? string.Empty;

                return new Uri(lookup["Server"], lookup["Port"], index, scheme, user, password);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("'{0}' is not a valid connection string", connectionString),
                                            "connectionString", ex);
            }
        }
    }
}
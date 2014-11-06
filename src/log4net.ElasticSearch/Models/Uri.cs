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

        Uri(string server, string port, string index)
        {
            this.server = server;
            this.port = port;
            this.index = index;
        }

        public static implicit operator System.Uri(Uri uri)
        {
            return new System.Uri(string.Format("http://{0}:{1}/{2}/logEvent", uri.server, uri.port, uri.index));
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
                    if (lookup["rolling"] == "true")
                        index = string.Format("{0}-{1}", index, DateTime.Now.ToString("yyyy.MM.dd"));

                return
                    new Uri(lookup["Server"], lookup["Port"], index);
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("'{0}' is not a valid connection string", connectionString),
                                            "connectionString", ex);
            }
        }
    }
}
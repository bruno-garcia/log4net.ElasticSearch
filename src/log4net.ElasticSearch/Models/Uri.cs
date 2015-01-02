using System;
using System.Collections.Specialized;

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
                var parts = connectionString.GetParts();

                var index = GetIndex(parts);

                return new Uri(parts["Server"], parts["Port"], index);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("'{0}' is not a valid connection string".With(connectionString),
                                            "connectionString", ex);
            }
        }

        static string GetIndex(StringDictionary parts)
        {
            var index = parts["Index"];

            if (ShouldUseRollingIndex(parts))
                index = "{0}-{1}".With(index, DateTime.Now.ToString("yyyy.MM.dd"));

            return index;
        }

        static bool ShouldUseRollingIndex(StringDictionary parts)
        {
            return parts.Contains("rolling") && parts["rolling"].ToBool();
        }
    }
}
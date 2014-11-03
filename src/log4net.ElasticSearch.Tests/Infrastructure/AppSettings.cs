using System.Collections.Specialized;
using System.Configuration;

namespace log4net.ElasticSearch.Tests.Infrastructure
{
    public static class AppSettings
    {
        public static readonly NameValueCollection Instance = ConfigurationManager.AppSettings;

        public static bool UseFiddler(this NameValueCollection self)
        {
            return self.GetOrDefault("UseFiddler", "false").ToBool();
        }

        private static string GetOrDefault(this NameValueCollection self, string key, string @default)
        {
            return self[key] ?? @default;
        }
    }
}
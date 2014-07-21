using log4net.ElasticSearch.Models;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class RenameKeyFilter : IElasticAppenderFilter
    {
        private const string RenameFailed = "RenameFailed";

        public string Key { get; set; }
        public string RenameTo { get; set; }
        public bool OverrideExisting { get; set; }

        public RenameKeyFilter()
        {
            OverrideExisting = true;
        }

        public void PrepareConfiguration(ElasticClient client)
        {

        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            JToken token;
            if (logEvent.TryGetValue(Key, out token))
            {
                if (!OverrideExisting && logEvent.HasKey(RenameTo))
                {
                    logEvent.AddTag(RenameFailed);
                    return;
                }

                token.Remove();
                logEvent[RenameTo] = token;
            }
        }
    }
}
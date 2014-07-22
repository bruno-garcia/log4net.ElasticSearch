using log4net.ElasticSearch.Models;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class RenameKeyFilter : IElasticAppenderFilter
    {
        private SmartFormatter _key;
        private SmartFormatter _renameTo;
        private const string RenameFailed = "RenameFailed";

        public string Key
        {
            set { _key = value; }
        }

        public string RenameTo
        {
            set { _renameTo = value; }
        }

        public bool OverrideExisting { get; set; }

        public RenameKeyFilter()
        {
            OverrideExisting = true;
        }

        public void PrepareConfiguration(ElasticClient client)
        {
            // TODO: validate?
        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            JToken token;
            if (logEvent.TryGetValue(_key.Format(logEvent), out token))
            {
                var renameTo = _renameTo.Format(logEvent);

                if (!OverrideExisting && logEvent.HasKey(renameTo))
                {
                    logEvent.AddTag(RenameFailed);
                    return;
                }

                token.Remove();
                logEvent[renameTo] = token;
            }
        }
    }
}
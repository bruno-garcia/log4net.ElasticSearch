using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.SmartFormatter;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class RenameKeyFilter : FilterPropertiesValidator
    {
        private SmartFormatter<LogEventProcessor> _key;
        private SmartFormatter<LogEventProcessor> _renameTo;
        private const string FailedToRename = "RenameFailed";

        public bool Overwrite { get; set; }

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public string RenameTo
        {
            get { return _renameTo; }
            set { _renameTo = value; }
        }

        public RenameKeyFilter()
        {
            Overwrite = true;
        }

        public override void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            JToken token;
            if (logEvent.TryGetValue(_key.Format(logEvent), out token))
            {
                token.Parent.Remove();

                var renameTo = _renameTo.Format(logEvent);

                if (!Overwrite && logEvent.HasKey(renameTo))
                {
                    logEvent.AddTag(FailedToRename);
                    return;
                }

                logEvent[renameTo] = token;
            }
        }
    }
}
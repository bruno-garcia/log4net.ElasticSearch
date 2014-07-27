using log4net.ElasticSearch.Models;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class RemoveKeyFilter : FilterPropertiesValidator 
    {
        private SmartFormatter<LogEventProcessor> _key;

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public override void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            logEvent.Remove(_key.Format(logEvent));
        }
    }
}
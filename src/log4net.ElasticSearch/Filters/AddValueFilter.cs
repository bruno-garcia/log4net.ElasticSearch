using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.SmartFormatter;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class AddValueFilter : FilterPropertiesValidator
    {
        private SmartFormatter<LogEventProcessor> _key;
        private SmartFormatter<LogEventProcessor> _value;

        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            logEvent.AddOrSet(_key.Format(logEvent), _value.Format(logEvent));
        }
    }
}
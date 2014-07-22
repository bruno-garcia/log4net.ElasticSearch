using log4net.ElasticSearch.Models;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class AddValueFilter : IElasticAppenderFilter
    {
        private SmartFormatter _key;
        private SmartFormatter _value;

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

        public void PrepareConfiguration(ElasticClient client)
        {
            // TODO: validate?
        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            logEvent.AddOrSet(_key.Format(logEvent), _value.Format(logEvent));
        }
    }
}
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
            set { _key = value; }
        }

        public string Value
        {
            set { _value = value; }
        }

        public void PrepareConfiguration(ElasticClient client)
        {

        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            logEvent.AddOrSet(_key.Format(logEvent), _value.Format(logEvent));
        }
    }
}
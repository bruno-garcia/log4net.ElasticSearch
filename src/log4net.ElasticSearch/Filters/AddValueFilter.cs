using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.SmartFormatters;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class AddValueFilter : IElasticAppenderFilter
    {
        private LogEventSmartFormatter _key;
        private LogEventSmartFormatter _value;

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

        public bool Overwrite { get; set; }

        public void PrepareConfiguration(ElasticClient client)
        {
            ElasticAppenderFilters.ValidateFilterProperties(this);
        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            var key = _key.Format(logEvent);
            var value = _value.Format(logEvent);

            if (Overwrite)
            {
                logEvent[key] = value;
            }
            else
            {
                logEvent.AddOrSet(key, value);
            }
        }
    }
}
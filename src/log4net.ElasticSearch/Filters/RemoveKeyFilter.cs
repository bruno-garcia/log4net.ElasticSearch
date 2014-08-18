using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.SmartFormatters;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class RemoveKeyFilter : IElasticAppenderFilter
    {
        private LogEventSmartFormatter _key;

        [PropertyNotEmpty]
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public void PrepareConfiguration(ElasticClient client)
        {
            ElasticAppenderFilters.ValidateFilterProperties(this);
        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            logEvent.Remove(_key.Format(logEvent));
        }
    }
}
using log4net.ElasticSearch.Models;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class RemoveKeyFilter : IElasticAppenderFilter
    {
        private SmartFormatter _key;

        public string Key
        {
            set { _key = value; }
        }

        public void PrepareConfiguration(ElasticClient client)
        {

        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            logEvent.Remove(_key.Format(logEvent));
        }
    }
}
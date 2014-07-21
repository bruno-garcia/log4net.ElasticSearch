using log4net.ElasticSearch.Models;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class AddValueFilter : IElasticAppenderFilter
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public void PrepareConfiguration(ElasticClient client)
        {

        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            logEvent.AddOrSet(Key, Value);
        }
    }
}
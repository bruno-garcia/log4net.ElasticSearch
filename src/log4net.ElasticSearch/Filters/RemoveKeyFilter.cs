using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class RemoveKeyFilter : IElasticAppenderFilter
    {
        public string Key { get; set; }

        public void PrepareConfiguration(ElasticClient client)
        {

        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            logEvent.Remove(Key);
        }
    }
}
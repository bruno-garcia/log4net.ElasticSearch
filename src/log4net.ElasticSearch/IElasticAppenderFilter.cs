using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch
{
    public interface IElasticAppenderFilter 
    {
        void PrepareConfiguration(ElasticClient client);
        void PrepareEvent(JObject logEvent, ElasticClient client);
    }
}
using log4net.Core;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch
{
    public interface IElasticOption : IOptionHandler
    {
        void PrepareConfiguration(ElasticClient client);
        void PrepareEvent(JObject loggingEvent);
    }

    class AddValue : IElasticOption
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public void PrepareConfiguration(ElasticClient client)
        {
            
        }

        public void PrepareEvent(JObject loggingEvent)
        {
            loggingEvent[Name] = Value;
        }

        public void ActivateOptions()
        {
            
        }
    }
}
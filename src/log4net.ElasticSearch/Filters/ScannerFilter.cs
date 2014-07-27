using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public abstract class ScannerFilter : FilterPropertiesValidator
    {
        public string SourceKey { get; set; }

        protected ScannerFilter()
        {
            SourceKey = "Message";
        }

        protected abstract void ScanMessage(JObject logEvent, string input);

        public override void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            var token = logEvent[SourceKey];
            if (token == null)
                return;

            var input = token.Value<string>();
            ScanMessage(logEvent, input);
        }
    }
}
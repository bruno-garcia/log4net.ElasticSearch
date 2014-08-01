using System.Linq;
using System.Text.RegularExpressions;
using Nest;
using Newtonsoft.Json.Linq;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch.Filters
{
    public class KvFilter : IElasticAppenderFilter
    {
        private readonly Regex _regex;
        private const string FailedKv = "KvFilterFailed";

        public string SourceKey { get; set; }
        public string ValueSplit { get; set; }
        public string FieldSplit { get; set; }
        public bool Recursive { get; set; }

        public KvFilter()
        {
            SourceKey = "Message";
            ValueSplit = "=:";
            FieldSplit = " ,";

            var valueRxString = "(?:\"([^\"]+)\"" +
                                "|'([^']+)'" +
                                "|\\(([^\\)]+)\\)" +
                                "|\\[([^\\]]+)\\]" +
                                "|([^" + FieldSplit + "]+))";
            _regex = new Regex(
                string.Format("([^{0}{1}]+)\\s*[{1}]\\s*{2}", FieldSplit, ValueSplit, valueRxString)
                , RegexOptions.Compiled | RegexOptions.Multiline);
        }

        public void PrepareConfiguration(ElasticClient client)
        {
            ElasticAppenderFilters.ValidateFilterProperties(this);   
        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            string input;
            if (!logEvent.TryGetStringValue(SourceKey, out input))
            {
                //logEvent.AddTag(FailedKv);
                return;
            }

            ScanMessage(logEvent, input);
        }

        protected void ScanMessage(JObject logEvent, string input)
        {
            foreach (Match match in _regex.Matches(input))
            {
                var groups = match.Groups.Cast<Group>().Where(g => g.Success).ToList();
                var key = groups[1].Value;
                var value = groups[2].Value;
                
                ProcessValueAndStore(logEvent, key, value);
            }
        }

        private void ProcessValueAndStore(JObject logEvent, string key, string value)
        {   
            if (Recursive)
            {
                var innerEvent = new JObject();
                ScanMessage(innerEvent, value);

                if (innerEvent.HasValues)
                {
                    logEvent.AddOrSet(key, innerEvent);
                    return;
                }
            }

            logEvent.AddOrSet(key, value);
        }
    }
}

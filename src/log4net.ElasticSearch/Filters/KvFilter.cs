using System.Linq;
using System.Text.RegularExpressions;
using Nest;
using Newtonsoft.Json.Linq;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch.Filters
{
    public class KvFilter : IElasticAppenderFilter
    {
        private const string FailedKv = "KvFilterFailed";
        private Regex _kvRegex;
        private char[] _trimValue;
        private char[] _trimKey;

        [PropertyNotEmpty]
        public string ValueSplit { get; set; }
        [PropertyNotEmpty]
        public string FieldSplit { get; set; }
        [PropertyNotEmpty]
        public string SourceKey { get; set; }

        public string TrimValue
        {
            get { return string.Join("", _trimValue ?? Enumerable.Empty<char>()); }
            set { _trimValue = value.ToCharArray(); }
        }

        public string TrimKey
        {
            get { return string.Join("", _trimKey ?? Enumerable.Empty<char>()); }
            set { _trimKey = value.ToCharArray(); }
        }

        public bool Recursive { get; set; }

        public KvFilter()
        {
            SourceKey = "Message";
            ValueSplit = "=:";
            FieldSplit = " ,";
            TrimValue = "";
            TrimKey = "";
        }

        public void PrepareConfiguration(ElasticClient client)
        {
            ElasticAppenderFilters.ValidateFilterProperties(this);

            var valueRxString = "(?:\"([^\"]+)\"" +
                         "|'([^']+)'" +
                         "|\\(([^\\)]+)\\)" +
                         "|\\[([^\\]]+)\\]" +
                         "|([^" + FieldSplit + "]+))";
            _kvRegex = new Regex(
                string.Format("([^{0}{1}]+)\\s*[{1}]\\s*{2}", FieldSplit, ValueSplit, valueRxString)
                , RegexOptions.Compiled | RegexOptions.Multiline);
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
            foreach (Match match in _kvRegex.Matches(input))
            {
                var groups = match.Groups.Cast<Group>().Where(g => g.Success).ToList();
                var key = groups[1].Value;
                var value = groups[2].Value;

                if (_trimKey.Length > 0)
                {
                    key = key.Trim(_trimKey);
                }
                if (_trimValue.Length > 0)
                {
                    value = value.Trim(_trimValue);
                }

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

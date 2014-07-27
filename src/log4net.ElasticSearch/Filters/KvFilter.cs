using System.Linq;
using System.Text.RegularExpressions;
using Nest;
using Newtonsoft.Json.Linq;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch.Filters
{
    internal class KvFilter : ScannerFilter
    {
        private readonly Regex _regex;
        private readonly Regex _checkInputRegex;

        public string ValueSplit { get; set; }
        public string FieldSplit { get; set; }
        public DigValuesOption DigValues { get; set; }

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
            _checkInputRegex = new Regex("[" + ValueSplit + "]", RegexOptions.Compiled | RegexOptions.Multiline);
        }

        protected override void ScanMessage(JObject logEvent, string input)
        {
            if(!_checkInputRegex.IsMatch(input))
                return;

            foreach (Match match in _regex.Matches(input))
            {
                var groups = match.Groups.Cast<Group>().Where(g => g.Success).ToList();
                var key = groups[1].Value;
                var value = groups[2].Value;

                if (DigValues != null)
                {
                    var innerEvent = new JObject();
                    ScanMessage(innerEvent, value); 

                    // preserve the original value if RootKey specefied
                    if (!string.IsNullOrEmpty(DigValues.RootKey))
                    {
                        innerEvent[DigValues.RootKey] = value;
                    }

                    logEvent.AddOrSet(key, innerEvent);
                }
                else
                {
                    logEvent.AddOrSet(key, value);
                }
            }
        }
    }

    internal class DigValuesOption
    {
        public string RootKey { get; set; }
    }
}

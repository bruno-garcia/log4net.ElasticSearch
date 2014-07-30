using System.Linq;
using System.Text.RegularExpressions;
using log4net.ElasticSearch.Models;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class GrokFilter : FilterPropertiesValidator
    {
        private const string FailedGrok = "GrokMatchFailed";
        private readonly string[] _exceptGroups = new[] { "0" };
        private Regex _regex;
        private string[] _groupNames;
        
        public string SourceKey { get; set; }

        public bool Overwrite { get; set; }
        
        public string Pattern
        {
            get { return _regex != null ? _regex.ToString(): string.Empty; }
            set
            {
                _regex = new Regex(value, RegexOptions.Compiled);
                _groupNames = _regex.GetGroupNames().Except(_exceptGroups).ToArray();
            }
        }

        public GrokFilter()
        {
            SourceKey = "Message";
        }

        public override void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            string input;
            if (!logEvent.TryGetStringValue(SourceKey, out input))
            {
                logEvent.AddTag(FailedGrok);
                return;
            }

            ScanMessage(logEvent, input);
        }
        
        protected void ScanMessage(JObject logEvent, string input)
        {
            var match = _regex.Match(input);
            
            if (!match.Success)
            {
                logEvent.AddTag(FailedGrok);
            }

            foreach (var groupName in _groupNames)
            {
                var key = match.Groups[groupName];
                if (!key.Success) continue;

                if (Overwrite)
                {
                    logEvent[groupName] = key.Value;
                }
                else
                {
                    logEvent.AddOrSet(groupName, key.Value);
                }
            }
        }
    }
}

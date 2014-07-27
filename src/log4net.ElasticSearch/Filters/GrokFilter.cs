using System.Text.RegularExpressions;
using log4net.ElasticSearch.Models;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class GrokFilter : ScannerFilter
    {
        private const string FailedGrok = "GrokMatchFailed";
        private Regex _regex;
        private string[] _groupNames;

        public bool Overwrite { get; set; }
        
        public string Pattern
        {
            get { return _regex.ToString(); }
            set
            {
                _regex = new Regex(value, RegexOptions.Compiled);
                _groupNames = _regex.GetGroupNames();
            }
        }
        
        protected override void ScanMessage(JObject logEvent, string input)
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

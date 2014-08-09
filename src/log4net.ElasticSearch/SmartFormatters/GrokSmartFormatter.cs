using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.SmartFormatters
{
    /// <summary>
    /// A SmartFormatter that generate regex patterns by thier names.
    /// For example: the value might be "%{WORD} guid: %{UUID:guid}",
    /// "UUID" is the name of the pattern and "guid" is the name of ther group.
    /// The formatter will search for the name "WORD" and "UUID" on the dictionary
    /// and replace them with the pattern enclose between "(" and ")" and group name if mentioned.
    /// </summary>
    public class GrokSmartFormatter : SmartFormatter
    {
        private static readonly Regex InnerRegex = new Regex(@"%\{([^\}:]+)(?::([^\}]+))?\}", RegexOptions.Compiled);

        public GrokSmartFormatter(string input) : base(input, InnerRegex.Matches(input))
        {
        }

        protected override bool TryProcessMatch(IDictionary<string, JToken> jObj, Match match, out string replacementString)
        {
            var patternName = match.Groups[1].Value;
            var groupNameMatch = match.Groups[2];

            JToken token;
            if (jObj.TryGetValue(patternName, out token))
            {
                var pattern = token.Value<string>();
                if (groupNameMatch.Success)
                {
                    replacementString = string.Format("(?<{0}>{1})", groupNameMatch.Value, pattern);
                }
                else
                {
                    replacementString = string.Format("({0})", pattern);
                }

                return true;
            }

            replacementString = string.Empty;
            return false;
        }

        public static implicit operator GrokSmartFormatter(string s)
        {
            return new GrokSmartFormatter(s);
        }
    }
}

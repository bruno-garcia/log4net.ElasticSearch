using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using log4net.ElasticSearch.Models;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.SmartFormatters
{
    /// <summary>
    /// A SmartFormatter that replace all the keys in the input using the LogEvent.
    /// Key might look like this "sometext {key}".
    /// It also formats keys that start with "+" as time.
    /// For example: "the day is {+yyyy-MM-dd}"
    /// </summary>
    public class LogEventSmartFormatter : SmartFormatter
    {
        private static readonly Regex InnerRegex = new Regex(@"%\{([^\}]+)\}", RegexOptions.Compiled);
        

        public LogEventSmartFormatter(string input) 
            : base(input, InnerRegex.Matches(input))
        {

        }

        protected override bool TryProcessMatch(IDictionary<string, JToken> logEvent, Match match, out string replacementString)
        {
            replacementString = string.Empty;
            string innerMatch = match.Groups[1].Value;

            // "+" means dateTime format
            if (innerMatch.StartsWith("+"))
            {
                replacementString = DateTime.Now.ToString(innerMatch.Substring(1), CultureInfo.InvariantCulture);
                return true;
            }

            JToken token;
            if (logEvent.TryGetValue(innerMatch, out token))
            {
                replacementString = token.Value<string>();
                return true;
            }

            return false;
        }

        public static implicit operator LogEventSmartFormatter(string s)
        {
            return new LogEventSmartFormatter(s);
        }
    }
}
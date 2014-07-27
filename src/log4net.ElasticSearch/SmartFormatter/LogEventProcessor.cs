using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using log4net.ElasticSearch.Models;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.SmartFormatter
{
    /// <summary>
    /// A Processor that replace all the keys in the input using the LogEvent.
    /// Key might look like this "sometext {key}".
    /// It also formats keys that start with "+" as time.
    /// For example: "the day is {+yyyy-MM-dd}"
    /// </summary>
    public class LogEventProcessor : ISmartFormatterProcessor
    {
        private static readonly Regex InnerRegex = new Regex(@"%\{([^\}]+)\}", RegexOptions.Compiled);

        public string Raw { get; set; }

        public Regex GetRegex()
        {
            return InnerRegex;
        }

        public bool ToSkip(JObject jObj)
        {
            return jObj.HasKey(Raw);
        }

        public void ProcessMatch(JObject logEvent, Match match, StringBuilder resultSb)
        {
            string wholeMatch = match.Value;
            string innerMatch = match.Groups[1].Value;

            // "+" means dateTime format
            if (innerMatch.StartsWith("+"))
            {
                resultSb.Replace(wholeMatch, DateTime.Now.ToString(innerMatch.Substring(1), CultureInfo.InvariantCulture));
                return;
            }

            JToken token;
            if (logEvent.TryGetValue(innerMatch, out token))
            {
                resultSb.Replace(wholeMatch, token.Value<string>());
            }
        }
    }
}
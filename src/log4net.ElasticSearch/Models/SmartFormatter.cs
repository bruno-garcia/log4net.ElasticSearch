using System;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Models
{
    class SmartFormatter
    {
        private static readonly Regex FormatRegex = new Regex(@"%\{([^\}]+)\}", RegexOptions.Compiled);
        private readonly string _rawFormat;
        private readonly MatchCollection _match;
        private readonly bool _dontBother;

        public string Raw
        {
            get { return _rawFormat; }
        }

        public SmartFormatter(string format)
        {
            _rawFormat = format;
            _match = FormatRegex.Matches(_rawFormat);
            _dontBother = !FormatRegex.IsMatch(format);
        }

        public string Format(JObject logEvent)
        {
            if (_dontBother || logEvent.HasKey(_rawFormat))
            {
                return _rawFormat;
            }

            var sb = new StringBuilder(_rawFormat);
            foreach (Match match in _match)
            {
                string wholeMatch = match.Value;
                string innerMatch = match.Groups[1].Value;

                // "+" means dateTime format
                if (innerMatch.StartsWith("+"))
                {
                    sb.Replace(wholeMatch, DateTime.Now.ToString(innerMatch.Substring(1)));
                    continue;
                }

                JToken token;
                if (logEvent.TryGetValue(innerMatch, out token))
                {
                    sb.Replace(wholeMatch, token.Value<string>());
                }
            }

            return sb.ToString();
        }

        public static implicit operator string(SmartFormatter f)
        {
            return f == null ? null : f.Raw;
        }

        public static implicit operator SmartFormatter(string s)
        {
            return new SmartFormatter(s);
        }
    }
}

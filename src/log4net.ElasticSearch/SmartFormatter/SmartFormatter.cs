using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Models
{
    public interface ISmartFormatterProcessor
    {
        /// <summary>
        /// Returns the raw original input.
        /// </summary>
        string Raw { get; set; }

        /// <summary>
        /// Returns the regex object of the specific processor.
        /// </summary>
        Regex GetRegex();

        /// <summary>
        /// Before processing check the jObject whether we need to process or not.
        /// </summary>
        bool ToSkip(JObject jObj);

        /// <summary>
        /// Process the match and edit the result.
        /// Using the JObject if needed.
        /// </summary>
        void ProcessMatch(JObject jObj, Match match, StringBuilder result);
    }

    public class SmartFormatter<T> where T : ISmartFormatterProcessor, new()
    {
        private readonly MatchCollection _match;
        private readonly bool _dontBother;
        private readonly T _processor;

        public string Raw
        {
            get { return _processor.Raw; }
        }

        protected SmartFormatter(string input)
        {
            _processor = new T {Raw = input};

            var regex = _processor.GetRegex();
            _match = regex.Matches(input);
            _dontBother = !regex.IsMatch(input);
        }

        public string Format()
        {
            return Format(new JObject());
        }

        public string Format(JObject jObj)
        {
            if (_dontBother || _processor.ToSkip(jObj))
            {
                return _processor.Raw;
            }

            var result = new StringBuilder(_processor.Raw);
            foreach (Match match in _match)
            {
                _processor.ProcessMatch(jObj, match, result);
            }

            return result.ToString();
        }

        public static implicit operator string(SmartFormatter<T> f)
        {
            return f == null ? null : f.Raw;
        }

        public static implicit operator SmartFormatter<T>(string s)
        {
            return new SmartFormatter<T>(s);
        }
    }

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

    //public class GrokProcessor : ISmartFormatterProcessor
    //{
    //    private static readonly Regex InnerRegex = new Regex(@"%\{([^\}:]+)(?::([^\}]+))?\}", RegexOptions.Compiled);

    //    public Regex GetRegex()
    //    {
    //        return InnerRegex;
    //    }

    //    public void ProcessMatch(JObject logEvent, Match match, StringBuilder resultSb)
    //    {
    //        // todo
    //    }
    //}
}

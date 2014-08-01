using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.SmartFormatters
{
    public abstract class SmartFormatter
    {
        private readonly MatchCollection _matches;
        private readonly bool _dontBother;

        public string Raw { get; protected set; }

        protected SmartFormatter(string input, MatchCollection matches)
        {
            Raw = input;
            _matches = matches;
            _dontBother = _matches.Count == 0;
        }

        public string Format()
        {
            return Format(new JObject());
        }

        public string Format(JObject jObj)
        {
            if (_dontBother)
            {
                return Raw;
            }

            var result = new StringBuilder(Raw);
            foreach (Match match in _matches)
            {
                ProcessMatch(jObj, match, result);
            }

            return result.ToString();
        }

        protected abstract void ProcessMatch(JObject jObj, Match match, StringBuilder result);

        public override string ToString()
        {
            return Raw;
        }

        public static implicit operator string(SmartFormatter f)
        {
            return f == null ? null : f.Raw;
        }
    }

    //public class GrokProcessor : ISmartFormatter
    //{
    //    private static readonly Regex InnerRegex = new Regex(@"%\{([^\}:]+)(?::([^\}]+))?\}", RegexOptions.Compiled);

    //    public MatchCollection GetMatches()
    //    {
    //        return InnerRegex.Matches(Raw);
    //    }

    //    public void ProcessMatch(JObject logEvent, Match match, StringBuilder resultSb)
    //    {
    //        // todo
    //    }
    //}
}

using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.SmartFormatter
{
    /// <summary>
    /// SmartFormatter is a class that gets an format as input
    /// and parse it using <see cref="ISmartFormatterProcessor"/>.
    /// When you call to the <see cref="Format(Newtonsoft.Json.Linq.JObject)"/> function 
    /// its format the input using the Processor and the JObject.
    /// </summary>
    /// <typeparam name="T">the Processor</typeparam>
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

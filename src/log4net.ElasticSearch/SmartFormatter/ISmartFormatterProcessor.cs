using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.SmartFormatter
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
}
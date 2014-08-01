using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.SmartFormatters
{
    public interface ISmartFormatter
    {
        /// <summary>
        /// Returns the raw original input.
        /// </summary>
        string Raw { get; set; }

        /// <summary>
        /// Process the match and edit the result.
        /// Using the JObject if needed.
        /// </summary>
        void ProcessMatch(JObject jObj, Match match, StringBuilder result);
    }
}
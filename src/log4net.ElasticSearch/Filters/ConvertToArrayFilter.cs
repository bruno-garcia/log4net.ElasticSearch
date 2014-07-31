using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using log4net.ElasticSearch.Models;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public class ConvertToArrayFilter : IElasticAppenderFilter
    {
        private Regex _seperateRegex;
        public string SourceKey { get; set; }

        public string Seperator
        {
            get { return _seperateRegex != null ? _seperateRegex.ToString() : string.Empty; }
            set { _seperateRegex = new Regex("[" + value + "]+", RegexOptions.Compiled | RegexOptions.Multiline); }
        }

        public ConvertToArrayFilter()
        {
            SourceKey = "Message";
            Seperator = ", ";
        }

        public void PrepareConfiguration(ElasticClient client)
        {
            ElasticAppenderFilters.ValidateFilterProperties(this);
        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            string value;
            if (!logEvent.TryGetStringValue(SourceKey, out value))
            {
                return;
            }

            logEvent[SourceKey] = new JArray(_seperateRegex.Split(value).Where(s => !string.IsNullOrEmpty(s)));
        }
    }
}

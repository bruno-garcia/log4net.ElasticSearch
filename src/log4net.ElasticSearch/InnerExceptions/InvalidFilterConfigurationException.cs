using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log4net.ElasticSearch.InnerExceptions
{
    public class InvalidFilterConfigurationException : Exception
    {
        public InvalidFilterConfigurationException()
        {
        }

        public InvalidFilterConfigurationException(string message)
            : base(message)
        {
        }
    }
}

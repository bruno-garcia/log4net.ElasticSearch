using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log4net.ElasticSearch.InnerExceptions
{
    public class InvalidFilterConfigException : Exception
    {
        public InvalidFilterConfigException()
        {
        }

        public InvalidFilterConfigException(string message)
            : base(message)
        {
        }
    }
}

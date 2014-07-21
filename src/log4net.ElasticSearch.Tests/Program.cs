using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace log4net.ElasticSearch.Tests
{
    public static class Program
    {
        public static void Main()
        {
            ILog logger = LogManager.GetLogger(typeof (ElasticSearchAppender));
            logger.Info("test");
        }
    }
}

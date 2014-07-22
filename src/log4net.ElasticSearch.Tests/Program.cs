using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace log4net.ElasticSearch.Tests
{
    public static class Program
    {
        public static void Main()
        {
            ILog logger = LogManager.GetLogger(typeof (ElasticSearchAppender));
            
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 100000; i++)
            {
                log4net.GlobalContext.Properties["currentNumber"] = i;
                logger.InfoFormat("test #{0}", i); 
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}

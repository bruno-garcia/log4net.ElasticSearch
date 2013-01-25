using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.ElasticSearch;

namespace TestHarness
{
    class Program
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            _log.Info("connection string");
            Console.ReadKey();
        }

        
    }
}

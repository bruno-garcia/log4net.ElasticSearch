using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace log4net.ElasticSearch.Tests
{
    public static class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ElasticSearchAppender));
        public static void Main()
        {
            Logger.Info("someIds=[123, 124 ,125 , 007]");
            Logger.Info("someIds=[123]");
            Logger.Info("someIds=[123]");
            Logger.Info("someIds=[222, 007, 1]");
            Logger.Info("someIds=[1]");
            //var tasks = new List<Task>();
            //for (int i = 0; i < 1; i++)
            //{
            //    int i1 = i;
            //    tasks.Add(Task.Run(() => Runner(i1)));
            //}
            //Task.WaitAll(tasks.ToArray());
        }

        public static void Runner(int t)
        {
            log4net.ThreadContext.Properties["taskNumber"] = t;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                Logger.InfoFormat("test #{0}", i);
                //Thread.Sleep(1);
            }
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}

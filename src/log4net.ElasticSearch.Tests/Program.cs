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
            Logger.InfoFormat("yoyo=yiyi b='this is id {0}', a=[this is id:1, o:2]", 1);
            Logger.InfoFormat("yoyo=yiyi aa='this is id {0}', c=[this is id:1, o:2]", 1);
            Thread.Sleep(2000);
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

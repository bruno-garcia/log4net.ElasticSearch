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
            var tests = new ElasticSearchAppenderTests();
            tests.FixtureSetup();

            var tasks = new List<Task>();
            var numberOfTasks = 1;
            for (int i = 0; i < numberOfTasks; i++)
            {
                int i1 = i;
                tasks.Add(Task.Run(() => Runner(i1)));
            }
            Task.WaitAll(tasks.ToArray());
            Console.ReadLine();

            tests.FixtureTearDown();
        }

        public static void Runner(int t)
        {
            log4net.ThreadContext.Properties["taskNumber"] = t;
            var numberOfCycles = 10000;
            
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < numberOfCycles; i++)
            {
                Logger.InfoFormat("testNum: {0}, name is someName and guid {1}", i, Guid.NewGuid());
                //Thread.Sleep(1);
            }
            sw.Stop();

            Console.WriteLine("Ellapsed: {0}, numPerSec: {1}",
                sw.ElapsedMilliseconds, numberOfCycles/(sw.ElapsedMilliseconds/(double) 1000));
        }
    }
}

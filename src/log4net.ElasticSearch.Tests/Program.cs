using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace log4net.ElasticSearch.Tests
{
    public static class Program
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ElasticSearchAppender));

        public static void Main()
        {
            ElasticSearchAppenderTests.Performance();
        }

        public static void PerformanceTest(int numberOfTasks, int numberOfCycles)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < numberOfTasks; i++)
            {
                int i1 = i;
                tasks.Add(Task.Run(() => Runner(i1, numberOfCycles)));
            }
            Task.WaitAll(tasks.ToArray());
            //Console.ReadLine();
        }

        public static void Runner(int t, int numberOfCycles)
        {
            log4net.ThreadContext.Properties["taskNumber"] = t;
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < numberOfCycles; i++)
            {
                Logger.InfoFormat("testNum: {0}, name is someName and guid {1}", i, Guid.NewGuid());
            }
            sw.Stop();

            Console.WriteLine("Ellapsed: {0}, numPerSec: {1}",
                sw.ElapsedMilliseconds, numberOfCycles/(sw.ElapsedMilliseconds/(double) 1000));
        }
    }
}

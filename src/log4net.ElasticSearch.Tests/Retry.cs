using System;
using System.Threading;

namespace log4net.ElasticSearch.Tests
{
    public static class Retry
    {
        const int DefaultRetryCount = 20;
        const int DefaultRetryDelay = 250;

        public static void Ignoring<TException>(Action action, int retryCount = DefaultRetryCount, int retryDelay = DefaultRetryDelay) 
            where TException : Exception
        {
            var exceptionType = typeof (TException);
            var attempt = 0;

            while (true)
            {
                attempt++;

                try
                {
                    action();

                    return;
                }
                catch (Exception exception)
                {
                    if (exception.GetType() != exceptionType || attempt >= retryCount) throw;

                    Thread.Sleep(retryDelay);
                }
            }
        }
    }
}
using System;

namespace log4net.ElasticSearch.Infrastructure
{
    public static class Clock
    {
        static DateTime? frozen;

        public static DateTime Date
        {
            get { return Now.Date; }
        }

        public static IDisposable Freeze(DateTime dateTime)
        {
            frozen = dateTime;

            return new AnonymousDisposable(() => Unfreeze());
        }

        public static DateTime Now
        {
            get { return frozen.HasValue ? frozen.Value : DateTime.UtcNow; }
        }

        static void Unfreeze()
        {
            frozen = null;
        }    
    }
}
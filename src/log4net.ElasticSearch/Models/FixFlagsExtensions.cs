using log4net.Core;

namespace log4net.ElasticSearch.Models
{
    public static class FixFlagsExtensions
    {
        public static bool IsSwitched(this FixFlags flags, FixFlags check)
        {
            return (flags & check) != FixFlags.None;
        }
    }
}

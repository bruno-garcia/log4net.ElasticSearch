namespace log4net.ElasticSearch.Tests.Infrastructure
{
    public static class ExtensionMethods
    {
        public static bool ToBool(this string self)
        {
            return bool.Parse(self);
        }
    }
}
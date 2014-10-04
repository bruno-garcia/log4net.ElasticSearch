namespace log4net.ElasticSearch.Tests
{
    public static class ExtensionMethods
    {
        public static bool ToBool(this string self)
        {
            return bool.Parse(self);
        }
    }
}
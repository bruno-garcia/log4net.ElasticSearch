namespace log4net.ElasticSearch.Models
{
    public class ElasticSearchConnection
    {
        public string Server { get; set; }
        public string Port { get; set; }
        public string Index { get; set; }

        public override string ToString()
        {
            return string.Format("http://{0}:{1}/{2}/LogEvent", Server, Port, Index);
        }
    }
}

using System.Net;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{
    public static class JsonWebRequest
    {
        const string ContentType = "text/json";
        const string Method = "POST";

        public static HttpWebRequest For(Connection connection)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(connection.ToString());

            httpWebRequest.ContentType = ContentType;
            httpWebRequest.Method = Method;

            return httpWebRequest;
        }
    }
}
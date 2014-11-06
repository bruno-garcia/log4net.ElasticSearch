using System;
using System.Net;

namespace log4net.ElasticSearch
{
    public static class JsonWebRequest
    {
        const string ContentType = "text/json";
        const string Method = "POST";

        public static HttpWebRequest For(Uri uri)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);

            httpWebRequest.ContentType = ContentType;
            httpWebRequest.Method = Method;

            return httpWebRequest;
        }
    }
}
using System;
using System.Net;
using System.Text;

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

            if (uri.Scheme == "https" && !string.IsNullOrWhiteSpace(uri.UserInfo))
            {
                httpWebRequest.Headers.Remove(HttpRequestHeader.Authorization);
                httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(uri.UserInfo)));
            }

            return httpWebRequest;
        }
    }
}
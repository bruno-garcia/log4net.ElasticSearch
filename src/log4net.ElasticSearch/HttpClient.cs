using System;
using System.IO;
using System.Net;

namespace log4net.ElasticSearch
{
    public interface IHttpClient
    {
        void Post<T>(Uri uri, T item);
    }

    public class HttpClient : IHttpClient
    {
        const string ContentType = "text/json";
        const string Method = "POST";

        public void Post<T>(Uri uri, T item)
        {
            var httpWebRequest = RequestFor(uri);

            using (var streamWriter = GetRequestStream(httpWebRequest))
            {
                streamWriter.Write(item.ToJson());
                streamWriter.Flush();

                var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                httpResponse.Close();

                if (httpResponse.StatusCode != HttpStatusCode.Created)
                {
                    throw new WebException(
                        "Failed to post {0} to {1}.".With(item.GetType().Name, uri));
                }
            }
        }

        static HttpWebRequest RequestFor(Uri uri)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(uri);

            httpWebRequest.ContentType = ContentType;
            httpWebRequest.Method = Method;

            return httpWebRequest;
        }

        static StreamWriter GetRequestStream(WebRequest httpWebRequest)
        {
            return new StreamWriter(httpWebRequest.GetRequestStream());
        }
    }
}
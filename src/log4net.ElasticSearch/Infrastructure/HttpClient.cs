using System;
using System.IO;
using System.Net;
using System.Text;

namespace log4net.ElasticSearch.Infrastructure
{
    public interface IHttpClient
    {
        void Post<T>(Uri uri, T item);
        void PostBulk<T>(Uri uri, T items);
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

        /// <summary>
        /// Post the events to the Elasticsearch _bulk API for faster inserts
        /// </summary>
        /// <typeparam name="T">Type/item being inserted. Should be a list of events</typeparam>
        /// <param name="uri">Fully formed URI to the bulk API</param>
        /// <param name="item"></param>
        public void PostBulk<T>(Uri uri, T items)
        {
            var bulkUri = new Uri(uri, "_bulk");

            var httpWebRequest = RequestFor(bulkUri);

            using (var streamWriter = GetRequestStream(httpWebRequest))
            {
                streamWriter.Write(item.ToJson());
                streamWriter.Flush();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                httpResponse.Close();

                if (httpResponse.StatusCode != HttpStatusCode.Created)
                {
                    throw new WebException(
                        "Failed to post {0} to {1}.".With(item.GetType().Name, uri));
                }
            }
        }

        public static HttpWebRequest RequestFor(Uri uri)
        {
            var httpWebRequest = (HttpWebRequest) WebRequest.Create(uri);

            httpWebRequest.ContentType = ContentType;
            httpWebRequest.Method = Method;
            
            if (!string.IsNullOrWhiteSpace(uri.UserInfo))
            {
                httpWebRequest.Headers.Remove(HttpRequestHeader.Authorization);
                httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(uri.UserInfo)));
            }

            return httpWebRequest;
        }

        static StreamWriter GetRequestStream(WebRequest httpWebRequest)
        {
            return new StreamWriter(httpWebRequest.GetRequestStream());
        }
    }
}
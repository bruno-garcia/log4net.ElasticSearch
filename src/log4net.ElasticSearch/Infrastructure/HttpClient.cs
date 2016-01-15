using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using log4net.ElasticSearch.Models;
using Uri = System.Uri;

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
        /// <param name="uri">Fully formed URI to the ES endpoint</param>
        /// <param name="items">List of logEvents</param>
        public void PostBulk<T>(Uri uri, T items)
        {
            var httpWebRequest = RequestFor(uri);

            var postBody = new StringBuilder();

            // For each logEvent, we build a bulk API request which consists of one line for
            // the action, one line for the document. In this case "create" and then the doc
            // Since we're appending _bulk to the end of the Uri, ES will default to using the
            // index and type already specified in the Uri segments
            foreach (var item in (IEnumerable<logEvent>)items)
            {
                postBody.AppendLine("{\"index\" : {} }");
                postBody.AppendLine(item.ToJson());
            }

            using (var streamWriter = GetRequestStream(httpWebRequest))
            {
                streamWriter.Write(postBody.ToString());
                streamWriter.Flush();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                httpResponse.Close();

                if (httpResponse.StatusCode != HttpStatusCode.Created && httpResponse.StatusCode != HttpStatusCode.OK)
                {
                    throw new WebException(
                        "Failed to post {0} to {1}.".With(postBody.ToString(), uri));
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
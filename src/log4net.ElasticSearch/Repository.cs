using System.Collections.Generic;
using log4net.ElasticSearch.Infrastructure;
using log4net.ElasticSearch.Models;
using System;

namespace log4net.ElasticSearch
{
    public interface IRepository
    {
        void Add(IEnumerable<logEvent> logEvents, int bufferSize);
    }

    public class Repository : IRepository
    {
        readonly Uri uri;
        readonly IHttpClient httpClient;

        Repository(Uri uri, IHttpClient httpClient)
        {
            this.uri = uri;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Post the event(s) to the Elasticsearch API. If the bufferSize in the connection
        /// string is set to more than 1, assume we use the _bulk API for better speed and
        /// efficiency
        /// </summary>
        /// <param name="logEvents">A collection of logEvents</param>
        /// <param name="bufferSize">The BufferSize as set in the connection string details</param>
        public void Add(IEnumerable<logEvent> logEvents, int bufferSize)
        {
            try
            {
                if (bufferSize <= 1)
                {
                    // Post the logEvents one at a time throught the ES insert API
                    logEvents.Do(logEvent => httpClient.Post(uri, logEvent));
                }
                else
                {
                    // Post the logEvents all at once using the ES _bulk API
                    httpClient.PostBulk(uri, logEvents);
                }   
            }
            catch(Exception ex)
            {
                //DO NOTHING.
            }
        }

        public static IRepository Create(string connectionString)
        {
            return Create(connectionString, new HttpClient());
        }

        public static IRepository Create(string connectionString, IHttpClient httpClient)
        {
            return new Repository(Uri.For(connectionString), httpClient);
        }
    }
}

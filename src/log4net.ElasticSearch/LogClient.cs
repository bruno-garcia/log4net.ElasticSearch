using System;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{
    public interface IRepository
    {
        void Add(LogEvent logEvent);
    }

    public class Repository : IRepository
    {
        private readonly HttpWebRequest httpWebRequest;

        Repository(ElasticSearchConnection connection)
        {
            ServicePointManager.Expect100Continue = false;
            httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format("http://{0}:{1}/{2}/LogEvent", 
                connection.Server, connection.Port, connection.Index));
            
            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "POST";
        }

        public void Add(LogEvent logEvent)
        {
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = new JavaScriptSerializer().Serialize(logEvent);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                httpResponse.Close();

                if (httpResponse.StatusCode != HttpStatusCode.Created)
                {
                    throw new InvalidOperationException("Failed to correctly add the event to the Elasticsearch index.");
                }
            }
        }        

        public static IRepository Create(string connectionString)
        {
            return new Repository(ElasticSearchConnectionBuilder.Build(connectionString));
        }
    }
}

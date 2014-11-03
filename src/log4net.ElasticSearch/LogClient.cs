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
        readonly HttpWebRequest httpWebRequest;
        readonly JavaScriptSerializer serializer;

        Repository(HttpWebRequest httpWebRequest, JavaScriptSerializer serializer)
        {            
            this.httpWebRequest = httpWebRequest;
            this.serializer = serializer;
        }

        public void Add(LogEvent logEvent)
        {
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                var json = serializer.Serialize(logEvent);

                streamWriter.Write(json);
                streamWriter.Flush();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                httpResponse.Close();                

                if (httpResponse.StatusCode != HttpStatusCode.Created)
                {
                    throw new WebException(string.Format("Failed to correctly add {0} to the ElasticSearch index.", logEvent.GetType().Name));
                }
            }
        }        

        public static IRepository Create(string connectionString)
        {
            return new Repository(JsonWebRequest.For(ElasticSearchConnectionBuilder.Build(connectionString)), new JavaScriptSerializer());
        }

        private static class JsonWebRequest
        {
            const string ContentType = "text/json";
            const string Method = "POST";

            public static HttpWebRequest For(ElasticSearchConnection connection)
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(connection.ToString());

                httpWebRequest.ContentType = ContentType;
                httpWebRequest.Method = Method;                

                return httpWebRequest;
            }
        }
    }
}

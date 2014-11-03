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
        readonly JavaScriptSerializer serializer;
        readonly Connection connection;

        Repository(JavaScriptSerializer serializer, Connection connection)
        {            
            this.serializer = serializer;
            this.connection = connection;
        }

        public void Add(LogEvent logEvent)
        {
            var httpWebRequest = JsonWebRequest.For(connection);
            
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
            return new Repository(new JavaScriptSerializer(), Connection.Create(connectionString));
        }
    }
}

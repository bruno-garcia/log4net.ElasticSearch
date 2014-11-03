using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using log4net.ElasticSearch.Models;
using Uri = System.Uri;

namespace log4net.ElasticSearch
{
    public interface IRepository
    {
        void Add(IEnumerable<LogEvent> logEvents);
    }

    public class Repository : IRepository
    {
        readonly JavaScriptSerializer serializer;
        readonly Uri uri;

        public static IRepository Create(string connectionString)
        {
            return new Repository(new JavaScriptSerializer(), Models.Uri.Create(connectionString));
        }

        Repository(JavaScriptSerializer serializer, Uri uri)
        {
            this.serializer = serializer;
            this.uri = uri;
        }

        public void Add(IEnumerable<LogEvent> logEvents)
        {
            //TODO - hoping the guys who know ES can provide guidance for bulk operations
            foreach (var logEvent in logEvents)
            {
                var httpWebRequest = JsonWebRequest.For(uri);

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    var json = serializer.Serialize(logEvent);

                    streamWriter.Write(json);
                    streamWriter.Flush();

                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    httpResponse.Close();

                    if (httpResponse.StatusCode != HttpStatusCode.Created)
                    {
                        throw new WebException(string.Format("Failed to correctly add {0} to the ElasticSearch index.",
                                                             logEvents.GetType().Name));
                    }
                }
            }
        }
    }
}
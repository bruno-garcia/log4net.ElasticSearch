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
        void Add(IEnumerable<logEvent> logEvents);
    }

    public class Repository : IRepository
    {
        readonly JavaScriptSerializer serializer;
        readonly Uri uri;

        Repository(JavaScriptSerializer serializer, Uri uri)
        {
            this.serializer = serializer;
            this.uri = uri;
        }

        public void Add(IEnumerable<logEvent> logEvents)
        {
            logEvents.Do(logEvent =>
                {
                    var httpWebRequest = JsonWebRequest.For(uri);

                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        var json = serializer.Serialize(logEvent);

                        streamWriter.Write(json);
                        streamWriter.Flush();

                        var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                        httpResponse.Close();

                        if (httpResponse.StatusCode != HttpStatusCode.Created)
                        {
                            throw new WebException(
                                "Failed to correctly add {0} to the ElasticSearch index.".With(logEvents.GetType().Name));
                        }
                    }
                });
        }

        public static IRepository Create(string connectionString)
        {
            return new Repository(new JavaScriptSerializer(), Models.Uri.Create(connectionString));
        }
    }
}
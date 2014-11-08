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
        readonly Uri uri;

        Repository(Uri uri)
        {
            this.uri = uri;
        }

        public void Add(IEnumerable<logEvent> logEvents)
        {            
            logEvents.Do(logEvent =>
                {
                    var httpWebRequest = JsonWebRequest.For(uri);

                    using (var streamWriter = GetRequestStream(httpWebRequest))
                    {
                        streamWriter.Write(logEvent.ToJson());
                        streamWriter.Flush();

                        var httpResponse = (HttpWebResponse) httpWebRequest.GetResponse();
                        httpResponse.Close();

                        if (httpResponse.StatusCode != HttpStatusCode.Created)
                        {
                            throw new WebException("Failed to correctly add {0} to the ElasticSearch index.".With(logEvent.GetType().Name));
                        }
                    }
                });

        }

        static StreamWriter GetRequestStream(WebRequest httpWebRequest)
        {
            return new StreamWriter(httpWebRequest.GetRequestStream());
        }

        public static IRepository Create(string connectionString)
        {
            return new Repository(Models.Uri.Create(connectionString));
        }
    }
}
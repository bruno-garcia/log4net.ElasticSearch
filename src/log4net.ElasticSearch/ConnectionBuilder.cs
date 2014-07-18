using System;
using System.Collections.Specialized;
using Nest;

namespace log4net.ElasticSearch
{
    /// <summary>
    /// Class to support passing in traditional style connection strings and then parsing
    /// them into ElasticSearch components. We create a NEST ElasticSearch connection object and pass
    /// it back to the calling function.
    /// </summary>
    public class ConnectionBuilder
    {
        public static ConnectionSettings BuildElsticSearchConnection(string connectionString, string dateFormat = "yyyy-MM-dd")
        {
            try
            {
                var builder = new System.Data.Common.DbConnectionStringBuilder();
                builder.ConnectionString = connectionString.Replace("{", "\"").Replace("}", "\"");

                var lookup = new StringDictionary();
                foreach (string key in builder.Keys)
                {
                    lookup[key] = Convert.ToString(builder[key]);
                }

                var index = lookup["Index"];

                // If the user asked for rolling logs, setup the index by day
                if (!string.IsNullOrEmpty(lookup["rolling"]))
                {
                    if (lookup["rolling"] == "true")
                    {
                        index = string.Format("{0}-{1}", index, DateTime.Now.ToString(dateFormat));
                    }
                }

                return
                    new ConnectionSettings(new Uri(string.Format("http://{0}:{1}", lookup["Server"], 
                        Convert.ToInt32(lookup["Port"])))).SetDefaultIndex(index);
            }
            catch
            {
                throw new InvalidOperationException("Not a valid connection string");
            }
        }
    }
}

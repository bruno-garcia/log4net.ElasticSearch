using System;
using System.Collections.Specialized;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{
    /// <summary>
    /// Class to support passing in traditional style connection strings and then parsing
    /// them into ElasticSearch components. We create a generic connection string to use for 
    /// basic http request
    /// </summary>
    public class ConnectionBuilder
    {
        public static ElasticsearchConnection BuildElsticSearchConnection(string connectionString)
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
                    if (lookup["rolling"] == "true")
                        index = string.Format("{0}-{1}", index, DateTime.Now.ToString("yyyy.MM.dd"));

                return
                    new ElasticsearchConnection
                    {
                        Server = lookup["Server"],
                        Port = lookup["Port"],
                        Index = index
                    };
            }
            catch
            {
                throw new InvalidOperationException("Not a valid connection string");
            }
        }
    }
}

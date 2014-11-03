using System;
using System.Collections.Specialized;
using System.Data.Common;
using log4net.ElasticSearch.Models;

namespace log4net.ElasticSearch
{
    public static class ConnectionBuilder
    {
        public static Connection Build(string connectionString)
        {
            try
            {
                var builder = new DbConnectionStringBuilder
                    {
                        ConnectionString = connectionString.Replace("{", "\"").Replace("}", "\"")
                    };

                var lookup = new StringDictionary();
                foreach (string key in builder.Keys)
                {
                    lookup[key] = Convert.ToString(builder[key]);
                }

                var index = lookup["Index"];

                if (!string.IsNullOrEmpty(lookup["rolling"]))
                    if (lookup["rolling"] == "true")
                        index = string.Format("{0}-{1}", index, DateTime.Now.ToString("yyyy.MM.dd"));

                return
                    new Connection
                        {
                            Server = lookup["Server"],
                            Port = lookup["Port"],
                            Index = index
                        };
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("'{0}' is not a valid connection string", connectionString), "connectionString", ex);
            }
        }
    }
}
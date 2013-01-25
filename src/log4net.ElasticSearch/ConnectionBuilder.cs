using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace log4net.ElasticSearch
{
    public class ConnectionBuilder
    {
        public static ConnectionSettings BuildElsticSearchConnection(string connectionString)
        {
            string conn = connectionString;
            var builder = new System.Data.Common.DbConnectionStringBuilder();
            builder.ConnectionString = conn.Replace("{", "\"").Replace("}", "\"");
            
            StringDictionary lookup = new StringDictionary();
            foreach (string key in builder.Keys)
            {
                lookup[key] = Convert.ToString(builder[key]);
            }

            return new ConnectionSettings(lookup["Server"], Convert.ToInt32(lookup["Port"])).SetDefaultIndex(lookup["Index"]);
           
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;

namespace log4net.ElasticSearch.InnerExceptions
{
    public class ErrorSettingTemplateException : Exception
    {
        public ErrorSettingTemplateException(ElasticsearchResponse<DynamicDictionary> conn)
            : base(string.Format("{1}{0}Request:{2}{0}Response:{3}{0}",
                Environment.NewLine, conn.ServerError.Error, conn.Request, conn.Response),
                conn.OriginalException)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace log4net.ElasticSearch.Models
{
    public class JsonSerializableException
    {
        public string Type { get; set;  }
        public string Message { get; set; }
        public string HelpLink { get; set; }
        public string Source { get; set; }
        public int HResult { get; set; }
        public string StackTrace { get; set; }
        public System.Collections.IDictionary Data { get; set; }
        public JsonSerializableException InnerException { get; set; }

        public static JsonSerializableException Create(Exception ex)
        {
            if (ex == null)
                return null;

            var serializable = new JsonSerializableException
            {
                Type = ex.GetType().FullName,
                Message = ex.Message,
                HelpLink = ex.HelpLink,
                Source = ex.Source,
#if NET45
                HResult = ex.HResult,
#endif
                StackTrace = ex.StackTrace,
                Data = ex.Data
            };

            if (ex.InnerException != null)
            {
                serializable.InnerException = JsonSerializableException.Create(ex.InnerException);
            }
            return serializable;
        }
    }
}

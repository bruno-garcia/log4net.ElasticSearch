using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Nest;

namespace log4net.ElasticSearch.Models
{
    [ElasticType(Name = "LogEvent")]
    public class LogEvent
    {
        public LogEvent()
        {
            Id = GenerateUniqueId();
        }

        public string Id { get; set; }

        [ElasticProperty(Name = "TimeStamp")]
        public DateTime TimeStamp { get; set; }

        [ElasticProperty(Name = "Message")]
        public string Message { get; set; }

        [ElasticProperty(Name = "MessageObject")]
        public string MessageObject { get; set; }

        [ElasticProperty(Name = "Exception")]
        public string Exception { get; set; }

        [ElasticProperty(Name = "LoggerName")]
        public string LoggerName { get; set; }

        [ElasticProperty(Name = "Domain")]
        public string Domain { get; set; }

        [ElasticProperty(Name = "Identity")]
        public string Identity { get; set; }

        [ElasticProperty(Name = "Level")]
        public string Level { get; set; }

        [ElasticProperty(Name = "ClassName")]
        public string ClassName { get; set; }

        [ElasticProperty(Name = "FileName")]
        public string FileName { get; set; }

        [ElasticProperty(Name = "Name")]
        public string LineNumber { get; set; }

        [ElasticProperty(Name = "FullInfo")]
        public string FullInfo { get; set; }

        [ElasticProperty(Name = "MethodName")]
        public string MethodName { get; set; }

        [ElasticProperty(Name = "Fix")]
        public string Fix { get; set; }

        [ElasticProperty(Name = "Properties")]
        public IDictionary<string, string> Properties { get; set; }

        [ElasticProperty(Name = "UserName")]
        public string UserName { get; set; }

        [ElasticProperty(Name = "ThreadName")]
        public string ThreadName { get; set; }

        [ElasticProperty(Name = "HostName")]
        public string HostName { get; set; }

        /// <summary>
        /// We'll generate an _id for ElasticSearch so it's a predictable format
        /// </summary>
        /// <returns></returns>
        private string GenerateUniqueId()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                // change the size of the array depending on your requirements
                var rndBytes = new byte[8];
                rng.GetBytes(rndBytes);
                return BitConverter.ToString(rndBytes).Replace("-", "");
            }
        }
    }
}

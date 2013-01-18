using System;
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

        [ElasticProperty(Name = "Exception")]
        public string Exception { get; set; }

        [ElasticProperty(Name = "Message")]
        public string Message { get; set; }

        [ElasticProperty(Name = "Date")]
        public DateTime Date { get; set; }

        [ElasticProperty(Name = "Level")]
        public string Level { get; set; }

        [ElasticProperty(Name = "Thread")]
        public string Thread { get; set; }

        [ElasticProperty(Name = "Logger")]
        public string Logger { get; set; }

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

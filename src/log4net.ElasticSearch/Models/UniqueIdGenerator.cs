using System;
using System.Security.Cryptography;

namespace log4net.ElasticSearch.Models
{
    public class UniqueIdGenerator
    {
        public static UniqueIdGenerator Instance = new UniqueIdGenerator();

        /// <summary>
        /// We'll generate an _id for ElasticSearch so it's a predictable format
        /// </summary>
        /// <returns></returns>
        public string GenerateUniqueId()
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
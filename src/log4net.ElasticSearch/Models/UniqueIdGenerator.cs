using System;
using System.Security.Cryptography;

namespace log4net.ElasticSearch.Models
{
    public static class UniqueIdGenerator
    {
        public static string GenerateUniqueId()
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[8];
                randomNumberGenerator.GetBytes(randomBytes);
                return BitConverter.ToString(randomBytes).Replace("-", "");
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public static class ExtensionMethods
    {
         public static int TotalCount<T>(this IEnumerable<IEnumerable<T>> self)
         {
             return self.Sum(inner => inner.Count());
         }
    }
}
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace log4net.ElasticSearch.Extensions
{
    public static class DynamicExtensions
    {
        public static ExpandoObject ToDynamic(this object value)
        {
            IDictionary<string, object> expando = new ExpandoObject();

            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
                expando.Add(property.Name, property.GetValue(value));

            return expando as ExpandoObject;
        }
    }
}

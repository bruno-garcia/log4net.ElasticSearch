using System;
using System.Collections.Generic;
using System.Reflection;
using log4net.ElasticSearch.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace log4net.ElasticSearch
{
    public class CustomDataContractResolver : DefaultContractResolver
    {
        public Dictionary<string, string> FieldNameChanges { get; set; }
        public List<FieldValueReplica> FieldValueReplica { get; set; }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.DeclaringType != typeof(logEvent)) return property;

            if (FieldNameChanges.Count > 0 && FieldNameChanges.TryGetValue(property.PropertyName, out var newValue))
                property.PropertyName = newValue;
            
            return property;
        }
   }
}
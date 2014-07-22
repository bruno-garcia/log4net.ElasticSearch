using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Models
{
    public static class LoggingEventExtensions
    {
        public static readonly string TagsKeyName = "@Tags";

        public static void AddOrSet(this JObject jObject, string key, JToken value)
        {
            JToken token;
            if (jObject.TryGetValue(key, out token))
            {
                var array = token as JArray;
                if (array == null)
                {
                    array = new JArray(token);
                    jObject[key] = array;
                }
                array.Add(value);
            }
            else
            {
                jObject[key] = value;
            }
        }

        public static bool HasKey(this JObject jObject, string key)
        {
            return jObject[key] != null;
        }

        public static void AddTag(this JObject jObject, string tag)
        {
            jObject.AddOrSet(TagsKeyName, tag);
        }
    }
}
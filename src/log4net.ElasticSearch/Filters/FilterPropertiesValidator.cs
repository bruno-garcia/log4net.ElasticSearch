using System;
using System.Linq;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch.Filters
{
    public abstract class FilterPropertiesValidator : IElasticAppenderFilter
    {
        public abstract void PrepareEvent(JObject logEvent, ElasticClient client);

        public virtual void PrepareConfiguration(ElasticClient client)
        {
            Validate();
        }

        public void Validate()
        {
            var invalidProperties =
                GetType().GetProperties()
                    .Where(prop => prop.PropertyType == typeof(string)
                                   && string.IsNullOrEmpty((string)prop.GetValue(this)))
                    .Select(p => p.Name).ToList();

            if (invalidProperties.Any())
            {
                var properties = string.Join(",", invalidProperties);
                throw new InvalidFilterConfigException(
                    string.Format("The properties ({0}) of {1} must be set.", properties, GetType().Name));
            }
        }
    }

    public class InvalidFilterConfigException : Exception
    {
        public InvalidFilterConfigException()
        {
        }

        public InvalidFilterConfigException(string message)
            : base(message)
        {
        }
    }
}
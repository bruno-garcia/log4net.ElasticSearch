using System.Collections.Generic;
using System.Linq;
using log4net.ElasticSearch.Filters;
using log4net.ElasticSearch.InnerExceptions;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch
{
    public class ElasticAppenderFilters : IElasticAppenderFilter
    {
        private readonly List<IElasticAppenderFilter> _filters = new List<IElasticAppenderFilter>();

        public void PrepareConfiguration(ElasticClient client)
        {
            foreach (var filter in _filters)
            {
                filter.PrepareConfiguration(client);
            }
        }

        public void PrepareEvent(JObject logEvent, ElasticClient client)
        {
            foreach (var filter in _filters)
            {
                filter.PrepareEvent(logEvent, client);
            }
        }

        public void AddFilter(IElasticAppenderFilter filter)
        {
            _filters.Add(filter);
        }

        public static void ValidateFilterProperties(IElasticAppenderFilter filter)
        {
            var invalidProperties =
                filter.GetType().GetProperties()
                    .Where(prop => prop.PropertyType == typeof(string)
                                   && string.IsNullOrEmpty((string)prop.GetValue(filter, null)))
                    .Select(p => p.Name).ToList();

            if (invalidProperties.Any())
            {
                var properties = string.Join(",", invalidProperties);
                throw new InvalidFilterConfigException(
                    string.Format("The properties ({0}) of {1} must be set.", properties, filter.GetType().Name));
            }
        }

        #region Helpers for common filters

        // note: the functions are private but the log4net XmlConfigurator can find them

        private void AddAdd(AddValueFilter filter)
        {
            AddFilter(filter);
        }

        private void AddRemove(RemoveKeyFilter filter)
        {
            AddFilter(filter);
        }

        private void AddRename(RenameKeyFilter filter)
        {
            AddFilter(filter);
        }

        private void AddKv(KvFilter filter)
        {
            AddFilter(filter);
        }

        private void AddGrok(GrokFilter filter)
        {
            AddFilter(filter);
        }

        private void AddConvertToArray(ConvertToArrayFilter filter)
        {
            AddFilter(filter);
        }

        #endregion
    }
}
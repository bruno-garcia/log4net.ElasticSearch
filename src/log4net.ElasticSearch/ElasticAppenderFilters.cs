using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                ValidateFilterProperties(filter);
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
                    .Where(prop => !IsValidProperty(prop, filter))
                    .Select(p => p.Name).ToList();

            if (invalidProperties.Any())
            {
                var properties = string.Join(",", invalidProperties);
                throw new InvalidFilterConfigurationException(
                    string.Format("The properties ({0}) of {1} are invalid.", properties, filter.GetType().Name));
            }
        }

        private static bool IsValidProperty(PropertyInfo prop, IElasticAppenderFilter filter)
        {
            var validation = prop.GetCustomAttributes(typeof (IPropertyValidationAttribute), true).FirstOrDefault() as IPropertyValidationAttribute;
            if (validation == null)
            {
                return true;
            }

            return validation.IsValid(prop.GetValue(filter, null));
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

    public interface IPropertyValidationAttribute
    {
        bool IsValid<T>(T value);
    }

    public class PropertyNotEmptyAttribute : Attribute, IPropertyValidationAttribute
    {
        public bool IsValid<T>(T value)
        {
            return InnerIsValid(value as string);
        }

        private bool InnerIsValid(string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }
}
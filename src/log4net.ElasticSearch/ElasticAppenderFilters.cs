using System.Collections.Generic;
using log4net.Core;
using log4net.ElasticSearch.Filters;
using Nest;
using Newtonsoft.Json.Linq;

namespace log4net.ElasticSearch
{
    public class ElasticAppenderFilters : IElasticAppenderFilter, IOptionHandler
    {
        private readonly List<IElasticAppenderFilter> _filters = new List<IElasticAppenderFilter>();

        public void ActivateOptions()
        {
            // todo: validate filters
        }

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

        #endregion
    }
}
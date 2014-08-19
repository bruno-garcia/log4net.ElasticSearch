using System.Collections.Generic;
using Nest;

namespace log4net.ElasticSearch
{
    class BulkProxy
    {
        private readonly object _syncObject;
        private readonly BulkRequest _bulkRequest; 

        public BulkProxy()
        {
            _bulkRequest = new BulkRequest();
            _bulkRequest.Operations = new List<IBulkOperation>();
            _syncObject = new object();
        }

        public int Size
        {
            get { return _bulkRequest.Operations.Count; }
        }

        public void AddIndexOperation<T>(T obj, string indexName, string indexType) where T : class
        {
            var operation = new BulkIndexOperation<T>(obj);
            operation.Index = indexName;
            operation.Type = indexType;

            lock (_syncObject)
            {
                _bulkRequest.Operations.Add(operation);
            }
        }

        public void DoIndex(ElasticClient client, bool async)
        {
            lock (_syncObject)
            {
                if (_bulkRequest.Operations.Count == 0)
                {
                    return;
                }

                if (async)
                {
                    client.BulkAsync(_bulkRequest);
                }
                else
                {
                    client.Bulk(_bulkRequest);
                }

                _bulkRequest.Operations = new List<IBulkOperation>();
            }
        }
    }
}

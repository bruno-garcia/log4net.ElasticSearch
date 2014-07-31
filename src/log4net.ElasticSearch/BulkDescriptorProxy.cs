using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net.ElasticSearch.Filters;
using Nest;

namespace log4net.ElasticSearch
{
    class BulkDescriptorProxy
    {
        private readonly BulkDescriptor _bulkDescriptor;
        private readonly object _syncObject;
        private int _size;

        public BulkDescriptorProxy()
        {
            _bulkDescriptor = new BulkDescriptor();
            _syncObject = new object();
            _size = 0;
        }

        public int Size
        {
            get { return _size; }
        }

        public void AddIndexOperation<T>(Func<BulkIndexDescriptor<T>, BulkIndexDescriptor<T>> bulkIndexSelector) where T : class
        {
            lock (_syncObject)
            {
                _bulkDescriptor.Index(bulkIndexSelector);
                _size++;
            }
        }

        public void DoIndex(ElasticClient client, bool async)
        {
            lock (_syncObject)
            {
                if (_size == 0)
                {
                    return;
                }

                if (async)
                {
                    client.BulkAsync(_bulkDescriptor);
                }
                else
                {
                    client.Bulk(_bulkDescriptor);
                }
                _size = 0;
            }
        }
    }
}

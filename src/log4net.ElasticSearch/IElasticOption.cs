namespace log4net.ElasticSearch
{
    public interface IElasticOption
    {
        void LoadConfiguration();
        void PrepareEvent();
    }

    class AddValue : IElasticOption
    {
        public void LoadConfiguration()
        {
            throw new System.NotImplementedException();
        }

        public void PrepareEvent()
        {
            throw new System.NotImplementedException();
        }
    }
}
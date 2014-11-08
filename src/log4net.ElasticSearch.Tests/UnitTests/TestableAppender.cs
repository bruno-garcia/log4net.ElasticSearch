namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class TestableAppender : ElasticSearchAppender
    {
        readonly IRepository repository;

        public TestableAppender(IRepository repository)
        {
            this.repository = repository;
        }

        protected override IRepository CreateRepository(string connectionString)
        {
            return repository;
        }
    }
}
namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class TestableAppender : ElasticSearchAppender
    {
        readonly IRepository repository;

        public TestableAppender(IRepository repository)
        {
            this.repository = repository;
        }

        public bool? FailSend { private get; set; }

        public bool? FailClose { private get; set; }

        protected override IRepository CreateRepository(string connectionString)
        {
            return repository;
        }

        protected override bool TryAsyncSend(System.Collections.Generic.IEnumerable<Core.LoggingEvent> events)
        {
            return FailSend.HasValue ? !FailSend.Value : base.TryAsyncSend(events);
        }

        protected override bool TryWaitAsyncSendFinish()
        {
            return FailClose.HasValue ? !FailClose.Value : base.TryWaitAsyncSendFinish();
        }
    }
}
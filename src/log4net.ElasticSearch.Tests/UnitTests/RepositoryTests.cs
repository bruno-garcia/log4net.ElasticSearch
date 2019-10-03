using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using log4net.ElasticSearch.Infrastructure;
using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.Tests.UnitTests.Stubs;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class RepositoryTests
    {
        [Fact]
        public void Index_rolls_over_when_date_changes_during_single_call_to_add_multiple_log_entries()
        {
            var logEvents = new[]
                    {
                        new logEvent(), new logEvent(), new logEvent(), new logEvent()
                    };

            using (Clock.Freeze(new DateTime(2015, 01, 01, 23, 59, 58)))
            {
                var httpClientStub = new HttpClientStub(() => Clock.Freeze(Clock.Now.AddSeconds(1)));

                var repository = Repository.Create("Server=localhost;Index=log;Port=9200;rolling=true", httpClientStub, "yyyy.MM.dd");

                repository.Add(logEvents, 0);

                httpClientStub.Items.Count().Should().Be(2);
                httpClientStub.Items.First().Value.Count.Should().Be(2);
                httpClientStub.Items.Second().Value.Count.Should().Be(2);
            }
        }
    }
}
using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using log4net.ElasticSearch.Infrastructure;
using log4net.ElasticSearch.Models;
using log4net.ElasticSearch.Tests.UnitTests.Stubs;
using Uri = log4net.ElasticSearch.Models.Uri;

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

        [Fact]
        public void When_a_user_or_password_have_special_characters_it_should_still_work()
        {
            const string user = "!@#<>,./\\|$%?&*()";
            const string password = "!@#$%=^&*=()_{}:>?><";

            const string encodedUser = "!%40%23%3C%3E%2C.%2F%5C%7C%24%25%3F%26*()";
            const string encodedPass = "!%40%23%24%25%3d%5e%26*%3d()_%7b%7d%3a%3e%3f%3e%3c";
            
            var connectionString = $"User={user};Pwd={password};Server=127.0.0.1;Index=log_test;Port=9200;rolling=false";
            
            var uriBuilder = Uri.For(connectionString);
            var uri = uriBuilder;

            var request = HttpClient.RequestFor(uri);
            request.Address.ToString().Should().ContainEquivalentOf(encodedUser);
            request.Address.ToString().Should().ContainEquivalentOf(encodedPass);
            
            var header = request.Headers["Authorization"];
            header.Should().Be("Basic IUAjPD4sLi9cfCQlPyYqKCk6IUAjJCU9XiYqPSgpX3t9Oj4/Pjw=");
        }
    }
}
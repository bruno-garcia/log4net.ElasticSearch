using System;
using FluentAssertions;
using Xunit;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class UriTests
    {
        [Fact]
        public void Implicit_non_rolling_connectionstring_is_parsed_into_index_uri_without_date_suffix()
        {
            UriFor("Server=localhost;Index=log;Port=9200").
                AbsoluteUri.Should().
                Be("http://localhost:9200/log/logEvent");
        }

        [Fact]
        public void Explicit_non_rolling_connectionstring_is_parsed_into_index_uri_without_date_suffix()
        {
            UriFor("Server=localhost;Index=log;Port=9200;rolling=false").
                AbsoluteUri.Should().
                Be("http://localhost:9200/log/logEvent");
        }

        [Fact]
        public void Rolling_connectionstring_is_parsed_into_index_uri_with_date_suffix()
        {
            using (Clock.Freeze(new DateTime(2015, 1, 5)))
            {
                UriFor("Server=localhost;Index=log;Port=9200;rolling=true").
                    AbsoluteUri.Should().
                    Be("http://localhost:9200/log-2015.01.05/logEvent");
            }
        }

        static Uri UriFor(string connectionString)
        {
            return Models.Uri.For(connectionString);
        }
    }
}
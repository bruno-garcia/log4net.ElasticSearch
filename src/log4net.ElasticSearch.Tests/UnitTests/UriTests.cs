using System;
using FluentAssertions;
using Xunit;
using log4net.ElasticSearch.Infrastructure;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class UriTests
    {
        const string RollingConnectionString = "Server=localhost;Index=log;Port=9200;rolling=true";
        const string ImplicityNonRollingConnectionString = "Server=localhost;Index=log;Port=9200";
        const string ExplicitlyNonRollingConnectionString = "Server=localhost;Index=log;Port=9200;rolling=false";

        [Fact]
        public void Implicit_non_rolling_connectionstring_is_parsed_into_index_uri_without_date_suffix()
        {
            UriFor(ImplicityNonRollingConnectionString).
                AbsoluteUri.Should().
                Be("http://localhost:9200/log/logEvent");
        }

        [Fact]
        public void Explicit_non_rolling_connectionstring_is_parsed_into_index_uri_without_date_suffix()
        {
            UriFor(ExplicitlyNonRollingConnectionString).
                AbsoluteUri.Should().
                Be("http://localhost:9200/log/logEvent");
        }

        [Fact]
        public void Rolling_connectionstring_is_parsed_into_index_uri_with_date_suffix()
        {
            using (Clock.Freeze(new DateTime(2015, 1, 5)))
            {
                UriFor(RollingConnectionString).
                    AbsoluteUri.Should().
                    Be("http://localhost:9200/log-2015.01.05/logEvent");
            }
        }

        [Fact]
        public void Subsequent_calls_for_rolling_connection_string_over_two_days_creates_different_indexes()
        {
            using (Clock.Freeze(new DateTime(2015, 1, 5)))
            {
                UriFor(RollingConnectionString).
                    AbsoluteUri.Should().
                    Be("http://localhost:9200/log-2015.01.05/logEvent");
            }
            using (Clock.Freeze(new DateTime(2015, 1, 6)))
            {
                UriFor(RollingConnectionString).
                    AbsoluteUri.Should().
                    Be("http://localhost:9200/log-2015.01.06/logEvent");
            }
        }

        static Uri UriFor(string connectionString)
        {
            return Models.Uri.For(connectionString);
        }
    }
}
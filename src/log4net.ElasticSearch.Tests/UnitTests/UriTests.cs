using System;
using FluentAssertions;
using log4net.ElasticSearch.Infrastructure;
using Xunit;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class UriTests
    {
        const string RollingConnectionString = "Server=localhost;Index=log;Port=9200;rolling=true";
        const string ImplicityNonRollingConnectionString = "Server=localhost;Index=log;Port=9200";
        const string ExplicitlyNonRollingConnectionString = "Server=localhost;Index=log;Port=9200;rolling=false";
        const string RollingPortLessConnectionString = "Server=localhost;Index=log;rolling=true";
        const string ImplicitlyNonRollingPortLessConnectionString = "Server=localhost;Index=log";
        const string ExplicitlyNonRollingPortLessConnectionString = "Server=localhost;Index=log;rolling=false";
        const string BulkConnectionString = "Server=localhost;Index=log;BufferSize=10";

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
            using (Clock.Freeze(new DateTime(2015, 01, 05)))
            {
                UriFor(RollingConnectionString).
                    AbsoluteUri.Should().
                    Be("http://localhost:9200/log-2015.01.05/logEvent");
            }
        }

        [Fact]
        public void Subsequent_calls_for_rolling_connection_string_over_two_days_creates_different_indexes()
        {
            using (Clock.Freeze(new DateTime(2015, 01, 05)))
            {
                UriFor(RollingConnectionString).
                    AbsoluteUri.Should().
                    Be("http://localhost:9200/log-2015.01.05/logEvent");
            }
            using (Clock.Freeze(new DateTime(2015, 01, 06)))
            {
                UriFor(RollingConnectionString).
                    AbsoluteUri.Should().
                    Be("http://localhost:9200/log-2015.01.06/logEvent");
            }
        }

        [Fact]
        public void Implicit_non_rolling_portless_connectionstring_is_parsed_into_index_uri_without_date_suffix()
        {
            UriFor(ImplicitlyNonRollingPortLessConnectionString).
                AbsoluteUri.Should().
                Be("http://localhost/log/logEvent");
        }

        [Fact]
        public void Connection_string_with_buffersize_over_one_uses_bulk_api_call()
        {
            UriFor(BulkConnectionString).
                AbsoluteUri.Should().
                Be("http://localhost/log/logEvent/_bulk");
        }

        [Fact]
        public void Explicit_non_rolling_portless_connectionstring_is_parsed_into_index_uri_without_date_suffix()
        {
            UriFor(ExplicitlyNonRollingPortLessConnectionString).
                AbsoluteUri.Should().
                Be("http://localhost/log/logEvent");
        }

        [Fact]
        public void Rolling_portless_connectionstring_is_parsed_into_index_uri_wit_date_suffix()
        {
            using (Clock.Freeze(new DateTime(2015,3,31)))
            {
                UriFor(RollingPortLessConnectionString).
                    AbsoluteUri.Should().
                    Be("http://localhost/log-2015.03.31/logEvent");
            }
        }

        static Uri UriFor(string connectionString)
        {
            return Models.Uri.For(connectionString);
        }
    }
}
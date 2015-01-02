using System;
using System.Linq;
using FluentAssertions;
using log4net.ElasticSearch.Tests.Infrastructure.Builders;
using log4net.ElasticSearch.Tests.UnitTests.Stubs;
using Xunit;

namespace log4net.ElasticSearch.Tests.UnitTests
{
    public class SslTests : IDisposable
    {
        public void Dispose()
        {
            
        }

        [Fact]
        public void When_a_https_scheme_is_provided_it_should_create_a_URI_with_https()
        {
            string ConnectionString = "Scheme=https;Server=localhost;Index=log_test;Port=9200;rolling=true";
            var uriBuilder = log4net.ElasticSearch.Models.Uri.Create(ConnectionString);
            Uri uri = uriBuilder;
            uri.Scheme.Should().Be("https");
        }

        [Fact]
        public void When_no_scheme_is_provided_it_should_create_a_URI_with_http()
        {
            string ConnectionString = "Server=localhost;Index=log_test;Port=9200;rolling=true";
            var uriBuilder = log4net.ElasticSearch.Models.Uri.Create(ConnectionString);
            Uri uri = uriBuilder;
            uri.Scheme.Should().Be("http");
        }
    }
}
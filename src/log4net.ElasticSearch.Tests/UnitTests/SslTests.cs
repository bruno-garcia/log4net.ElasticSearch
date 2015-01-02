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
        const string ConnectionString = "Scheme=https;Server=localhost;Index=log_test;Port=9200;rolling=true";

        public void Dispose()
        {
            
        }

        [Fact]
        public void When_number_of_LogEvents_is_less_than_Buffer_nothing_is_sent_to_ElasticSearch()
        {
            var uriBuilder = log4net.ElasticSearch.Models.Uri.Create(ConnectionString);
            Uri uri = uriBuilder;
            uri.Scheme.Should().Be("https");
        }
    }
}
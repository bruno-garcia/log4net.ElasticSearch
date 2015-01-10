using System;
using System.Linq;
using FluentAssertions;
using log4net.ElasticSearch.Infrastructure;
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
            var uriBuilder = log4net.ElasticSearch.Models.Uri.For(ConnectionString);
            Uri uri = uriBuilder;
            uri.Scheme.Should().Be("https");
        }

        [Fact]
        public void When_no_scheme_is_provided_it_should_create_a_URI_with_http()
        {
            string ConnectionString = "Server=localhost;Index=log_test;Port=9200;rolling=true";
            var uriBuilder = log4net.ElasticSearch.Models.Uri.For(ConnectionString);
            Uri uri = uriBuilder;
            uri.Scheme.Should().Be("http");
        }

        [Fact]
        public void When_no_user_password_is_provided_it_should_create_a_request_with_no_auth_header()
        {
            string ConnectionString = "Server=localhost;Index=log_test;Port=9200;rolling=true";
            var uriBuilder = log4net.ElasticSearch.Models.Uri.For(ConnectionString);
            Uri uri = uriBuilder;
            uri.Scheme.Should().Be("http");

            var request = HttpClient.RequestFor(uri);

            request.Headers.AllKeys.Should().NotContain("Authorization");
        }

        [Fact]
        public void When_a_user_and_password_are_provided_it_should_create_a_URI_with_user_password()
        {
            string ConnectionString = "Scheme=https;User=user1;Pwd=password1;Server=localhost;Index=log_test;Port=9200;rolling=true";
            var uriBuilder = log4net.ElasticSearch.Models.Uri.For(ConnectionString);
            Uri uri = uriBuilder;
            uri.UserInfo.Should().Be("user1:password1");
        }

        [Fact]
        public void When_a_user_and_password_are_provided_it_should_create_a_web_request_with_a_basic_auth_header()
        {
            string ConnectionString = "Scheme=https;User=user1;Pwd=password1;Server=localhost;Index=log_test;Port=9200;rolling=true";
            var uriBuilder = log4net.ElasticSearch.Models.Uri.For(ConnectionString);
            Uri uri = uriBuilder;

            var request = HttpClient.RequestFor(uri);

            request.Headers.AllKeys.Should().Contain("Authorization");

            var header = request.Headers["Authorization"];
            header.Should().Be("Basic dXNlcjE6cGFzc3dvcmQx");
        }
    }
}
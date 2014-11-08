using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using log4net.Core;

namespace log4net.ElasticSearch.Tests.UnitTests.Stubs
{
    public class ErrorHandlerStub : IErrorHandler
    {
        readonly ConcurrentBag<Exception> exceptions;

        public ErrorHandlerStub()
        {
            exceptions = new ConcurrentBag<Exception>();
        }

        public IEnumerable<Exception> Exceptions
        {
            get { return exceptions; }
        }

        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            Error(message, e);
        }

        public void Error(string message, Exception e)
        {
            exceptions.Add(e);
        }

        public void Error(string message) {}
    }
}
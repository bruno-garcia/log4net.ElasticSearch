using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using log4net.Core;

namespace log4net.ElasticSearch.Tests.UnitTests.Stubs
{
    public class ErrorHandlerStub : IErrorHandler
    {
        readonly ConcurrentBag<Exception> exceptions;
        readonly ConcurrentBag<string> messages; 

        public ErrorHandlerStub()
        {
            exceptions = new ConcurrentBag<Exception>();
            messages = new ConcurrentBag<string>();
        }

        public IEnumerable<Exception> Exceptions
        {
            get { return exceptions; }
        }

        public IEnumerable<string> Messages
        {
            get { return messages; }
        }

        public void Error(string message, Exception e, ErrorCode errorCode)
        {
            Error(message, e);
        }

        public void Error(string message, Exception e)
        {
            exceptions.Add(e);
            messages.Add(message);
        }

        public void Error(string message)
        {
            messages.Add(message);
        }
    }
}
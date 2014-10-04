using System;

namespace log4net.ElasticSearch.Tests
{
    public class LogEventBuilder
    {
        string className;
        string domain;
        string exception;
        string fileName;
        string fix;
        string fullInfo;
        string identity;
        string level;
        string lineNumber;
        string message;
        DateTime timeStamp;

        LogEventBuilder()
        {
        }

        public static LogEventBuilder Default
        {
            get { return new LogEventBuilder().WithDefaults(); }
        }

        public static LogEventBuilder Empty
        {
            get { return new LogEventBuilder(); }
        }

        public LogEvent LogEvent
        {
            get { return this; }
        }

        public LogEventBuilder WithMessage(string value)
        {
            message = value;
            return this;
        }

        public LogEventBuilder WithClassName(string value)
        {
            className = value;
            return this;
        }

        LogEventBuilder WithDefaults()
        {
            className = "IntegrationTestClass";
            domain = "TestDomain";
            exception = "This is a test exception";
            fileName = "c:\test\file.txt";
            fix = "none";
            fullInfo = "A whole bunch of error info dump";
            identity = "localhost\\user";
            level = "9";
            lineNumber = "99";
            timeStamp = DateTime.UtcNow;

            return this;
        }

        public static implicit operator LogEvent(LogEventBuilder builder)
        {
            return new LogEvent
                {
                    ClassName = builder.className,
                    Domain = builder.domain,
                    Exception = builder.exception,
                    FileName = builder.fileName,
                    Fix = builder.fix,
                    FullInfo = builder.fullInfo,
                    Identity = builder.identity,
                    Level = builder.level,
                    LineNumber = builder.lineNumber,
                    TimeStamp = builder.timeStamp,
                    Message = builder.message
                };
        }
    }
}
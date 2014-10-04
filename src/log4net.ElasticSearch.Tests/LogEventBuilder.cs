using System;

namespace log4net.ElasticSearch.Tests
{
    public class LogEventBuilder
    {
        readonly string className;
        readonly string domain;
        readonly string exception;
        readonly string fileName;
        readonly string fix;
        readonly string fullInfo;
        readonly string identity;
        readonly string level;
        readonly string lineNumber;
        readonly DateTime timeStamp;

        LogEventBuilder()
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
        }

        public static LogEventBuilder Default
        {
            get { return new LogEventBuilder(); }
        }

        public LogEvent LogEvent
        {
            get { return this; }
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
                };
        }
    }
}
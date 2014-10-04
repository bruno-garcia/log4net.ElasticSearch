using System;
using System.Globalization;
using System.Linq;

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

        LogEventBuilder() {}

        public static LogEventBuilder Default
        {
            get { return new LogEventBuilder().WithDefaults(); }
        }

        public LogEvent LogEvent
        {
            get { return this; }
        }

        LogEventBuilder WithDefaults()
        {            
            className = Faker.Lorem.Words(1).Single();
            domain = Faker.Lorem.Words(1).Single();
            exception = Faker.Lorem.Sentence(50);
            fileName = Faker.Lorem.Sentence(5);
            fix = Faker.Lorem.Words(1).Single();
            fullInfo = Faker.Lorem.Sentence(100);
            identity = Faker.Lorem.Sentence(2);
            level = Faker.RandomNumber.Next(10).ToString(CultureInfo.InvariantCulture);
            lineNumber = Faker.RandomNumber.Next(1000).ToString(CultureInfo.InvariantCulture);
            timeStamp = DateTime.UtcNow;
            message = Faker.Lorem.Sentence(20);

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
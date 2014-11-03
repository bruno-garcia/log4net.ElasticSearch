using System;
using System.Globalization;
using System.Linq;

namespace log4net.ElasticSearch.Tests.Infrastructure
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

        public Models.logEvent LogEvent
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

        public static implicit operator Models.logEvent(LogEventBuilder builder)
        {
            return new Models.logEvent
                {
                    className = builder.className,
                    domain = builder.domain,
                    exception = builder.exception,
                    fileName = builder.fileName,
                    fix = builder.fix,
                    fullInfo = builder.fullInfo,
                    identity = builder.identity,
                    level = builder.level,
                    lineNumber = builder.lineNumber,
                    timeStamp = builder.timeStamp.ToString(CultureInfo.InvariantCulture),
                    message = builder.message
                };
        }
    }
}
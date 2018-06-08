using System;
using System.Globalization;
using System.Linq;

namespace log4net.ElasticSearch.Tests.Infrastructure.Builders
{
    public class LogEventBuilder
    {
        string className;
        string domain;
        object exception;
        string fileName;
        string fix;
        string fullInfo;
        string identity;
        string level;
        string lineNumber;
        string message;
        string timeStamp;

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
            var faker = new Bogus.Faker();
            className = faker.Lorem.Words(1).Single();
            domain = faker.Lorem.Words(1).Single();
            exception = new object();
            fileName = faker.Lorem.Sentence(5);
            fix = faker.Lorem.Words(1).Single();
            fullInfo = faker.Lorem.Sentence(100);
            identity = faker.Lorem.Sentence(2);
            level = faker.Random.Number(10).ToString(CultureInfo.InvariantCulture);
            lineNumber = faker.Random.Number(1000).ToString(CultureInfo.InvariantCulture);
            timeStamp = DateTime.UtcNow.ToString("O");
            message = faker.Lorem.Sentence(20);

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
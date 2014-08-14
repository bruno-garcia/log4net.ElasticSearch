log4net.ElasticSearch
=====================

log4net.ElasticSearch is a module for the [log4net](http://logging.apache.org/log4net/) library to log messages to the [ElasticSearch](http://www.elasticsearch.org) document database. ElasticSearch offers robust full-text search engine and analyzation so that errors and messages can be indexed quickly and searched easily.

### Features:
* Supports .NET 4.0+
* Easy installation and setup via [Nuget](https://nuget.org/packages/log4net.ElasticSearch/)
* Uses the excellent [NEST](https://github.com/Mpdreamz/NEST) library which has great support for the newest ElasticSearch revisions
* Ability to analyze the log event before sending it to elasticsearch using built-in filters and custom filters like [logstash]().

### Filters:
* Add - add new key and value to the event.
* Remove - remove key from the event.
* Rename - rename key to another name.
* Kv - analyze value (default is to analyze the 'Message' value) and export key-value pairs using regex (similar to logstash's kv filter).
* Grok - analyze value (default is 'Message') using custom regex and saved patterns (similar to logstash's grok filter).

#### Custom filter:
To add your own filters you just need to implement the interface IElasticAppenderFilter on your assembly and configure it on the log4net configuration file.

### Usage:
Please see the [DOCUMENTATION](https://github.com/jptoto/log4net.ElasticSearch/wiki/0-Documentation) Wiki page to begin logging errors to ElasticSearch!

### Issues:
I do my best to reply to issues or questions ASAP. Please use the [ISSUES](https://github.com/jptoto/log4net.ElasticSearch/issues) page to submit questions or errors.

### Configuration Example:
```xml
<appender name="ElasticSearchAppender" type="log4net.ElasticSearch.ElasticSearchAppender, log4net.ElasticSearch">
      <Server>localhost</Server>
      <Port>9200</Port>
      <IndexName>log_test_%{+yyyy-MM-dd}</IndexName>
      <IndexType>LogEvent</IndexType>
      <Bulksize>2000</Bulksize>
      <BulkIdleTimeout>10000</BulkIdleTimeout>
      <IndexAsync>False</IndexAsync>
      <FixedFields>Partial</FixedFields>
      <Template>
        <Name>templateName</Name>
        <FileName>path2template.json</FileName>
      </Template>

      <!-- all filters goes in ElasticFilters tag -->
      <ElasticFilters>
        <Add>
          <Key>@type</Key>
          <Value>Special</Value>
        </Add>
        <Add>
          <Key>SmartValue</Key>
          <Value>the type is %{@type}</Value>
        </Add>
        <Remove>
          <Key>@type</Key>
        </Remove>
        <Filter type="log4net.ElasticSearch.Filters.RenameKeyFilter, log4net.ElasticSearch">
          <Key>SmartValue</Key>
          <RenameTo>SmartValue2</RenameTo>
        </Filter>
        <Kv>
        	<SourceKey>Message</SourceKey>
        	<ValueSplit>:=</ValueSplit>
        	<FieldSplit> ,</FieldSplit>
        </kv>
        <Grok>
          <SourceKey>Message</SourceKey>
          <Pattern>the message is %{WORD:Message} and guid %{UUID:the_guid}</Pattern>
          <Overwrite>true</Overwrite>
        </Grok>
      </ElasticFilters>
    </appender>
```

### License:
[MIT License](https://github.com/jptoto/log4net.ElasticSearch/blob/master/LICENSE)

### Thanks:
Many thanks to [@mpdreamz](https://github.com/Mpdreamz) and the team for their great work on the NEST library!
Thanks to [@gluck](https://github.com/gluck) for the package [il-repack](https://github.com/gluck/il-repack)

### Build status:

| Status | Provider |
| ------ | -------- |
| [![Build status][TravisImg]][TravisLink] | Mono CI provided by [travis-ci][] |
| [![Build Status][AppVeyorImg]][AppVeyorLink] | Windows CI provided by [AppVeyor][] (without tests for now) |

[TravisImg]:https://travis-ci.org/urielha/log4net.ElasticSearch.svg?branch=master
[TravisLink]:https://travis-ci.org/urielha/log4net.ElasticSearch
[AppVeyorImg]:https://ci.appveyor.com/api/projects/status/a1nc4olvjw42728s
[AppVeyorLink]:https://ci.appveyor.com/project/urielha/log4net-elasticsearch

[travis-ci]:https://travis-ci.org/
[AppVeyor]:http://www.appveyor.com/

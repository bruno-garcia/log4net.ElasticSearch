log4net.ElasticSearch
=====================

log4net.ElasticSearch is a module for the [log4net](http://logging.apache.org/log4net/) library to log messages to the [ElasticSearch](http://www.elasticsearch.org) document database. ElasticSearch offers robust full-text searchign an analyzation so that errors and messages can be indexed quickly and searched easily.

### Features:
* Supports .NET 4.0+
* Easy installation and setup via [Nuget](https://nuget.org/packages/log4net.ElasticSearch/)
* Uses the excellent [NEST](https://github.com/Mpdreamz/NEST) library which has great support for the newest ElasticSearch revisions

### Usage:
Please see the [DOCUMENTATION](https://github.com/jptoto/log4net.ElasticSearch/wiki/0-Documentation) Wiki page to begin logging errors to ElasticSearch!

### Issues:
I do my best to reply to issues or questions ASAP. Please use the [ISSUES](https://github.com/jptoto/log4net.ElasticSearch/issues) page to submit questions or errors.

### License:
[MIT License](https://github.com/jptoto/log4net.ElasticSearch/blob/master/LICENSE)

### Thanks:
Many thanks to [@mpdreamz](https://github.com/Mpdreamz) and the team for their great work on the NEST library!

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

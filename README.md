log4net.ElasticSearch
=====================

[![NuGet Status](http://img.shields.io/badge/nuget-2.3.0-green.svg)](https://www.nuget.org/packages/log4net.ElasticSearch/)

[![Build status](https://ci.appveyor.com/api/projects/status/t877sp1e5eleye4n/branch/master)](https://ci.appveyor.com/project/jptoto/log4net-elasticsearch/branch/master)

log4net.ElasticSearch is a module for the [log4net](http://logging.apache.org/log4net/) library to log messages to the [ElasticSearch](http://www.elasticsearch.org) document database. ElasticSearch offers robust full-text searching an analyzation so that errors and messages can be indexed quickly and searched easily.

### Features:
* Supports .NET 4.0+
* Easy installation and setup via [Nuget](https://nuget.org/packages/log4net.ElasticSearch/)
* Full support for the Elasticsearch _bulk API for buffered logging

### Usage:
Please see the [DOCUMENTATION](https://github.com/jptoto/log4net.ElasticSearch/wiki/0-Documentation) Wiki page to begin logging errors to ElasticSearch!

### Issues:
I do my best to reply to issues or questions ASAP. Please use the [ISSUES](https://github.com/jptoto/log4net.ElasticSearch/issues) page to submit questions or errors.

### License:
[MIT License](https://github.com/jptoto/log4net.ElasticSearch/blob/master/LICENSE)

### Thanks:
- [@mpdreamz](https://github.com/Mpdreamz) and the team for their great work on the NEST library!
- [@mastoj](https://github.com/mastoj)
- [@kjersti](https://github.com/kjersti)
- [@hippasus](https://github.com/hippasus)
- [@jc74](https://github.com/jc74)
- [@mickdelaney](https://github.com/mickdelaney)
- [@yavari](https://github.com/yavari)
- [@nickcanz](https://github.com/nickcanz)
- [@wallymathieu](https://github.com/mwallymathieu)
- [@TheSpy](https://github.com/TheSpy)
- [@ttingen](https://github.com/ttingen)
- [@aateeque](https://github.com/aateeque)

### How to build
Use the psake.cmd file in the base directory for all build tasks.

.\psake.cmd

This will run the default task which compiles and runs the tests.

.\psake.cmd package

This task compiles the solution, runs the tests then creates a nuget package

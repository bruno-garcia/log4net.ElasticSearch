log4net.ElasticSearch
=====================

[![NuGet Status](http://img.shields.io/badge/nuget-2.3.4-green.svg)](https://www.nuget.org/packages/log4net.ElasticSearch/)

[![Build status](https://ci.appveyor.com/api/projects/status/t877sp1e5eleye4n/branch/master)](https://ci.appveyor.com/project/jptoto/log4net-elasticsearch/branch/master)

log4net.ElasticSearch is a module for the [log4net](http://logging.apache.org/log4net/) library to log messages to the [ElasticSearch](http://www.elasticsearch.org) document database. ElasticSearch offers robust full-text searching an analyzation so that errors and messages can be indexed quickly and searched easily.

### Features:
* Supports .NET 4.0+
* Easy installation and setup via [Nuget](https://nuget.org/packages/log4net.ElasticSearch/)
* Full support for the Elasticsearch _bulk API for buffered logging

### Usage:
Please see the [DOCUMENTATION](https://github.com/jptoto/log4net.ElasticSearch/wiki) Wiki page to begin logging errors to ElasticSearch!

### Example log4net Document in Elasticsearch

```json
{
	"_index": "log-2016.02.12",
	"_type": "logEvent",
	"_id": "AVLXHEwEJfnUYPcgkJ5r",
	"_version": 1,
	"_score": 1,
	"_source": {
		"timeStamp": "2016-02-12T20:11:41.5864254Z",
		"message": "Something broke.",
		"messageObject": {},
		"exception": {
			"Type": "System.Exception",
			"Message": "There was a system error",
			"HelpLink": null,
			"Source": null,
			"HResult": -2146233088,
			"StackTrace": null,
			"Data": {
				"CustomProperty": "CustomPropertyValue",
				"SystemUserID": "User43"
			},
			"InnerException": null
		},
		"loggerName": "log4net.ES.Example.Program",
		"domain": "log4net.ES.Example.vshost.exe",
		"identity": "",
		"level": "ERROR",
		"className": "log4net.ES.Example.Program",
		"fileName": "C:\\Users\\jtoto\\projects\\log4net.ES.Example\\log4net.ES.Example\\Program.cs",
		"lineNumber": "26",
		"fullInfo": "log4net.ES.Example.Program.Main(C:\\Users\\jtoto\\projects\\log4net.ES.Example\\log4net.ES.Example\\Program.cs:26)",
		"methodName": "Main",
		"fix": "LocationInfo, UserName, Identity, Partial",
		"properties": {
			"log4net:Identity": "",
			"log4net:UserName": "JToto",
			"log4net:HostName": "JToto01",
			"@timestamp": "2016-02-12T20:11:41.5864254Z"
		},
		"userName": "JToto",
		"threadName": "9",
		"hostName": "JTOTO01"
	}
}
```

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
- [@moconnell](https://github.com/moconnell)

### How to build
Use the psake.cmd file in the base directory for all build tasks.

.\psake.cmd

This will run the default task which compiles and runs the tests.

.\psake.cmd package

This task compiles the solution, runs the tests then creates a nuget package

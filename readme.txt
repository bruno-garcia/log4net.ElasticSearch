***										***
*** Potentially breaking changes in 1.2 ***
***										***

- Reverted to a more strongly typed class for logging messages which should make searching easier
- DROPPED dependencies on NEST, Elasticsearch.NET, and Json.NET.
- Added a @timestamp field to the properties array to match logstash formatted timestamps
- Modified the rolling timestamp index format to use YYYY.MM.DD instead of YYYY-MM-DD, also to match standard ES/logstash format
- Modified the TimeStamp field to use UTC formatted date/times

The log4net.ElasticSearch test suite is a set of integration tests, meaning you should
have an instance of ElasticSearch installed in order to run them.

You can always find the latest version at http://www.elasticsearch.org/download/

Convenient Windows installers can be found at https://github.com/rgl/elasticsearch-setup
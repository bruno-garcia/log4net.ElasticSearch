curl -X DELETE http://localhost:9200/Log

curl -X POST http://localhost:9200/log -d @log-index-spec.json

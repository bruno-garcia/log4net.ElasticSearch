echo "Attempting to delete log index (OK if not found)"
curl -X DELETE http://localhost:9200/log
echo ""
echo "Attempting to create log index"
curl -X POST http://localhost:9200/log -d @log-index-spec.json
echo ""

#!/bin/bash

# download elasticsearch and unzip:
wget "https://download.elasticsearch.org/elasticsearch/elasticsearch/elasticsearch-1.3.1.zip"
unzip "elasticsearch-1.3.1.zip" -d ElasticSearchDirectory

ls 
ls ElasticSearchDirectory
sleep 20 &
kill $!
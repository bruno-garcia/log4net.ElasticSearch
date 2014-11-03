#!/usr/bin/env bash

sudo apt-get install mono-devel mono-xbuild mono-runtime -y
sudo apt-get install openjdk-7-jre-headless -y
sudo wget -O - http://packages.elasticsearch.org/GPG-KEY-elasticsearch | sudo apt-key add -

echo "deb http://packages.elasticsearch.org/elasticsearch/1.3/debian stable main" >> /etc/apt/sources.list
sudo apt-get update
sudo apt-get install elasticsearch -y
sudo update-rc.d elasticsearch defaults 95 10
sudo /etc/init.d/elasticsearch start

echo cd \/vagrant > /home/vagrant/.bashrc
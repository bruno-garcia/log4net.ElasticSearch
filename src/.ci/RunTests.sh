#!/bin/sh

mono --runtime=v4.0 .nuget/NuGet.exe install NUnit.Runners -Version 2.6.1 -o packages

runTest(){
    mono --runtime=v4.0 packages/NUnit.Runners.2.6.1/tools/nunit-console.exe -noxml -nodots -labels $@
   if [ $? -ne 0 ]
   then   
     exit 1
   fi
}

runTest log4net.ElasticSearch.Tests/bin/Debug/log4net.ElasticSearch.Tests.dll -exclude=Performance

exit $?

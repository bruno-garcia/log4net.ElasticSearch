%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe src\log4net.ElasticSearch.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger
copy LICENSE bin
copy readme.txt bin
NuGet.exe pack log4net.ElasticSearch.nuspec -Basepath bin
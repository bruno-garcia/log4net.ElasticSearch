%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe src\log4net.ElasticSearch.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger
rem %~dp0/tools/xunit/xunit.console.clr4.exe %~dp0/src/log4net.ElasticSearch.Tests/bin/Release/log4net.ElasticSearch.Tests.dll
call src\ilrepack.bat

copy LICENSE bin_pack
copy readme.txt bin_pack

src\.nuget\NuGet.exe pack log4stash.nuspec -Basepath bin_pack

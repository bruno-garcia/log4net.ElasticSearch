%windir%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe src\log4net.ElasticSearch.sln /t:Clean,Rebuild /p:Configuration=Release /fileLogger
%~dp0src\packages\NUnit.Runners.2.6.3\tools\nunit-console.exe -noxml -nodots -labels %~dp0src\log4net.ElasticSearch.Tests\bin\Release\log4stash.Tests.dll

call src\ilrepack.bat

copy LICENSE bin_pack
copy readme.txt bin_pack

src\.nuget\NuGet.exe pack log4stash.nuspec -Basepath bin_pack

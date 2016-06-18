@echo off

if '%1'=='/?' goto help
if '%1'=='-help' goto help
if '%1'=='-h' goto help

tools\nuget\nuget.exe restore src\log4net.ElasticSearch.sln

powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0\src\packages\psake.4.6.0\tools\psake.ps1' '%~dp0\scripts\build.ps1' %*; if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }"
exit /B %errorlevel%

:help
powershell -NoProfile -ExecutionPolicy Bypass -Command "& '%~dp0\src\packages\psake.4.6.0\tools\psake.ps1' -help"

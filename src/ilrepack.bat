echo off
set solutionDir=%~dp0
set sourceDir=%solutionDir%..\bin\lib\
set targetDir=%solutionDir%..\bin_pack\lib\

set ILRepackExe=%solutionDir%packages\ILRepack.1.25.0\tools\ILRepack.exe

mkdir "%targetDir%"
mkdir "%targetDir%net40"
mkdir "%targetDir%net45"

"%ILRepackExe%" "%sourceDir%net40\log4stash.dll" "%sourceDir%net40\Elasticsearch.Net.dll" "%sourceDir%net40\Nest.dll" "%sourceDir%net40\Newtonsoft.Json.dll" /internalize /out:"%targetDir%net40\log4stash.dll"

"%ILRepackExe%" "%sourceDir%net45\log4stash.dll" "%sourceDir%net45\Elasticsearch.Net.dll" "%sourceDir%net45\Nest.dll" "%sourceDir%net45\Newtonsoft.Json.dll" /internalize /out:"%targetDir%net45\log4stash.dll"
echo on
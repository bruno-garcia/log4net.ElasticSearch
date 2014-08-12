set solutionDir=%~p0
set targetPath=%~1
set targetDir=%~p1

mkdir "%targetDir%merged"
"%solutionDir%packages\ILRepack.1.25.0\tools\ILRepack.exe" "%targetPath%" "%targetDir%Elasticsearch.Net.dll" "%targetDir%Nest.dll" "%targetDir%Newtonsoft.Json.dll" /parallel /internalize /out:"%targetDir%merged\merged.log4net.ElasticSearch.dll"
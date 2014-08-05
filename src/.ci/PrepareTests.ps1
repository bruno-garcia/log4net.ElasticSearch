# download elasticsearch:
$clnt = new-object System.Net.WebClient
$url = "https://download.elasticsearch.org/elasticsearch/elasticsearch/elasticsearch-1.3.1.zip"
$file = (Get-Location).Path + "\elasticsearch-1.3.1.zip"
$clnt.DownloadFile($url,$file)

# Unzip the file to specified location
$shell_app=new-object -com shell.application 
$zip_file = $shell_app.namespace($file) 

rmdir elasticsearch-1.3.1
$destination = $shell_app.namespace((Get-Location).Path) 
$destination.Copyhere($zip_file.items())

Start-Job -ScriptBlock {elasticsearch-1.3.1\bin\elasticsearch.bat}

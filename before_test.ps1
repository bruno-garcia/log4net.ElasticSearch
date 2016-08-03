$ErrorActionPreference = "Continue"

function Expand-ZIPFile($file, $destination)
{
    $shell = new-object -com shell.application
    $zip = $shell.NameSpace($file)
    foreach($item in $zip.items())
    {
        $shell.Namespace($destination).copyhere($item)
    }
}

$url = "https://download.elastic.co/elasticsearch/release/org/elasticsearch/distribution/zip/elasticsearch/2.3.4/elasticsearch-2.3.4.zip"
$output = "c:\Windows\temp\es.zip"

Invoke-WebRequest -Uri $url -OutFile $output

Remove-Item 'c:\Elasticsearch' -Force -Recurse

New-Item -ItemType 'Directory' -Path 'c:\Elasticsearch' -Force

Expand-ZIPFile -File "C:\Windows\Temp\es.zip" -Destination "c:\Elasticsearch"

$env:JAVA_HOME = Get-ChildItem -Path 'C:\Program Files\Java\jdk1.8.0' | Select -ExpandProperty FullName

Start-Process -FilePath 'C:\Elasticsearch\elasticsearch-2.3.4\bin\elasticsearch.bat' -WorkingDirectory 'c:\Elasticsearch\elasticsearch-2.3.4\bin'
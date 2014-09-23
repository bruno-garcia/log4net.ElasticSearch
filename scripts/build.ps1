task default -depends Clean, Compile, Test

$solution = "C:\GitHub\log4net.ElasticSearch\src\log4net.ElasticSearch.sln"

task Clean {
    exec {
        msbuild $solution /t:Clean /v:Quiet /nologo
    }
}

task Compile {
    exec {
        msbuild $solution /t:Build /v:Quiet /nologo
    }
}

task Test {

}

task ? -Description "Helper to display task info" {
    Write-Documentation
}
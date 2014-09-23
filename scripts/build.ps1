properties {
    $base_dir   = (Get-Item (Resolve-Path .)).Parent.FullName
    $sln_path   = "$base_dir\src\log4net.ElasticSearch.sln"
}

task default -depends Clean, Compile, Test

task Clean {
    exec {
        msbuild $sln_path /t:Clean /v:Quiet /nologo
    }
}

task Compile {
    exec {
        msbuild $sln_path /t:Build /v:Quiet /nologo
    }
}

task Test {

}

task ? -Description "Helper to display task info" {
    Write-Documentation
}
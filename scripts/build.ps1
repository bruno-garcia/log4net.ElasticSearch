properties {
    $base_dir   = (Get-Item (Resolve-Path .)).Parent.FullName
    $sln_path   = "$base_dir\src\log4net.ElasticSearch.sln"
    $tests_path = "$base_dir\src\log4net.ElasticSearch.Tests\bin\Debug\log4net.ElasticSearch.Tests.dll"
    $xunit_path = "$base_dir\src\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.exe"
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
    exec {
        & $xunit_path $tests_path
    }
}

task ? -Description "Helper to display task info" {
    Write-Documentation
}
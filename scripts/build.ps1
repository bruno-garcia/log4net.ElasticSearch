properties {
    $base_dir   = (Get-Item (Resolve-Path .)).Parent.FullName
    $bin_dir    = "$base_dir\bin"
    $sln_path   = "$base_dir\src\log4net.ElasticSearch.sln"
    $config     = "Debug"
    $tests_path = "$base_dir\src\log4net.ElasticSearch.Tests\bin\$config\log4net.ElasticSearch.Tests.dll"
    $xunit_path = "$base_dir\src\packages\xunit.runners.1.9.2\tools\xunit.console.clr4.exe"
    $dirs       = @($bin_dir)
}

task default        -depends Clean, Compile, Test

task Clean {
    $dirs | % { Recreate-Directory $_ }
}

task Compile {
    exec {
        msbuild $sln_path /p:Configuration=$config /t:Rebuild /v:Quiet /nologo
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


function Recreate-Directory($directory) {
    if (Test-Path $directory) {
        Write-Host -NoNewline  "`tDeleting $directory"
        Remove-Item $directory -Recurse -Force | out-null
        Write-Host "...Done"
    }

    Write-Host -NoNewline  "`tCreating $directory"
    New-Item $directory -Type Directory | out-null
    Write-Host "...Done"
}
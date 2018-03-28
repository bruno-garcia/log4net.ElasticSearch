properties {
    $base_dir       = (Get-Item (Resolve-Path .)).Parent.FullName
    $bin_dir        = "$base_dir\bin"
    $sln_path       = "$base_dir\src\log4net.ElasticSearch.sln"
    $config         = "Debug"
    
	$nuget_csproj_path = "$base_dir\src\log4net.ElasticSearch\log4net.ElasticSearch.csproj"

	$tests_base_path_netcoreapp = "$base_dir\src\log4net.ElasticSearch.Tests\bin\$config\netcoreapp2.0"
	$tests_path_netcoreapp      = "$tests_base_path_netcoreapp\log4net.ElasticSearch.Tests.dll"
    $xunit_path_netcoreapp      = "$base_dir\src\packages\xunit.runner.console.2.3.1\tools\netcoreapp2.0\xunit.console.dll"

	$tests_base_path_net45 = "$base_dir\src\log4net.ElasticSearch.Tests\bin\$config\net45"
	$tests_path_net45      = "$tests_base_path_net45\log4net.ElasticSearch.Tests.dll"
    $xunit_path_net452     = "$base_dir\src\packages\xunit.runner.console.2.3.1\tools\net452\xunit.console.exe"

    $dirs           = @($bin_dir)
    $artefacts      = @("$base_dir\LICENSE", "$base_dir\readme.txt")
    $nuget_path     = "$base_dir\tools\nuget\NuGet.exe"
    $nuspec_path    = "$base_dir\scripts\log4net.ElasticSearch.nuspec"
}

task default        -depends Clean, Compile, Test
task Package        -depends default, CreateNugetPackage

task Clean {
    $dirs | % { Recreate-Directory $_ }
}

task Compile {
    exec {
        dotnet msbuild $sln_path /p:Configuration=$config /t:Rebuild /v:Quiet /nologo
    }
}

task Test {
    exec {
		cd $tests_base_path_net45
        & $xunit_path_net452 $tests_path_net45
		cd $tests_base_path_netcoreapp
		& dotnet $xunit_path_netcoreapp $tests_path_netcoreapp
    }
}

task CreateNugetPackage {
    exec {
        & dotnet pack $nuget_csproj_path
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


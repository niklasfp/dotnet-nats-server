param (
    [Parameter(Position = 0)]
    [ValidateSet("linux", "windows", "osx")]
    $os = "linux",

    [Parameter(Position = 1)]
    [ValidateSet("amd64", "arm64")]
    $arch = "amd64"
)
$ErrorActionPreference = "Stop"

Push-Location ./src

$libname = "nats-serverlib"

$go = "go"
$env:GOARCH = $arch
$env:GOOS = $os
$env:CGO_ENABLED = "1"
$outputPath = "../../runtimes/"


switch ($os) {
    "linux" { 
        $outputPath += "linux-x64"
        $libname += ".so"
    }
    "windows" { 
        $outputPath += "win-x64"
        $libname += ".dll"
    }
    "osx" {
        Write-Error 'OSX is not yet supported.'
    }
}

$buildflags = 'build', "-o", "$($outputPath)/$($libname)",  '-buildmode=c-shared', '-ldflags', '-s -w' 

Write-Host "Building for '$($os)' $($arch)"
Write-Host "Output: " $outputPath


$sw = [System.Diagnostics.Stopwatch]::StartNew()

& $go $buildflags 

$buildExitCode = $LastExitCode

$sw.Stop()

Pop-Location

if ($buildExitCode -ne 0)
{
    Write-Error 'Build Failed.'
}

Write-Host -ForegroundColor Green "Sucessfully built " $outputPath/$libname 
Write-Host -ForegroundColor Green "Build completed in " $sw.Elapsed.TotalSeconds "seconds"



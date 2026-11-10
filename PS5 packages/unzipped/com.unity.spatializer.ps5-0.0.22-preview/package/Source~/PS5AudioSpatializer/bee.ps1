#Bee bootstrap script. If you use powershell as your shell, runnning ./bee will directly run this script. If you're running cmd.exe
#running ./bee will run the bee.cmd and that will run powershell to run this script. This script is responsible for
#downloading a dotnet runtime if required, and for downloading a bee distribution if required, and for starting the actual bee
#driver executable.

Add-Type -AssemblyName System.IO.Compression.FileSystem

#these two values will be replaced by the buildprocess when this script is used in a bootstrap distribution. When not replaced
#this script assumes it lives next to the standalone driver
$use_bee_from_steve="bee-nda/fae6b27b5789_6ab79e67e0f48633cc051c0f95f83bbf2323d435d3e285ed46bd72916f2d18d6.zip"
$use_bee_from_steve_repo="http://stevedore.hq.unity3d.com/r/unity-internal"

if("$use_bee_from_steve_repo" -Match "testing")
{
  Write-Host "Warning this bee bootstrap script is using a bee from the testing repository. This fine for testing, but not to use in production."
}  

#stevedore artifact for the dotnet runtimes. these are produced by the yamato CI for our "netcorerun" repo. The zip file contains
#an info file that describes from which git commit / yamato-id it was built.  They are plain upstream packages, just unzipped & rezipped
#with this extra information added.
$dotnet_runtime_win_x64 = "dotnet-runtime-win-x64/b5015a8_47bb1ccdf69b01a0be5a9b670410f97c642e451cb024787ff25864934e9e5be1.zip"

$global:steve_artifact_return_value = ""
function Get-Stevedore-Artifact($steve_name, $steve_repo_url, $output_name)
{
    #we could extend this to parse the Stevedore.conf, and to attempt downloads from multiple mirrors
    $unzip_dir_path = "$HOME\.beebootstrap\$steve_name"
    
    if(![System.IO.Directory]::Exists($unzip_dir_path)) {
        
        $download_link = "$steve_repo_url/$steve_name"
        $random = Get-Random
        $temporary_dir = "$HOME/.beebootstrap/download_$random"
        New-Item -ItemType Directory -Force -Path $temporary_dir | Out-Null
        $downloaded_file = "$temporary_dir/download.zip"

        #turn off this weird powershell progress thing. Not only is it super ugly, it's also reported to be the cause of making the download superslow
        #https://stackoverflow.com/questions/28682642/powershell-why-is-using-invoke-webrequest-much-slower-than-a-browser-download
        $ProgressPreference = 'SilentlyContinue'
        
        Write-Host "Downloading $output_name"
        
        #download into the temporary directory
        Invoke-WebRequest $download_link -outfile "$downloaded_file"

        Write-Host "Unzipping $output_name"
        
        #extract into that same temporary directory
        [System.IO.Compression.ZipFile]::ExtractToDirectory("$downloaded_file", "$temporary_dir")
        #remove the orginial zip file
        Remove-Item $downloaded_file

        #make sure the parent directory of the target directory already exists
        $parent_dir = Split-Path -Parent $unzip_dir_path
        if( -Not (Test-Path -Path $parent_dir) )
        {
            New-Item -ItemType Directory -Path $parent_dir | Out-Null
        }
        
        #and we move the temporary directory to the final location. We use a move, so that it's atomic, and a failed download, or a failed unzp cannot
        #result in a situation where the targetdirectory did get created, but did not get properly populated. This way we can use the presence of the directory
        #as indication 
        Move-Item -Path "$temporary_dir" -Destination $unzip_dir_path
    }

    #we assign to a global instead of returning a value, since returning a value is somehow very brittle, as any command in this function that isn't piped to Out-Null
    #might actually pollute the return value
    $global:steve_artifact_return_value = $unzip_dir_path
}

if( $use_bee_from_steve -eq "no") {
    #this script supports running as part of a full bee distribution. In this case, use_bee_from_steve is not set,
    #and we find the path to the distribution by looking at where the script itself is. It shuold be placed in Standalone/Release of the distribution
    $distribution_path=$PSScriptRoot
}
else {
    #we also support downloading the bee distribution from a stevedore server. In this case use_bee_from_steve should be set to a stevedore artifact name.
    #we'll download it and run it. In this mode, the only thing a user needs to version in their repo is this script.
    Get-Stevedore-Artifact "$use_bee_from_steve" "$use_bee_from_steve_repo" "Bee distribution"
    $distribution_path = "$global:steve_artifact_return_value/Standalone/Release"
}

$standalone_path = "$distribution_path/Bee.StandaloneDriver.exe"

$run_on_net_5 = $True

# Check to see if we should run .Net 5 or not. Currently we support both running on system mono/framework and on .net5. You can control this in an optional
# bee.config file in your repo.
if(test-path "bee.config" -PathType Leaf ) {
    $bee_config = (Get-Content "bee.config" -Raw) | ConvertFrom-Json
    $run_on_net_5_json_value = $bee_config.psobject.properties.Where({$_.name -eq "RunBeeOnNet5"}).value

    $run_on_net_5 = $run_on_net_5_json_value -eq 'true'
}

if($run_on_net_5) {
    $dotnet_steve_artifact = "$dotnet_runtime_win_x64"
    Get-Stevedore-Artifact $dotnet_steve_artifact "https://public-stevedore.unity3d.com/r/public" "Dotnet runtime"
    $dotnet_exe = "$global:steve_artifact_return_value\dotnet.exe"

    #we assign BEE_DOTNET_MUXER env var here, so that the bee that is running knows how it can use this dotnet runtime to start other net5 framework dependent apps.
    #it uses this to run the stevedore downloader program on net5.
    $env:BEE_DOTNET_MUXER = $dotnet_exe
    $net5wrapper = "$distribution_path/Bee.Net5Wrapper.dll"
    $env:BEE_DOTNET_NET5WRAPPER = $net5wrapper;

    # Run the wrapper and pass on any user args
    & $dotnet_exe $net5wrapper $standalone_path $args
} else {
   #run on the system .netframework
   & $standalone_path $args
}

# Ensure exit code is preserved when running from another script.
exit $LastExitCode



# Automatically generated by Yamato Job: https://unity-ci.cds.internal.unity3d.com/job/8293592

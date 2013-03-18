[CmdletBinding()]
param (
    [Parameter(Mandatory=$true, Position=0)]
    [uri]
    $ServerUri
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
& $scriptDir\TFS_Jobs.ps1

$JobId = [guid]'66590D0D-3D89-4A04-878A-2204E9077E50'
$JobName = [string]"AD Image Sync Job"
$JobFullName = [string]"muhaha.TFS.ADImageSync.Job.SyncImagesJob"
$ScheduleInterval = [int]24*60*60

TFSInstallJob $ServerUri $JobId $JobName $JobFullName $ScheduleInterval
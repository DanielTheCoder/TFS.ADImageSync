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

$JobId = [guid]'8DB6C6A0-8C3E-476F-B7E0-21E692C1A53C'
$JobName = [string]"Gravatar User Image Sync Job"
$JobFullName = [string]"muhaha.TFS.GravatarSync.Job.SyncImagesJob"
$ScheduleInterval = [int]24*60*60

TFSInstallJob $ServerUri $JobId $JobName $JobFullName $ScheduleInterval
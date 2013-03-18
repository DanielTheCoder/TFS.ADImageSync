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

TFSScheduleImmediateJob $ServerUri $JobId
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

$IdentitySyncJobId = [guid]'544dd581-f72a-45a9-8de0-8cd3a5f29dfe'

TFSScheduleImmediateJob $ServerUri $IdentitySyncJobId

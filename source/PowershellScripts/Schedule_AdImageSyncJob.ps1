[CmdletBinding()]
param (
    [Parameter(Mandatory=$true, Position=0)]
    [uri]
    $ServerUri
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

Add-Type -AssemblyName 'Microsoft.TeamFoundation.Client, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

$Server = [Microsoft.TeamFoundation.Client.TfsConfigurationServerFactory]::GetConfigurationServer($ServerUri)
$Server.EnsureAuthenticated()
$JobService = $Server.GetService([Microsoft.TeamFoundation.Framework.Client.ITeamFoundationJobService])

$IdentitySyncJobId = [guid]'66590D0D-3D89-4A04-878A-2204E9077E50'
$IdentitySyncJobDef = $JobService.QueryJobs() |
    Where-Object { $_.JobId -eq $IdentitySyncJobId }

if ($IdentitySyncJobDef) {
    Write-Verbose "Queuing job '$($IdentitySyncJobDef.Name)' with high priority now"
    $QueuedCount = $JobService.QueueJobNow($IdentitySyncJobDef, $true)
    if ($QueuedCount -eq 0) {
        Write-Error "Failed to queue job"
    }
} else {
    Write-Error "Could not find Periodic Identity Synchronization job definition (id $IdentitySyncJobId)."
}
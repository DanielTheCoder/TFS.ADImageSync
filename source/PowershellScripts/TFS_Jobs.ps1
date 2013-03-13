function TFSScheduleImmediateJob (
	[ValidateNotNullOrEmpty()]
    [uri] 
	$ServerUri, 
	[ValidateNotNullOrEmpty()]
	[guid] 
	$IdentitySyncJobId) 
{
	Add-Type -AssemblyName 'Microsoft.TeamFoundation.Client, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

	$Server = [Microsoft.TeamFoundation.Client.TfsConfigurationServerFactory]::GetConfigurationServer($ServerUri)
	$Server.EnsureAuthenticated()
	$JobService = $Server.GetService([Microsoft.TeamFoundation.Framework.Client.ITeamFoundationJobService])

#	$IdentitySyncJobId = [guid]'544dd581-f72a-45a9-8de0-8cd3a5f29dfe'
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
}
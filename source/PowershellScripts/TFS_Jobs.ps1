function TFSScheduleImmediateJob (
	[ValidateNotNullOrEmpty()]
    [uri] 
	$ServerUri, 
	[ValidateNotNullOrEmpty()]
	[guid] 
	$JobId) 
{
	Add-Type -AssemblyName 'Microsoft.TeamFoundation.Client, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

	$Server = [Microsoft.TeamFoundation.Client.TfsConfigurationServerFactory]::GetConfigurationServer($ServerUri)
	$Server.EnsureAuthenticated()
	$JobService = $Server.GetService([Microsoft.TeamFoundation.Framework.Client.ITeamFoundationJobService])

	$JobDef = $JobService.QueryJobs() |
	    Where-Object { $_.JobId -eq $JobId }

	if ($JobDef) {
	    Write-Verbose "Queuing job '$($IdentitySyncJobDef.Name)' with high priority now"
	    $QueuedCount = $JobService.QueueJobNow($JobDef, $true)
	    if ($QueuedCount -eq 0) {
	        Write-Error "Failed to queue job"
	    }
	} else {
	    Write-Error "Could not find Periodic Identity Synchronization job definition (id $IdentitySyncJobId)."
	}
}

function TFSInstallJob(
	[ValidateNotNullOrEmpty()]
    [uri] $ServerUri, 
	[ValidateNotNullOrEmpty()]
	[guid] $JobId,
	[string] $JobName,
	[string] $JobFullName,
	[int] $ScheduleInterval)
{
	Add-Type -AssemblyName 'Microsoft.TeamFoundation.Common, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
	Add-Type -AssemblyName 'Microsoft.TeamFoundation.Client, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

	$Server = [Microsoft.TeamFoundation.Client.TfsConfigurationServerFactory]::GetConfigurationServer($ServerUri)
	$Server.EnsureAuthenticated()
	$JobService = $Server.GetService([Microsoft.TeamFoundation.Framework.Client.ITeamFoundationJobService])
	$JobState = [Microsoft.TeamFoundation.Framework.Common.TeamFoundationJobEnabledState]"Enabled"
	
	$Definition = New-Object -TypeName [ Microsoft.TeamFoundation.Framework.Client.TeamFoundationJobDefinition] ($JobId, $JobName, $JobFullName, $null, $JobState)
	
	$Schedule = New-Object -TypeName [Microsoft.TeamFoundation.Framework.Client.TeamFoundationJobSchedule] –ArgumentList Get-Date $ScheduleInterval
	$Definition.Schedule.Add($Schedule)
	
	$Server.UpdateJob($Definition)
	
	TFSScheduleImmediateJob $ServerUri $JobId
}
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
	# [appdomain]::currentdomain.getassemblies() | sort -property fullname | format-table fullname
	# Add-Type -AssemblyName 'Microsoft.TeamFoundation.Common, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
	Add-Type -AssemblyName 'Microsoft.TeamFoundation.Client, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

	#Add-Type -Path 'C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\ReferenceAssemblies\v2.0\Microsoft.TeamFoundation.Client.dll'
	#Add-Type -AssemblyName 'Microsoft.TeamFoundation.Client'

	$Server = [Microsoft.TeamFoundation.Client.TfsConfigurationServerFactory]::GetConfigurationServer($ServerUri)
	$Server.EnsureAuthenticated()
	$JobService = $Server.GetService([Microsoft.TeamFoundation.Framework.Client.ITeamFoundationJobService])
	$JobState = [Microsoft.TeamFoundation.Framework.Common.TeamFoundationJobEnabledState]::Enabled
	$xml = [System.Xml.XmlNode]$null
	
	$Definition = New-Object -TypeName Microsoft.TeamFoundation.Framework.Client.TeamFoundationJobDefinition ($JobId, $JobName, $JobFullName, $xml, $JobState)
	
	$Now = Get-Date
	$Schedule = New-Object -TypeName Microsoft.TeamFoundation.Framework.Client.TeamFoundationJobSchedule ($Now, $ScheduleInterval)
	
	$Definition.Schedule.Add($Schedule)
	$JobService.UpdateJob($Definition)
	
	TFSScheduleImmediateJob $ServerUri $JobId
}
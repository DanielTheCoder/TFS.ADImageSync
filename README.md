TFS.ADImageSync
==================

Overview
------------------

### Features
- synchronize images from Active Directory to Team Foundation Server 2012
- create square images from non-square images

### Components
- ADImageSync.Console
- ADImageSync.Job
- ADImageSync.JobInstaller

### Orignial implementation
Original implementation provided by [Betty](http://bzbetty.blogspot.com "Betty")  
More details can be found here: [tfs-automation-set-user-images-to-be](http://bzbetty.blogspot.co.at/2013/02/tfs-automation-set-user-images-to-be.html/)

ADImageSync.Console
------------------
Console client for one time synchronisation

ADImageSyncJob
------------------
ADImageSyncJob is a Team Foundation Server 2012 server extension which can sync your Active Directory images to Team Foundation Server 2012 on regular basis.  

### Installation
1. Copy the compiled job to the following folder and restart the tfs agent service.  
c:\Program Files\Microsoft Team Foundation Server 11.0\Application Tier\TFSJobAgent\plugins
2. Then run the Install_AdImageSyncJob.ps1 powershell script to register the job. 
3. Finally check the state of the job by looking in the tfs database  
select * from tfs_configuration.dbo.tbl_JobHistory where jobid = '66590D0D-3D89-4A04-878A-2204E9077E50'


### Diagnostic
To check if the job is running without any errors TFS 2012 provides the not documented diagnostic page at
 
- http://servername:port/tfs/_oi/   

Job details can be found here:

- http://servername:port/tfs/_oi/_JobMonitoring#_a=history&id=66590D0D-3D89-4A04-878A-2204E9077E50


How to build
------------------
Copy the following files to the folder:  libs\TFS.Server\

- Microsoft.TeamFoundation.Framework.Server.dll

You should find them here: C:\Program Files\Microsoft Team Foundation Server 11.0\Application Tier\TFSJobAgent\
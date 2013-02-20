TFS.ADImageSyncJob
==================

Overview
------------------
TFS.ADImageSyncJob is a Team Foundation Server 2012 server extension which can sync your Active Directory images to Team Foundation Server 2012 on regular basis.  

### Features
- synchronize images from Active Directory to Team Foundation Server 2012
- create square images from non-square images

### Orignial implementation
Original implementation provided by [Betty](http://bzbetty.blogspot.com "Betty")  
More details can be found here: [tfs-automation-set-user-images-to-be](http://bzbetty.blogspot.co.at/2013/02/tfs-automation-set-user-images-to-be.html/)

How to build
------------------
Copy the following files to the folder:  libs\TFS.Server\

- Microsoft.TeamFoundation.Framework.Server.dll

You should find them here: C:\Program Files\Microsoft Team Foundation Server 11.0\Application Tier\TFSJobAgent\


Installation
------------------
1. Copy the compiled job to the following folder and restart the agent service.  
c:\Program Files\Microsoft Team Foundation Server 11.0\Application Tier\TFSJobAgent\plugins
2. Then run the console application to register the job. 
3. Finally check the state of the job by looking in the tfs database  
select * from tfs_configuration.dbo.tbl_JobHistory where jobid = 'fa60c04e-c996-413e-8151-15933f5a2bac'



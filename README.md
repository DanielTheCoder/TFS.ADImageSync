TFS.ADImageSyncJob
==================

Overview
------------------

Original implementation provided by Betty <http://bzbetty.blogspot.com>  
<http://bzbetty.blogspot.co.at/2013/02/tfs-automation-set-user-images-to-be.html>
<http://bzbetty.blogspot.co.at/2012/09/tfs-2012-user-image-api.html>

Installation
------------------
1. Copy the compiled job to the following folder and restart the agent service.  
c:\Program Files\Microsoft Team Foundation Server 11.0\Application Tier\TFSJobAgent\plugins
2. Then run the console application to register the job. 
3. Finally check the state of the job by looking in the tfs database  
select * from tfs_configuration.dbo.tbl_JobHistory where jobid = 'fa60c04e-c996-413e-8151-15933f5a2bac'
using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace muhaha.ADImageSync.JobInstaller
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri tfsCollectionUri = new Uri(String.Format("http://urltotfs"));
            var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsCollectionUri);
            var service = tfs.GetService<ITeamFoundationJobService>();

            var definition = new TeamFoundationJobDefinition(
                new Guid("fa60c04e-c996-413e-8151-15933f5a2bac"),
                "AD Image Sync Job",
                "muhaha.TFS.ADImageSync.Job.SyncImagesJob",
                //"Enlighten.Tfs.ActiveDirectoryImageSync.SyncImagesJob",
                null,
                TeamFoundationJobEnabledState.Enabled);

            var schedule = new TeamFoundationJobSchedule(DateTime.Now, 24 * 60 * 60);
            definition.Schedule.Add(schedule);

            service.UpdateJob(definition);
            service.QueueJobNow(definition, false);

            Console.ReadLine();
        }
    }
}

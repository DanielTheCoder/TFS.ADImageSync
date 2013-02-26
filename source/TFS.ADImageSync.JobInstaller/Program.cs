using System;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using muhaha.TFS.ADImageSync.Job;

namespace muhaha.ADImageSync.JobInstaller
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Sync Active Directory user images to Team Foundation Server");
            Console.WriteLine("Install Job:");
            Console.WriteLine();

            try
            {
                Console.WriteLine("Enter TFS Uri (https://servername:port/tfs)");
                var input = Console.ReadLine();
                var tfsCollectionUri = new Uri(input);
                
                QueueJob(tfsCollectionUri);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }

            Console.ReadLine();
        }

        private static void QueueJob(Uri tfsCollectionUri)
        {
            TfsTeamProjectCollection tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(tfsCollectionUri);
            var service = tfs.GetService<ITeamFoundationJobService>();

            var definition = new TeamFoundationJobDefinition(
                SyncImagesJob.JobId,
                SyncImagesJob.JobName,
                typeof (SyncImagesJob).FullName,
                null,
                TeamFoundationJobEnabledState.Enabled);

            var schedule = new TeamFoundationJobSchedule(DateTime.Now, 24*60*60);
            definition.Schedule.Add(schedule);

            service.UpdateJob(definition);
            service.QueueJobNow(definition, false);
        }
    }
}
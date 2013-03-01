using System;
using Microsoft.TeamFoundation.Framework.Server;
using muhaha.TFS.Jobs.ImageSync;

namespace muhaha.TFS.GravatarSync.Job
{
    public class SyncImagesJob : ITeamFoundationJobExtension
    {
        public static readonly Guid JobId = new Guid("8DB6C6A0-8C3E-476F-B7E0-21E692C1A53C");
        public const string JobName = "Gravatar Image Sync Job";

        public TeamFoundationJobExecutionResult Run(TeamFoundationRequestContext requestContext, TeamFoundationJobDefinition jobDefinition, DateTime queueTime, out string resultMessage)
        {
            //https://gist.github.com/danesparza/973923
            //http://danesparza.net/2010/10/using-gravatar-images-with-c-asp-net/
            //http://www.codeproject.com/Articles/332404/Gravatar-avatars-in-Csharp-for-NET
            Func<TeamFoundationIdentity, byte[]> imageProviderFunc = null; //todo => getimage from Gravatar
            var run = TfsImageUploader.Run(requestContext, imageProviderFunc);
            resultMessage = run.Item2;
            return run.Item1;
        }
    }
}
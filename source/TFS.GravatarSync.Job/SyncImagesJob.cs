using System;
using System.IO;
using System.Linq;
using Microsoft.TeamFoundation.Framework.Common;
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
            Func<TeamFoundationIdentity, byte[]> imageProviderFunc = tfi => GetGravatarImage(requestContext, tfi); //todo => getimage from Gravatar
            var run = TfsImageUploader.Run(requestContext, imageProviderFunc);
            resultMessage = run.Item2;
            return run.Item1;
        }

        private byte[] GetGravatarImage(TeamFoundationRequestContext requestContext, TeamFoundationIdentity identity)
        {
            var service = requestContext.GetService<TeamFoundationIdentityService>();

            //File.AppendAllText(@"C:\temp\attributes.txt", identity.UniqueName);

            string preferredEmailAddress = service.GetPreferredEmailAddress(requestContext, identity.TeamFoundationId);
            //File.AppendAllText(@"C:\temp\attributes.txt", preferredEmailAddress);

            if (string.IsNullOrWhiteSpace(preferredEmailAddress))
                return null;

            //IdentityDescriptor identityDescriptor = identity.Descriptor;
            //TeamFoundationIdentity teamFoundationIdentity = service.ReadIdentity(requestContext, identityDescriptor, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties);

            //var attributesSet = teamFoundationIdentity.AttributesSet;
            //if (attributesSet == null)
            //    return null;

            //var enumerable = attributesSet.Select(i => i.Key + " - " + i.Value).ToList();
            //enumerable.Insert(0, identity.DisplayName);
            //var contents = string.Join(Environment.NewLine, enumerable);
            ////ConfirmedNotificationAddress
            //File.AppendAllText(@"C:\temp\attributes.txt", contents);

            //return null;
            return GravatarHelper.GetGravatarImage(preferredEmailAddress);
        }
    }
}
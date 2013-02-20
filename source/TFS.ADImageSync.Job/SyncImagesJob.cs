using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using muhaha.Utils.DirectoryServices;
using muhaha.Utils.Drawing.Imaging;

namespace muhaha.TFS.ADImageSync.Job
{
    public class SyncImagesJob : ITeamFoundationJobExtension
    {
        public TeamFoundationJobExecutionResult Run(TeamFoundationRequestContext requestContext, TeamFoundationJobDefinition jobDefinition, DateTime queueTime,
                                                    out string resultMessage)
        {
            resultMessage = "";
            try
            {
                var service = requestContext.GetService<TeamFoundationIdentityService>();

                var identities = service.ReadFilteredIdentities(requestContext,
                                                                "Microsoft.TeamFoundation.Identity.DisplayName CONTAINS '' AND Microsoft.TeamFoundation.Identity.Type == 'User'",
                                                                5000, null, true, MembershipQuery.None);
                if (identities != null)
                {
                    foreach (var identity in identities.Items)
                    {
                        UpdateImageFromAD(requestContext, identity, service);
                    }
                }
            }
            catch (Exception e)
            {
                resultMessage = e.Message + e.StackTrace;
                return TeamFoundationJobExecutionResult.Failed;
            }

            return TeamFoundationJobExecutionResult.Succeeded;
        }
         
        private static void UpdateImageFromAD(TeamFoundationRequestContext requestContext, TeamFoundationIdentity identity, TeamFoundationIdentityService service)
        {
            var thumbNail = ADHelper.GetImageFromAD(identity.UniqueName);

            if (thumbNail == null)
                return;

            var imageFormat = ImageFormatHelper.GetImageFormat(thumbNail);
            var propertyUpdates = new List<PropertyValue>
                                      {
                                          new PropertyValue("Microsoft.TeamFoundation.Identity.Image.Data", thumbNail),
                                          new PropertyValue("Microsoft.TeamFoundation.Identity.Image.Type", "image/" + imageFormat.ToString()),
                                          new PropertyValue("Microsoft.TeamFoundation.Identity.Image.Id", Guid.NewGuid().ToByteArray()),
                                          new PropertyValue("Microsoft.TeamFoundation.Identity.CandidateImage.Data", null),
                                          new PropertyValue("Microsoft.TeamFoundation.Identity.CandidateImage.UploadDate", null),
                                      };

            service.UpdateExtendedProperties(requestContext, identity.Descriptor, propertyUpdates);
            service.RefreshIdentity(requestContext, identity.Descriptor);
        }
    }
}
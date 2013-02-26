using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using muhaha.Utils.DirectoryServices;
using muhaha.Utils.Drawing.Imaging;

namespace muhaha.TFS.ADImageSync.Job
{
    public class SyncImagesJob : ITeamFoundationJobExtension
    {
        public static readonly Guid JobId = new Guid("66590D0D-3D89-4A04-878A-2204E9077E50");
        public const string JobName = "AD Image Sync Job";

        public TeamFoundationJobExecutionResult Run(TeamFoundationRequestContext requestContext, TeamFoundationJobDefinition jobDefinition, DateTime queueTime, out string resultMessage)
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
            var adImage = ADHelper.GetImageFromAD(identity.UniqueName);
            if (adImage == null) return;
            
            //to quadrasize image
            var tuple = QuadrasizeImage(adImage);
            var tfsImageFormat = tuple.Item2;
            var tfsImage = tuple.Item1;

            var propertyUpdates = new List<PropertyValue>
                                      {
                                          new PropertyValue("Microsoft.TeamFoundation.Identity.Image.Data", tfsImage),
                                          new PropertyValue("Microsoft.TeamFoundation.Identity.Image.Type", "image/" + tfsImageFormat.ToString()),
                                          new PropertyValue("Microsoft.TeamFoundation.Identity.Image.Id", Guid.NewGuid().ToByteArray()),
                                          new PropertyValue("Microsoft.TeamFoundation.Identity.CandidateImage.Data", null),
                                          new PropertyValue("Microsoft.TeamFoundation.Identity.CandidateImage.UploadDate", null),
                                      };

            service.UpdateExtendedProperties(requestContext, identity.Descriptor, propertyUpdates);
            service.RefreshIdentity(requestContext, identity.Descriptor);
        }

        private static Tuple<byte[], ImageFormat> QuadrasizeImage(byte[] adImage)
        {
            Image image = ImageHelper.ByteArrayToImage(adImage);
            Image newImage = ImageHelper.QuadrasizeImage(image);
            byte[] tfsImage = ImageHelper.ImageToByteArray(newImage, System.Drawing.Imaging.ImageFormat.Png);
            ImageFormat tfsImageFormat = ImageFormatHelper.GetImageFormat(tfsImage);
            return Tuple.Create(tfsImage, tfsImageFormat);
        }
    }
}
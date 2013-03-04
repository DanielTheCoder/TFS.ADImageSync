using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using muhaha.Utils.Drawing.Imaging;

namespace muhaha.TFS.Jobs.ImageSync
{
    public static class TfsImageUploader
    {
        public static Tuple<TeamFoundationJobExecutionResult, string> Run(TeamFoundationRequestContext requestContext, Func<TeamFoundationIdentity, byte[]> imageProviderFunc)
        {
            string resultMessage = "";
            try
            {
                var service = requestContext.GetService<TeamFoundationIdentityService>();

                var identities = service.ReadFilteredIdentities(requestContext,
                                                                "Microsoft.TeamFoundation.Identity.DisplayName CONTAINS '' AND Microsoft.TeamFoundation.Identity.Type == 'User'",
                                                                5000, null, true, MembershipQuery.None);
                if (identities != null)
                {
                    foreach (TeamFoundationIdentity identity in identities.Items)
                    {
                        var adImage = imageProviderFunc(identity);
                        if (adImage == null)
                            continue;

                        UploadImage(requestContext, identity, service, adImage);
                    }
                }
            }
            catch (Exception e)
            {
                resultMessage = e.Message + e.StackTrace;
                return Tuple.Create(TeamFoundationJobExecutionResult.Failed, resultMessage);
            }

            return Tuple.Create(TeamFoundationJobExecutionResult.Succeeded, resultMessage);
        }

        private static void UploadImage(TeamFoundationRequestContext requestContext, TeamFoundationIdentity identity, TeamFoundationIdentityService service, byte[] adImage)
        {
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
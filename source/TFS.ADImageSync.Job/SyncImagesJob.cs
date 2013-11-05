using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using muhaha.Utils.DirectoryServices;
using muhaha.Utils.Drawing.Imaging;

namespace muhaha.TFS.ADImageSync.Job
{
    public class SyncImagesJob : ITeamFoundationJobExtension
    {
        public static readonly Guid JobId = new Guid("66590D0D-3D89-4A04-878A-2204E9077E50");
        public const string JobName = "AD Image Sync Job";

        //public void Temp(TeamFoundationRequestContext requestContext)
        //{
        //    Identity identity = null;
        //    identity.SetAllModifiedProperties();
        //    IdentityService identityService = requestContext.GetService<IdentityService>();
        //    identityService.ReadFilteredIdentities()
        //    identityService.UpdateIdentities(requestContext, (IList<Identity>)new Identity[1]
        //                                                                                            });
        //}

        public IQueryable<Identity> ReadIdentities(TeamFoundationRequestContext requestContext, string descriptors = "", string identityIds = "", string searchFilter = "", string filterValue = "", QueryMembership queryMembership = QueryMembership.None, string properties = "", bool includeRestrictedVisibility = false)
        {
            var descriptorsFromString = (IList<IdentityDescriptor>)new List<IdentityDescriptor>().AsReadOnly();
            var identityIdsFromString = (IList<Guid>)new List<Guid>().AsReadOnly();
            var filtersFromString = Enumerable.Empty<string>();

            var service = requestContext.GetService<IdentityService>();

            if (descriptorsFromString.Count > 0)
                return service.ReadIdentities(requestContext, descriptorsFromString, queryMembership, filtersFromString, includeRestrictedVisibility).AsQueryable();
            if (identityIdsFromString.Count > 0)
                return service.ReadIdentities(requestContext, identityIdsFromString, queryMembership, filtersFromString, includeRestrictedVisibility).AsQueryable();
            if (string.IsNullOrEmpty(searchFilter))
                throw new ArgumentException("Either descriptors or identityIds or searchFactor/factorValue must be specified");
            
            var searchFactor = (IdentitySearchFilter)Enum.Parse(typeof(IdentitySearchFilter), searchFilter);
            return service.ReadIdentities(requestContext, searchFactor, filterValue, queryMembership, filtersFromString).AsQueryable();
        }

        public TeamFoundationJobExecutionResult Run(TeamFoundationRequestContext requestContext, TeamFoundationJobDefinition jobDefinition, DateTime queueTime,
            out string resultMessage)
        {
            resultMessage = "";

            var service = requestContext.GetService<IdentityService>();
            var identities = service.ReadIdentities(requestContext,
                (IList<IdentityDescriptor>) new List<IdentityDescriptor>()/* new IdentityDescriptor[1] {GroupWellKnownIdentityDescriptors.ServiceUsersGroup}*/,
                QueryMembership.None, null);

            var identities2 = service.ReadIdentities(requestContext, IdentitySearchFilter.AccountName, "*", QueryMembership.None, null);
            //TFS 2012
            //var identities = service.ReadFilteredIdentities(requestContext,scopeId,descriptors,
            //                                                    "Microsoft.TeamFoundation.Identity.DisplayName CONTAINS '' AND Microsoft.TeamFoundation.Identity.Type == 'User'",
            //                                                    5000, null, true, MembershipQuery.None);
            try
            {
                if (identities != null)
                {
                    foreach (var identity in identities)
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

        private static void UpdateImageFromAD(TeamFoundationRequestContext requestContext, Identity identity, IdentityService service)
        {
            var adImage = ADHelper.GetImageFromAD(identity.ProviderDisplayName);
            if (adImage == null) return;
            
            //to quadrasize image
            var tuple = QuadrasizeImage(adImage);
            var tfsImageFormat = tuple.Item2;
            var tfsImage = tuple.Item1;

            identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Data", tfsImage);
            identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Type", "image/" + tfsImageFormat.ToString());
            identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Id", Guid.NewGuid().ToByteArray());
            identity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.Data", null);
            identity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.UploadDate", null);

            service.UpdateIdentities(requestContext, new List<Identity> {identity});
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
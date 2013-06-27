using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using muhaha.Utils.DirectoryServices;
using muhaha.Utils.Drawing.Imaging;
using ImageFormat = muhaha.Utils.Drawing.Imaging.ImageFormat;

namespace muhaha.ADImageSync.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            System.Console.WriteLine("Sync Active Directory user images to Team Foundation Server");
            System.Console.WriteLine();

            try
            {
                //System.Console.WriteLine("Enter TFS Uri (https://servername:port/tfs)");
                //var input = System.Console.ReadLine();
                var input = "https://tfs02.techtalk.at/tfs";
                ChangeImage(input);
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception.ToString());
            }

            System.Console.ReadLine();
        }

        public static void ChangeImage(string tfsUri)
        {
            var teamFoundationServer = new TfsTeamProjectCollection(new Uri(tfsUri));
            var service = teamFoundationServer.GetService<FilteredIdentityService>();
            var identityService = teamFoundationServer.GetService<IIdentityManagementService2>();
            
            foreach (TeamFoundationIdentity identity in service.SearchForUsers(""))
            {
                if (identity.UniqueName.ToLower() != "techtalk\\test.ds".ToLower())
                    continue;

                if (!identity.IsActive) continue;

                //NULL as extended properties are not fetched by default!
                string CustomNotificationAddresses = identity.GetAttribute("CustomNotificationAddresses", string.Empty);
                string ConfirmedNotificationAddress = identity.GetAttribute("ConfirmedNotificationAddress", string.Empty);

                var fetchProperties = new[] {"Microsoft.TeamFoundation.Identity.Image.Id", "CustomNotificationAddresses", "ConfirmedNotificationAddress"};

                var i = identityService.ReadIdentity(IdentitySearchFactor.MailAddress, @"test.ds",
                                                     MembershipQuery.Direct,
                                                     ReadIdentityOptions.ExtendedProperties, 
                                                     fetchProperties, 
                                                     IdentityPropertyScope.Both);

                CustomNotificationAddresses = (string)i.GetProperty("CustomNotificationAddresses");﻿
                ConfirmedNotificationAddress = (string)i.GetProperty("ConfirmedNotificationAddress");﻿

                System.Console.WriteLine(CustomNotificationAddresses);
                System.Console.WriteLine(ConfirmedNotificationAddress);
                //byte[] adImage = ADHelper.GetImageFromAD(identity.UniqueName);
                //if (adImage == null) continue;


                ////to quadrasize image
                //Image image = ImageHelper.ByteArrayToImage(adImage);
                //var newImage = ImageHelper.QuadrasizeImage(image);

                ////store image local
                //ImageFormat adImageFormat = ImageFormatHelper.GetImageFormat(adImage);
                //DumpImageToLocalPath(identity, adImageFormat, newImage);

                ////Convert back to byte[]
                //byte[] tfsImage = ImageHelper.ImageToByteArray(newImage, System.Drawing.Imaging.ImageFormat.Png);
                //ImageFormat tfsImageFormat = ImageFormatHelper.GetImageFormat(tfsImage);

                //identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Data", tfsImage);
                //identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Type", "image/" + tfsImageFormat.ToString());
                //identity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Id", Guid.NewGuid().ToByteArray());
                //identity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.Data", null);
                //identity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.UploadDate", null);

                identityService.UpdateExtendedProperties(identity);
            }
        }

        private static void DumpImageToLocalPath(TeamFoundationIdentity identity, ImageFormat imageFormat, Image image)
        {
            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            
            const string imageFoldername = "Images";
            
            string filename = imageFoldername + "/" + identity.DisplayName + "." + imageFormat.ToString();
            if (!Directory.Exists(imageFoldername))
                Directory.CreateDirectory(imageFoldername);

            image.Save(filename, info[1], encoderParameters);
        }
    }
}
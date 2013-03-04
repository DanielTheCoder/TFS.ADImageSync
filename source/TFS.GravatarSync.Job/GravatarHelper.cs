using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using muhaha.Utils.Drawing.Imaging;

namespace muhaha.TFS.GravatarSync.Job
{
    public static class GravatarHelper
    {
        public static byte[] GetGravatarImage(string email)
        {
            var url = GetGravatarUrl(email);

            try
            {
                WebClient webClient = new WebClient();
                Stream largeImage = webClient.OpenRead(url);
                Image fromStream = Image.FromStream(largeImage);
                byte[] imageToByteArray = ImageHelper.ImageToByteArray(fromStream, System.Drawing.Imaging.ImageFormat.Png);
                return imageToByteArray;
            }
            catch (Exception exception)
            {
                File.AppendAllText(@"C:\temp\attributes.txt", exception.ToString());
                File.AppendAllText(@"C:\temp\attributes.txt", Environment.NewLine);
            }

            return null;
        }

        private static string GetGravatarUrl(string email)
        {
            //  Compute the hashstring 
            var hash = HashEmailForGravatar(email);

            //  Assemble the url and
            return String.Format("http://www.gravatar.com/avatar/{0}", hash);
        }


        /// Hashes an email with MD5.  Suitable for use with Gravatar profile
        /// image urls
        private static string HashEmailForGravatar(string email)
        {
            var emailInternal = email.ToLower();

            // Create a new instance of the MD5CryptoServiceProvider object.  
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.  
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(emailInternal));

            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();  // Return the hexadecimal string. 
        }
    }
}
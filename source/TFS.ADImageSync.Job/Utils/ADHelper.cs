using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace muhaha.Utils.DirectoryServices
{
    public static class ADHelper
    {
        public static byte[] GetImageFromAD(string identityUniqueName)
        {
            var user = UserPrincipal.FindByIdentity(new PrincipalContext(ContextType.Domain), identityUniqueName);
            if (user == null) return null;

            var de = new DirectoryEntry("LDAP://" + user.DistinguishedName);
            var thumbNail = de.Properties["thumbnailPhoto"].Value as byte[];
            return thumbNail;
        }
    }
}
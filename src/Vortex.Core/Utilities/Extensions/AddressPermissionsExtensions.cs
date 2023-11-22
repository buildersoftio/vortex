namespace Vortex.Core.Utilities.Extensions
{
    public static class AddressPermissionsExtensions
    {
        public static string[] ToAddressArray(this string addresses)
        {
            return addresses.Split(',');
        }
    }
}

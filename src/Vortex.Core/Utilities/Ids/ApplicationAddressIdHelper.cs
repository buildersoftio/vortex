namespace Vortex.Core.Utilities.Ids
{
    public static class ApplicationAddressIdHelper
    {
        public static string ToApplicationAddressId(int applicationId, int addressId, string subscriptionName)
        {
            return $"{applicationId}:{addressId}:{subscriptionName}";
        }
    }
}

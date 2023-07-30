using Cerebro.Core.Models.Common.Addresses;

namespace Cerebro.Core.Utilities.Extensions
{
    public static class AddressIndexExtensions
    {
        public static string GenerateAddressIndex(this DateTime dateTime, MessageIndexTypes messageIndexTypes)
        {
            switch (messageIndexTypes)
            {
                case MessageIndexTypes.HOURLY:
                    return dateTime.ToString("yyyy-MM-dd HH");
                case MessageIndexTypes.DAILY:
                    return dateTime.ToString("yyyy-MM-dd");
                case MessageIndexTypes.MONTHLY:
                    return dateTime.ToString("yyyy-MM");
                default:
                    break;
            }
            return "";
        }
    }
}

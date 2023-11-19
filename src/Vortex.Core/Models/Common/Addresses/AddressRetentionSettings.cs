
namespace Cerebro.Core.Models.Common.Addresses
{
    public class AddressRetentionSettings
    {
        public RetentionTypes RetentionType { get; set; }
        public long TimeToLiveInMinutes { get; set; }

        public AddressRetentionSettings()
        {
            RetentionType = RetentionTypes.DELETE;
            TimeToLiveInMinutes = -1;
        }
    }

    public enum RetentionTypes
    {
        DELETE,
        ARCHIVE
    }
}

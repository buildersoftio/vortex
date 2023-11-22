namespace Vortex.Core.Models.Common.Addresses
{
    public class AddressSchemaSettings
    {
        public int SchemaId { get; set; }
        public CompatibilityTypes CompatibilityType { get; set; }

        public AddressSchemaSettings()
        {
            // move this to settings as default parameter when address is created
            CompatibilityType = CompatibilityTypes.NONE;
        }
    }

    public enum CompatibilityTypes
    {
        NONE,
        BACKWARD,
        BACKWARD_TRANSITIVE,
        FORWARD,
        FORWARD_TRANSITIVE,
        FULL,
        FULL_TRANSITIVE,
    }
}

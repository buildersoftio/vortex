namespace Vortex.Core.Models.Common.Clients.Applications
{
    public class ConsumptionSettings
    {
        public bool AutoCommitEntry { get; set; }

        // in case of Auto Commit is false;
        public AcknowledgmentTypes? AcknowledgmentType { get; set; }

        public ReadInitialPositions? ReadInitialPositions { get; set; }
    }

    public enum AcknowledgmentTypes
    {
        Batch,
        Record
    }

    public enum ReadInitialPositions
    {
        /// <summary>
        /// Starts reading address from the beginning.
        /// </summary>
        Earliest,
        /// <summary>
        /// Starts reading address from the moment it connects to it.
        /// </summary>
        Latest
    }
}

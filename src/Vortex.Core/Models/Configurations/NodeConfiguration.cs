using Vortex.Core.Models.Common.Clients.Applications;

namespace Vortex.Core.Models.Configurations
{
    public class NodeConfiguration
    {
        public string NodeId { get; set; }

        public int IdleClientConnectionInterval { get; set; }
        public int IdleClientConnectionTimeout { get; set; }
        //VORTEX_CHECK_RETRY_COUNT
        public int CheckRetryCount { get; set; }

        //VORTEX_POSITION_FLUSH_PERIOD_SEC
        public int BackgroundPositionEntry_FlushInterval { get; set; }


        public bool DefaultAutoCommitEntry { get; set; }
        public AcknowledgmentTypes DefaultAcknowledgmentType { get; set; }
        public ReadInitialPositions DefaultReadInitialPosition { get; set; }

    }
}

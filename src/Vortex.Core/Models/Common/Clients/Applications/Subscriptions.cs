namespace Vortex.Core.Models.Common.Clients.Applications
{
    public enum SubscriptionTypes
    {
        /// <summary>
        /// Only one consumer
        /// </summary>
        Unique,
        /// <summary>
        /// One consumer with one backup
        /// </summary>
        Failover,
        /// <summary>
        /// Shared to more than one consumer.
        /// </summary>
        Shared
    }

    public enum ReadInitialPositions
    {
        /// <summary>
        /// Starts reading address from the begining.
        /// </summary>
        Earliest,
        /// <summary>
        /// Starts reading address from the moment it connects to it.
        /// </summary>
        Latest
    }

    public enum SubscriptionModes
    {
        /// <summary>
        /// Durable
        /// </summary>
        Resilient,

        /// <summary>
        /// Non Durable
        /// </summary>
        NonResilient
    }
}

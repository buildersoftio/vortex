namespace Vortex.Core.Models.Common.Clusters
{
    public enum NodeStatus
    {
        Online,
        Reconnecting,
        Offline
    }

    public enum NodeState
    {
        Follower,
        Candidate,
        Leader
    }
}

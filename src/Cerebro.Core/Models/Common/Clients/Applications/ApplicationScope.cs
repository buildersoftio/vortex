namespace Cerebro.Core.Models.Common.Clients.Applications
{
    public enum ApplicationScope
    {
        SingleScope,    // Application is for a single node
        ClusterScope    // Application is created across the cluster
    }
}

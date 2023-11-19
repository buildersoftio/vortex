namespace Vortex.Core.Utilities.Consts
{
    public static class EnvironmentConstants
    {

        /// <summary>
        /// VORTEX Root Directories
        /// </summary>
        public const string RootDataLocation = "VORTEX_ROOT_DATA_DIRECTORY";
        public const string RootConfigLocation = "VORTEX_ROOT_CONFIG_DIRECTORY";
        public const string RootLogLocation = "VORTEX_ROOT_LOG_DIRECTORY";
        public const string RootTempLocation = "VORTEX_ROOT_TEMP_DIRECTORY";


        public const string NodeId = "VORTEX_NODE_ID";

        public const string BackgroundServiceFaildTaskInterval = "VORTEX_BACKGROUND_TASK_INTERVAL";


        /// <summary>
        ///  VORTEX Cluster Environment Variables
        /// </summary>
        public const string CerebroClusterId = "VORTEX_CLUSTER_ID";
        public const string CerebroClusterHost = "VORTEX_CLUSTER_HOST";
        public const string CerebroClusterConnectionIsSecure = "VORTEX_CLUSTER_ISSECURE";
        public const string CerebroClusterConnectionPort = "VORTEX_CLUSTER_PORT";
        public const string CerebroClusterConnectionSSLPort = "VORTEX_CLUSTER_SSLPORT";

        public const string CerebroClusterCertificateFileName = "VORTEX_CLUSTER_X509CERTIFICATE_PATH";
        public const string CerebroClusterCertificatePassword = "VORTEX_CLUSTER_X509CERTIFICATE_PASSWORD";

        /// <summary>
        ///  VORTEX Broker Environment Variables
        /// </summary>
        public const string BrokerHost = "VORTEX_BROKER_HOST";
        public const string BrokerPort = "VORTEX_BROKER_PORT";
        public const string BrokerConnectionIsSecure = "VORTEX_BROKER_ISSECURE";
        public const string BrokerConnectionSSLPort = "VORTEX_BROKER_SSLPORT";

        public const string CertificateFileName = "VORTEX_X509CERTIFICATE_PATH";
        public const string CertificatePassword = "VORTEX_X509CERTIFICATE_PASSWORD";
    }
}

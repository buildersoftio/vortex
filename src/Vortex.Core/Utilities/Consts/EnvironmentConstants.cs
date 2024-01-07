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

        public const string BackgroundServiceFailTaskInterval = "VORTEX_BACKGROUND_TASK_INTERVAL";
        public const string BackgroundIdleClientConnectionInterval = "VORTEX_IDLE_CONN_CHECK"; 
        public const string BackgroundIdleClientConnectionTimeout = "VORTEX_IDLE_CONN_TIMEOUT";
        public const string BackgroundCheckRetryCount = "VORTEX_CHECK_RETRY_COUNT";


        public const string BackgroundPositionEntityFlushInterval = "VORTEX_POSITION_FLUSH_INTERVAL";


        /// <summary>
        ///  VORTEX Cluster Environment Variables
        /// </summary>
        public const string VortexClusterId = "VORTEX_CLUSTER_ID";
        public const string VortexClusterHost = "VORTEX_CLUSTER_HOST";
        public const string VortexClusterConnectionIsSecure = "VORTEX_CLUSTER_ISSECURE";
        public const string VortexClusterConnectionPort = "VORTEX_CLUSTER_PORT";
        public const string VortexClusterConnectionSSLPort = "VORTEX_CLUSTER_SSLPORT";

        public const string VortexClusterCertificateFileName = "VORTEX_CLUSTER_X509CERTIFICATE_PATH";
        public const string VortexClusterCertificatePassword = "VORTEX_CLUSTER_X509CERTIFICATE_PASSWORD";

        /// <summary>
        ///  VORTEX Broker Environment Variables
        /// </summary>
        public const string BrokerHost = "VORTEX_BROKER_HOST";
        public const string BrokerPort = "VORTEX_BROKER_PORT";
        public const string BrokerConnectionIsSecure = "VORTEX_BROKER_ISSECURE";
        public const string BrokerConnectionSSLPort = "VORTEX_BROKER_SSLPORT";

        public const string CertificateFileName = "VORTEX_X509CERTIFICATE_PATH";
        public const string CertificatePassword = "VORTEX_X509CERTIFICATE_PASSWORD";


        /// <summary>
        /// Default Values for Consumption
        /// </summary>
        public const string DefaultAutoCommitEntry = "VORTEX_DEFAULT_AUTO_COMMIT_ENTRY";
        public const string DefaultAcknowledgmentType = "VORTEX_DEFAULT_ACKNOWLEDGMENT_TYPE";
        public const string DefaultReadInitialPosition = "VORTEX_DEFAULT_READ_INITIAL_POSITION";
    }
}

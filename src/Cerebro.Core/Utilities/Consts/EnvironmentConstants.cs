namespace Cerebro.Core.Utilities.Consts
{
    public static class EnvironmentConstants
    {

        /// <summary>
        /// CEREBRO Root Directories
        /// </summary>
        public const string RootDataLocation = "CEREBRO_ROOT_DATA_DIRECTORY";
        public const string RootConfigLocation = "CEREBRO_ROOT_CONFIG_DIRECTORY";
        public const string RootLogLocation = "CEREBRO_ROOT_LOG_DIRECTORY";
        public const string RootTempLocation = "CEREBRO_ROOT_TEMP_DIRECTORY";


        public const string NodeId = "CEREBRO_NODE_ID";


        /// <summary>
        ///  CEREBRO Cluster Environment Variables
        /// </summary>
        public const string CerebroClusterId = "CEREBRO_CLUSTER_ID";
        public const string CerebroClusterHost = "CEREBRO_CLUSTER_HOST";
        public const string CerebroClusterConnectionIsSecure = "CEREBRO_CLUSTER_ISSECURE";
        public const string CerebroClusterConnectionPort = "CEREBRO_CLUSTER_PORT";
        public const string CerebroClusterConnectionSSLPort = "CEREBRO_CLUSTER_SSLPORT";

        public const string CerebroClusterCertificateFileName = "CEREBRO_CLUSTER_X509CERTIFICATE_PATH";
        public const string CerebroClusterCertificatePassword = "CEREBRO_CLUSTER_X509CERTIFICATE_PASSWORD";

        /// <summary>
        ///  CEREBRO Broker Environment Variables
        /// </summary>
        public const string BrokerHost = "CEREBRO_BROKER_HOST";
        public const string BrokerPort = "CEREBRO_BROKER_PORT";
        public const string BrokerConnectionIsSecure = "CEREBRO_BROKER_ISSECURE";
        public const string BrokerConnectionSSLPort = "CEREBRO_BROKER_SSLPORT";

        public const string CertificateFileName = "CEREBRO_X509CERTIFICATE_PATH";
        public const string CertificatePassword = "CEREBRO_X509CERTIFICATE_PASSWORD";
    }
}

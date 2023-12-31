﻿namespace Vortex.Core.IO
{
    public static class ConfigLocations
    {
        public static string GetActiveDirectory()
        {
            return Path.Combine(RootLocations.GetConfigRootDirectory(), "active");
        }

        public static string GetApplicationStateStoreFile()
        {
            return Path.Combine(GetActiveDirectory(), "application_state.cbs");
        }

        public static string GetActiveDefaultStorageConfigurationFile()
        {
            return Path.Combine(GetActiveDirectory(), "storage_config.json");
        }

        public static string GetDefaultStorageConfigurationFile()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "settings", "storage_initial.json");
        }

        public static string GetActiveClusterConfigurationFile()
        {
            return Path.Combine(GetActiveDirectory(), "clusters_config.json");
        }

        public static string GetClusterConfigurationFile()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "settings", "clusters_initial_config.json");
        }
    }
}

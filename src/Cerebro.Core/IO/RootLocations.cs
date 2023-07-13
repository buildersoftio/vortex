using Cerebro.Core.Utilities.Consts;

namespace Cerebro.Core.IO
{
    public static class RootLocations
    {
        public static string GetConfigRootDirectory()
        {
            string preSelectedLocation = Environment.GetEnvironmentVariable(EnvironmentConstants.RootConfigLocation)!;
            if (preSelectedLocation != null)
                return preSelectedLocation;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cerebro_config");
        }

        public static string GetDataRootDirectory()
        {
            string preSelectedLocation = Environment.GetEnvironmentVariable(EnvironmentConstants.RootDataLocation)!;
            if (preSelectedLocation != null)
                return preSelectedLocation;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cerebro_data");
        }

        public static string GetLogsRootDirectory()
        {
            string preSelectedLocation = Environment.GetEnvironmentVariable(EnvironmentConstants.RootLogLocation)!;
            if (preSelectedLocation != null)
                return preSelectedLocation;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cerebro_logs");
        }

        public static string GetTempRootDirectory()
        {
            string preSelectedLocation = Environment.GetEnvironmentVariable(EnvironmentConstants.RootTempLocation)!;
            if (preSelectedLocation != null)
                return preSelectedLocation;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cerebro_temp");
        }
    }
}

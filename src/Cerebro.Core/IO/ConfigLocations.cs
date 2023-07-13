namespace Cerebro.Core.IO
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
    }
}

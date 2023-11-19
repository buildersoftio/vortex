namespace Vortex.Core.IO
{
    public static class TemporaryLocations
    {
        public static string GetBackgroundServiceDirectory()
        {
            return Path.Combine(RootLocations.GetTempRootDirectory(), "backgrounds");
        }
        public static string GetBackgroundServiceFailedFile(string fileName)
        {
            return Path.Combine(GetBackgroundServiceDirectory(), $"{fileName}");
        }
    }
}

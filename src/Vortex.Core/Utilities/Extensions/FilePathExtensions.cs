namespace Vortex.Core.Utilities.Extensions
{
    public static class FilePathExtensions
    {
        public static string GetLastThreeLevels(this string filePath)
        {
            string[] pathLevels = filePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            int count = pathLevels.Length;

            // Make sure the path has at least three levels
            if (count >= 3)
            {
                string lastThreeLevels = Path.Combine(pathLevels[count - 3], pathLevels[count - 2], pathLevels[count - 1]);
                return lastThreeLevels;
            }
            else
            {
                // If the path has less than three levels, just return the original path.
                return filePath;
            }
        }
    }
}

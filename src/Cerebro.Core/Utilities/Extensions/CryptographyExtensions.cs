using System.Security.Cryptography;
using System.Text;

namespace Cerebro.Core.Utilities.Extensions
{
    public static class CryptographyExtensions
    {

        public static string GenerateApiSecret()
        {
            var key = new byte[64];
            using (var generator = RandomNumberGenerator.Create())
                generator.GetBytes(key);

            return Convert.ToBase64String(key);
        }


        public static string ToSHA512_HashString(this string text, string stalt = "")
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // Uses SHA512 to create the hash
            using (HashAlgorithm algorithm = SHA512.Create())
            {
                // Convert the string to a byte array first, to be processed
                byte[] hashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(text + stalt));

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }

        public static string ToSHA256_HashString(this string text, string stalt = "")
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // Uses SHA512 to create the hash
            using (HashAlgorithm algorithm = SHA256.Create())
            {
                // Convert the string to a byte array first, to be processed
                byte[] hashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(text + stalt));

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }

        public static string ToSHA384_HashString(this string text, string stalt = "")
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // Uses SHA512 to create the hash
            using (HashAlgorithm algorithm = SHA384.Create())
            {
                // Convert the string to a byte array first, to be processed
                byte[] hashBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(text + stalt));

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }
    }
}

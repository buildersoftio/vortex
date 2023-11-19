using System.Net;
using System.Text.RegularExpressions;

namespace Vortex.Core.Utilities.Validators
{
    public static class DataValidationExtensions
    {
        public static bool IsValidIpAddress(string ipAddressString)
        {
            // Try to parse the string as an IP address
            if (IPAddress.TryParse(ipAddressString, out IPAddress ipAddress))
            {
                // Check if the parsed IP address is either IPv4 or IPv6
                return ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ||
                       ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6;
            }

            // The parsing failed, so it's not a valid IP address
            return false;
        }

        public static bool IsValidIpAddress(this HashSet<string> listIpAddressStrings)
        {
            foreach (var ipAddressString in listIpAddressStrings)
            {
                if (IsValidIpAddress(ipAddressString) != true)
                    return false;
            }

            return true;
        }

        public static string ToReplaceDuplicateSymbols(this string input)
        {
            // Use regex to remove duplicate dots, underscores, and dashes
            string result = Regex.Replace(input, @"(\.)+|(_)+|(-)+", m => m.Groups[0].Value.Substring(0, 1));

            return result;
        }
    }
}

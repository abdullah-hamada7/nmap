using System.Net;
using System.Text.RegularExpressions;

namespace NmapCli.Utilities;

public static class IpAddressHelper
{
    public static bool IsValidIpAddress(string ip)
    {
        return IPAddress.TryParse(ip, out _);
    }

    public static bool IsValidCidr(string cidr)
    {
        var parts = cidr.Split('/');
        if (parts.Length != 2)
            return false;

        if (!IsValidIpAddress(parts[0]))
            return false;

        if (!int.TryParse(parts[1], out int prefix))
            return false;

        return prefix >= 0 && prefix <= 32;
    }

    public static bool IsValidIpRange(string range)
    {
        var regex = new Regex(@"^(\d{1,3}\.){3}(\d{1,3}|\d{1,3}-\d{1,3})$");
        return regex.IsMatch(range);
    }

    public static bool IsValidHostname(string hostname)
    {
        if (string.IsNullOrWhiteSpace(hostname))
            return false;

        var regex = new Regex(@"^([a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?\.)*[a-zA-Z0-9]([a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])?$");
        return regex.IsMatch(hostname);
    }

    public static bool ValidateTarget(string target)
    {
        return IsValidIpAddress(target) ||
               IsValidCidr(target) ||
               IsValidIpRange(target) ||
               IsValidHostname(target);
    }

    public static List<string> ReadTargetsFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Target file not found: {filePath}");
        }

        var targets = new List<string>();
        var lines = File.ReadAllLines(filePath);

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                continue;

            if (ValidateTarget(trimmed))
            {
                targets.Add(trimmed);
            }
            else
            {
                Console.WriteLine($"Warning: Invalid target '{trimmed}' in file, skipping...");
            }
        }

        return targets;
    }
}

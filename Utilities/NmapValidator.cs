using System.Diagnostics;

namespace NmapCli.Utilities;

public static class NmapValidator
{
    public static bool IsNmapInstalled()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "nmap",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    public static string? GetNmapVersion()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "nmap",
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode == 0)
            {
                var lines = output.Split('\n');
                return lines.Length > 0 ? lines[0].Trim() : null;
            }
        }
        catch
        {
            // Ignore
        }

        return null;
    }

    public static bool IsRunningAsRoot()
    {
        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "id",
                    Arguments = "-u",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            var output = process.StandardOutput.ReadToEnd().Trim();
            process.WaitForExit();

            return output == "0";
        }
        catch
        {
            return false;
        }
    }

    public static void CheckNmapAvailability()
    {
        if (!IsNmapInstalled())
        {
            throw new InvalidOperationException(
                "Nmap is not installed or not in PATH.\n" +
                "Please install nmap:\n" +
                "  - Debian/Ubuntu: sudo apt-get install nmap\n" +
                "  - RHEL/CentOS: sudo yum install nmap\n" +
                "  - Arch: sudo pacman -S nmap");
        }

        var version = GetNmapVersion();
        if (version != null)
        {
            Console.WriteLine($"Using {version}");
        }
    }
}

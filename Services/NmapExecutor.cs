using System.Diagnostics;
using NmapCli.Models;

namespace NmapCli.Services;

public class NmapExecutor
{
    private readonly CommandBuilder _commandBuilder;

    public NmapExecutor()
    {
        _commandBuilder = new CommandBuilder();
    }

    public async Task<ScanResult> ExecuteScanAsync(ScanOptions options, bool useSudo = false)
    {
        var nmapArgs = _commandBuilder.BuildCommand(options);
        var command = useSudo ? "sudo" : "nmap";
        var arguments = useSudo ? $"nmap {nmapArgs}" : nmapArgs;

        var result = new ScanResult
        {
            Command = $"{command} {(useSudo ? nmapArgs : arguments)}",
            StartTime = DateTime.Now
        };

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var outputBuilder = new System.Text.StringBuilder();
            var errorBuilder = new System.Text.StringBuilder();

            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    outputBuilder.AppendLine(e.Data);
                    Console.WriteLine(e.Data);
                }
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    errorBuilder.AppendLine(e.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            await process.WaitForExitAsync();

            result.EndTime = DateTime.Now;
            result.Output = outputBuilder.ToString();
            result.ErrorOutput = errorBuilder.ToString();
            result.ExitCode = process.ExitCode;
            result.Success = process.ExitCode == 0;

            return result;
        }
        catch (Exception ex)
        {
            result.EndTime = DateTime.Now;
            result.Success = false;
            result.ErrorOutput = $"Failed to execute nmap: {ex.Message}";
            return result;
        }
    }

    public ScanResult ExecuteScan(ScanOptions options, bool useSudo = false)
    {
        return ExecuteScanAsync(options, useSudo).GetAwaiter().GetResult();
    }
}

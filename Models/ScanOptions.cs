using NmapCli.ScanTypes;

namespace NmapCli.Models;

public class ScanOptions
{
    public ScanType ScanType { get; set; } = ScanType.TcpSyn;
    public TargetSpecification Targets { get; set; } = new();
    public List<int> Ports { get; set; } = new();
    public string? PortRange { get; set; }
    public bool ScanAllPorts { get; set; }
    public bool FastMode { get; set; }
    public int? TopPorts { get; set; }
    public TimingTemplate Timing { get; set; } = TimingTemplate.Normal;
    public string? OutputFile { get; set; }
    public OutputFormat OutputFormat { get; set; } = OutputFormat.Console;
    public bool Verbose { get; set; }
    public int? MaxRetries { get; set; }
    public int? HostTimeout { get; set; }
    public int? MaxParallel { get; set; }

    public void Validate()
    {
        if (!Targets.HasTargets())
        {
            throw new InvalidOperationException("No targets specified");
        }

        if (Ports.Count == 0 && string.IsNullOrWhiteSpace(PortRange) && !ScanAllPorts && !FastMode && TopPorts == null)
        {
            PortRange = "1-1000";
        }

        if (Ports.Any(p => p < 1 || p > 65535))
        {
            throw new ArgumentException("Port numbers must be between 1 and 65535");
        }
    }
}

public enum OutputFormat
{
    Console,
    Text,
    Json,
    Xml
}

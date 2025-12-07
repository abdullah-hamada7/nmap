using System.Text;
using NmapCli.Models;
using NmapCli.ScanTypes;

namespace NmapCli.Services;

public class CommandBuilder
{
    public string BuildCommand(ScanOptions options)
    {
        options.Validate();

        var args = new StringBuilder();

        args.Append(options.ScanType.ToNmapFlag());

        BuildPortSpecification(args, options);
        BuildTimingOptions(args, options);
        BuildTargetSpecification(args, options);
        BuildOutputOptions(args, options);
        BuildPerformanceOptions(args, options);

        if (options.Verbose)
        {
            args.Append(" -v");
        }

        return args.ToString().Trim();
    }

    private void BuildPortSpecification(StringBuilder args, ScanOptions options)
    {
        if (options.ScanAllPorts)
        {
            args.Append(" -p-");
        }
        else if (options.FastMode)
        {
            args.Append(" -F");
        }
        else if (options.TopPorts.HasValue)
        {
            args.Append($" --top-ports {options.TopPorts.Value}");
        }
        else if (options.Ports.Count > 0)
        {
            args.Append($" -p {string.Join(",", options.Ports)}");
        }
        else if (!string.IsNullOrWhiteSpace(options.PortRange))
        {
            args.Append($" -p {options.PortRange}");
        }
    }

    private void BuildTimingOptions(StringBuilder args, ScanOptions options)
    {
        args.Append($" {options.Timing.ToNmapFlag()}");
    }

    private void BuildTargetSpecification(StringBuilder args, ScanOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Targets.InputFile))
        {
            args.Append($" -iL {options.Targets.InputFile}");
        }
        else if (options.Targets.Targets.Count > 0)
        {
            args.Append($" {string.Join(" ", options.Targets.Targets)}");
        }

        if (options.Targets.ExcludedTargets.Count > 0)
        {
            args.Append($" --exclude {string.Join(",", options.Targets.ExcludedTargets)}");
        }
    }

    private void BuildOutputOptions(StringBuilder args, ScanOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.OutputFile))
        {
            var flag = options.OutputFormat switch
            {
                OutputFormat.Text => "-oN",
                OutputFormat.Json => "-oJ",
                OutputFormat.Xml => "-oX",
                _ => null
            };

            if (flag != null)
            {
                args.Append($" {flag} {options.OutputFile}");
            }
        }
    }

    private void BuildPerformanceOptions(StringBuilder args, ScanOptions options)
    {
        if (options.MaxRetries.HasValue)
        {
            args.Append($" --max-retries {options.MaxRetries.Value}");
        }

        if (options.HostTimeout.HasValue)
        {
            args.Append($" --host-timeout {options.HostTimeout.Value}s");
        }

        if (options.MaxParallel.HasValue)
        {
            args.Append($" --min-parallelism {options.MaxParallel.Value}");
        }
    }
}

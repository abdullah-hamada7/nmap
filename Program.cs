using NmapCli.Models;
using NmapCli.ScanTypes;
using NmapCli.Services;
using NmapCli.UI;
using NmapCli.Utilities;

namespace NmapCli;

class Program
{
    static int Main(string[] args)
    {
        try
        {
            Console.WriteLine("Nmap CLI Wrapper v1.0");
            Console.WriteLine("=".PadRight(80, '='));
            Console.WriteLine();

            NmapValidator.CheckNmapAvailability();
            Console.WriteLine();

            if (args.Length == 0)
            {
                var menu = new InteractiveMenu();
                menu.Run();
                return 0;
            }

            return RunCommandLine(args);
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Fatal error: {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }

    static int RunCommandLine(string[] args)
    {
        var options = new ScanOptions();
        var executor = new NmapExecutor();
        var formatter = new OutputFormatter();
        bool useSudo = false;

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            switch (arg)
            {
                case "-sS":
                    options.ScanType = ScanType.TcpSyn;
                    break;
                case "-sT":
                    options.ScanType = ScanType.TcpConnect;
                    break;
                case "-sA":
                    options.ScanType = ScanType.TcpAck;
                    break;
                case "-sF":
                    options.ScanType = ScanType.TcpFin;
                    break;
                case "-sN":
                    options.ScanType = ScanType.TcpNull;
                    break;
                case "-sX":
                    options.ScanType = ScanType.TcpXmas;
                    break;
                case "-sW":
                    options.ScanType = ScanType.TcpWindow;
                    break;
                case "-sU":
                    options.ScanType = ScanType.Udp;
                    break;
                case "-p":
                    if (i + 1 < args.Length)
                    {
                        options.PortRange = args[++i];
                    }
                    break;
                case "-p-":
                    options.ScanAllPorts = true;
                    break;
                case "-F":
                    options.FastMode = true;
                    break;
                case "--top-ports":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int topPorts))
                    {
                        options.TopPorts = topPorts;
                    }
                    break;
                case "-T0":
                    options.Timing = TimingTemplate.Paranoid;
                    break;
                case "-T1":
                    options.Timing = TimingTemplate.Sneaky;
                    break;
                case "-T2":
                    options.Timing = TimingTemplate.Polite;
                    break;
                case "-T3":
                    options.Timing = TimingTemplate.Normal;
                    break;
                case "-T4":
                    options.Timing = TimingTemplate.Aggressive;
                    break;
                case "-T5":
                    options.Timing = TimingTemplate.Insane;
                    break;
                case "-oN":
                    if (i + 1 < args.Length)
                    {
                        options.OutputFormat = OutputFormat.Text;
                        options.OutputFile = args[++i];
                    }
                    break;
                case "-oJ":
                    if (i + 1 < args.Length)
                    {
                        options.OutputFormat = OutputFormat.Json;
                        options.OutputFile = args[++i];
                    }
                    break;
                case "-oX":
                    if (i + 1 < args.Length)
                    {
                        options.OutputFormat = OutputFormat.Xml;
                        options.OutputFile = args[++i];
                    }
                    break;
                case "-iL":
                    if (i + 1 < args.Length)
                    {
                        options.Targets.InputFile = args[++i];
                    }
                    break;
                case "--exclude":
                    if (i + 1 < args.Length)
                    {
                        var exclusions = args[++i].Split(',');
                        foreach (var exclusion in exclusions)
                        {
                            options.Targets.AddExclusion(exclusion);
                        }
                    }
                    break;
                case "-v":
                    options.Verbose = true;
                    break;
                case "--sudo":
                    useSudo = true;
                    break;
                case "-h":
                case "--help":
                    ShowHelp();
                    return 0;
                default:
                    if (!arg.StartsWith("-"))
                    {
                        options.Targets.AddTarget(arg);
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Unknown option '{arg}', ignoring...");
                    }
                    break;
            }
        }

        try
        {
            options.Validate();

            if (options.ScanType.RequiresRoot() && !NmapValidator.IsRunningAsRoot())
            {
                useSudo = true;
                Console.WriteLine("Note: This scan requires root privileges, using sudo...");
                Console.WriteLine();
            }

            var result = executor.ExecuteScan(options, useSudo);
            formatter.DisplayResults(result);

            if (!string.IsNullOrWhiteSpace(options.OutputFile) && options.OutputFormat != OutputFormat.Console)
            {
                formatter.SaveToFile(result, options.OutputFile, options.OutputFormat);
            }

            return result.Success ? 0 : 1;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }

    static void ShowHelp()
    {
        Console.WriteLine(@"
Nmap CLI Wrapper - Usage
========================

INTERACTIVE MODE:
  Run without arguments to launch interactive menu

COMMAND LINE MODE:
  NmapCli [OPTIONS] <target>

SCAN TYPES:
  -sS                TCP SYN scan (stealth, requires root)
  -sT                TCP Connect scan
  -sA                TCP ACK scan
  -sF                TCP FIN scan
  -sN                TCP NULL scan
  -sX                TCP Xmas scan
  -sW                TCP Window scan
  -sU                UDP scan (requires root)

TARGET SPECIFICATION:
  <single IP>        Single IP address (192.168.1.1)
  <hostname>         Domain name (scanme.nmap.org)
  <CIDR>             CIDR notation (192.168.1.0/24)
  <range>            IP range (192.168.1.1-50)
  -iL <file>         Import targets from file
  --exclude <list>   Exclude targets (comma-separated)

PORT SPECIFICATION:
  -p <ports>         Specific ports (80,443,8080)
  -p <range>         Port range (1-1000)
  -p-                All 65535 ports
  -F                 Fast mode (top 100 ports)
  --top-ports <N>    Scan top N ports

TIMING:
  -T0                Paranoid (IDS evasion)
  -T1                Sneaky
  -T2                Polite
  -T3                Normal (default)
  -T4                Aggressive
  -T5                Insane

OUTPUT:
  -oN <file>         Save to text file
  -oJ <file>         Save to JSON file
  -oX <file>         Save to XML file

OTHER OPTIONS:
  -v                 Verbose output
  --sudo             Force sudo usage
  -h, --help         Show this help

EXAMPLES:
  NmapCli -sS 192.168.1.1
  NmapCli -sT -p 80,443 scanme.nmap.org
  NmapCli -sS -T4 192.168.1.0/24 -oX results.xml
  NmapCli -F -iL targets.txt --exclude 192.168.1.1
");
    }
}

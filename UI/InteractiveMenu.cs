using NmapCli.Models;
using NmapCli.ScanTypes;
using NmapCli.Services;
using NmapCli.Utilities;

namespace NmapCli.UI;

public class InteractiveMenu
{
    private readonly ScanOptions _scanOptions = new();
    private readonly NmapExecutor _executor = new();
    private readonly OutputFormatter _formatter = new();

    public void Run()
    {
        while (true)
        {
            ShowMainMenu();
            var choice = MenuHelpers.GetMenuChoice("Select an option", 0, 9);

            switch (choice)
            {
                case 1:
                    ConfigureScanType();
                    break;
                case 2:
                    ConfigureTargets();
                    break;
                case 3:
                    ConfigurePorts();
                    break;
                case 4:
                    ConfigureTiming();
                    break;
                case 5:
                    ConfigureOutput();
                    break;
                case 6:
                    ConfigureAdvanced();
                    break;
                case 7:
                    ShowCurrentConfiguration();
                    break;
                case 8:
                    ExecuteScan();
                    break;
                case 9:
                    ShowHelp();
                    break;
                case 0:
                    return;
            }
        }
    }

    private void ShowMainMenu()
    {
        MenuHelpers.ShowHeader("NMAP CLI Wrapper - Interactive Menu");

        Console.WriteLine("1. Configure Scan Type");
        Console.WriteLine("2. Configure Targets");
        Console.WriteLine("3. Configure Ports");
        Console.WriteLine("4. Configure Timing");
        Console.WriteLine("5. Configure Output");
        Console.WriteLine("6. Advanced Options");
        Console.WriteLine("7. Show Current Configuration");
        Console.WriteLine("8. Execute Scan");
        Console.WriteLine("9. Help");
        Console.WriteLine("0. Exit");
        Console.WriteLine();
    }

    private void ConfigureScanType()
    {
        MenuHelpers.ShowHeader("Select Scan Type");

        var scanTypes = Enum.GetValues<ScanType>();
        for (int i = 0; i < scanTypes.Length; i++)
        {
            var scanType = scanTypes[i];
            var rootWarning = scanType.RequiresRoot() ? " [REQUIRES ROOT]" : "";
            Console.WriteLine($"{i + 1}. {scanType.GetDescription()}{rootWarning}");
        }

        var choice = MenuHelpers.GetMenuChoice("Select scan type", 1, scanTypes.Length);
        _scanOptions.ScanType = scanTypes[choice - 1];

        if (_scanOptions.ScanType.RequiresRoot() && !NmapValidator.IsRunningAsRoot())
        {
            MenuHelpers.ShowWarning("This scan type requires root privileges. You will be prompted for sudo.");
        }

        MenuHelpers.ShowSuccess($"Scan type set to: {_scanOptions.ScanType.GetDescription()}");
        MenuHelpers.PressAnyKey();
    }

    private void ConfigureTargets()
    {
        MenuHelpers.ShowHeader("Configure Targets");

        Console.WriteLine("1. Add single target (IP/hostname)");
        Console.WriteLine("2. Add CIDR range");
        Console.WriteLine("3. Add IP range");
        Console.WriteLine("4. Import from file");
        Console.WriteLine("5. Add exclusion");
        Console.WriteLine("6. Clear all targets");
        Console.WriteLine("7. Show current targets");
        Console.WriteLine("0. Back");
        Console.WriteLine();

        var choice = MenuHelpers.GetMenuChoice("Select an option", 0, 7);

        switch (choice)
        {
            case 1:
                AddSingleTarget();
                break;
            case 2:
                AddCidrTarget();
                break;
            case 3:
                AddRangeTarget();
                break;
            case 4:
                ImportTargetsFromFile();
                break;
            case 5:
                AddExclusion();
                break;
            case 6:
                _scanOptions.Targets = new TargetSpecification();
                MenuHelpers.ShowSuccess("All targets cleared");
                MenuHelpers.PressAnyKey();
                break;
            case 7:
                ShowCurrentTargets();
                break;
        }
    }

    private void AddSingleTarget()
    {
        var target = MenuHelpers.GetInput("Enter IP address or hostname");
        if (IpAddressHelper.ValidateTarget(target))
        {
            _scanOptions.Targets.AddTarget(target);
            MenuHelpers.ShowSuccess($"Target added: {target}");
        }
        else
        {
            MenuHelpers.ShowError("Invalid target format");
        }
        MenuHelpers.PressAnyKey();
    }

    private void AddCidrTarget()
    {
        var cidr = MenuHelpers.GetInput("Enter CIDR notation (e.g., 192.168.1.0/24)");
        if (IpAddressHelper.IsValidCidr(cidr))
        {
            _scanOptions.Targets.AddTarget(cidr);
            MenuHelpers.ShowSuccess($"CIDR target added: {cidr}");
        }
        else
        {
            MenuHelpers.ShowError("Invalid CIDR format");
        }
        MenuHelpers.PressAnyKey();
    }

    private void AddRangeTarget()
    {
        var range = MenuHelpers.GetInput("Enter IP range (e.g., 192.168.1.1-50)");
        if (IpAddressHelper.IsValidIpRange(range))
        {
            _scanOptions.Targets.AddTarget(range);
            MenuHelpers.ShowSuccess($"IP range added: {range}");
        }
        else
        {
            MenuHelpers.ShowError("Invalid IP range format");
        }
        MenuHelpers.PressAnyKey();
    }

    private void ImportTargetsFromFile()
    {
        var filePath = MenuHelpers.GetInput("Enter file path");
        try
        {
            var targets = IpAddressHelper.ReadTargetsFromFile(filePath);
            foreach (var target in targets)
            {
                _scanOptions.Targets.AddTarget(target);
            }
            MenuHelpers.ShowSuccess($"Imported {targets.Count} targets from file");
        }
        catch (Exception ex)
        {
            MenuHelpers.ShowError($"Failed to import targets: {ex.Message}");
        }
        MenuHelpers.PressAnyKey();
    }

    private void AddExclusion()
    {
        var exclusion = MenuHelpers.GetInput("Enter target to exclude");
        if (IpAddressHelper.ValidateTarget(exclusion))
        {
            _scanOptions.Targets.AddExclusion(exclusion);
            MenuHelpers.ShowSuccess($"Exclusion added: {exclusion}");
        }
        else
        {
            MenuHelpers.ShowError("Invalid exclusion format");
        }
        MenuHelpers.PressAnyKey();
    }

    private void ShowCurrentTargets()
    {
        MenuHelpers.ShowHeader("Current Targets");
        
        if (_scanOptions.Targets.Targets.Count > 0)
        {
            Console.WriteLine("Targets:");
            foreach (var target in _scanOptions.Targets.Targets)
            {
                Console.WriteLine($"  - {target}");
            }
        }
        else if (!string.IsNullOrWhiteSpace(_scanOptions.Targets.InputFile))
        {
            Console.WriteLine($"Targets from file: {_scanOptions.Targets.InputFile}");
        }
        else
        {
            Console.WriteLine("No targets configured");
        }

        if (_scanOptions.Targets.ExcludedTargets.Count > 0)
        {
            Console.WriteLine("\nExclusions:");
            foreach (var exclusion in _scanOptions.Targets.ExcludedTargets)
            {
                Console.WriteLine($"  - {exclusion}");
            }
        }

        MenuHelpers.PressAnyKey();
    }

    private void ConfigurePorts()
    {
        MenuHelpers.ShowHeader("Configure Ports");

        Console.WriteLine("1. Scan all ports (1-65535)");
        Console.WriteLine("2. Fast mode (top 100 ports)");
        Console.WriteLine("3. Top N ports");
        Console.WriteLine("4. Specific ports");
        Console.WriteLine("5. Port range");
        Console.WriteLine("0. Back");
        Console.WriteLine();

        var choice = MenuHelpers.GetMenuChoice("Select an option", 0, 5);

        switch (choice)
        {
            case 1:
                _scanOptions.ScanAllPorts = true;
                _scanOptions.FastMode = false;
                _scanOptions.TopPorts = null;
                _scanOptions.Ports.Clear();
                _scanOptions.PortRange = null;
                MenuHelpers.ShowSuccess("Will scan all 65535 ports");
                MenuHelpers.PressAnyKey();
                break;
            case 2:
                _scanOptions.FastMode = true;
                _scanOptions.ScanAllPorts = false;
                _scanOptions.TopPorts = null;
                _scanOptions.Ports.Clear();
                _scanOptions.PortRange = null;
                MenuHelpers.ShowSuccess("Fast mode enabled (top 100 ports)");
                MenuHelpers.PressAnyKey();
                break;
            case 3:
                var topN = MenuHelpers.GetInput("Enter number of top ports");
                if (int.TryParse(topN, out int n) && n > 0)
                {
                    _scanOptions.TopPorts = n;
                    _scanOptions.FastMode = false;
                    _scanOptions.ScanAllPorts = false;
                    _scanOptions.Ports.Clear();
                    _scanOptions.PortRange = null;
                    MenuHelpers.ShowSuccess($"Will scan top {n} ports");
                }
                else
                {
                    MenuHelpers.ShowError("Invalid number");
                }
                MenuHelpers.PressAnyKey();
                break;
            case 4:
                var ports = MenuHelpers.GetInput("Enter ports (comma-separated, e.g., 80,443,8080)");
                try
                {
                    _scanOptions.Ports = ports.Split(',').Select(p => int.Parse(p.Trim())).ToList();
                    _scanOptions.FastMode = false;
                    _scanOptions.ScanAllPorts = false;
                    _scanOptions.TopPorts = null;
                    _scanOptions.PortRange = null;
                    MenuHelpers.ShowSuccess($"Ports set: {string.Join(", ", _scanOptions.Ports)}");
                }
                catch
                {
                    MenuHelpers.ShowError("Invalid port format");
                }
                MenuHelpers.PressAnyKey();
                break;
            case 5:
                var range = MenuHelpers.GetInput("Enter port range (e.g., 1-1000)");
                _scanOptions.PortRange = range;
                _scanOptions.FastMode = false;
                _scanOptions.ScanAllPorts = false;
                _scanOptions.TopPorts = null;
                _scanOptions.Ports.Clear();
                MenuHelpers.ShowSuccess($"Port range set: {range}");
                MenuHelpers.PressAnyKey();
                break;
        }
    }

    private void ConfigureTiming()
    {
        MenuHelpers.ShowHeader("Configure Timing Template");

        var templates = Enum.GetValues<TimingTemplate>();
        for (int i = 0; i < templates.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {templates[i].GetDescription()}");
        }

        var choice = MenuHelpers.GetMenuChoice("Select timing template", 1, templates.Length);
        _scanOptions.Timing = templates[choice - 1];

        MenuHelpers.ShowSuccess($"Timing set to: {_scanOptions.Timing.GetDescription()}");
        MenuHelpers.PressAnyKey();
    }

    private void ConfigureOutput()
    {
        MenuHelpers.ShowHeader("Configure Output");

        Console.WriteLine("1. Console only");
        Console.WriteLine("2. Save to text file");
        Console.WriteLine("3. Save to JSON file");
        Console.WriteLine("4. Save to XML file");
        Console.WriteLine("0. Back");
        Console.WriteLine();

        var choice = MenuHelpers.GetMenuChoice("Select output format", 0, 4);

        switch (choice)
        {
            case 1:
                _scanOptions.OutputFormat = OutputFormat.Console;
                _scanOptions.OutputFile = null;
                MenuHelpers.ShowSuccess("Output: Console only");
                MenuHelpers.PressAnyKey();
                break;
            case 2:
            case 3:
            case 4:
                var format = choice switch
                {
                    2 => OutputFormat.Text,
                    3 => OutputFormat.Json,
                    4 => OutputFormat.Xml,
                    _ => OutputFormat.Console
                };
                var extension = choice switch
                {
                    2 => ".txt",
                    3 => ".json",
                    4 => ".xml",
                    _ => ".txt"
                };
                var file = MenuHelpers.GetInput($"Enter output file path (will use {extension} extension)");
                if (!file.EndsWith(extension))
                {
                    file += extension;
                }
                _scanOptions.OutputFormat = format;
                _scanOptions.OutputFile = file;
                MenuHelpers.ShowSuccess($"Output will be saved to: {file}");
                MenuHelpers.PressAnyKey();
                break;
        }
    }

    private void ConfigureAdvanced()
    {
        MenuHelpers.ShowHeader("Advanced Options");

        Console.WriteLine("1. Set max retries");
        Console.WriteLine("2. Set host timeout");
        Console.WriteLine("3. Set max parallel hosts");
        Console.WriteLine("4. Toggle verbose mode");
        Console.WriteLine("0. Back");
        Console.WriteLine();

        var choice = MenuHelpers.GetMenuChoice("Select an option", 0, 4);

        switch (choice)
        {
            case 1:
                var retries = MenuHelpers.GetInput("Enter max retries (default: 10)");
                if (int.TryParse(retries, out int r))
                {
                    _scanOptions.MaxRetries = r;
                    MenuHelpers.ShowSuccess($"Max retries set to: {r}");
                }
                else
                {
                    MenuHelpers.ShowError("Invalid number");
                }
                MenuHelpers.PressAnyKey();
                break;
            case 2:
                var timeout = MenuHelpers.GetInput("Enter host timeout in seconds");
                if (int.TryParse(timeout, out int t))
                {
                    _scanOptions.HostTimeout = t;
                    MenuHelpers.ShowSuccess($"Host timeout set to: {t}s");
                }
                else
                {
                    MenuHelpers.ShowError("Invalid number");
                }
                MenuHelpers.PressAnyKey();
                break;
            case 3:
                var parallel = MenuHelpers.GetInput("Enter max parallel hosts");
                if (int.TryParse(parallel, out int p))
                {
                    _scanOptions.MaxParallel = p;
                    MenuHelpers.ShowSuccess($"Max parallel hosts set to: {p}");
                }
                else
                {
                    MenuHelpers.ShowError("Invalid number");
                }
                MenuHelpers.PressAnyKey();
                break;
            case 4:
                _scanOptions.Verbose = !_scanOptions.Verbose;
                MenuHelpers.ShowSuccess($"Verbose mode: {(_scanOptions.Verbose ? "ON" : "OFF")}");
                MenuHelpers.PressAnyKey();
                break;
        }
    }

    private void ShowCurrentConfiguration()
    {
        MenuHelpers.ShowHeader("Current Configuration");

        Console.WriteLine($"Scan Type: {_scanOptions.ScanType.GetDescription()}");
        Console.WriteLine($"Timing: {_scanOptions.Timing.GetDescription()}");
        
        Console.Write("Targets: ");
        if (_scanOptions.Targets.Targets.Count > 0)
        {
            Console.WriteLine(string.Join(", ", _scanOptions.Targets.Targets));
        }
        else if (!string.IsNullOrWhiteSpace(_scanOptions.Targets.InputFile))
        {
            Console.WriteLine($"From file: {_scanOptions.Targets.InputFile}");
        }
        else
        {
            Console.WriteLine("None configured");
        }

        Console.Write("Ports: ");
        if (_scanOptions.ScanAllPorts)
        {
            Console.WriteLine("All ports (1-65535)");
        }
        else if (_scanOptions.FastMode)
        {
            Console.WriteLine("Fast mode (top 100)");
        }
        else if (_scanOptions.TopPorts.HasValue)
        {
            Console.WriteLine($"Top {_scanOptions.TopPorts.Value}");
        }
        else if (_scanOptions.Ports.Count > 0)
        {
            Console.WriteLine(string.Join(", ", _scanOptions.Ports));
        }
        else if (!string.IsNullOrWhiteSpace(_scanOptions.PortRange))
        {
            Console.WriteLine(_scanOptions.PortRange);
        }
        else
        {
            Console.WriteLine("Default (1-1000)");
        }

        Console.WriteLine($"Output: {_scanOptions.OutputFormat}" + 
            (_scanOptions.OutputFile != null ? $" -> {_scanOptions.OutputFile}" : ""));
        Console.WriteLine($"Verbose: {_scanOptions.Verbose}");

        if (_scanOptions.MaxRetries.HasValue)
            Console.WriteLine($"Max Retries: {_scanOptions.MaxRetries}");
        if (_scanOptions.HostTimeout.HasValue)
            Console.WriteLine($"Host Timeout: {_scanOptions.HostTimeout}s");
        if (_scanOptions.MaxParallel.HasValue)
            Console.WriteLine($"Max Parallel: {_scanOptions.MaxParallel}");

        MenuHelpers.PressAnyKey();
    }

    private void ExecuteScan()
    {
        MenuHelpers.ShowHeader("Execute Scan");

        try
        {
            _scanOptions.Validate();
        }
        catch (Exception ex)
        {
            MenuHelpers.ShowError($"Configuration error: {ex.Message}");
            MenuHelpers.PressAnyKey();
            return;
        }

        ShowCurrentConfiguration();
        Console.WriteLine();

        var useSudo = _scanOptions.ScanType.RequiresRoot() && !NmapValidator.IsRunningAsRoot();
        if (useSudo)
        {
            MenuHelpers.ShowWarning("This scan requires root privileges and will use sudo.");
        }

        if (!MenuHelpers.GetYesNo("Proceed with scan?", true))
        {
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Starting scan...");
        Console.WriteLine();

        var result = _executor.ExecuteScan(_scanOptions, useSudo);

        Console.WriteLine();
        _formatter.DisplayResults(result);

        if (!string.IsNullOrWhiteSpace(_scanOptions.OutputFile) && _scanOptions.OutputFormat != OutputFormat.Console)
        {
            _formatter.SaveToFile(result, _scanOptions.OutputFile, _scanOptions.OutputFormat);
        }

        MenuHelpers.PressAnyKey();
    }

    private void ShowHelp()
    {
        MenuHelpers.ShowHeader("Help - Nmap CLI Wrapper");

        Console.WriteLine("This interactive menu allows you to configure and execute nmap scans.");
        Console.WriteLine();
        Console.WriteLine("SCAN TYPES:");
        Console.WriteLine("  - TCP SYN: Stealth scan, requires root, most popular");
        Console.WriteLine("  - TCP Connect: Full connection, doesn't require root");
        Console.WriteLine("  - TCP ACK: Used for firewall rule mapping");
        Console.WriteLine("  - TCP FIN/NULL/Xmas: Stealthy scans for specific scenarios");
        Console.WriteLine("  - UDP: Scans UDP ports, requires root, can be slow");
        Console.WriteLine();
        Console.WriteLine("TIMING TEMPLATES:");
        Console.WriteLine("  - T0/T1: Very slow, for IDS evasion");
        Console.WriteLine("  - T2: Polite, uses less bandwidth");
        Console.WriteLine("  - T3: Normal/default timing");
        Console.WriteLine("  - T4: Aggressive, assumes good network");
        Console.WriteLine("  - T5: Insane, extremely fast but may miss results");
        Console.WriteLine();
        Console.WriteLine("TARGET FORMATS:");
        Console.WriteLine("  - Single IP: 192.168.1.1");
        Console.WriteLine("  - Hostname: scanme.nmap.org");
        Console.WriteLine("  - CIDR: 192.168.1.0/24");
        Console.WriteLine("  - Range: 192.168.1.1-50");
        Console.WriteLine();
        Console.WriteLine("For more information, visit: https://nmap.org/book/man.html");

        MenuHelpers.PressAnyKey();
    }
}

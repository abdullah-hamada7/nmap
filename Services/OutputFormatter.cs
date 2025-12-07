using NmapCli.Models;

namespace NmapCli.Services;

public class OutputFormatter
{
    public void DisplayResults(ScanResult result)
    {
        Console.WriteLine();
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("SCAN RESULTS");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();

        Console.WriteLine($"Command: {result.Command}");
        Console.WriteLine($"Duration: {result.Duration.TotalSeconds:F2} seconds");
        Console.WriteLine($"Exit Code: {result.ExitCode}");
        Console.WriteLine($"Status: {(result.Success ? "SUCCESS" : "FAILED")}");
        Console.WriteLine();

        if (!string.IsNullOrWhiteSpace(result.Output))
        {
            Console.WriteLine("Output:");
            Console.WriteLine("-".PadRight(80, '-'));
            Console.WriteLine(result.Output);
        }

        if (!string.IsNullOrWhiteSpace(result.ErrorOutput))
        {
            Console.WriteLine();
            Console.WriteLine("Errors:");
            Console.WriteLine("-".PadRight(80, '-'));
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(result.ErrorOutput);
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.WriteLine("=".PadRight(80, '='));
    }

    public void SaveToFile(ScanResult result, string filePath, OutputFormat format)
    {
        switch (format)
        {
            case OutputFormat.Text:
                SaveAsText(result, filePath);
                break;
            case OutputFormat.Json:
                Console.WriteLine($"JSON output saved by nmap to: {filePath}");
                break;
            case OutputFormat.Xml:
                Console.WriteLine($"XML output saved by nmap to: {filePath}");
                break;
            default:
                throw new ArgumentException($"Unsupported output format: {format}");
        }
    }

    private void SaveAsText(ScanResult result, string filePath)
    {
        var content = $@"Nmap Scan Results
==================
Command: {result.Command}
Start Time: {result.StartTime}
End Time: {result.EndTime}
Duration: {result.Duration.TotalSeconds:F2} seconds
Exit Code: {result.ExitCode}
Status: {(result.Success ? "SUCCESS" : "FAILED")}

Output:
{result.Output}

Errors:
{result.ErrorOutput}
";
        File.WriteAllText(filePath, content);
        Console.WriteLine($"Results saved to: {filePath}");
    }
}

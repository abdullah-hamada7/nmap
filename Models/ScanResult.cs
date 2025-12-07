namespace NmapCli.Models;

public class ScanResult
{
    public bool Success { get; set; }
    public string Output { get; set; } = string.Empty;
    public string ErrorOutput { get; set; } = string.Empty;
    public int ExitCode { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public TimeSpan Duration => EndTime - StartTime;
    public string Command { get; set; } = string.Empty;

    public static ScanResult Failed(string error, string command)
    {
        return new ScanResult
        {
            Success = false,
            ErrorOutput = error,
            Command = command,
            StartTime = DateTime.Now,
            EndTime = DateTime.Now
        };
    }
}

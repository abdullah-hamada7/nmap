namespace NmapCli.Models;

public class TargetSpecification
{
    public List<string> Targets { get; set; } = new();
    public List<string> ExcludedTargets { get; set; } = new();
    public string? InputFile { get; set; }

    public void AddTarget(string target)
    {
        if (!string.IsNullOrWhiteSpace(target))
        {
            Targets.Add(target.Trim());
        }
    }

    public void AddExclusion(string exclusion)
    {
        if (!string.IsNullOrWhiteSpace(exclusion))
        {
            ExcludedTargets.Add(exclusion.Trim());
        }
    }

    public bool HasTargets()
    {
        return Targets.Count > 0 || !string.IsNullOrWhiteSpace(InputFile);
    }
}

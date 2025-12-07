namespace NmapCli.ScanTypes;

public enum TimingTemplate
{
    Paranoid = 0,
    Sneaky = 1,
    Polite = 2,
    Normal = 3,
    Aggressive = 4,
    Insane = 5
}

public static class TimingTemplateExtensions
{
    public static string ToNmapFlag(this TimingTemplate timing)
    {
        return $"-T{(int)timing}";
    }

    public static string GetDescription(this TimingTemplate timing)
    {
        return timing switch
        {
            TimingTemplate.Paranoid => "T0: Paranoid (IDS evasion, extremely slow)",
            TimingTemplate.Sneaky => "T1: Sneaky (IDS evasion, very slow)",
            TimingTemplate.Polite => "T2: Polite (Less bandwidth, slower)",
            TimingTemplate.Normal => "T3: Normal (Default timing)",
            TimingTemplate.Aggressive => "T4: Aggressive (Fast, assumes good network)",
            TimingTemplate.Insane => "T5: Insane (Extremely fast, may sacrifice accuracy)",
            _ => throw new ArgumentOutOfRangeException(nameof(timing))
        };
    }
}

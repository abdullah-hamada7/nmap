namespace NmapCli.ScanTypes;

public enum ScanType
{
    TcpSyn,
    TcpConnect,
    TcpAck,
    TcpFin,
    TcpNull,
    TcpXmas,
    TcpWindow,
    Udp
}

public static class ScanTypeExtensions
{
    public static string ToNmapFlag(this ScanType scanType)
    {
        return scanType switch
        {
            ScanType.TcpSyn => "-sS",
            ScanType.TcpConnect => "-sT",
            ScanType.TcpAck => "-sA",
            ScanType.TcpFin => "-sF",
            ScanType.TcpNull => "-sN",
            ScanType.TcpXmas => "-sX",
            ScanType.TcpWindow => "-sW",
            ScanType.Udp => "-sU",
            _ => throw new ArgumentOutOfRangeException(nameof(scanType))
        };
    }

    public static string GetDescription(this ScanType scanType)
    {
        return scanType switch
        {
            ScanType.TcpSyn => "TCP SYN Scan (Stealth/Half-open) - Requires root",
            ScanType.TcpConnect => "TCP Connect Scan (Full connection)",
            ScanType.TcpAck => "TCP ACK Scan (Firewall rule mapping)",
            ScanType.TcpFin => "TCP FIN Scan",
            ScanType.TcpNull => "TCP NULL Scan",
            ScanType.TcpXmas => "TCP Xmas Scan",
            ScanType.TcpWindow => "TCP Window Scan",
            ScanType.Udp => "UDP Scan - Requires root",
            _ => throw new ArgumentOutOfRangeException(nameof(scanType))
        };
    }

    public static bool RequiresRoot(this ScanType scanType)
    {
        return scanType is ScanType.TcpSyn or ScanType.Udp;
    }
}

# Nmap CLI Wrapper

### Scan Types

- **TCP SYN Scan** (`-sS`): Stealth/half-open scan (requires root)
- **TCP Connect Scan** (`-sT`): Full connection scan
- **TCP ACK Scan** (`-sA`): Firewall rule mapping
- **TCP FIN Scan** (`-sF`): Stealth scan
- **TCP NULL Scan** (`-sN`): Stealth scan
- **TCP Xmas Scan** (`-sX`): Stealth scan
- **TCP Window Scan** (`-sW`): Window size analysis
- **UDP Scan** (`-sU`): UDP port scanning (requires root)

### Target Specification

- Single IP addresses (`192.168.1.1`)
- Hostnames (`scanme.nmap.org`)
- CIDR notation (`192.168.1.0/24`)
- IP ranges (`192.168.1.1-50`)
- File input (`-iL targets.txt`)
- Exclusion lists (`--exclude 192.168.1.1,192.168.1.5`)

### Port Specification

- Specific ports (`-p 80,443,8080`)
- Port ranges (`-p 1-1000`)
- All ports (`-p-`)
- Fast mode - top 100 ports (`-F`)
- Top N ports (`--top-ports 1000`)

### Timing Templates

- **T0**: Paranoid (IDS evasion, extremely slow)
- **T1**: Sneaky (IDS evasion, very slow)
- **T2**: Polite (Less bandwidth)
- **T3**: Normal (Default)
- **T4**: Aggressive (Fast)
- **T5**: Insane (Extremely fast)

### Output Formats

- Console output (default)
- Text file (`-oN output.txt`)
- JSON file (`-oJ output.json`)
- XML file (`-oX output.xml`)

### Interactive Menu

- User-friendly interactive interface
- Step-by-step scan configuration
- Real-time scan execution
- Configuration preview

## Command-Line Options

### Scan Types

- `-sS` - TCP SYN scan (stealth, requires root)
- `-sT` - TCP Connect scan
- `-sA` - TCP ACK scan
- `-sF` - TCP FIN scan
- `-sN` - TCP NULL scan
- `-sX` - TCP Xmas scan
- `-sW` - TCP Window scan
- `-sU` - UDP scan (requires root)

### Target Specification

- `<target>` - IP address, hostname, CIDR, or range
- `-iL <file>` - Import targets from file
- `--exclude <list>` - Exclude targets (comma-separated)

### Port Specification

- `-p <ports>` - Specific ports (comma-separated)
- `-p <range>` - Port range
- `-p-` - All 65535 ports
- `-F` - Fast mode (top 100 ports)
- `--top-ports <N>` - Scan top N ports

### Timing

- `-T0` - Paranoid
- `-T1` - Sneaky
- `-T2` - Polite
- `-T3` - Normal (default)
- `-T4` - Aggressive
- `-T5` - Insane

### Output

- `-oN <file>` - Save to text file
- `-oJ <file>` - Save to JSON file
- `-oX <file>` - Save to XML file

### Other

- `-v` - Verbose output
- `--sudo` - Force sudo usage
- `-h, --help` - Show help

## Target File Format

Create a text file with one target per line:

```
192.168.1.1
192.168.2.0/24
scanme.nmap.org
10.0.0.1-50
```

#!/bin/bash

# Setup script to configure passwordless sudo for nmap
# This allows the NmapCli application to run nmap scans without password prompts

echo "Setting up passwordless sudo for nmap..."
echo ""
echo "This will create a sudoers rule that allows your user to run nmap without a password."
echo "This is safer than running the entire application as root."
echo ""

# Create sudoers rule
SUDOERS_FILE="/etc/sudoers.d/nmap-cli"
USERNAME=$(whoami)

echo "Creating sudoers rule at: $SUDOERS_FILE"
echo ""

# Create the sudoers entry
sudo tee "$SUDOERS_FILE" > /dev/null << EOF
# Allow $USERNAME to run nmap without password
$USERNAME ALL=(ALL) NOPASSWD: /usr/bin/nmap
EOF

# Set correct permissions
sudo chmod 0440 "$SUDOERS_FILE"

# Validate the sudoers file
if sudo visudo -c -f "$SUDOERS_FILE" > /dev/null 2>&1; then
    echo "✅ Sudoers configuration created successfully!"
    echo ""
    echo "You can now run nmap scans without password prompts:"
    echo "  dotnet run -- -sS 192.168.1.1"
    echo ""
    echo "To remove this configuration later, run:"
    echo "  sudo rm $SUDOERS_FILE"
else
    echo "❌ Error: Invalid sudoers configuration"
    sudo rm "$SUDOERS_FILE"
    exit 1
fi

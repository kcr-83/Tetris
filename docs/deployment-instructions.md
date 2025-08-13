# Tetris Application - Deployment Instructions

This document provides comprehensive deployment instructions for the Tetris application, including system requirements, installation process, and configuration options.

## Table of Contents

1. [System Requirements](#system-requirements)
2. [Installation Methods](#installation-methods)
3. [Configuration](#configuration)
4. [Deployment Options](#deployment-options)
5. [Troubleshooting](#troubleshooting)
6. [Post-Deployment Verification](#post-deployment-verification)

## System Requirements

### Minimum Requirements

- **Operating System**: Windows 10/11, macOS 10.15+, or Linux (Ubuntu 18.04+ / equivalent)
- **Runtime**: .NET 9.0 Runtime or SDK
- **Memory**: 100 MB RAM minimum
- **Storage**: 50 MB available disk space
- **Display**: Console terminal supporting UTF-8 encoding
- **Keyboard**: Standard keyboard for game controls

### Recommended Requirements

- **Operating System**: Windows 11, macOS 12+, or Linux (Ubuntu 20.04+)
- **Runtime**: .NET 9.0 SDK (for development and advanced features)
- **Memory**: 256 MB RAM or higher
- **Storage**: 200 MB available disk space (for saves and settings)
- **Display**: Modern terminal with Unicode support and color capabilities
- **Keyboard**: Gaming keyboard for enhanced responsiveness

### Platform-Specific Notes

#### Windows
- Windows Terminal or PowerShell Core recommended for best experience
- Command Prompt supported but with limited visual features
- Console encoding must support UTF-8

#### macOS
- Terminal.app or iTerm2 recommended
- Homebrew can be used for .NET installation

#### Linux
- Modern terminal emulator (GNOME Terminal, Konsole, etc.)
- Package manager support for .NET installation

## Installation Methods

### Method 1: Pre-built Release (Recommended for End Users)

#### Step 1: Download Release Package
```bash
# Download the latest release from GitHub
curl -L -o tetris-release.zip https://github.com/kcr-83/Tetris/releases/latest/download/tetris-win-x64.zip

# Or for Linux
curl -L -o tetris-release.tar.gz https://github.com/kcr-83/Tetris/releases/latest/download/tetris-linux-x64.tar.gz

# Or for macOS
curl -L -o tetris-release.tar.gz https://github.com/kcr-83/Tetris/releases/latest/download/tetris-osx-x64.tar.gz
```

#### Step 2: Extract and Install
```bash
# Windows (PowerShell)
Expand-Archive -Path tetris-release.zip -DestinationPath "C:\Program Files\Tetris"

# Linux/macOS
sudo mkdir -p /opt/tetris
sudo tar -xzf tetris-release.tar.gz -C /opt/tetris
sudo chmod +x /opt/tetris/Tetris.Console.Responsive
```

#### Step 3: Create Desktop Shortcut (Optional)
```bash
# Windows - Create shortcut on desktop
$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$Home\Desktop\Tetris.lnk")
$Shortcut.TargetPath = "C:\Program Files\Tetris\Tetris.Console.Responsive.exe"
$Shortcut.Save()

# Linux - Create desktop entry
cat > ~/.local/share/applications/tetris.desktop << EOF
[Desktop Entry]
Version=1.0
Type=Application
Name=Tetris Game
Comment=Classic Tetris Game
Exec=/opt/tetris/Tetris.Console.Responsive
Icon=/opt/tetris/icon.png
Terminal=true
Categories=Game;
EOF
```

### Method 2: Build from Source (Recommended for Developers)

#### Step 1: Install Prerequisites
```bash
# Install .NET 9.0 SDK
# Windows (using winget)
winget install Microsoft.DotNet.SDK.9

# macOS (using Homebrew)
brew install --cask dotnet

# Linux (Ubuntu/Debian)
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-9.0
```

#### Step 2: Clone Repository
```bash
git clone https://github.com/kcr-83/Tetris.git
cd Tetris
```

#### Step 3: Build Application
```bash
# Navigate to source directory
cd src

# Restore dependencies
dotnet restore

# Build the application
dotnet build --configuration Release

# Publish self-contained executable (optional)
dotnet publish Tetris.Console.Responsive/Tetris.Console.Responsive.csproj \
  --configuration Release \
  --output ../publish \
  --self-contained true \
  --runtime win-x64  # or linux-x64, osx-x64
```

#### Step 4: Run Tests (Optional)
```bash
# Run unit tests
dotnet test Tetris.Core/Tetris.Core.csproj

# Run all tests
dotnet test
```

### Method 3: Docker Deployment

#### Step 1: Create Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Tetris.Console.Responsive/Tetris.Console.Responsive.csproj", "Tetris.Console.Responsive/"]
COPY ["src/Tetris.Core/Tetris.Core.csproj", "Tetris.Core/"]
RUN dotnet restore "Tetris.Console.Responsive/Tetris.Console.Responsive.csproj"
COPY src/ .
WORKDIR "/src/Tetris.Console.Responsive"
RUN dotnet build "Tetris.Console.Responsive.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tetris.Console.Responsive.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tetris.Console.Responsive.dll"]
```

#### Step 2: Build and Run Docker Container
```bash
# Build Docker image
docker build -t tetris-game .

# Run container (interactive mode for console game)
docker run -it --rm tetris-game
```

## Configuration

### User Settings Configuration

The application creates configuration files in the following locations:

#### Windows
```
%USERPROFILE%\AppData\Local\Tetris\
├── Settings\
│   └── settings.json
└── Saves\
    └── *.tetris
```

#### macOS
```
~/Library/Application Support/Tetris/
├── Settings/
│   └── settings.json
└── Saves/
    └── *.tetris
```

#### Linux
```
~/.local/share/Tetris/
├── Settings/
│   └── settings.json
└── Saves/
    └── *.tetris
```

### Settings File Structure

```json
{
  "controls": {
    "moveLeft": "A",
    "moveRight": "D",
    "rotateClockwise": "W",
    "rotateCounterClockwise": "Q",
    "softDrop": "S",
    "hardDrop": "Spacebar",
    "pause": "P",
    "quit": "Escape"
  },
  "audio": {
    "soundEffectsEnabled": true,
    "musicEnabled": true,
    "masterVolume": 0.8,
    "soundEffectsVolume": 1.0,
    "musicVolume": 0.6
  },
  "visual": {
    "theme": "Classic",
    "showGhost": true,
    "showGrid": true,
    "animationsEnabled": true,
    "particleEffectsEnabled": true
  },
  "gameplay": {
    "autoSaveEnabled": true,
    "defaultDifficulty": "Medium",
    "showNextPieces": 3,
    "enableHold": true
  }
}
```

### Environment Variables

You can override default settings using environment variables:

```bash
# Set default difficulty
export TETRIS_DEFAULT_DIFFICULTY=Hard

# Set save directory
export TETRIS_SAVE_PATH="/custom/save/path"

# Set settings directory
export TETRIS_SETTINGS_PATH="/custom/settings/path"

# Enable debug mode
export TETRIS_DEBUG_MODE=true

# Set log level
export TETRIS_LOG_LEVEL=Information
```

### Command Line Arguments

```bash
# Start with specific difficulty
./Tetris.Console.Responsive --difficulty Hard

# Start in specific game mode
./Tetris.Console.Responsive --mode Challenge

# Load specific save file
./Tetris.Console.Responsive --load "savefile.tetris"

# Enable debug mode
./Tetris.Console.Responsive --debug

# Show help
./Tetris.Console.Responsive --help
```

## Deployment Options

### Option 1: Standalone Deployment

For environments without .NET runtime installed:

```bash
dotnet publish \
  --configuration Release \
  --self-contained true \
  --runtime win-x64 \
  --output ./deploy/standalone
```

**Pros:**
- No .NET runtime dependency
- Single folder deployment
- Predictable behavior

**Cons:**
- Larger file size (~70-100 MB)
- Platform-specific builds required

### Option 2: Framework-Dependent Deployment

For environments with .NET runtime:

```bash
dotnet publish \
  --configuration Release \
  --self-contained false \
  --output ./deploy/framework-dependent
```

**Pros:**
- Smaller file size (~10-20 MB)
- Cross-platform compatibility

**Cons:**
- Requires .NET 9.0 runtime on target machine

### Option 3: Container Deployment

Using Docker for consistent deployment:

```yaml
# docker-compose.yml
version: '3.8'
services:
  tetris:
    build: .
    stdin_open: true
    tty: true
    volumes:
      - ./saves:/app/Saves
      - ./settings:/app/Settings
```

**Pros:**
- Consistent environment
- Easy scaling and management
- Isolated dependencies

**Cons:**
- Requires Docker runtime
- Interactive console games require special configuration

### Option 4: CI/CD Automated Deployment

#### GitHub Actions Example

```yaml
name: Deploy Tetris

on:
  release:
    types: [published]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    
    - name: Publish
      run: |
        dotnet publish src/Tetris.Console.Responsive/Tetris.Console.Responsive.csproj \
          -c Release \
          -o ./publish \
          --self-contained true \
          --runtime linux-x64
    
    - name: Upload to server
      run: |
        scp -r ./publish/* user@server:/opt/tetris/
        ssh user@server "sudo systemctl restart tetris"
```

## Troubleshooting

### Common Issues

#### Issue 1: "dotnet command not found"
**Solution:**
```bash
# Verify .NET installation
dotnet --version

# If not installed, download from https://dotnet.microsoft.com/download
# Or use package manager installation commands above
```

#### Issue 2: "Application failed to start"
**Solution:**
```bash
# Check runtime dependencies
dotnet --list-runtimes

# Verify target framework compatibility
dotnet --info

# Check file permissions (Linux/macOS)
chmod +x Tetris.Console.Responsive
```

#### Issue 3: "UTF-8 encoding issues"
**Solution:**
```bash
# Windows PowerShell
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# Linux/macOS
export LANG=en_US.UTF-8
export LC_ALL=en_US.UTF-8
```

#### Issue 4: "Save files not persisting"
**Solution:**
```bash
# Check write permissions for save directory
# Windows
icacls "%USERPROFILE%\AppData\Local\Tetris" /grant Users:F

# Linux/macOS
chmod 755 ~/.local/share/Tetris
```

#### Issue 5: "Console colors not displaying"
**Solution:**
```bash
# Enable color support in terminal
# Windows - Use Windows Terminal or PowerShell Core
# Linux/macOS - Verify TERM environment variable
echo $TERM  # Should show terminal type like 'xterm-256color'
```

### Performance Issues

#### Low Frame Rate
- Reduce terminal size
- Disable animations in settings
- Close other applications
- Use dedicated terminal application

#### High Memory Usage
- Enable auto-save to reduce memory accumulation
- Restart application periodically for long sessions
- Check for memory leaks in custom builds

### Logging and Diagnostics

#### Enable Debug Logging
```bash
# Set environment variable
export TETRIS_LOG_LEVEL=Debug

# Or use command line argument
./Tetris.Console.Responsive --log-level Debug
```

#### Log File Locations
- **Windows**: `%TEMP%\Tetris\logs\`
- **macOS**: `~/Library/Logs/Tetris/`
- **Linux**: `/tmp/Tetris/logs/` or `~/.local/share/Tetris/logs/`

## Post-Deployment Verification

### Verification Checklist

1. **Application Startup**
   ```bash
   # Test application starts without errors
   ./Tetris.Console.Responsive --help
   ```

2. **Game Functionality**
   - Start new game
   - Test all controls (movement, rotation, drop)
   - Verify scoring system
   - Test pause/resume functionality
   - Verify game over detection

3. **Save/Load System**
   - Create a save file
   - Exit and restart application
   - Load the save file
   - Verify game state restoration

4. **Settings System**
   - Modify control settings
   - Change audio/visual settings
   - Restart application
   - Verify settings persistence

5. **Performance Test**
   - Play for extended period (30+ minutes)
   - Monitor memory usage
   - Check for lag or responsiveness issues

### Health Check Script

```bash
#!/bin/bash
# tetris-health-check.sh

echo "Tetris Application Health Check"
echo "==============================="

# Check .NET runtime
if command -v dotnet &> /dev/null; then
    echo "✓ .NET Runtime: $(dotnet --version)"
else
    echo "✗ .NET Runtime: Not found"
    exit 1
fi

# Check application executable
if [ -f "./Tetris.Console.Responsive" ]; then
    echo "✓ Application executable found"
else
    echo "✗ Application executable not found"
    exit 1
fi

# Test application startup
if ./Tetris.Console.Responsive --version &> /dev/null; then
    echo "✓ Application starts successfully"
else
    echo "✗ Application failed to start"
    exit 1
fi

# Check save directory
SAVE_DIR="$HOME/.local/share/Tetris/Saves"
if [ -d "$SAVE_DIR" ] || mkdir -p "$SAVE_DIR" 2>/dev/null; then
    echo "✓ Save directory accessible: $SAVE_DIR"
else
    echo "✗ Save directory not accessible"
    exit 1
fi

# Check settings directory
SETTINGS_DIR="$HOME/.local/share/Tetris/Settings"
if [ -d "$SETTINGS_DIR" ] || mkdir -p "$SETTINGS_DIR" 2>/dev/null; then
    echo "✓ Settings directory accessible: $SETTINGS_DIR"
else
    echo "✗ Settings directory not accessible"
    exit 1
fi

echo ""
echo "✓ All health checks passed!"
echo "Tetris application is ready for use."
```

### Monitoring and Maintenance

#### Regular Maintenance Tasks

1. **Update Application** (Monthly)
   ```bash
   # Check for updates
   git pull origin main
   dotnet build --configuration Release
   ```

2. **Clean Save Files** (As needed)
   ```bash
   # Remove old save files (older than 30 days)
   find ~/.local/share/Tetris/Saves -name "*.tetris" -mtime +30 -delete
   ```

3. **Check Log Files** (Weekly)
   ```bash
   # Review error logs
   grep -i "error\|exception" ~/.local/share/Tetris/logs/*.log
   ```

4. **Performance Monitoring** (Ongoing)
   ```bash
   # Monitor memory usage during gameplay
   ps aux | grep Tetris
   ```

---

## Support and Resources

- **GitHub Repository**: https://github.com/kcr-83/Tetris
- **Issue Tracker**: https://github.com/kcr-83/Tetris/issues
- **Documentation**: See `/docs` directory
- **Community Discord**: [Link if available]

For deployment assistance or technical support, please create an issue in the GitHub repository with the following information:
- Operating system and version
- .NET runtime version
- Deployment method used
- Full error messages or logs
- Steps to reproduce the issue

---

*Last updated: August 13, 2025*
*Version: 1.0.0*

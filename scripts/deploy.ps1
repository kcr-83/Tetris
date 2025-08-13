# Tetris Application - Quick Deployment Script (PowerShell)
# This script automates the deployment process for the Tetris application

param(
    [Parameter(HelpMessage="Deployment target (standalone|framework|docker)")]
    [ValidateSet("standalone", "framework", "docker")]
    [string]$Target = "standalone",
    
    [Parameter(HelpMessage="Target runtime (win-x64|linux-x64|osx-x64)")]
    [ValidateSet("win-x64", "linux-x64", "osx-x64")]
    [string]$Runtime = "win-x64",
    
    [Parameter(HelpMessage="Build configuration (Debug|Release)")]
    [ValidateSet("Debug", "Release")]
    [string]$Config = "Release",
    
    [Parameter(HelpMessage="Output directory")]
    [string]$Output = "",
    
    [Parameter(HelpMessage="Show help")]
    [switch]$Help
)

# Configuration
$ScriptDir = $PSScriptRoot
$ProjectDir = Split-Path $ScriptDir -Parent
$BuildDir = Join-Path $ProjectDir "build"
$DeployDir = Join-Path $ProjectDir "deploy"

if ([string]::IsNullOrEmpty($Output)) {
    $Output = $DeployDir
}

# Functions
function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "[SUCCESS] $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

function Show-Help {
    @"
Tetris Application Deployment Script (PowerShell)

Usage: .\deploy.ps1 [OPTIONS]

PARAMETERS:
    -Target TARGET       Deployment target (standalone|framework|docker)
    -Runtime RUNTIME     Target runtime (win-x64|linux-x64|osx-x64)
    -Config CONFIG       Build configuration (Debug|Release)
    -Output OUTPUT       Output directory
    -Help                Show this help message

EXAMPLES:
    .\deploy.ps1 -Target standalone -Runtime win-x64
    .\deploy.ps1 -Target framework -Config Release
    .\deploy.ps1 -Target docker

TARGETS:
    standalone    Self-contained deployment (includes .NET runtime)
    framework     Framework-dependent deployment (requires .NET runtime)
    docker        Create Docker image and container

RUNTIMES:
    win-x64       Windows 64-bit
    linux-x64     Linux 64-bit
    osx-x64       macOS 64-bit

"@
}

function Test-Prerequisites {
    Write-Info "Checking prerequisites..."
    
    # Check if dotnet is installed
    try {
        $dotnetVersion = & dotnet --version 2>$null
        Write-Success ".NET SDK found: $dotnetVersion"
    }
    catch {
        Write-Error ".NET SDK not found. Please install .NET 9.0 SDK."
        exit 1
    }
    
    # Check if git is available (optional)
    try {
        $gitVersion = & git --version 2>$null
        Write-Success "Git found: $gitVersion"
    }
    catch {
        Write-Warning "Git not found. Version information may not be available."
    }
    
    # Check if Docker is available for Docker builds
    if ($Target -eq "docker") {
        try {
            $dockerVersion = & docker --version 2>$null
            Write-Success "Docker found: $dockerVersion"
        }
        catch {
            Write-Error "Docker not found. Please install Docker for Docker deployment."
            exit 1
        }
    }
}

function Clear-BuildDirectories {
    Write-Info "Cleaning build directories..."
    
    if (Test-Path $BuildDir) {
        Remove-Item $BuildDir -Recurse -Force
        Write-Success "Cleaned build directory"
    }
    
    if (Test-Path $DeployDir) {
        Remove-Item $DeployDir -Recurse -Force
        Write-Success "Cleaned deploy directory"
    }
    
    New-Item -ItemType Directory -Path $BuildDir -Force | Out-Null
    New-Item -ItemType Directory -Path $Output -Force | Out-Null
}

function Restore-Dependencies {
    Write-Info "Restoring NuGet packages..."
    
    Push-Location (Join-Path $ProjectDir "src")
    try {
        & dotnet restore
        if ($LASTEXITCODE -ne 0) {
            throw "Failed to restore dependencies"
        }
        Write-Success "Dependencies restored successfully"
    }
    finally {
        Pop-Location
    }
}

function Invoke-Tests {
    Write-Info "Running unit tests..."
    
    Push-Location (Join-Path $ProjectDir "src")
    try {
        $testResults = Join-Path $BuildDir "TestResults"
        New-Item -ItemType Directory -Path $testResults -Force | Out-Null
        
        & dotnet test --collect:"XPlat Code Coverage" --logger trx --results-directory $testResults
        
        if ($LASTEXITCODE -eq 0) {
            Write-Success "All tests passed"
        } else {
            Write-Warning "Some tests failed or no tests found"
        }
    }
    finally {
        Pop-Location
    }
}

function Build-Standalone {
    param([string]$Runtime, [string]$Config, [string]$OutputPath)
    
    Write-Info "Building standalone deployment for $Runtime..."
    
    Push-Location (Join-Path $ProjectDir "src")
    try {
        $publishPath = Join-Path $OutputPath "standalone-$Runtime"
        
        & dotnet publish "Tetris.Console.Responsive/Tetris.Console.Responsive.csproj" `
            --configuration $Config `
            --runtime $Runtime `
            --self-contained true `
            --output $publishPath `
            -p:PublishSingleFile=true `
            -p:PublishTrimmed=true
        
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed"
        }
        
        Write-Success "Standalone build completed: $publishPath"
        return $publishPath
    }
    finally {
        Pop-Location
    }
}

function Build-FrameworkDependent {
    param([string]$Config, [string]$OutputPath)
    
    Write-Info "Building framework-dependent deployment..."
    
    Push-Location (Join-Path $ProjectDir "src")
    try {
        $publishPath = Join-Path $OutputPath "framework-dependent"
        
        & dotnet publish "Tetris.Console.Responsive/Tetris.Console.Responsive.csproj" `
            --configuration $Config `
            --self-contained false `
            --output $publishPath
        
        if ($LASTEXITCODE -ne 0) {
            throw "Build failed"
        }
        
        Write-Success "Framework-dependent build completed: $publishPath"
        return $publishPath
    }
    finally {
        Pop-Location
    }
}

function Build-Docker {
    Write-Info "Building Docker image..."
    
    Push-Location $ProjectDir
    try {
        $dockerfilePath = Join-Path $ProjectDir "Dockerfile"
        
        # Create Dockerfile if it doesn't exist
        if (-not (Test-Path $dockerfilePath)) {
            $dockerfileContent = @"
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
"@
            Set-Content -Path $dockerfilePath -Value $dockerfileContent
            Write-Info "Created Dockerfile"
        }
        
        # Build Docker image
        & docker build -t tetris-game:latest .
        if ($LASTEXITCODE -ne 0) {
            throw "Docker build failed"
        }
        
        Write-Success "Docker image built: tetris-game:latest"
        
        # Create docker-compose.yml if it doesn't exist
        $dockerComposePath = Join-Path $ProjectDir "docker-compose.yml"
        if (-not (Test-Path $dockerComposePath)) {
            $dockerComposeContent = @"
version: '3.8'
services:
  tetris:
    image: tetris-game:latest
    container_name: tetris-game
    stdin_open: true
    tty: true
    volumes:
      - ./Saves:/app/Saves
      - ./Settings:/app/Settings
    environment:
      - TETRIS_LOG_LEVEL=Information
"@
            Set-Content -Path $dockerComposePath -Value $dockerComposeContent
            Write-Info "Created docker-compose.yml"
        }
    }
    finally {
        Pop-Location
    }
}

function New-Package {
    param([string]$Target, [string]$Runtime, [string]$OutputPath)
    
    Write-Info "Creating deployment package..."
    
    Push-Location $OutputPath
    try {
        switch ($Target) {
            "standalone" {
                $packageName = "tetris-standalone-$Runtime.zip"
                $sourcePath = "standalone-$Runtime"
                
                if (Test-Path $sourcePath) {
                    Compress-Archive -Path "$sourcePath\*" -DestinationPath $packageName -Force
                    Write-Success "Created package: $packageName"
                }
            }
            "framework" {
                $packageName = "tetris-framework-dependent.zip"
                $sourcePath = "framework-dependent"
                
                if (Test-Path $sourcePath) {
                    Compress-Archive -Path "$sourcePath\*" -DestinationPath $packageName -Force
                    Write-Success "Created package: $packageName"
                }
            }
        }
    }
    finally {
        Pop-Location
    }
}

function New-InstallScript {
    param([string]$Target, [string]$Runtime, [string]$OutputPath)
    
    Write-Info "Creating installation script..."
    
    switch ($Target) {
        "standalone" {
            if ($Runtime -eq "win-x64") {
                $installScriptContent = @"
@echo off
echo Installing Tetris Game...

set INSTALL_DIR=%PROGRAMFILES%\Tetris
set SHORTCUT_PATH=%USERPROFILE%\Desktop\Tetris.lnk

echo Creating installation directory...
mkdir "%INSTALL_DIR%" 2>nul

echo Copying files...
xcopy /E /I /Y standalone-win-x64\* "%INSTALL_DIR%\"

echo Creating desktop shortcut...
powershell -Command "`$WshShell = New-Object -comObject WScript.Shell; `$Shortcut = `$WshShell.CreateShortcut('%SHORTCUT_PATH%'); `$Shortcut.TargetPath = '%INSTALL_DIR%\Tetris.Console.Responsive.exe'; `$Shortcut.Save()"

echo Installation completed!
echo You can now run Tetris from: %INSTALL_DIR%\Tetris.Console.Responsive.exe
echo Or use the desktop shortcut.
pause
"@
                Set-Content -Path (Join-Path $OutputPath "install.bat") -Value $installScriptContent
            }
        }
    }
    
    Write-Success "Installation script created"
}

function New-DeploymentReadme {
    param([string]$Target, [string]$Runtime, [string]$OutputPath)
    
    Write-Info "Generating deployment README..."
    
    $buildDate = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $version = try { & git describe --tags --always 2>$null } catch { "unknown" }
    
    $readmeContent = @"
# Tetris Game - Deployment Package

This package contains the Tetris game application built for deployment.

## Package Information

- **Target**: $Target
- **Runtime**: $Runtime
- **Build Date**: $buildDate
- **Version**: $version

## Installation

"@
    
    switch ($Target) {
        "standalone" {
            $readmeContent += @"

### Standalone Installation

This package includes the .NET runtime and doesn't require any additional dependencies.

#### Windows
1. Run ``install.bat`` as Administrator
2. The game will be installed to ``C:\Program Files\Tetris``
3. A desktop shortcut will be created

#### Manual Installation
Extract the contents of this package to your desired location and run the executable.

"@
        }
        "framework" {
            $readmeContent += @"

### Framework-Dependent Installation

This package requires .NET 9.0 Runtime to be installed on the target system.

#### Prerequisites
- .NET 9.0 Runtime or SDK

#### Installation
1. Install .NET 9.0 Runtime from https://dotnet.microsoft.com/download
2. Extract this package to your desired location
3. Run: ``dotnet Tetris.Console.Responsive.dll``

"@
        }
        "docker" {
            $readmeContent += @"

### Docker Installation

#### Prerequisites
- Docker Engine

#### Running the Game
```bash
# Pull and run the image
docker run -it --rm tetris-game:latest

# Or use docker-compose
docker-compose up
```

"@
        }
    }
    
    $readmeContent += @"

## System Requirements

- **Operating System**: Windows 10+, macOS 10.15+, or Linux
- **Memory**: 100 MB RAM minimum
- **Storage**: 50 MB available space
- **Display**: Terminal supporting UTF-8 encoding

## Game Controls

- **A/D or Arrow Keys**: Move left/right
- **W or Up**: Rotate clockwise
- **Q**: Rotate counterclockwise
- **S or Down**: Soft drop
- **Space**: Hard drop
- **P**: Pause/Resume
- **Escape**: Quit to menu

## Support

For issues or questions, please visit:
https://github.com/kcr-83/Tetris/issues

"@
    
    Set-Content -Path (Join-Path $OutputPath "README.md") -Value $readmeContent
    Write-Success "Deployment README generated"
}

# Main execution
if ($Help) {
    Show-Help
    exit 0
}

# Start deployment process
Write-Info "Starting Tetris deployment process..."
Write-Info "Target: $Target"
Write-Info "Runtime: $Runtime"
Write-Info "Configuration: $Config"
Write-Info "Output: $Output"

try {
    Test-Prerequisites
    Clear-BuildDirectories
    Restore-Dependencies
    Invoke-Tests
    
    switch ($Target) {
        "standalone" {
            Build-Standalone $Runtime $Config $Output
            New-Package $Target $Runtime $Output
            New-InstallScript $Target $Runtime $Output
        }
        "framework" {
            Build-FrameworkDependent $Config $Output
            New-Package $Target $Runtime $Output
            New-InstallScript $Target $Runtime $Output
        }
        "docker" {
            Build-Docker
        }
    }
    
    New-DeploymentReadme $Target $Runtime $Output
    
    Write-Success "Deployment completed successfully!"
    Write-Info "Deployment artifacts available in: $Output"
}
catch {
    Write-Error "Deployment failed: $_"
    exit 1
}

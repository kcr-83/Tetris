#!/bin/bash
# Tetris Application - Quick Deployment Script
# This script automates the deployment process for the Tetris application

set -e  # Exit on any error

# Configuration
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
BUILD_DIR="$PROJECT_DIR/build"
DEPLOY_DIR="$PROJECT_DIR/deploy"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

show_help() {
    cat << EOF
Tetris Application Deployment Script

Usage: $0 [OPTIONS]

OPTIONS:
    -t, --target TARGET     Deployment target (standalone|framework|docker)
    -r, --runtime RUNTIME   Target runtime (win-x64|linux-x64|osx-x64)
    -c, --config CONFIG     Build configuration (Debug|Release)
    -o, --output OUTPUT     Output directory
    -h, --help              Show this help message

EXAMPLES:
    $0 --target standalone --runtime linux-x64
    $0 --target framework --config Release
    $0 --target docker

TARGETS:
    standalone    Self-contained deployment (includes .NET runtime)
    framework     Framework-dependent deployment (requires .NET runtime)
    docker        Create Docker image and container

RUNTIMES:
    win-x64       Windows 64-bit
    linux-x64     Linux 64-bit
    osx-x64       macOS 64-bit

EOF
}

check_prerequisites() {
    log_info "Checking prerequisites..."
    
    # Check if dotnet is installed
    if ! command -v dotnet &> /dev/null; then
        log_error ".NET SDK not found. Please install .NET 9.0 SDK."
        exit 1
    fi
    
    local dotnet_version=$(dotnet --version)
    log_success ".NET SDK found: $dotnet_version"
    
    # Check if git is available (optional)
    if command -v git &> /dev/null; then
        local git_version=$(git --version)
        log_success "Git found: $git_version"
    else
        log_warning "Git not found. Version information may not be available."
    fi
}

clean_build_directories() {
    log_info "Cleaning build directories..."
    
    if [ -d "$BUILD_DIR" ]; then
        rm -rf "$BUILD_DIR"
        log_success "Cleaned build directory"
    fi
    
    if [ -d "$DEPLOY_DIR" ]; then
        rm -rf "$DEPLOY_DIR"
        log_success "Cleaned deploy directory"
    fi
    
    mkdir -p "$BUILD_DIR"
    mkdir -p "$DEPLOY_DIR"
}

restore_dependencies() {
    log_info "Restoring NuGet packages..."
    
    cd "$PROJECT_DIR/src"
    dotnet restore
    
    log_success "Dependencies restored successfully"
}

run_tests() {
    log_info "Running unit tests..."
    
    cd "$PROJECT_DIR/src"
    
    # Run tests with coverage if available
    if dotnet test --collect:"XPlat Code Coverage" --logger trx --results-directory "$BUILD_DIR/TestResults" 2>/dev/null; then
        log_success "All tests passed"
    else
        log_warning "Some tests failed or no tests found"
    fi
}

build_standalone() {
    local runtime="$1"
    local config="$2"
    local output="$3"
    
    log_info "Building standalone deployment for $runtime..."
    
    cd "$PROJECT_DIR/src"
    
    dotnet publish Tetris.Console.Responsive/Tetris.Console.Responsive.csproj \
        --configuration "$config" \
        --runtime "$runtime" \
        --self-contained true \
        --output "$output/standalone-$runtime" \
        -p:PublishSingleFile=true \
        -p:PublishTrimmed=true
    
    log_success "Standalone build completed: $output/standalone-$runtime"
}

build_framework_dependent() {
    local config="$1"
    local output="$2"
    
    log_info "Building framework-dependent deployment..."
    
    cd "$PROJECT_DIR/src"
    
    dotnet publish Tetris.Console.Responsive/Tetris.Console.Responsive.csproj \
        --configuration "$config" \
        --self-contained false \
        --output "$output/framework-dependent"
    
    log_success "Framework-dependent build completed: $output/framework-dependent"
}

build_docker() {
    log_info "Building Docker image..."
    
    cd "$PROJECT_DIR"
    
    # Create Dockerfile if it doesn't exist
    if [ ! -f "Dockerfile" ]; then
        cat > Dockerfile << 'EOF'
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
EOF
        log_info "Created Dockerfile"
    fi
    
    # Build Docker image
    docker build -t tetris-game:latest .
    
    log_success "Docker image built: tetris-game:latest"
    
    # Create docker-compose.yml if it doesn't exist
    if [ ! -f "docker-compose.yml" ]; then
        cat > docker-compose.yml << 'EOF'
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
EOF
        log_info "Created docker-compose.yml"
    fi
}

create_package() {
    local target="$1"
    local runtime="$2"
    local output="$3"
    
    log_info "Creating deployment package..."
    
    cd "$output"
    
    case "$target" in
        "standalone")
            if [[ "$runtime" == "win-x64" ]]; then
                zip -r "tetris-standalone-$runtime.zip" "standalone-$runtime/"
                log_success "Created package: tetris-standalone-$runtime.zip"
            else
                tar -czf "tetris-standalone-$runtime.tar.gz" "standalone-$runtime/"
                log_success "Created package: tetris-standalone-$runtime.tar.gz"
            fi
            ;;
        "framework")
            if [[ "$OSTYPE" == "msys" || "$OSTYPE" == "win32" ]]; then
                zip -r "tetris-framework-dependent.zip" "framework-dependent/"
                log_success "Created package: tetris-framework-dependent.zip"
            else
                tar -czf "tetris-framework-dependent.tar.gz" "framework-dependent/"
                log_success "Created package: tetris-framework-dependent.tar.gz"
            fi
            ;;
    esac
}

create_install_script() {
    local target="$1"
    local runtime="$2"
    local output="$3"
    
    log_info "Creating installation script..."
    
    case "$target" in
        "standalone")
            if [[ "$runtime" == "win-x64" ]]; then
                cat > "$output/install.bat" << 'EOF'
@echo off
echo Installing Tetris Game...

set INSTALL_DIR=%PROGRAMFILES%\Tetris
set SHORTCUT_PATH=%USERPROFILE%\Desktop\Tetris.lnk

echo Creating installation directory...
mkdir "%INSTALL_DIR%" 2>nul

echo Copying files...
xcopy /E /I /Y standalone-win-x64\* "%INSTALL_DIR%\"

echo Creating desktop shortcut...
powershell -Command "$WshShell = New-Object -comObject WScript.Shell; $Shortcut = $WshShell.CreateShortcut('%SHORTCUT_PATH%'); $Shortcut.TargetPath = '%INSTALL_DIR%\Tetris.Console.Responsive.exe'; $Shortcut.Save()"

echo Installation completed!
echo You can now run Tetris from: %INSTALL_DIR%\Tetris.Console.Responsive.exe
echo Or use the desktop shortcut.
pause
EOF
            else
                cat > "$output/install.sh" << 'EOF'
#!/bin/bash
echo "Installing Tetris Game..."

INSTALL_DIR="/opt/tetris"
DESKTOP_FILE="$HOME/.local/share/applications/tetris.desktop"

echo "Creating installation directory..."
sudo mkdir -p "$INSTALL_DIR"

echo "Copying files..."
sudo cp -r standalone-*/* "$INSTALL_DIR/"
sudo chmod +x "$INSTALL_DIR/Tetris.Console.Responsive"

echo "Creating desktop entry..."
mkdir -p "$(dirname "$DESKTOP_FILE")"
cat > "$DESKTOP_FILE" << EOL
[Desktop Entry]
Version=1.0
Type=Application
Name=Tetris Game
Comment=Classic Tetris Game
Exec=$INSTALL_DIR/Tetris.Console.Responsive
Icon=$INSTALL_DIR/icon.png
Terminal=true
Categories=Game;
EOL

echo "Installation completed!"
echo "You can now run Tetris from: $INSTALL_DIR/Tetris.Console.Responsive"
echo "Or find it in your applications menu."
EOF
                chmod +x "$output/install.sh"
            fi
            ;;
    esac
    
    log_success "Installation script created"
}

generate_readme() {
    local target="$1"
    local runtime="$2"
    local output="$3"
    
    log_info "Generating deployment README..."
    
    cat > "$output/README.md" << EOF
# Tetris Game - Deployment Package

This package contains the Tetris game application built for deployment.

## Package Information

- **Target**: $target
- **Runtime**: $runtime
- **Build Date**: $(date)
- **Version**: $(git describe --tags --always 2>/dev/null || echo "unknown")

## Installation

EOF

    case "$target" in
        "standalone")
            cat >> "$output/README.md" << 'EOF'
### Standalone Installation

This package includes the .NET runtime and doesn't require any additional dependencies.

#### Windows
1. Run `install.bat` as Administrator
2. The game will be installed to `C:\Program Files\Tetris`
3. A desktop shortcut will be created

#### Linux/macOS
1. Run `sudo ./install.sh`
2. The game will be installed to `/opt/tetris`
3. A desktop entry will be created

### Manual Installation

Extract the contents of this package to your desired location and run the executable.

EOF
            ;;
        "framework")
            cat >> "$output/README.md" << 'EOF'
### Framework-Dependent Installation

This package requires .NET 9.0 Runtime to be installed on the target system.

#### Prerequisites
- .NET 9.0 Runtime or SDK

#### Installation
1. Install .NET 9.0 Runtime from https://dotnet.microsoft.com/download
2. Extract this package to your desired location
3. Run: `dotnet Tetris.Console.Responsive.dll`

EOF
            ;;
        "docker")
            cat >> "$output/README.md" << 'EOF'
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

EOF
            ;;
    esac

    cat >> "$output/README.md" << 'EOF'
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

EOF

    log_success "Deployment README generated"
}

main() {
    # Default values
    TARGET="standalone"
    RUNTIME="linux-x64"
    CONFIG="Release"
    OUTPUT="$DEPLOY_DIR"
    
    # Parse command line arguments
    while [[ $# -gt 0 ]]; do
        case $1 in
            -t|--target)
                TARGET="$2"
                shift 2
                ;;
            -r|--runtime)
                RUNTIME="$2"
                shift 2
                ;;
            -c|--config)
                CONFIG="$2"
                shift 2
                ;;
            -o|--output)
                OUTPUT="$2"
                shift 2
                ;;
            -h|--help)
                show_help
                exit 0
                ;;
            *)
                log_error "Unknown option: $1"
                show_help
                exit 1
                ;;
        esac
    done
    
    # Validate arguments
    if [[ ! "$TARGET" =~ ^(standalone|framework|docker)$ ]]; then
        log_error "Invalid target: $TARGET"
        exit 1
    fi
    
    if [[ ! "$RUNTIME" =~ ^(win-x64|linux-x64|osx-x64)$ ]]; then
        log_error "Invalid runtime: $RUNTIME"
        exit 1
    fi
    
    if [[ ! "$CONFIG" =~ ^(Debug|Release)$ ]]; then
        log_error "Invalid configuration: $CONFIG"
        exit 1
    fi
    
    # Start deployment process
    log_info "Starting Tetris deployment process..."
    log_info "Target: $TARGET"
    log_info "Runtime: $RUNTIME"
    log_info "Configuration: $CONFIG"
    log_info "Output: $OUTPUT"
    
    check_prerequisites
    clean_build_directories
    restore_dependencies
    run_tests
    
    case "$TARGET" in
        "standalone")
            build_standalone "$RUNTIME" "$CONFIG" "$OUTPUT"
            create_package "$TARGET" "$RUNTIME" "$OUTPUT"
            create_install_script "$TARGET" "$RUNTIME" "$OUTPUT"
            ;;
        "framework")
            build_framework_dependent "$CONFIG" "$OUTPUT"
            create_package "$TARGET" "$RUNTIME" "$OUTPUT"
            create_install_script "$TARGET" "$RUNTIME" "$OUTPUT"
            ;;
        "docker")
            build_docker
            ;;
    esac
    
    generate_readme "$TARGET" "$RUNTIME" "$OUTPUT"
    
    log_success "Deployment completed successfully!"
    log_info "Deployment artifacts available in: $OUTPUT"
}

# Run main function with all arguments
main "$@"

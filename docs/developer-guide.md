# Tetris Game - Developer Documentation

Comprehensive technical documentation for developers working on the Tetris game application.

## Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Project Structure](#project-structure)
4. [Core Components](#core-components)
5. [Design Patterns](#design-patterns)
6. [Development Guidelines](#development-guidelines)
7. [Extension Points](#extension-points)
8. [Testing Strategy](#testing-strategy)
9. [Performance Considerations](#performance-considerations)
10. [Deployment Architecture](#deployment-architecture)
11. [Contributing Guidelines](#contributing-guidelines)

## Project Overview

### Technology Stack

- **Framework**: .NET 9.0
- **Language**: C# 12
- **UI Framework**: Console Application (Terminal-based)
- **Testing**: xUnit
- **Build System**: MSBuild
- **Deployment**: Self-contained executables, Docker

### Key Features

- **Complete Tetris Implementation**: All 7 standard pieces with proper rotation mechanics
- **Multiple Game Modes**: Classic, Timed, and Challenge modes
- **Configurable Difficulty**: Easy, Medium, Hard with different timing and scoring
- **Responsive Interface**: Adaptive layout for different terminal sizes
- **Save/Load System**: Persistent game state management
- **Statistics Tracking**: Comprehensive gameplay analytics
- **Settings Management**: Customizable controls, themes, and preferences

## Architecture

### High-Level Architecture

The Tetris application follows a **layered architecture** with clear separation of concerns:

```text
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                       │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │   Console   │  │  Gameplay   │  │  Settings & Menu    │  │
│  │ Application │  │ Interface   │  │   Components        │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│                    Application Layer                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │  Game       │  │  Settings   │  │   Statistics &      │  │
│  │  Engine     │  │ Management  │  │   Save/Load         │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│                      Domain Layer                           │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │   Game      │  │ Tetromino   │  │   Board & Game      │  │
│  │   Models    │  │   Models    │  │   State Models      │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────┬───────────────────────────────────┘
                          │
┌─────────────────────────▼───────────────────────────────────┐
│                 Infrastructure Layer                        │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐  │
│  │ File System │  │    JSON     │  │   Configuration     │  │
│  │ Persistence │  │ Serializer  │  │     Manager         │  │
│  └─────────────┘  └─────────────┘  └─────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### Design Principles

1. **Single Responsibility Principle**: Each class has one reason to change
2. **Dependency Inversion**: High-level modules don't depend on low-level modules
3. **Interface Segregation**: Interfaces are focused and specific
4. **Composition over Inheritance**: Favor composition for extensibility
5. **Command Pattern**: Game actions are encapsulated as commands
6. **Observer Pattern**: UI components observe game state changes

## Project Structure

### Solution Layout

```text
Tetris/
├── src/
│   ├── Tetris.sln                              # Solution file
│   ├── Tetris.Core/                            # Core game library
│   │   ├── Models/                             # Domain models
│   │   │   ├── Board.cs                        # Game board representation
│   │   │   ├── GameEngine.cs                   # Main game engine
│   │   │   ├── Tetromino.cs                    # Abstract base tetromino
│   │   │   ├── ITetromino.cs                   # Tetromino interface
│   │   │   ├── [I,J,L,O,S,T,Z]Tetromino.cs     # Specific tetromino pieces
│   │   │   ├── TetrominoFactory.cs             # Factory for creating pieces
│   │   │   ├── TetrominoController.cs          # Piece movement logic
│   │   │   ├── GameState.cs                    # Game state representation
│   │   │   ├── GameMode.cs                     # Game mode enumerations
│   │   │   ├── DifficultyLevel.cs              # Difficulty configurations
│   │   │   ├── UserSettings.cs                 # User preference models
│   │   │   └── GameStatistics.cs               # Statistics models
│   │   ├── Services/                           # Application services
│   │   │   ├── IUserSettingsService.cs         # Settings service interface
│   │   │   ├── UserSettingsService.cs          # Settings implementation
│   │   │   ├── IGameStatisticsService.cs       # Statistics interface
│   │   │   ├── GameStatisticsService.cs        # Statistics implementation
│   │   │   ├── GameSaveService.cs              # Save/load functionality
│   │   │   └── SettingsApplicator.cs           # Settings application logic
│   │   ├── UI/                                 # User interface components
│   │   │   ├── TetrisGame.cs                   # Main game controller
│   │   │   ├── TetrisGameExtensions.cs         # Extension methods
│   │   │   ├── MainMenuInterface.cs            # Main menu UI
│   │   │   ├── GameplayInterface.cs            # Core gameplay UI
│   │   │   ├── GameplayInterface.Improved.cs   # Enhanced responsive UI
│   │   │   ├── GameplayInterface.Complete.cs   # Full-featured UI
│   │   │   ├── GameOverDisplay.cs              # Game over screen
│   │   │   ├── SettingsInterface.cs            # Settings management UI
│   │   │   ├── StatisticsInterface.cs          # Statistics display
│   │   │   └── [Various]Dialog.cs              # Dialog components
│   │   └── Tests/                              # Unit and integration tests
│   │       ├── BoardTests.cs                   # Board logic tests
│   │       ├── GameEngineTests.cs              # Game engine tests
│   │       ├── TetrominoTests.cs               # Tetromino tests
│   │       ├── UserSettingsServiceTests.cs     # Settings tests
│   │       └── [Various]Demo.cs                # Demo/testing classes
│   └── Tetris.Console.Responsive/              # Console application
│       ├── Program.cs                          # Application entry point
│       └── Tetris.Console.Responsive.csproj    # Project file
├── docs/                                       # Documentation
├── scripts/                                    # Build and deployment scripts
└── README.md                                   # Project overview
```

## Core Components

### 1. GameEngine Class

**Purpose**: Central orchestrator of game logic and state management.

**Key Responsibilities**:

- Game loop management and timing
- Tetromino spawning and control
- Collision detection and line clearing
- Score calculation and level progression
- Game mode implementation (Classic, Timed, Challenge)

**Key Methods**:

```csharp
public class GameEngine : IDisposable
{
    // Game lifecycle
    public void StartNewGame(GameMode mode, DifficultyLevel difficulty)
    public void PauseGame()
    public void ResumeGame()
    public void EndGame()
    
    // Game mechanics
    public bool MovePiece(Direction direction)
    public bool RotatePiece(RotationDirection direction)
    public void DropPiece(bool hardDrop = false)
    
    // State management
    public GameState GetCurrentState()
    public void LoadGameState(GameState state)
    
    // Events
    public event EventHandler<GameOverEventArgs> GameOver;
    public event EventHandler<ScoreChangedEventArgs> ScoreChanged;
    public event EventHandler<LevelChangedEventArgs> LevelChanged;
}
```

### 2. Board Class

**Purpose**: Represents the 10×20 Tetris playing field.

**Key Features**:

- Grid state management (empty/occupied cells)
- Collision detection algorithms
- Line clearing mechanics
- Board state serialization

**Key Methods**:

```csharp
public class Board
{
    public const int Width = 10;
    public const int Height = 20;
    
    // Core operations
    public bool CanPlacePiece(ITetromino piece, Point position)
    public void PlacePiece(ITetromino piece, Point position)
    public List<int> GetFullRows()
    public int ClearRows(List<int> rowsToClear)
    
    // State management
    public Board Clone()
    public void CopyFrom(Board other)
}
```

### 3. Tetromino Hierarchy

**Purpose**: Represents the seven standard Tetris pieces with rotation logic.

**Design Pattern**: Template Method Pattern with abstract base class.

```csharp
public abstract class Tetromino : ITetromino
{
    // Template method defining common behavior
    public virtual bool Rotate(RotationDirection direction)
    
    // Abstract methods implemented by concrete classes
    public abstract int Id { get; }
    public abstract Color Color { get; }
    public abstract Point[] Blocks { get; }
    
    // Concrete implementations: ITetromino, JTetromino, LTetromino, 
    // OTetromino, STetromino, TTetromino, ZTetromino
}
```

### 4. Services Layer

#### UserSettingsService

**Purpose**: Manages user preferences and configuration persistence.

**Features**:

- JSON-based configuration storage
- Settings validation and defaults
- Event-driven settings updates
- Key mapping management

#### GameStatisticsService

**Purpose**: Tracks and analyzes gameplay metrics.

**Features**:

- Session and lifetime statistics
- High score tracking
- Performance analytics
- Statistics persistence

#### GameSaveService

**Purpose**: Handles game state serialization and persistence.

**Features**:

- Multiple save slots
- Game state validation
- Corruption detection and recovery
- Quick save/auto-save functionality

### 5. UI Components

#### Responsive Design System

The UI system features adaptive layouts that work across different terminal sizes:

```csharp
public partial class GameplayInterface
{
    // Core rendering
    public void Render()
    public void Initialize()
    
    // Responsive features
    private void CalculateLayout()
    private void HandleResize()
    private bool CheckForResize()
    
    // Rendering modes
    private void RenderNormalMode()
    private void RenderCompactMode()
    private void RenderMinimalMode()
}
```

## Design Patterns

### 1. Factory Pattern

**Usage**: Tetromino creation
**Implementation**: `TetrominoFactory.cs`

```csharp
public static class TetrominoFactory
{
    public static ITetromino CreateRandomTetromino()
    public static ITetromino CreateTetromino(TetrominoType type)
    public static ITetromino CreateTetromino(int id)
}
```

### 2. Observer Pattern

**Usage**: Game state change notifications
**Implementation**: Event-driven architecture

```csharp
// Publisher
public class GameEngine
{
    public event EventHandler<GameOverEventArgs> GameOver;
    public event EventHandler<ScoreChangedEventArgs> ScoreChanged;
}

// Subscriber
public class GameplayInterface
{
    public GameplayInterface(GameEngine engine)
    {
        engine.ScoreChanged += OnScoreChanged;
        engine.GameOver += OnGameOver;
    }
}
```

### 3. Command Pattern

**Usage**: User input handling and undo functionality
**Implementation**: Encapsulated game actions

```csharp
public interface IGameCommand
{
    bool Execute(GameEngine engine);
    bool CanExecute(GameEngine engine);
}

public class MovePieceCommand : IGameCommand
{
    private readonly Direction _direction;
    
    public bool Execute(GameEngine engine)
    {
        return engine.MovePiece(_direction);
    }
}
```

### 4. Strategy Pattern

**Usage**: Different game modes and difficulty levels
**Implementation**: Configurable game behavior

```csharp
public interface IGameModeStrategy
{
    bool IsGameWon(GameEngine engine);
    bool IsGameOver(GameEngine engine);
    int CalculateScore(int linesCleared, int level);
}

public class ClassicModeStrategy : IGameModeStrategy
public class TimedModeStrategy : IGameModeStrategy
public class ChallengeModeStrategy : IGameModeStrategy
```

### 5. Repository Pattern

**Usage**: Data persistence abstraction
**Implementation**: Service layer abstraction

```csharp
public interface IGameRepository
{
    Task<GameState> LoadGameAsync(string saveId);
    Task SaveGameAsync(string saveId, GameState state);
    Task<List<string>> GetSaveListAsync();
}
```

## Development Guidelines

### Code Style

1. **Naming Conventions**:
   - Use PascalCase for public members
   - Use camelCase for private fields and parameters
   - Prefix interfaces with 'I'
   - Use descriptive names for everything

2. **Documentation**:
   - All public classes and members must have XML documentation
   - Include `<summary>`, `<param>`, `<returns>`, and `<exception>` tags
   - Provide usage examples for complex APIs

3. **Error Handling**:
   - Use exceptions for exceptional cases only
   - Validate all public method parameters
   - Log errors appropriately
   - Provide meaningful error messages

### File Organization

1. **File-Scoped Namespaces**: Use C# 10+ file-scoped namespace syntax
2. **One Class Per File**: Except for tightly coupled classes
3. **Logical Grouping**: Organize related classes in appropriate folders
4. **Consistent Naming**: File names match class names exactly

### Dependencies

1. **Minimize External Dependencies**: Avoid unnecessary NuGet packages
2. **Interface Segregation**: Define focused interfaces
3. **Dependency Injection**: Use constructor injection for dependencies
4. **Configuration**: Use appsettings.json and environment variables

## Extension Points

### 1. Adding New Tetromino Pieces

To add new tetromino pieces:

**Create New Tetromino Class**:

```csharp
public class XTetromino : Tetromino
{
    public override int Id => 8; // Unique ID
    public override Color Color => Color.Purple;
    public override string Name => "X";
    
    public override Point[] Blocks => GetBlocksForRotation(RotationState);
    
    private Point[] GetBlocksForRotation(int rotation)
    {
        // Define block positions for each rotation
        return rotation switch
        {
            0 => new[] { new Point(0, 0), new Point(1, 0), /* ... */ },
            1 => new[] { /* rotated positions */ },
            // ... other rotations
        };
    }
}
```

**Update TetrominoFactory**:

```csharp
public static ITetromino CreateTetromino(int id)
{
    return id switch
    {
        // ... existing pieces
        8 => new XTetromino(),
        _ => throw new ArgumentException($"Unknown tetromino ID: {id}")
    };
}
```

**Update Random Generation**:

```csharp
private static readonly int[] PieceIds = { 1, 2, 3, 4, 5, 6, 7, 8 };
```

### 2. Adding New Game Modes

To implement new game modes:

**Add to GameMode Enum**:

```csharp
public enum GameMode
{
    Classic = 0,
    Timed = 1,
    Challenge = 2,
    Survival = 3  // New mode
}
```

**Implement Game Mode Logic**:

```csharp
public static class SurvivalMode
{
    public static bool IsGameWon(GameEngine engine)
    {
        // Define victory condition
        return engine.SurvivalTime >= TimeSpan.FromMinutes(10);
    }
    
    public static bool ShouldAddObstacles(GameEngine engine)
    {
        // Mode-specific mechanics
        return engine.Level % 5 == 0;
    }
}
```

**Update GameEngine**:

```csharp
private void HandleGameModeSpecificLogic()
{
    switch (CurrentGameMode)
    {
        case GameMode.Survival:
            if (SurvivalMode.ShouldAddObstacles(this))
                AddRandomObstacles();
            break;
    }
}
```

### 3. Adding New UI Themes

To add new visual themes:

**Define Theme Class**:

```csharp
public class NeonTheme : IGameTheme
{
    public ConsoleColor BoardBorder => ConsoleColor.Cyan;
    public ConsoleColor EmptyCell => ConsoleColor.Black;
    public Dictionary<int, ConsoleColor> PieceColors => new()
    {
        { 1, ConsoleColor.Magenta },  // I-piece
        { 2, ConsoleColor.Yellow },   // J-piece
        // ... other pieces
    };
    
    public string GetBlockCharacter(int pieceId) => "██";
}
```

**Register in Settings**:

```csharp
public enum ColorTheme
{
    Classic,
    Dark,
    HighContrast,
    Neon  // New theme
}
```

### 4. Adding New Input Methods

To support new input methods:

**Create Input Handler**:

```csharp
public class GamepadInputHandler : IInputHandler
{
    public bool IsActionPressed(GameAction action)
    {
        // Implement gamepad input detection
        return CheckGamepadInput(action);
    }
    
    public ConsoleKeyInfo ReadKey()
    {
        // Convert gamepad input to key equivalent
        return ConvertGamepadToKey();
    }
}
```

**Update Settings Service**:

```csharp
public class UserSettings
{
    public InputMethod PreferredInputMethod { get; set; }
    public GamepadSettings GamepadSettings { get; set; }
}
```

### 5. Adding New Statistics

To track additional statistics:

**Extend GameStatistics Model**:

```csharp
public class GameStatistics
{
    // Existing properties...
    
    // New statistics
    public int PerfectClears { get; set; }
    public TimeSpan AverageGameDuration { get; set; }
    public Dictionary<TetrominoType, int> PieceUsageCount { get; set; }
}
```

**Update Statistics Service**:

```csharp
public void RecordPerfectClear()
{
    _statistics.PerfectClears++;
    OnStatisticsUpdated();
}
```

## Testing Strategy

### Unit Testing

**Framework**: xUnit with FluentAssertions and Moq

**Test Categories**:

- **Model Tests**: Domain logic validation
- **Service Tests**: Business logic verification
- **UI Tests**: Interface behavior testing

**Example Test Structure**:

```csharp
public class BoardTests
{
    [Theory]
    [InlineData(0, 0, true)]   // Valid position
    [InlineData(-1, 0, false)] // Invalid X
    [InlineData(0, 20, false)] // Invalid Y
    public void CanPlacePiece_WithVariousPositions_ReturnsExpectedResult(
        int x, int y, bool expected)
    {
        // Arrange
        var board = new Board();
        var piece = TetrominoFactory.CreateTetromino(1); // I-piece
        var position = new Point(x, y);
        
        // Act
        var result = board.CanPlacePiece(piece, position);
        
        // Assert
        result.Should().Be(expected);
    }
}
```

### Integration Testing

**Focus Areas**:

- **Game Flow**: Complete gameplay scenarios
- **Save/Load**: State persistence verification
- **Settings**: Configuration management testing

### Performance Testing

**Key Metrics**:

- **Rendering Performance**: Frame rate in different terminal sizes
- **Memory Usage**: Memory consumption during extended play
- **Input Latency**: Response time for user inputs

## Performance Considerations

### 1. Rendering Optimization

**Double Buffering**: Minimize screen flicker
```csharp
private void RenderWithDoubleBuffering()
{
    if (_useDoubleBuffering)
    {
        // Prepare frame in memory buffer
        PrepareFrameBuffer();
        
        // Write entire frame at once
        WriteFrameToConsole();
    }
}
```

**Partial Updates**: Only redraw changed areas
```csharp
private void UpdateChangedRegions()
{
    if (_gameBoard.HasChanged)
        RenderBoard();
    
    if (_score.HasChanged)
        RenderScore();
    
    // Only update what's necessary
}
```

### 2. Memory Management

**Object Pooling**: Reuse tetromino instances
```csharp
public class TetrominoPool
{
    private readonly Queue<ITetromino>[] _pools = new Queue<ITetromino>[7];
    
    public ITetromino GetTetromino(int id)
    {
        if (_pools[id].Count > 0)
            return _pools[id].Dequeue();
        
        return TetrominoFactory.CreateTetromino(id);
    }
    
    public void ReturnTetromino(ITetromino piece)
    {
        piece.Reset();
        _pools[piece.Id].Enqueue(piece);
    }
}
```

**Efficient Data Structures**: Use appropriate collections
```csharp
// Use arrays for fixed-size collections
private readonly int?[,] _grid = new int?[Width, Height];

// Use readonly collections for immutable data
public static readonly IReadOnlyList<Point> ITetrominoBlocks = 
    new[] { new Point(0, 0), new Point(0, 1), new Point(0, 2), new Point(0, 3) };
```

### 3. Algorithm Optimization

**Collision Detection**: Optimized bounds checking
```csharp
public bool CanPlacePiece(ITetromino piece, Point position)
{
    foreach (var block in piece.Blocks)
    {
        int x = position.X + block.X;
        int y = position.Y + block.Y;
        
        // Early termination for out-of-bounds
        if (x < 0 || x >= Width || y < 0 || y >= Height)
            return false;
        
        // Check for collision with existing pieces
        if (_grid[x, y].HasValue)
            return false;
    }
    
    return true;
}
```

## Deployment Architecture

### Local Development

**Requirements**:
- .NET 9.0 SDK
- Modern terminal with UTF-8 support
- 100 MB RAM minimum

**Setup**:
```bash
git clone https://github.com/kcr-83/Tetris.git
cd Tetris/src
dotnet restore
dotnet build
dotnet run --project Tetris.Console.Responsive
```

### Production Deployment

**Self-Contained Deployment**:
```bash
dotnet publish --configuration Release \
  --self-contained true \
  --runtime win-x64 \
  --output ./publish
```

**Docker Deployment**:
```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["dotnet", "Tetris.Console.Responsive.dll"]
```

### Configuration Management

**Environment Variables**:
- `TETRIS_SAVE_PATH`: Custom save directory
- `TETRIS_SETTINGS_PATH`: Custom settings directory
- `TETRIS_LOG_LEVEL`: Logging verbosity
- `TETRIS_DEBUG_MODE`: Enable debug features

**Configuration Files**:
- `appsettings.json`: Application configuration
- `settings.json`: User preferences
- `statistics.json`: Gameplay statistics

## Contributing Guidelines

### Development Workflow

1. **Fork Repository**: Create personal fork on GitHub
2. **Create Feature Branch**: `git checkout -b feature/new-feature`
3. **Implement Changes**: Follow coding standards
4. **Write Tests**: Ensure adequate test coverage
5. **Update Documentation**: Keep docs current
6. **Submit Pull Request**: Provide clear description

### Code Review Checklist

- [ ] Code follows established patterns and conventions
- [ ] All public APIs are documented
- [ ] Unit tests cover new functionality
- [ ] No breaking changes to existing APIs
- [ ] Performance impact is considered
- [ ] Security implications are addressed

### Testing Requirements

- **Unit Test Coverage**: Minimum 80% for new code
- **Integration Tests**: For cross-component functionality
- **Manual Testing**: Verify UI/UX changes
- **Performance Testing**: For optimization changes

### Documentation Updates

- Update relevant documentation files
- Add code examples for new features
- Update API reference documentation
- Review and update user-facing documentation

---

## API Reference

### Core Classes Quick Reference

#### GameEngine
```csharp
// Game control
StartNewGame(GameMode, DifficultyLevel)
PauseGame() / ResumeGame()
MovePiece(Direction) : bool
RotatePiece(RotationDirection) : bool
DropPiece(bool hardDrop)

// State access
Board : Board (readonly)
CurrentPiece : ITetromino (readonly)
Score : int (readonly)
Level : int (readonly)
IsGameOver : bool (readonly)
```

#### Board
```csharp
// Constants
Width : int = 10
Height : int = 20

// Operations
CanPlacePiece(ITetromino, Point) : bool
PlacePiece(ITetromino, Point)
GetFullRows() : List<int>
ClearRows(List<int>) : int
Clone() : Board
```

#### ITetromino
```csharp
// Properties
Id : int (readonly)
Color : Color (readonly)
Name : string (readonly)
Position : Point
RotationState : int
Blocks : Point[] (readonly)

// Operations
Rotate(RotationDirection) : bool
Clone() : ITetromino
```

### Services Quick Reference

#### IUserSettingsService
```csharp
LoadSettingsAsync() : Task<UserSettings>
SaveSettingsAsync(UserSettings) : Task
UpdateSettingAsync(Action<UserSettings>) : Task
ResetToDefaultsAsync() : Task
GetKeyForAction(GameAction) : ConsoleKey
```

#### IGameStatisticsService
```csharp
GetStatisticsAsync() : Task<GameStatistics>
RecordGameEnd(GameResult) : Task
UpdateStatistic(string, object) : Task
ResetStatisticsAsync() : Task
```

---

**Version**: 1.0.0  
**Last Updated**: August 13, 2025  
**Maintainer**: Development Team

For questions or clarifications, please create an issue in the GitHub repository.

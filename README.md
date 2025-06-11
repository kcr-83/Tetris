# Tetris Game Application

A modern implementation of the classic Tetris game built with .NET Core 9.0 and C#.

![Tetris Game](https://placehold.co/800x400/23C7D9/FFFFFF?text=Tetris+Game&font=montserrat)

## Table of Contents

- [Tetris Game Application](#tetris-game-application)
  - [Table of Contents](#table-of-contents)
  - [Project Overview](#project-overview)
    - [Key Features](#key-features)
  - [Technologies](#technologies)
    - [Backend](#backend)
    - [Frontend](#frontend)
    - [Database](#database)
    - [Testing](#testing)
    - [DevOps](#devops)
  - [Architecture](#architecture)
  - [Project Structure](#project-structure)
  - [Game Mechanics](#game-mechanics)
    - [Board](#board)
    - [Tetrominos](#tetrominos)
    - [Game Flow](#game-flow)
  - [Class Diagrams](#class-diagrams)
    - [Core Game Components](#core-game-components)
  - [Data Model](#data-model)
  - [Setup and Installation](#setup-and-installation)
    - [Prerequisites](#prerequisites)
    - [Development Setup](#development-setup)
    - [Docker Setup](#docker-setup)
  - [Game Features](#game-features)
    - [Game Modes](#game-modes)
    - [Difficulty Levels](#difficulty-levels)
    - [Scoring System](#scoring-system)
  - [Development Roadmap](#development-roadmap)
  - [Contributing](#contributing)
  - [License](#license)

## Project Overview

This project is a complete implementation of the classic Tetris game with modern architecture and additional features. The game includes standard Tetris mechanics along with user account management, statistics tracking, different game modes, and customizable settings.

### Key Features

- Standard 10x20 Tetris board gameplay
- All 7 classic Tetromino blocks with proper rotation logic
- Intuitive block controls for movement, rotation, and dropping
- Block drop preview visualization (ghost piece)
- Comprehensive gameplay interface with:
  - Current game board display
  - Score, level, and cleared rows tracking
  - Next piece preview
  - Difficulty level indicator
  - Line clear statistics
- Multiple game modes (Classic, Timed, Challenge)
- Variable difficulty levels
- Special animations for level increases and line clears
- Pause functionality with overlay
- Game state saving and loading
- User statistics and high scores
- Responsive UI design for various screen sizes
- Cross-browser compatibility

## Technologies

### Backend
- **.NET Core 6.0**: Main framework
- **C#**: Primary programming language
- **ASP.NET Core**: Web framework
- **Entity Framework Core**: ORM for database operations
- **SignalR**: Real-time communication (optional for multiplayer features)

### Frontend
- **Blazor WebAssembly**: Client-side framework
- **HTML5 / CSS3**: Markup and styling
- **JavaScript**: Additional interactivity

### Database
- **SQL Server**: Production database
- **SQLite**: Development/testing database

### Testing
- **xUnit**: Unit testing
- **Selenium**: UI testing

### DevOps
- **Docker**: Containerization
- **Azure**: Cloud hosting and services
- **CI/CD**: GitHub Actions / Azure DevOps

## Architecture

The application follows a layered architecture pattern with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────────┐
│                      Client (Web Browser)                       │
└───────────────────────────────┬─────────────────────────────────┘
                                │
┌───────────────────────────────▼─────────────────────────────────┐
│                       Presentation Layer                        │
│                                                                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │  Blazor WASM    │  │  HTML/CSS       │  │  JavaScript     │  │
│  │  Components     │  │  Templates      │  │  Interactivity  │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└───────────────────────────────┬─────────────────────────────────┘
                                │
┌───────────────────────────────▼─────────────────────────────────┐
│                          API Layer                              │
│                                                                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │  REST API       │  │  SignalR Hubs   │  │  Controllers    │  │
│  │  Endpoints      │  │  Real-time      │  │  Request        │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└───────────────────────────────┬─────────────────────────────────┘
                                │
┌───────────────────────────────▼─────────────────────────────────┐
│                      Business Logic Layer                       │
│                                                                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │  Game Engine    │  │  Game State     │  │  User Service   │  │
│  │  Services       │  │  Management     │  │  & Settings     │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└───────────────────────────────┬─────────────────────────────────┘
                                │
┌───────────────────────────────▼─────────────────────────────────┐
│                        Data Access Layer                        │
│                                                                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │  Repositories   │  │  Entity         │  │  Data Context   │  │
│  │  & Interfaces   │  │  Framework Core │  │  & Models       │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└───────────────────────────────┬─────────────────────────────────┘
                                │
┌───────────────────────────────▼─────────────────────────────────┐
│                        Database Layer                           │
│                                                                 │
│  ┌─────────────────────────────────────────────────────────────┐│
│  │           SQL Server / SQLite (for development)             ││
│  └─────────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────────┘
```

For more detailed architecture information, see [tetris-architecture.md](copilot/tetris-architecture.md).

## Project Structure

```
Tetris/
├── src/
│   ├── Tetris.Core/              # Core game logic and models
│   │   ├── Models/               # Game models (Board, Tetrominos)
│   │   ├── Services/             # Game services (Score, State)
│   │   └── Tests/                # Unit tests for core components
│   │
│   ├── Tetris.API/               # API endpoints and controllers
│   │   ├── Controllers/          # REST API controllers
│   │   ├── Hubs/                 # SignalR hubs
│   │   └── Middleware/           # Custom middleware
│   │
│   ├── Tetris.Data/              # Data access layer
│   │   ├── Repositories/         # Data repositories
│   │   ├── Context/              # EF Core context
│   │   └── Models/               # Database entity models
│   │
│   └── Tetris.Web/               # Blazor WebAssembly client
│       ├── Pages/                # Blazor pages
│       ├── Components/           # Reusable UI components
│       └── wwwroot/              # Static assets
│
├── docs/                         # Documentation
│   ├── diagrams/                 # Architecture diagrams
│   └── api/                      # API documentation
│
└── tests/                        # Integration and UI tests
    ├── Tetris.IntegrationTests/  # Integration tests
    └── Tetris.UITests/           # Selenium UI tests
```

## Game Mechanics

### Board

The game board is represented by a 10x20 grid where each cell can be empty or contain a block.

```csharp
// Board representation
public class Board
{
    public const int Width = 10;
    public const int Height = 20;
    public int?[,] Grid { get; private set; } // null = empty, otherwise contains block type
    // ...
}
```

### Tetrominos

The game features the 7 standard Tetris blocks (I, J, L, O, S, T, Z) with their unique shapes, colors, and rotation logic.

```
I-Block (Cyan):
    ■ ■ ■ ■

J-Block (Blue):
    ■
    ■ ■ ■

L-Block (Orange):
        ■
    ■ ■ ■

O-Block (Yellow):
    ■ ■
    ■ ■

S-Block (Green):
      ■ ■
    ■ ■

T-Block (Purple):
      ■
    ■ ■ ■

Z-Block (Red):
    ■ ■
      ■ ■
```

### Game Controls

The TetrominoController provides an intuitive interface for controlling Tetris blocks:

- **Move Left/Right**: Move the current Tetromino horizontally
- **Rotate Clockwise/Counter-clockwise**: Change the orientation of the Tetromino
- **Soft Drop**: Accelerate the falling speed temporarily
- **Hard Drop**: Immediately place the Tetromino at the lowest possible position

The controller also provides a preview of where the piece would land if hard-dropped, helping players plan their moves.

For more details on the controller, see [tetromino-controller.md](docs/tetromino-controller.md).

### Game Flow

1. A Tetromino spawns at the top of the board
2. Player controls the Tetromino (move left/right, rotate, soft/hard drop)
3. Tetromino falls at a constant rate determined by current level
4. When a Tetromino can't fall further, it locks in place
5. Full rows are checked and cleared, awarding points
6. If the board is filled to the top, the game ends

## Class Diagrams

### Core Game Components

```ascii
┌────────────────┐     ┌────────────────┐     ┌────────────────┐
│   GameEngine   │     │     Board      │     │   Tetromino    │
├────────────────┤     ├────────────────┤     ├────────────────┤
│ - board        │1    │ - grid         │     │ - position     │
│ - currentPiece ├─────┤ - rowsCleared  │     │ - rotationState│
│ - nextPiece    │     │                │     │ - blocks       │
│ - score        │     │ + AddBlock()   │1    │ + Move()       │
│ - level        │     │ + RemoveRow()  ├─────┤ + Rotate()     │
│ - gameOver     │     │ + IsRowFull()  │0..* │ + GetPositions │
└───────┬────────┘     └────────────────┘     └─────────────┬──┘
        │                                                   │
        │                                                   │
        │        ┌────────────────────┐                     │
        │        │ TetrominoController│                     │
        │        ├────────────────────┤                     │
        └────────┤ - gameEngine       │                     │
                 ├────────────────────┤                     │
                 │ + MoveLeft()       │                     │
                 │ + MoveRight()      │                     │
                 │ + RotateClockwise()│                     │
                 │ + HardDrop()       │─────────────────────┘
                 │ + GetBoardPreview()│
                 └────────────────────┘
                                                            │
                      ┌────────────┐         ┌─────────┐    │
                      │            │         │         │    │
                      │ITetromino  │         │ZTetromino    │
                      └────────────┘         └─────────┘    │
                            │                     │         │
                            │                     │         │
                  ┌─────────┴─────────────────────┴─────────┘
                  │
      ┌───────────┼───────────┬───────────┬───────────┬───────────┐
      │           │           │           │           │           │
┌─────┴───┐ ┌─────┴───┐ ┌─────┴───┐ ┌─────┴───┐ ┌─────┴───┐ ┌─────┴───┐
│JTetromino│ │LTetromino│ │OTetromino│ │STetromino│ │TTetromino│ │ZTetromino│
└─────────┘ └─────────┘ └─────────┘ └─────────┘ └─────────┘ └─────────┘
```

## Data Model

The database schema includes the following main entities:

```
┌───────────────────┐       ┌───────────────────┐       ┌───────────────────┐
│      Users        │       │   UserSettings    │       │   GameStates      │
├───────────────────┤       ├───────────────────┤       ├───────────────────┤
│ UserId (PK)       │━━━━━━━┫ UserId (FK)       │       │ GameStateId (PK)  │
│ Username          │       │ SettingsId (PK)   │       │ UserId (FK)       │
│ Email             │       │ ControlSettings   │       │ BoardState        │
│ PasswordHash      │       │ SoundEnabled      │       │ CurrentScore      │
│ RegistrationDate  │       │ MusicEnabled      │       │ CurrentLevel      │
└───────────────────┘       └───────────────────┘       └───────────────────┘
        │                                                       ┃
        │                                                       ┃
┌───────────────────┐                               ┌───────────────────────┐
│ GameStatistics    │                               │    GameHistories      │
├───────────────────┤                               ├───────────────────────┤
│ StatisticsId (PK) │                               │ GameHistoryId (PK)    │
│ UserId (FK)       │                               │ UserId (FK)           │
│ HighestScore      │                               │ GameDate              │
│ TotalGamesPlayed  │                               │ Duration              │
│ TotalTimePlayed   │                               │ Score                 │
└───────────────────┘                               └───────────────────────┘
```

For more detailed data model information, see [tetris-data-model.md](copilot/tetris-data-model.md).

## Setup and Installation

### Prerequisites

- .NET Core 6.0 SDK or later
- SQL Server/SQLite
- Node.js (optional for frontend development)

### Development Setup

1. Clone the repository:
   ```
   git clone https://github.com/yourusername/tetris.git
   cd tetris
   ```

2. Restore dependencies:
   ```
   dotnet restore
   ```

3. Update the database:
   ```
   cd src/Tetris.Data
   dotnet ef database update
   ```

4. Run the application:
   ```
   cd ../Tetris.Web
   dotnet run
   ```

5. Access the application at `https://localhost:5001`

### Docker Setup

```
docker-compose up -d
```

## Game Features

### Game Modes

- **Classic Mode**: Play until the board fills up
- **Timed Mode**: Score as many points as possible within a time limit
- **Challenge Mode**: Clear a specific number of rows as quickly as possible

### Difficulty Levels

- **Easy**: Slow falling speed, forgiving scoring
- **Medium**: Standard falling speed and scoring
- **Hard**: Fast falling speed, challenging scoring

### Scoring System

- **Single Row**: 100 points × current level
- **Double Row**: 300 points × current level
- **Triple Row**: 500 points × current level
- **Tetris (4 rows)**: 800 points × current level

## Development Roadmap

- [x] Core game mechanics
- [x] Basic UI implementation
- [ ] User account system
- [ ] Multiplayer functionality
- [ ] Mobile optimization
- [ ] Game themes and customization
- [ ] Tournament mode

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/amazing-feature`
3. Commit your changes: `git commit -m 'Add amazing feature'`
4. Push to the branch: `git push origin feature/amazing-feature`
5. Open a Pull Request

Please see [CONTRIBUTING.md](CONTRIBUTING.md) for more details.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

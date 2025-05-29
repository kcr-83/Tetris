# Tetris Web Application - Project Structure

## Overview
This document outlines the project structure for a Tetris web application built using .NET Core 6.0 and C#. The application follows an MVC architecture with additional services and components to handle game logic.

## Solution Structure

```
Tetris.sln
├── Tetris.Web/                      # Main web application project
│   ├── Program.cs                   # Entry point for the web application
│   ├── Startup.cs                   # Application configuration
│   ├── appsettings.json             # Application settings
│   ├── appsettings.Development.json # Development settings
│   ├── Controllers/                 # MVC Controllers
│   │   ├── HomeController.cs        # Main page controller
│   │   ├── GameController.cs        # Game interactions controller
│   │   └── AccountController.cs     # User account management
│   ├── Models/                      # Data models
│   │   ├── GameState.cs             # Game state model
│   │   ├── UserStatistics.cs        # User statistics model
│   │   └── UserSettings.cs          # User settings model
│   ├── Views/                       # MVC Views
│   │   ├── Home/
│   │   │   └── Index.cshtml         # Main landing page
│   │   ├── Game/
│   │   │   ├── Play.cshtml          # Game screen
│   │   │   └── GameOver.cshtml      # Game over screen
│   │   ├── Account/
│   │   │   ├── Login.cshtml         # Login page
│   │   │   └── Profile.cshtml       # User profile page
│   │   └── Shared/
│   │       └── _Layout.cshtml       # Main layout template
│   ├── wwwroot/                     # Static files
│   │   ├── css/                     # Stylesheets
│   │   │   ├── site.css             # Main site styles
│   │   │   └── game.css             # Game-specific styles
│   │   ├── js/                      # JavaScript files
│   │   │   ├── game/                # Game-specific scripts
│   │   │   │   ├── board.js         # Game board handling
│   │   │   │   ├── tetrominos.js    # Block definitions and behavior
│   │   │   │   ├── gameLogic.js     # Game mechanics implementation
│   │   │   │   ├── scoring.js       # Scoring system
│   │   │   │   └── controls.js      # User input handling
│   │   │   └── site.js              # Main site scripts
│   │   ├── lib/                     # Third-party libraries
│   │   └── sounds/                  # Game sound effects
│   └── Services/                    # Application services
│       ├── GameService.cs           # Game state management
│       ├── UserService.cs           # User data management
│       └── StatisticsService.cs     # Game statistics management
├── Tetris.Core/                     # Core game logic library
│   ├── Models/                      # Domain models
│   │   ├── Board.cs                 # Game board representation
│   │   ├── Tetromino.cs             # Abstract tetromino class
│   │   ├── Tetrominos/              # Specific tetromino implementations
│   │   │   ├── I_Tetromino.cs       # I-shaped tetromino
│   │   │   ├── J_Tetromino.cs       # J-shaped tetromino
│   │   │   ├── L_Tetromino.cs       # L-shaped tetromino
│   │   │   ├── O_Tetromino.cs       # O-shaped tetromino
│   │   │   ├── S_Tetromino.cs       # S-shaped tetromino
│   │   │   ├── T_Tetromino.cs       # T-shaped tetromino
│   │   │   └── Z_Tetromino.cs       # Z-shaped tetromino
│   │   └── GameSettings.cs          # Game configuration settings
│   ├── Services/                    # Core service interfaces
│   │   ├── IGameLogic.cs            # Game logic interface
│   │   ├── IScoreCalculator.cs      # Score calculation interface
│   │   └── IDifficultyManager.cs    # Difficulty management interface
│   └── Implementations/             # Core service implementations
│       ├── GameLogic.cs             # Game logic implementation
│       ├── ScoreCalculator.cs       # Score calculation implementation
│       └── DifficultyManager.cs     # Difficulty management implementation
├── Tetris.Data/                     # Data access layer
│   ├── ApplicationDbContext.cs      # EF Core database context
│   ├── Repositories/                # Data repositories
│   │   ├── GameStateRepository.cs   # Game state persistence
│   │   ├── UserRepository.cs        # User data persistence
│   │   └── StatisticsRepository.cs  # Statistics persistence
│   └── Migrations/                  # Database migrations
└── Tetris.Tests/                    # Unit and integration tests
    ├── Core/                        # Core logic tests
    │   ├── BoardTests.cs            # Tests for board functionality
    │   ├── TetrominoTests.cs        # Tests for tetromino functionality
    │   └── GameLogicTests.cs        # Tests for game logic
    ├── Web/                         # Web application tests
    │   └── Controllers/             # Controller tests
    └── TestHelpers/                 # Test utilities
```

## Key Components Description

### 1. Core Game Logic (Tetris.Core)
This library contains the core game mechanics, independent of the presentation layer:
- **Board**: Represents the game board with its dimensions and state
- **Tetrominos**: Classes for each of the 7 standard tetromino shapes with their rotation logic
- **Game Logic**: Rules for piece movement, collision detection, line clearing, and scoring

### 2. Web Application (Tetris.Web)
The main web application provides the user interface and game interaction:
- **Controllers**: Handle HTTP requests, game state management, and user interactions
- **Views**: Razor pages for game rendering and user interface
- **JavaScript**: Client-side game rendering and user input handling
- **Services**: Handle game state synchronization between server and client

### 3. Data Layer (Tetris.Data)
Handles data persistence for game states, user profiles, and statistics:
- **Repositories**: Data access patterns for saving and loading game states
- **Entity Framework Core**: ORM for database interactions
- **Migrations**: Database schema evolution

### 4. Testing (Tetris.Tests)
Comprehensive test suite to ensure game logic works correctly:
- **Unit Tests**: Verify behavior of individual components
- **Integration Tests**: Test interaction between components

## Implementation Approach

### Phase 1: Basic Game Mechanics
- Implement core game board and tetromino models
- Develop basic game logic for piece movement and collision
- Create simple rendering in the web interface

### Phase 2: UI Development
- Build responsive game interface
- Implement main menu and game screens
- Add game controls and keyboard handling

### Phase 3: Advanced Features
- Implement scoring and difficulty progression
- Add game modes (classic, timed, challenge)
- Develop user profiles and statistics

### Phase 4: Polish and Refinement
- Add sound effects and visual polish
- Implement save/load functionality
- Optimize performance for mobile devices

## Technical Implementation Details

### Server-Side
- ASP.NET Core 6.0 MVC for the web application
- SignalR for real-time updates between server and client (optional for multiplayer features)
- Entity Framework Core for data persistence
- Identity for user authentication (if implementing user accounts)

### Client-Side
- HTML5 Canvas for game rendering
- JavaScript for game loop and user input
- CSS for responsive design and visual styling
- WebAudio API for sound effects

### Deployment
- Docker containerization for easy deployment
- CI/CD pipeline for automated testing and deployment

# Tetris Web Application - Project Structure

## Overview

This document defines the project structure for the Tetris web application, implementing a three-tier architecture (Frontend, Backend, Storage) with SignalR for real-time communication. The structure builds upon the existing `Tetris.Core` library and follows .NET best practices.

## Solution Structure

```
Tetris.sln
├── src/
│   ├── Tetris.Core/                     # ✅ Existing - Core game logic (reused)
│   │   ├── Models/                      # Game entities and logic
│   │   ├── Services/                    # Game services
│   │   ├── UI/                          # Console UI components
│   │   └── Tests/                       # Unit tests
│   │
│   ├── Tetris.Web.Api/                  # 🆕 Backend - ASP.NET Core Web API
│   │   ├── Program.cs                   # Application entry point
│   │   ├── appsettings.json             # Configuration
│   │   ├── appsettings.Development.json # Development settings
│   │   │
│   │   ├── Controllers/                 # REST API Controllers
│   │   │   ├── GameController.cs        # Game management endpoints
│   │   │   ├── UserController.cs        # User management endpoints
│   │   │   ├── StatisticsController.cs  # Statistics endpoints
│   │   │   └── SettingsController.cs    # User settings endpoints
│   │   │
│   │   ├── Hubs/                        # SignalR Hubs
│   │   │   ├── TetrisGameHub.cs         # Main game communication hub
│   │   │   └── ITetrisGameClient.cs     # Client interface definition
│   │   │
│   │   ├── Services/                    # Application services
│   │   │   ├── IGameSessionService.cs   # Game session management interface
│   │   │   ├── GameSessionService.cs    # Game session management
│   │   │   ├── IUserService.cs          # User management interface
│   │   │   ├── UserService.cs           # User management implementation
│   │   │   ├── IStatisticsService.cs    # Statistics service interface
│   │   │   ├── StatisticsService.cs     # Statistics service implementation
│   │   │   └── GameEngineWrapper.cs     # Wrapper for Tetris.Core integration
│   │   │
│   │   ├── Models/                      # API-specific models
│   │   │   ├── Requests/                # Request DTOs
│   │   │   │   ├── CreateGameRequest.cs # Game creation request
│   │   │   │   ├── GameActionRequest.cs # Game action request
│   │   │   │   ├── SaveGameRequest.cs   # Save game request
│   │   │   │   └── UserLoginRequest.cs  # User login request
│   │   │   │
│   │   │   ├── Responses/               # Response DTOs
│   │   │   │   ├── GameStateResponse.cs # Game state response
│   │   │   │   ├── UserProfileResponse.cs # User profile response
│   │   │   │   ├── StatisticsResponse.cs # Statistics response
│   │   │   │   └── ApiResponse.cs       # Generic API response wrapper
│   │   │   │
│   │   │   └── SignalR/                 # SignalR-specific models
│   │   │       ├── GameUpdateMessage.cs # Game state update message
│   │   │       ├── InputMessage.cs      # Player input message
│   │   │       └── GameEventMessage.cs  # Game event notification
│   │   │
│   │   ├── Middleware/                  # Custom middleware
│   │   │   ├── ExceptionHandlingMiddleware.cs # Global exception handling
│   │   │   ├── UserContextMiddleware.cs # User context setup
│   │   │   └── RequestLoggingMiddleware.cs # Request logging
│   │   │
│   │   ├── Extensions/                  # Extension methods
│   │   │   ├── ServiceCollectionExtensions.cs # DI extensions
│   │   │   ├── SignalRExtensions.cs     # SignalR configuration
│   │   │   └── ModelMappingExtensions.cs # Model mapping helpers
│   │   │
│   │   └── Configuration/               # Configuration classes
│   │       ├── CorsSettings.cs          # CORS configuration
│   │       ├── JwtSettings.cs           # JWT configuration
│   │       └── GameSettings.cs          # Game-specific settings
│   │
│   ├── Tetris.Web.Infrastructure/       # 🆕 Infrastructure - Data access layer
│   │   ├── Data/                        # Entity Framework context and configuration
│   │   │   ├── TetrisDbContext.cs       # Main database context
│   │   │   ├── DesignTimeDbContextFactory.cs # Design-time factory for migrations
│   │   │   │
│   │   │   ├── Configurations/          # Entity configurations
│   │   │   │   ├── UserConfiguration.cs # User entity configuration
│   │   │   │   ├── GameSessionConfiguration.cs # Game session configuration
│   │   │   │   ├── UserStatisticsConfiguration.cs # Statistics configuration
│   │   │   │   └── UserSettingsConfiguration.cs # Settings configuration
│   │   │   │
│   │   │   └── Migrations/              # EF Core migrations
│   │   │       └── [Generated migration files]
│   │   │
│   │   ├── Entities/                    # Database entities
│   │   │   ├── User.cs                  # User entity
│   │   │   ├── GameSession.cs           # Game session entity
│   │   │   ├── UserStatistics.cs        # User statistics entity
│   │   │   ├── UserSettings.cs          # User settings entity
│   │   │   └── BaseEntity.cs            # Base entity with common properties
│   │   │
│   │   ├── Repositories/                # Repository pattern implementation
│   │   │   ├── IRepository.cs           # Generic repository interface
│   │   │   ├── Repository.cs            # Generic repository implementation
│   │   │   ├── IUserRepository.cs       # User repository interface
│   │   │   ├── UserRepository.cs        # User repository implementation
│   │   │   ├── IGameSessionRepository.cs # Game session repository interface
│   │   │   ├── GameSessionRepository.cs # Game session repository implementation
│   │   │   ├── IStatisticsRepository.cs # Statistics repository interface
│   │   │   └── StatisticsRepository.cs  # Statistics repository implementation
│   │   │
│   │   ├── Services/                    # Infrastructure services
│   │   │   ├── IUnitOfWork.cs           # Unit of work interface
│   │   │   ├── UnitOfWork.cs            # Unit of work implementation
│   │   │   ├── ICacheService.cs         # Caching service interface
│   │   │   └── CacheService.cs          # Caching service implementation
│   │   │
│   │   └── Extensions/                  # Infrastructure extensions
│   │       └── InfrastructureExtensions.cs # DI registration for infrastructure
│   │
│   ├── Tetris.Web.Client/               # 🆕 Frontend - Client-side application
│   │   ├── wwwroot/                     # Static web assets
│   │   │   ├── index.html               # Main HTML page
│   │   │   ├── favicon.ico              # Site favicon
│   │   │   │
│   │   │   ├── css/                     # Stylesheets
│   │   │   │   ├── main.css             # Main styles
│   │   │   │   ├── game.css             # Game-specific styles
│   │   │   │   ├── components.css       # Component styles
│   │   │   │   └── responsive.css       # Responsive design styles
│   │   │   │
│   │   │   ├── js/                      # JavaScript/TypeScript files
│   │   │   │   ├── main.js               # Main application entry
│   │   │   │   ├── config.js            # Configuration
│   │   │   │   │
│   │   │   │   ├── game/                # Game-specific modules
│   │   │   │   │   ├── gameRenderer.js  # Canvas rendering engine
│   │   │   │   │   ├── inputHandler.js  # Input handling
│   │   │   │   │   ├── gameClient.js    # SignalR client wrapper
│   │   │   │   │   ├── boardRenderer.js # Game board rendering
│   │   │   │   │   ├── tetrominoRenderer.js # Tetromino rendering
│   │   │   │   │   ├── effectsRenderer.js # Visual effects
│   │   │   │   │   └── audioManager.js  # Audio management
│   │   │   │   │
│   │   │   │   ├── ui/                  # UI components
│   │   │   │   │   ├── gameInterface.js # Main game interface
│   │   │   │   │   ├── menuSystem.js    # Menu system
│   │   │   │   │   ├── settingsPanel.js # Settings panel
│   │   │   │   │   ├── statisticsPanel.js # Statistics display
│   │   │   │   │   ├── leaderboard.js   # Leaderboard component
│   │   │   │   │   └── modalSystem.js   # Modal dialogs
│   │   │   │   │
│   │   │   │   ├── services/            # Client-side services
│   │   │   │   │   ├── apiService.js    # REST API client
│   │   │   │   │   ├── signalrService.js # SignalR client service
│   │   │   │   │   ├── storageService.js # Local storage management
│   │   │   │   │   ├── authService.js   # Authentication service
│   │   │   │   │   └── settingsService.js # Settings management
│   │   │   │   │
│   │   │   │   └── utils/               # Utility functions
│   │   │   │       ├── eventEmitter.js  # Event system
│   │   │   │       ├── constants.js     # Constants and enums
│   │   │   │       ├── helpers.js       # Helper functions
│   │   │   │       └── validators.js    # Input validation
│   │   │   │
│   │   │   ├── assets/                  # Static assets
│   │   │   │   ├── sounds/              # Audio files
│   │   │   │   │   ├── effects/         # Sound effects
│   │   │   │   │   │   ├── line-clear.mp3 # Line clear sound
│   │   │   │   │   │   ├── piece-drop.mp3 # Piece drop sound
│   │   │   │   │   │   ├── piece-move.mp3 # Piece move sound
│   │   │   │   │   │   └── game-over.mp3  # Game over sound
│   │   │   │   │   │
│   │   │   │   │   └── music/           # Background music
│   │   │   │   │       ├── theme.mp3    # Main theme
│   │   │   │   │       └── menu.mp3     # Menu music
│   │   │   │   │
│   │   │   │   └── images/              # Image assets
│   │   │   │       ├── logo.png         # Game logo
│   │   │   │       ├── backgrounds/     # Background images
│   │   │   │       └── icons/           # UI icons
│   │   │   │
│   │   │   └── lib/                     # Third-party libraries
│   │   │       ├── signalr.min.js       # SignalR client library
│   │   │       └── [other third-party libraries]
│   │   │
│   │   ├── src/                         # TypeScript source files
│   │   │   ├── types/                   # TypeScript type definitions
│   │   │   │   ├── game.types.ts        # Game-related types
│   │   │   │   ├── api.types.ts         # API types
│   │   │   │   ├── signalr.types.ts     # SignalR types
│   │   │   │   └── ui.types.ts          # UI types
│   │   │   │
│   │   │   ├── models/                  # Client-side models
│   │   │   │   ├── GameState.ts         # Game state model
│   │   │   │   ├── User.ts              # User model
│   │   │   │   ├── Statistics.ts        # Statistics model
│   │   │   │   └── Settings.ts          # Settings model
│   │   │   │
│   │   │   └── interfaces/              # TypeScript interfaces
│   │   │       ├── IGameRenderer.ts     # Game renderer interface
│   │   │       ├── IInputHandler.ts     # Input handler interface
│   │   │       ├── IApiService.ts       # API service interface
│   │   │       └── ISignalRService.ts   # SignalR service interface
│   │   │
│   │   ├── package.json                 # Node.js dependencies
│   │   ├── tsconfig.json                # TypeScript configuration
│   │   ├── webpack.config.js            # Webpack build configuration
│   │   └── .gitignore                   # Git ignore rules
│   │
│   └── Tetris.Web.Shared/               # 🆕 Shared - Common models and contracts
│       ├── Models/                      # Shared data models
│       │   ├── GameModels/              # Game-related models
│       │   │   ├── GameStateDto.cs      # Game state transfer object
│       │   │   ├── TetrominoDto.cs      # Tetromino transfer object
│       │   │   ├── BoardDto.cs          # Board transfer object
│       │   │   └── GameEventDto.cs      # Game event transfer object
│       │   │
│       │   ├── UserModels/              # User-related models
│       │   │   ├── UserDto.cs           # User transfer object
│       │   │   ├── UserProfileDto.cs    # User profile transfer object
│       │   │   └── UserSettingsDto.cs   # User settings transfer object
│       │   │
│       │   └── Common/                  # Common models
│       │       ├── ResultDto.cs         # Generic result wrapper
│       │       ├── PagedResultDto.cs    # Paged result wrapper
│       │       └── ErrorDto.cs          # Error information
│       │
│       ├── Enums/                       # Shared enumerations
│       │   ├── GameMode.cs              # Game mode enumeration
│       │   ├── DifficultyLevel.cs       # Difficulty level enumeration
│       │   ├── GameEventType.cs         # Game event type enumeration
│       │   └── UserRole.cs              # User role enumeration
│       │
│       ├── Constants/                   # Shared constants
│       │   ├── GameConstants.cs         # Game-related constants
│       │   ├── ApiConstants.cs          # API-related constants
│       │   └── SignalRConstants.cs      # SignalR-related constants
│       │
│       └── Contracts/                   # Shared contracts and interfaces
│           ├── IGameService.cs          # Game service contract
│           ├── IUserService.cs          # User service contract
│           └── ISignalRClient.cs        # SignalR client contract
│
├── tests/                               # Test projects
│   ├── Tetris.Web.Api.Tests/           # API unit and integration tests
│   │   ├── Controllers/                 # Controller tests
│   │   ├── Services/                    # Service tests
│   │   ├── Hubs/                        # SignalR hub tests
│   │   └── Integration/                 # Integration tests
│   │
│   ├── Tetris.Web.Infrastructure.Tests/ # Infrastructure tests
│   │   ├── Repositories/               # Repository tests
│   │   ├── Services/                   # Infrastructure service tests
│   │   └── Data/                       # Database tests
│   │
│   └── Tetris.Web.Client.Tests/        # Frontend tests
│       ├── unit/                       # Unit tests
│       ├── integration/                # Integration tests
│       └── e2e/                        # End-to-end tests
│
├── docs/                               # Documentation
│   ├── api/                            # API documentation
│   │   ├── swagger.json                # OpenAPI specification
│   │   └── endpoints.md                # Endpoint documentation
│   │
│   ├── deployment/                     # Deployment documentation
│   │   ├── docker-compose.yml          # Docker composition
│   │   ├── Dockerfile.api              # API container
│   │   ├── Dockerfile.client           # Client container
│   │   └── kubernetes/                 # Kubernetes manifests
│   │
│   └── architecture/                   # Architecture documentation
│       ├── system-design.md            # System design overview
│       ├── signalr-design.md           # SignalR implementation design
│       └── database-schema.md          # Database schema documentation
│
├── scripts/                            # Build and deployment scripts
│   ├── build.ps1                       # Build script
│   ├── deploy.ps1                      # Deployment script
│   ├── test.ps1                        # Test script
│   └── setup-dev.ps1                   # Development environment setup
│
├── .github/                            # GitHub configuration
│   ├── workflows/                      # GitHub Actions workflows
│   │   ├── ci.yml                      # Continuous integration
│   │   ├── cd.yml                      # Continuous deployment
│   │   └── pr.yml                      # Pull request validation
│   │
│   └── copilot-instructions.md         # GitHub Copilot instructions
│
├── Directory.Build.props               # MSBuild properties
├── Directory.Packages.props            # NuGet package versions
├── .gitignore                          # Git ignore rules
├── .editorconfig                       # Editor configuration
├── README.md                           # Project documentation
└── tetris-web.sln                      # Solution file
```

## Key Components Description

### 1. Backend (Tetris.Web.Api)

**Purpose**: ASP.NET Core Web API with SignalR for real-time communication

**Key Features**:
- REST API endpoints for game management, user authentication, and statistics
- SignalR hubs for real-time game state synchronization
- Integration with existing `Tetris.Core` library
- JWT-based authentication and authorization
- Comprehensive error handling and logging

**Dependencies**:
- ASP.NET Core 9.0
- SignalR
- Entity Framework Core
- Tetris.Core (existing)
- Tetris.Web.Infrastructure
- Tetris.Web.Shared

### 2. Infrastructure (Tetris.Web.Infrastructure)

**Purpose**: Data access layer using Entity Framework Core

**Key Features**:
- Database context and entity configurations
- Repository pattern implementation
- Unit of work pattern for transaction management
- Caching layer for performance optimization
- Database migrations and seeding

**Dependencies**:
- Entity Framework Core
- SQL Server/SQLite provider
- Tetris.Web.Shared

### 3. Frontend (Tetris.Web.Client)

**Purpose**: HTML5 Canvas-based game client with TypeScript

**Key Features**:
- Canvas-based game rendering with 60fps performance
- Real-time communication via SignalR
- Responsive design for desktop and mobile
- Progressive Web App (PWA) capabilities
- Modular TypeScript architecture

**Dependencies**:
- SignalR JavaScript client
- TypeScript
- Webpack for bundling
- Modern browser APIs (Canvas, WebAudio)

### 4. Shared (Tetris.Web.Shared)

**Purpose**: Common models, DTOs, and contracts shared between projects

**Key Features**:
- Data transfer objects for API communication
- Shared enumerations and constants
- Service contracts and interfaces
- Serialization-friendly models

## Technology Integration

### SignalR Communication Flow
```
Client (Canvas) ↔ SignalR Hub ↔ GameEngine ↔ Database
     ↓                ↓            ↓           ↓
Input Events    Game State    Game Logic   Persistence
```

### Authentication Flow
```
Client → REST API → JWT Service → Database
   ↓         ↓          ↓           ↓
Login    Validate   Generate    User Store
```

### Game State Management
```
Tetris.Core.GameEngine → GameSessionService → SignalR Hub → Clients
         ↓                       ↓                ↓           ↓
   Game Logic            State Persistence   Broadcasting  Rendering
```

## Development Workflow

### Phase 1: Foundation (MVP)
1. Set up solution structure and project templates
2. Implement basic SignalR hub and game state synchronization
3. Create minimal Canvas renderer for game board
4. Implement basic REST API for game management

### Phase 2: Core Features
1. Complete game state synchronization via SignalR
2. Implement user authentication and session management
3. Add comprehensive Canvas rendering with animations
4. Implement save/load functionality

### Phase 3: Enhancement
1. Add statistics and leaderboards
2. Implement settings and customization
3. Add performance optimizations and caching
4. Complete responsive design and mobile support

## Build and Deployment

### Development Environment
```bash
# Backend
dotnet restore
dotnet build
dotnet run --project src/Tetris.Web.Api

# Frontend
npm install
npm run build
npm run dev
```

### Production Deployment
- Docker containers for API and client
- Kubernetes orchestration
- CI/CD pipeline with GitHub Actions
- Database migrations and seeding

## Configuration Management

### Backend Configuration (appsettings.json)
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "...",
    "CacheConnection": "..."
  },
  "JwtSettings": {
    "SecretKey": "...",
    "Issuer": "...",
    "Audience": "..."
  },
  "GameSettings": {
    "MaxConcurrentGames": 1000,
    "SessionTimeoutMinutes": 30
  }
}
```

### Frontend Configuration (config.js)
```javascript
const config = {
  apiBaseUrl: 'https://api.tetris.com',
  signalRUrl: 'https://api.tetris.com/gamehub',
  gameSettings: {
    canvasWidth: 800,
    canvasHeight: 600,
    targetFps: 60
  }
};
```

This structure provides a solid foundation for building a scalable, maintainable Tetris web application that leverages the existing game logic while providing modern web capabilities through SignalR real-time communication.

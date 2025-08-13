# Tetris Web Application - GitHub Copilot Implementation Prompts

## 🎯 Overview

This document contains detailed prompts for GitHub Copilot to implement the Tetris Web Application based on the project plan. Each prompt is designed to be used with GitHub Copilot to generate specific code components following the established architecture and requirements.

---

## 📋 Phase 1: Infrastructure Foundation (Weeks 1-3)

### **Sprint 1.1: Project Setup & Architecture (Week 1)**

#### **Prompt 1.1.1: Solution Structure Setup**
```
Create a .NET 9.0 solution structure for a Tetris web application with the following projects:
- Tetris.Web.Api (ASP.NET Core Web API with SignalR)
- Tetris.Web.Infrastructure (Entity Framework Core)
- Tetris.Web.Client (TypeScript/HTML5 frontend)
- Tetris.Web.Shared (Common DTOs and models)

Include Directory.Build.props with common properties for .NET 9.0, nullable reference types, and centralized package management via Directory.Packages.props.

Requirements:
- Use file-scoped namespaces
- Enable nullable reference types
- Configure for .NET 9.0 target framework
- Set up proper project dependencies
- Include development tools configuration
```

#### **Prompt 1.1.2: Entity Framework Models**
```
Based on this SQL schema, create Entity Framework Core models for a Tetris web application:

Tables needed:
- Users (Id, Username, Email, PasswordHash, Salt, DisplayName, IsGuest, IsActive, EmailConfirmed, LastLoginAt, CreatedAt, UpdatedAt)
- UserSessions (Id, UserId, SessionToken, SignalRConnectionId, IpAddress, UserAgent, IsActive, ExpiresAt, CreatedAt, LastActivityAt)
- UserSettings (Id, UserId, KeyMoveLeft, KeyMoveRight, KeyRotateClockwise, etc., ThemeName, HighContrastMode, etc.)
- GameSessions (Id, UserId, GameMode, DifficultyLevel, Status, BoardState, CurrentPieceState, Score, Level, LinesCleared, etc.)
- UserStatistics (Id, UserId, TotalGamesPlayed, HighScore, TotalLinesCleared, etc.)
- DailyLeaderboards (Id, UserId, GameSessionId, LeaderboardDate, GameMode, BestScore, etc.)
- Achievements (Id, Name, Description, Category, RequirementType, RequirementValue, Points, etc.)
- UserAchievements (Id, UserId, AchievementId, Progress, IsCompleted, CompletedAt, etc.)
- ApplicationLogs (Id, UserId, LogLevel, Category, Message, Exception, etc.)
- SystemSettings (Id, SettingKey, SettingValue, Description, etc.)

Create:
1. Entity classes with proper relationships
2. DbContext with DbSets and OnModelCreating configuration
3. Configuration classes for each entity
4. Database connection setup in Program.cs
```

#### **Prompt 1.1.3: Development Environment Configuration**
```
Create development environment configuration for a .NET 9.0 Tetris web application:

1. Docker setup:
   - Dockerfile for the API project
   - docker-compose.yml with API, database (SQL Server), and Redis
   - Development environment variables

2. Logging configuration:
   - Serilog setup with console and file sinks
   - Structured logging for SignalR events
   - Performance logging for game actions

3. Development tools:
   - .editorconfig for consistent formatting
   - .gitignore for .NET projects
   - appsettings.Development.json with development database connection
   - Hot reload configuration

4. Basic CI/CD pipeline structure (GitHub Actions):
   - Build and test workflow
   - Code quality checks
   - Deployment preparation
```

### **Sprint 1.2: Core Backend Services (Week 2)**

#### **Prompt 1.2.1: Authentication System**
```
Create a comprehensive authentication system for the Tetris web application:

1. JWT Authentication Service:
   - Generate and validate JWT tokens
   - Support for user accounts and guest sessions
   - Session management with expiration
   - Password hashing with salt

2. AuthController with endpoints:
   - POST /api/auth/register (username, email, password, displayName)
   - POST /api/auth/login (username, password)
   - POST /api/auth/guest (displayName optional)
   - POST /api/auth/logout
   - GET /api/auth/refresh

3. Authentication middleware:
   - JWT token validation
   - User context injection
   - Guest session handling
   - Session cleanup for expired tokens

4. DTOs and models:
   - UserRegistrationRequest/Response
   - LoginRequest/Response
   - GuestSessionRequest/Response
   - UserInfo model

Follow ASP.NET Core 9.0 best practices and use dependency injection.
```

#### **Prompt 1.2.2: Basic API Controllers**
```
Create basic API controllers for the Tetris web application with proper error handling:

1. GameController:
   - POST /api/games/start (gameMode, difficultyLevel, challengeType, timeLimitSeconds)
   - GET /api/games/{gameSessionId}
   - POST /api/games/{gameSessionId}/save
   - POST /api/games/{gameSessionId}/load
   - GET /api/games/saved

2. UserController:
   - GET /api/users/profile
   - PUT /api/users/profile
   - GET /api/users/settings
   - PUT /api/users/settings

3. Global error handling middleware:
   - Exception handling with proper HTTP status codes
   - Validation error responses
   - Logging integration
   - Standardized error response format

4. Request/Response DTOs:
   - StartGameRequest/Response
   - SaveGameRequest/Response
   - UserSettingsRequest/Response
   - ErrorResponse with validation details

Use [ApiController] attributes, model validation, and async/await patterns.
```

#### **Prompt 1.2.3: SignalR Hub Foundation**
```
Create SignalR hub infrastructure for real-time Tetris gameplay:

1. TetrisGameHub with methods:
   - JoinGame(gameSessionId)
   - LeaveGame(gameSessionId)
   - SendPlayerInput(gameSessionId, inputType, direction)
   - PauseGame(gameSessionId)
   - ResumeGame(gameSessionId)

2. ITetrisGameClient interface:
   - GameStateUpdate(gameState)
   - LinesCleared(linesCleared, clearType, pointsEarned)
   - GameOver(finalScore, statistics)
   - PlayerJoined(username)
   - PlayerLeft(username)
   - AchievementUnlocked(achievement)

3. Connection management:
   - User authentication for SignalR
   - Connection tracking with user sessions
   - Group management for game sessions
   - Cleanup on disconnect

4. SignalR configuration in Program.cs:
   - CORS policy for development
   - Authentication integration
   - Connection limits and timeouts
   - Message size limits

Include proper error handling and logging for all SignalR operations.
```

### **Sprint 1.3: Core Frontend Setup (Week 3)**

#### **Prompt 1.3.1: TypeScript Frontend Structure**
```
Create TypeScript frontend project structure for Tetris web application:

1. Project setup:
   - package.json with TypeScript, webpack, and SignalR client
   - tsconfig.json for strict TypeScript configuration
   - webpack.config.js for development and production builds
   - ESLint and Prettier configuration

2. Basic HTML structure:
   - index.html with responsive meta tags
   - Canvas element for game rendering
   - UI containers for game info, controls, and menus
   - Loading states and error boundaries

3. CSS/SCSS framework:
   - Responsive grid system
   - Game-specific styling (board, pieces, UI)
   - Theme system foundation
   - Mobile-friendly touch targets

4. TypeScript modules structure:
   - /src/services/ (API, SignalR, authentication)
   - /src/components/ (UI components)
   - /src/game/ (Canvas rendering, input handling)
   - /src/types/ (TypeScript interfaces)
   - /src/utils/ (helper functions)

Include proper error handling, type safety, and modern ES modules.
```

#### **Prompt 1.3.2: SignalR Client Integration**
```
Create SignalR TypeScript client for real-time Tetris communication:

1. SignalRService class:
   - Connection management with automatic reconnection
   - Authentication token integration
   - Event handling for game state updates
   - Error handling and logging
   - Connection state management

2. Game event handlers:
   - onGameStateUpdate(gameState)
   - onLinesCleared(clearData)
   - onGameOver(gameOverData)
   - onAchievementUnlocked(achievement)
   - onPlayerJoined/Left(playerData)

3. Input sending methods:
   - sendPlayerInput(inputType, direction)
   - sendGameAction(action, gameSessionId)
   - joinGame(gameSessionId)
   - leaveGame(gameSessionId)

4. TypeScript interfaces:
   - GameState interface
   - PlayerInput interface
   - GameEvent interfaces
   - Connection status types

Include proper TypeScript types, error boundaries, and connection resilience.
```

#### **Prompt 1.3.3: Basic UI Components**
```
Create basic UI components for Tetris web application using TypeScript:

1. Authentication components:
   - LoginForm with validation
   - RegistrationForm with email/username validation
   - GuestSessionButton
   - LogoutButton with confirmation

2. Game layout components:
   - GameBoard container for Canvas
   - ScorePanel (score, level, lines cleared)
   - NextPiecePreview
   - GameControls (pause, save, settings)
   - Navigation menu

3. Form components:
   - Input field with validation
   - Button with loading states
   - Select dropdown
   - Toggle switches for settings

4. Utility components:
   - Loading spinner
   - Error message display
   - Modal dialog
   - Toast notifications

Use modern JavaScript/TypeScript patterns, proper event handling, and responsive design. Include accessibility attributes and keyboard navigation support.
```

---

## 📋 Phase 2: Core Game Implementation (Weeks 4-7)

### **Sprint 2.1: Game Engine Integration (Week 4)**

#### **Prompt 2.1.1: Tetris.Core Integration**
```
Create integration layer for existing Tetris.Core library in the web application:

1. GameEngineWrapper service:
   - Wrap existing GameEngine from Tetris.Core
   - Convert console events to SignalR events
   - Handle game state serialization for web storage
   - Integrate TetrominoController for input processing

2. Game session management:
   - Create/start new game sessions
   - Pause/resume game functionality
   - Game state persistence to database
   - Integration with UserStatistics tracking

3. State conversion utilities:
   - Convert Tetris.Core models to web DTOs
   - Serialize game board state for SignalR
   - Handle tetromino state for client rendering
   - Convert game events to web notifications

4. Service interfaces:
   - IGameSessionService for session management
   - IGameEngineService for core game logic
   - IGameStateService for state persistence
   - IStatisticsService for tracking

Ensure proper error handling, logging, and async patterns throughout.
```

#### **Prompt 2.1.2: Game Session Management**
```
Create comprehensive game session management for Tetris web application:

1. GameSessionService implementation:
   - CreateGameSession(userId, gameMode, difficulty)
   - GetGameSession(gameSessionId)
   - UpdateGameState(gameSessionId, gameState)
   - SaveGame(gameSessionId, saveName)
   - LoadGame(gameSessionId)
   - GetSavedGames(userId)

2. Game state persistence:
   - Serialize/deserialize board state as JSON
   - Store current piece, next piece, held piece states
   - Track game statistics (score, level, lines, time)
   - Auto-save functionality every 30 seconds

3. Game lifecycle management:
   - Start game with initial state
   - Handle pause/resume states
   - Process game over scenarios
   - Clean up abandoned sessions

4. Statistics integration:
   - Update UserStatistics on game events
   - Track achievement progress
   - Calculate leaderboard rankings
   - Generate game completion reports

Include proper transaction handling, error recovery, and database optimization.
```

#### **Prompt 2.1.3: Real-time Game State Synchronization**
```
Implement real-time game state synchronization using SignalR:

1. Game state broadcasting:
   - Efficient delta updates for game board changes
   - Broadcast piece movements and rotations
   - Send line clear animations and effects
   - Update score, level, and statistics in real-time

2. SignalR hub methods in TetrisGameHub:
   - ProcessPlayerInput(gameSessionId, inputData)
   - BroadcastGameState(gameSessionId, gameState)
   - NotifyGameEvent(gameSessionId, eventType, eventData)
   - HandleGameOver(gameSessionId, finalStats)

3. Game event system:
   - LineCleared events with animation data
   - LevelUp events with celebration effects
   - Achievement unlock notifications
   - High score notifications

4. Client synchronization:
   - Handle network latency compensation
   - Client-side prediction for smooth gameplay
   - Conflict resolution for state mismatches
   - Reconnection state recovery

Ensure 60fps performance with minimal network overhead.
```

### **Sprint 2.2: Frontend Game Rendering (Week 5)**

#### **Prompt 2.2.1: HTML5 Canvas Game Rendering**
```
Create HTML5 Canvas rendering system for Tetris game:

1. GameRenderer class:
   - Initialize Canvas context with proper scaling
   - Implement 60fps rendering loop
   - Handle responsive canvas sizing
   - Optimize rendering performance

2. Board rendering:
   - Draw 10x20 game board grid
   - Render filled cells with tetromino colors
   - Draw grid lines (toggleable)
   - Handle board animations and effects

3. Rendering pipeline:
   - Clear canvas each frame
   - Render background and grid
   - Draw placed tetrominoes
   - Render current falling piece
   - Draw ghost piece preview
   - Apply visual effects and animations

4. Performance optimization:
   - Use requestAnimationFrame for smooth animation
   - Implement dirty rectangle rendering
   - Cache static elements
   - Minimize canvas state changes

Include proper TypeScript types and error handling.
```

#### **Prompt 2.2.2: Tetromino Rendering System**
```
Create tetromino rendering system with animations and effects:

1. TetrominoRenderer class:
   - Render all 7 tetromino types (I, J, L, O, S, T, Z)
   - Color system matching console version
   - Rotation animation handling
   - Smooth movement interpolation

2. Piece visualization:
   - Current falling piece with smooth movement
   - Ghost piece preview at landing position
   - Next piece preview in sidebar
   - Held piece display (if implemented)

3. Animation system:
   - Smooth piece dropping animation
   - Rotation animations with easing
   - Line clearing effects with particle systems
   - Piece placement visual feedback

4. Visual effects:
   - Line clear animations (flash, collapse)
   - Tetris completion celebration
   - Level up visual effects
   - Achievement unlock animations

Include configuration for animation speeds and effect toggles.
```

#### **Prompt 2.2.3: Game UI Components**
```
Create game UI components for displaying game information:

1. ScorePanel component:
   - Current score display with animation
   - Level indicator with progress
   - Lines cleared counter
   - Game timer (play time)

2. NextPiecePreview component:
   - Mini canvas for next piece
   - Proper scaling and centering
   - Multiple piece preview (queue)
   - Color matching game pieces

3. StatisticsPanel component:
   - Real-time statistics display
   - Pieces per minute calculation
   - Line clear type counters (single, double, triple, tetris)
   - Session statistics

4. GameControls component:
   - Pause/Resume button
   - Save game button
   - Settings menu button
   - Quit game with confirmation

All components should be responsive and update via SignalR events.
```

### **Sprint 2.3: Input System & Game Loop (Week 6)**

#### **Prompt 2.3.1: Input Handling System**
```
Create comprehensive input handling for Tetris web application:

1. InputManager class:
   - Keyboard event capture and processing
   - Touch/mobile input handling
   - Input validation and rate limiting
   - Customizable key bindings

2. Keyboard controls:
   - Arrow keys for movement (left, right, down)
   - Up arrow or other key for rotation
   - Space for hard drop
   - P for pause
   - C for hold piece (if implemented)

3. Mobile touch controls:
   - Touch areas for piece movement
   - Swipe gestures for rotation
   - Tap for soft drop, long press for hard drop
   - Touch-friendly button layout

4. Input processing:
   - Debouncing for rapid key presses
   - Key repeat handling with customizable rates
   - Input queue for smooth gameplay
   - Integration with SignalR for server communication

Include accessibility support and customizable controls.
```

#### **Prompt 2.3.2: Game Loop Implementation**
```
Implement client and server-side game loop for Tetris:

1. Client-side game loop:
   - 60fps rendering loop using requestAnimationFrame
   - Input processing and validation
   - Client-side prediction for responsiveness
   - Interpolation for smooth animations

2. Server-side timing:
   - Game tick system for piece falling
   - Difficulty-based speed progression
   - Timer management for timed game modes
   - Game state validation and authority

3. Synchronization:
   - Client prediction with server reconciliation
   - Lag compensation for input processing
   - Network optimization for real-time gameplay
   - Conflict resolution for state mismatches

4. Performance optimization:
   - Efficient update cycles
   - Memory management for long sessions
   - CPU usage optimization
   - Battery life consideration for mobile

Ensure smooth 60fps gameplay with minimal latency.
```

#### **Prompt 2.3.3: Game Modes Implementation**
```
Implement all three game modes for Tetris web application:

1. Classic Mode:
   - Standard Tetris gameplay with level progression
   - Speed increases every 10 lines cleared
   - Unlimited play until game over
   - Traditional scoring system

2. Timed Mode:
   - Fixed time limit (customizable: 2, 5, 10 minutes)
   - Countdown timer display
   - Score maximization objective
   - Bonus points for time efficiency

3. Challenge Mode:
   - Specific objectives (clear X lines, reach Y level, achieve Z score)
   - Progress tracking toward goal
   - Challenge completion rewards
   - Unlockable challenge types

4. Mode-specific UI:
   - Mode selection screen
   - Mode-specific HUD elements
   - Progress indicators for challenges
   - Mode-specific statistics tracking

Include proper state management and mode transition handling.
```

### **Sprint 2.4: Game Features & Polish (Week 7)**

#### **Prompt 2.4.1: Advanced Game Features**
```
Implement advanced Tetris features and polish:

1. Hold piece functionality:
   - Hold current piece for later use
   - Hold piece display in UI
   - One hold per piece limitation
   - Visual feedback for hold action

2. Piece placement system:
   - Ghost piece preview at landing position
   - Placement validation and confirmation
   - Lock delay for adjustment time
   - T-spin detection and scoring

3. Line clearing enhancements:
   - Simultaneous multiple line detection
   - Special scoring for Tetris (4 lines)
   - Combo system for consecutive clears
   - Back-to-back bonuses

4. Level progression:
   - Dynamic speed adjustment
   - Visual level indicators
   - Level-up celebrations
   - Difficulty curve balancing

Include smooth animations and proper game feel.
```

#### **Prompt 2.4.2: Visual Effects System**
```
Create comprehensive visual effects system:

1. Particle effects:
   - Line clear explosion effects
   - Tetris completion celebration
   - Level up sparkles
   - Achievement unlock animations

2. Animation system:
   - Smooth piece movement tweening
   - Rotation animations with easing
   - Board shake effects for Tetris
   - Fade transitions for UI changes

3. Theme system:
   - Multiple color schemes (Classic, Modern, High Contrast)
   - Customizable tetromino colors
   - Background patterns and effects
   - UI theme consistency

4. Performance optimization:
   - Effect pooling for particle systems
   - GPU acceleration where possible
   - Configurable effect quality levels
   - Frame rate impact monitoring

All effects should be toggleable for accessibility and performance.
```

#### **Prompt 2.4.3: Audio Integration**
```
Implement comprehensive audio system for Tetris web application:

1. Audio manager:
   - Sound effect loading and caching
   - Background music streaming
   - Volume controls (master, effects, music)
   - Audio context management for web browsers

2. Sound effects:
   - Piece movement sounds
   - Piece rotation feedback
   - Line clear sound effects
   - Game over and achievement sounds
   - UI interaction feedback

3. Background music:
   - Looping game music tracks
   - Menu music
   - Dynamic music based on game state
   - Smooth transitions between tracks

4. Audio settings:
   - Individual volume controls
   - Mute toggles for effects and music
   - Audio quality settings
   - Accessibility audio cues

Ensure cross-browser compatibility and proper audio loading.
```

---

## 📋 Phase 3: User Experience & Features (Weeks 8-11)

### **Sprint 3.1: User Management System (Week 8)**

#### **Prompt 3.1.1: User Profile & Settings System**
```
Create comprehensive user profile and settings management:

1. User profile management:
   - Display user information (username, email, join date)
   - Profile editing with validation
   - Avatar/profile picture support (optional)
   - Account status and verification

2. Settings system:
   - Game controls customization
   - Audio/visual preferences
   - Accessibility options
   - Privacy settings

3. Settings categories:
   - Controls: customizable key bindings
   - Gameplay: difficulty, auto-save, visual aids
   - Audio: volume levels, sound toggles
   - Visual: themes, animations, effects
   - Accessibility: high contrast, key repeat rates

4. Settings persistence:
   - Save settings to database
   - Real-time settings sync
   - Import/export settings
   - Default settings restoration

Include validation, error handling, and immediate setting application.
```

#### **Prompt 3.1.2: Statistics System**
```
Implement comprehensive statistics tracking and display:

1. Statistics calculation service:
   - Real-time statistics updates during gameplay
   - Historical data aggregation
   - Performance metrics calculation
   - Trend analysis and projections

2. Statistics categories:
   - Overall: games played, total score, play time
   - Performance: high score, average score, best level
   - Lines: total cleared, clear type distribution
   - Efficiency: pieces per minute, time per game
   - Achievements: completion rate, points earned

3. Statistics visualization:
   - Charts and graphs for trends
   - Comparison with personal bests
   - Progress indicators for improvements
   - Visual statistics dashboard

4. Data export:
   - CSV export for detailed analysis
   - Shareable statistics summaries
   - Integration with leaderboards
   - Historical data preservation

Include caching for performance and real-time updates.
```

#### **Prompt 3.1.3: Achievement System**
```
Create comprehensive achievement system with progress tracking:

1. Achievement definitions:
   - Score-based achievements (reach X points)
   - Line-clearing achievements (clear X lines, Y tetrises)
   - Speed achievements (pieces per minute, quick games)
   - Special achievements (perfect games, streaks)

2. Achievement tracking:
   - Real-time progress monitoring
   - Multiple criteria achievements
   - Hidden/secret achievements
   - Achievement dependencies and unlocks

3. Achievement notifications:
   - In-game unlock notifications
   - Visual celebrations for completion
   - Achievement showcase in profile
   - Social sharing of achievements

4. Achievement rewards:
   - Points system for achievements
   - Unlock new themes or features
   - Leaderboard badges
   - Special recognition for rare achievements

Include proper database design and efficient tracking algorithms.
```

### **Sprint 3.2: Leaderboards & Competition (Week 9)**

#### **Prompt 3.2.1: Leaderboard System**
```
Implement comprehensive leaderboard system:

1. Leaderboard types:
   - Daily leaderboards (reset at midnight)
   - Weekly leaderboards (Monday to Sunday)
   - Monthly leaderboards (calendar month)
   - All-time leaderboards

2. Ranking categories:
   - High score rankings
   - Level reached rankings
   - Lines cleared rankings
   - Game completion speed

3. Leaderboard features:
   - Real-time ranking updates
   - User's current position display
   - Top 100 player display
   - Pagination for large datasets

4. Performance optimization:
   - Cached ranking calculations
   - Efficient database queries
   - Background ranking updates
   - Minimal real-time data transfer

Include proper handling of ties and ranking algorithms.
```

#### **Prompt 3.2.2: Social Features**
```
Create social features for player engagement:

1. Player comparison:
   - Compare statistics with other players
   - Head-to-head performance metrics
   - Achievement comparison
   - Progress tracking relative to friends

2. Social statistics:
   - Shareable achievement unlocks
   - High score sharing
   - Challenge completion posts
   - Personal best celebrations

3. Friend system (optional):
   - Add/remove friends
   - Friend activity feed
   - Friend leaderboards
   - Challenge friends to beat scores

4. Community features:
   - Global player statistics
   - Community challenges
   - Seasonal events
   - Player recognition programs

Focus on privacy settings and user control over social features.
```

#### **Prompt 3.2.3: Performance Optimization**
```
Optimize performance for leaderboards and large datasets:

1. Database optimization:
   - Efficient indexing for ranking queries
   - Materialized views for complex calculations
   - Query optimization and caching
   - Pagination with minimal overhead

2. Caching strategies:
   - Redis cache for frequently accessed rankings
   - In-memory caching for active sessions
   - Cache invalidation strategies
   - Background cache warming

3. Real-time optimization:
   - Efficient SignalR group management
   - Minimal data transfer for updates
   - Batch updates for multiple changes
   - Connection pooling and management

4. Scalability considerations:
   - Horizontal scaling strategies
   - Load balancing for high traffic
   - Database partitioning for large datasets
   - CDN integration for static content

Monitor performance metrics and optimize based on usage patterns.
```

### **Sprint 3.3: Mobile & Accessibility (Week 10)**

#### **Prompt 3.3.1: Mobile Optimization**
```
Create mobile-optimized experience for Tetris web application:

1. Responsive design:
   - Flexible layout for different screen sizes
   - Touch-friendly UI elements
   - Optimized game board sizing
   - Portrait and landscape orientation support

2. Touch controls:
   - Touch areas for piece movement
   - Swipe gestures for rotation and hard drop
   - Long press for soft drop
   - Haptic feedback for supported devices

3. Performance optimization:
   - Reduced visual effects for mobile
   - Optimized canvas rendering
   - Battery usage optimization
   - Memory management for long sessions

4. Mobile-specific features:
   - Pause on app background
   - Save state on navigation away
   - Push notifications for achievements (optional)
   - Offline capability for single player

Test across different mobile devices and operating systems.
```

#### **Prompt 3.3.2: Accessibility Features**
```
Implement comprehensive accessibility features:

1. Visual accessibility:
   - High contrast mode for better visibility
   - Customizable color schemes
   - Scalable UI elements
   - Clear visual indicators for game state

2. Motor accessibility:
   - Customizable key repeat rates
   - Alternative input methods
   - Adjustable timing for difficult controls
   - One-handed play options

3. Screen reader support:
   - ARIA labels for all interactive elements
   - Descriptive text for game state
   - Keyboard navigation for all features
   - Audio cues for game events

4. Cognitive accessibility:
   - Clear instructions and tutorials
   - Simplified UI modes
   - Pause functionality in all modes
   - Progress saving and resumption

Follow WCAG 2.1 guidelines and test with accessibility tools.
```

#### **Prompt 3.3.3: Cross-browser Compatibility**
```
Ensure cross-browser compatibility for Tetris web application:

1. Browser testing:
   - Chrome/Chromium compatibility
   - Firefox compatibility and optimization
   - Safari compatibility (WebKit specifics)
   - Edge compatibility and testing

2. Polyfills and fallbacks:
   - WebSocket fallbacks for older browsers
   - Canvas rendering compatibility
   - Audio context handling differences
   - CSS Grid and Flexbox fallbacks

3. Performance optimization:
   - Browser-specific optimizations
   - Feature detection and graceful degradation
   - Memory usage optimization
   - GPU acceleration where available

4. Testing strategy:
   - Automated cross-browser testing
   - Manual testing on real devices
   - Performance benchmarking
   - Compatibility matrix maintenance

Include proper error handling for unsupported features.
```

### **Sprint 3.4: Advanced Features (Week 11)**

#### **Prompt 3.4.1: Game Customization System**
```
Create comprehensive game customization options:

1. Visual themes:
   - Multiple complete visual themes
   - Customizable color palettes
   - Background patterns and effects
   - UI skin customization

2. Tetromino customization:
   - Individual piece color selection
   - Custom color schemes creation
   - Pattern overlays for pieces
   - Accessibility color combinations

3. Board customization:
   - Adjustable board size (within limits)
   - Grid line visibility toggles
   - Background patterns
   - Border and frame styles

4. Effect customization:
   - Animation speed controls
   - Particle effect intensity
   - Visual feedback options
   - Performance vs. quality trade-offs

Save all customizations to user profile with sync capability.
```

#### **Prompt 3.4.2: Performance Monitoring**
```
Implement comprehensive performance monitoring and optimization:

1. Client-side monitoring:
   - Frame rate monitoring and alerts
   - Memory usage tracking
   - Network latency measurement
   - Battery usage optimization

2. Server-side monitoring:
   - API response time tracking
   - Database query performance
   - SignalR connection health
   - Resource utilization monitoring

3. Metrics dashboard:
   - Real-time performance metrics
   - Historical performance trends
   - Error rate monitoring
   - User experience metrics

4. Optimization features:
   - Automatic quality adjustment
   - Performance warnings for users
   - Background optimization tasks
   - Resource cleanup and garbage collection

Include alerting for performance degradation and automatic optimization.
```

#### **Prompt 3.4.3: Data Management Features**
```
Create comprehensive data management capabilities:

1. Data export:
   - Export user statistics to CSV/JSON
   - Game history export
   - Settings backup and restore
   - Achievement progress export

2. Data backup:
   - Automatic cloud backup of game data
   - Manual backup creation
   - Backup restoration functionality
   - Data integrity verification

3. GDPR compliance:
   - Data download requests
   - Account deletion with data removal
   - Privacy settings management
   - Data processing consent tracking

4. Data migration:
   - Import from other Tetris implementations
   - Version migration for database updates
   - Data format conversion utilities
   - Legacy data support

Ensure data security and privacy throughout all operations.
```

---

## 📋 Phase 4: Testing, Deployment & Launch (Weeks 12-16)

### **Sprint 4.1: Testing & Quality Assurance (Week 12)**

#### **Prompt 4.1.1: Unit Testing Framework**
```
Create comprehensive unit testing for Tetris web application:

1. Backend unit tests:
   - Service layer tests for all business logic
   - Repository pattern tests with in-memory database
   - Authentication and authorization tests
   - SignalR hub method testing

2. Frontend unit tests:
   - TypeScript component testing with Jest
   - Game logic unit tests
   - UI component testing
   - Utility function testing

3. Test infrastructure:
   - Test data builders and factories
   - Mock services and dependencies
   - Test configuration and setup
   - Coverage reporting and thresholds

4. Testing standards:
   - AAA pattern (Arrange, Act, Assert)
   - Proper test naming conventions
   - Test isolation and independence
   - Performance test benchmarks

Aim for 80%+ code coverage and fast test execution.
```

#### **Prompt 4.1.2: Integration Testing**
```
Implement comprehensive integration testing:

1. API integration tests:
   - End-to-end API workflow testing
   - Database integration with test containers
   - Authentication flow testing
   - Error handling and edge cases

2. SignalR integration tests:
   - Real-time communication testing
   - Connection management testing
   - Group messaging functionality
   - Performance under load

3. Database integration tests:
   - Entity Framework context testing
   - Complex query validation
   - Transaction handling tests
   - Migration testing

4. Test environment setup:
   - Docker test containers
   - Test database seeding
   - Clean test state management
   - Parallel test execution

Include automated test execution in CI/CD pipeline.
```

#### **Prompt 4.1.3: End-to-End Testing**
```
Create end-to-end testing for complete user workflows:

1. E2E test scenarios:
   - Complete user registration and login flow
   - Full game session from start to completion
   - Settings modification and persistence
   - Leaderboard functionality and updates

2. Cross-browser E2E testing:
   - Selenium WebDriver automation
   - Browser-specific behavior testing
   - Mobile device testing simulation
   - Performance testing across browsers

3. User journey testing:
   - New user onboarding flow
   - Returning user experience
   - Guest user limitations and upgrade
   - Error recovery scenarios

4. Test automation:
   - Continuous E2E testing in CI/CD
   - Visual regression testing
   - Performance regression detection
   - Automated test reporting

Use tools like Playwright or Cypress for reliable E2E testing.
```

### **Sprint 4.2: Performance & Security (Week 13)**

#### **Prompt 4.2.1: Performance Optimization**
```
Implement comprehensive performance optimization:

1. Database optimization:
   - Query performance analysis and optimization
   - Index optimization for leaderboards
   - Connection pooling configuration
   - Caching strategy implementation

2. Frontend optimization:
   - Bundle size optimization and code splitting
   - Image optimization and lazy loading
   - CSS and JavaScript minification
   - CDN integration for static assets

3. SignalR optimization:
   - Connection pooling and management
   - Message compression and batching
   - Efficient serialization
   - Connection scaling strategies

4. Monitoring and profiling:
   - Application Performance Monitoring (APM)
   - Real user monitoring (RUM)
   - Performance budgets and alerts
   - Continuous performance regression testing

Target specific performance metrics and monitor continuously.
```

#### **Prompt 4.2.2: Security Implementation**
```
Implement comprehensive security measures:

1. API security:
   - Rate limiting for all endpoints
   - Input validation and sanitization
   - SQL injection prevention
   - XSS protection measures

2. Authentication security:
   - Secure password hashing (bcrypt)
   - JWT token security and rotation
   - Session management security
   - Multi-factor authentication (optional)

3. Network security:
   - HTTPS enforcement
   - CORS policy configuration
   - Security headers implementation
   - CSP (Content Security Policy)

4. Data protection:
   - Encryption at rest and in transit
   - Personal data anonymization
   - Secure backup procedures
   - GDPR compliance measures

Include security scanning and penetration testing.
```

#### **Prompt 4.2.3: Load Testing**
```
Implement comprehensive load testing strategy:

1. Load test scenarios:
   - Concurrent user simulation (1000+ users)
   - SignalR connection stress testing
   - Database load testing
   - API endpoint stress testing

2. Performance benchmarks:
   - Response time under load
   - Throughput measurements
   - Resource utilization monitoring
   - Breaking point identification

3. Load testing tools:
   - Artillery.io for API load testing
   - SignalR-specific load testing
   - Database stress testing
   - Real-time game simulation

4. Scalability testing:
   - Horizontal scaling validation
   - Auto-scaling configuration
   - Load balancer testing
   - Database clustering performance

Document performance characteristics and scaling limits.
```

### **Sprint 4.3: Deployment Infrastructure (Week 14)**

#### **Prompt 4.3.1: Production Environment Setup**
```
Create production deployment infrastructure:

1. Cloud infrastructure:
   - Container orchestration (Kubernetes/Docker Swarm)
   - Load balancer configuration
   - Auto-scaling group setup
   - Health check configuration

2. Database setup:
   - Production database configuration
   - Backup and disaster recovery
   - High availability setup
   - Performance monitoring

3. Monitoring and alerting:
   - Application performance monitoring
   - Infrastructure monitoring
   - Log aggregation and analysis
   - Alert configuration for critical issues

4. Security configuration:
   - Network security groups
   - SSL certificate management
   - Secrets management
   - Compliance and audit logging

Include infrastructure as code and automated provisioning.
```

#### **Prompt 4.3.2: CI/CD Pipeline**
```
Create comprehensive CI/CD pipeline:

1. Build pipeline:
   - Automated build on code commit
   - Unit and integration test execution
   - Code quality analysis
   - Security vulnerability scanning

2. Deployment pipeline:
   - Environment-specific deployments
   - Blue-green deployment strategy
   - Database migration automation
   - Rollback capabilities

3. Quality gates:
   - Test coverage requirements
   - Performance regression testing
   - Security scan pass requirements
   - Manual approval for production

4. Pipeline monitoring:
   - Build and deployment metrics
   - Failure notification system
   - Pipeline performance optimization
   - Audit trail and compliance

Use GitHub Actions, Azure DevOps, or similar CI/CD platform.
```

#### **Prompt 4.3.3: Documentation and Operations**
```
Create comprehensive documentation and operational procedures:

1. Technical documentation:
   - API documentation with OpenAPI/Swagger
   - Database schema documentation
   - Architecture decision records (ADRs)
   - Code documentation and comments

2. Operational documentation:
   - Deployment procedures
   - Troubleshooting guides
   - Monitoring and alerting runbooks
   - Disaster recovery procedures

3. User documentation:
   - User manual and tutorials
   - FAQ and common issues
   - Accessibility guide
   - Mobile usage instructions

4. Development documentation:
   - Development environment setup
   - Contributing guidelines
   - Code review standards
   - Testing procedures

Keep documentation up-to-date and easily accessible.
```

### **Sprint 4.4: Launch Preparation (Week 15-16)**

#### **Prompt 4.4.1: Beta Testing Program**
```
Implement comprehensive beta testing program:

1. Beta testing infrastructure:
   - Beta user registration system
   - Feedback collection mechanisms
   - Bug reporting integration
   - Beta user communication tools

2. Testing scenarios:
   - Feature completeness testing
   - User experience validation
   - Performance testing under real conditions
   - Cross-platform compatibility validation

3. Feedback processing:
   - Feedback categorization and prioritization
   - Bug triage and fixing
   - Feature enhancement based on feedback
   - User satisfaction measurement

4. Beta graduation criteria:
   - Performance benchmarks met
   - Critical bug resolution
   - User satisfaction thresholds
   - Feature completeness validation

Plan for iterative beta releases with feedback incorporation.
```

#### **Prompt 4.4.2: Launch Monitoring**
```
Create comprehensive launch monitoring and support:

1. Launch monitoring dashboard:
   - Real-time user metrics
   - Performance monitoring
   - Error rate tracking
   - Resource utilization monitoring

2. Support infrastructure:
   - User support ticket system
   - FAQ and help documentation
   - Community forum setup (optional)
   - Direct support channels

3. Launch metrics:
   - User acquisition tracking
   - Feature usage analytics
   - Performance under real load
   - User retention metrics

4. Issue response procedures:
   - Critical issue escalation
   - Hotfix deployment procedures
   - User communication protocols
   - Rollback procedures if needed

Prepare for various launch scenarios and potential issues.
```

#### **Prompt 4.4.3: Production Launch**
```
Execute production launch with monitoring and support:

1. Launch execution:
   - Final production deployment
   - DNS and traffic routing
   - SSL certificate activation
   - CDN configuration

2. Real-time monitoring:
   - Application health monitoring
   - User experience monitoring
   - Performance metrics tracking
   - Error rate and issue detection

3. User onboarding:
   - New user experience optimization
   - Tutorial and help system
   - Community building and engagement
   - User feedback collection

4. Post-launch optimization:
   - Performance tuning based on real usage
   - Feature usage analysis
   - User feedback incorporation
   - Continuous improvement planning

Monitor closely for the first 48-72 hours post-launch.
```

---

## 🎯 Usage Instructions

### **How to Use These Prompts:**

1. **Sequential Implementation**: Use prompts in order within each sprint
2. **Context Preservation**: Provide relevant context from previous implementations
3. **Customization**: Adapt prompts based on specific project needs
4. **Code Review**: Review generated code for quality and consistency
5. **Testing**: Test each component thoroughly before moving to the next

### **Best Practices:**

- Always include error handling and logging in generated code
- Follow established architectural patterns and naming conventions
- Implement proper TypeScript types and interfaces
- Include unit tests for all generated components
- Document complex logic and architectural decisions

### **Quality Assurance:**

- Code should follow .NET and TypeScript best practices
- All database operations should be async and properly handled
- SignalR implementations should include proper error handling
- Frontend code should be responsive and accessible
- Security considerations should be implemented throughout

---

This comprehensive set of prompts provides detailed guidance for implementing the entire Tetris web application using GitHub Copilot, ensuring consistency, quality, and completeness throughout the development process.

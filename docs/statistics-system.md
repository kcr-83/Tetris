# Tetris Game Statistics System

## Overview
The Tetris Game Statistics System is a comprehensive feature that collects, stores, and displays detailed game statistics, including highest score, average score, number of games played, and various gameplay metrics. The system provides an option to reset statistics and tracks detailed performance data.

## System Components

### 1. Core Models

#### GameStatistics (`src/Tetris.Core/Models/GameStatistics.cs`)
A comprehensive model that tracks all game statistics:

**Basic Statistics:**
- Total games played
- Total games completed (vs abandoned)
- Highest score achieved
- Total cumulative score
- Average score (calculated property)

**Performance Metrics:**
- Total rows cleared
- Highest level reached
- Total time played
- Average time per game
- Completion rate percentage

**Detailed Row Clearing Statistics:**
- Single row clears
- Double row clears
- Triple row clears
- Tetris (4 rows) clears

**Metadata:**
- Creation timestamp
- Last updated timestamp
- Formatted time display

### 2. Service Layer

#### IGameStatisticsService (`src/Tetris.Core/Services/IGameStatisticsService.cs`)
Interface defining the contract for statistics management:
- Get current statistics
- Update game completion statistics
- Update row clearing statistics
- Reset all statistics
- Save statistics to persistent storage

#### GameStatisticsService (`src/Tetris.Core/Services/GameStatisticsService.cs`)
Concrete implementation providing:
- JSON-based file persistence
- Automatic directory creation
- Error handling and recovery
- Asynchronous operations
- Statistics calculation and aggregation

### 3. User Interface

#### StatisticsInterface (`src/Tetris.Core/UI/StatisticsInterface.cs`)
Comprehensive UI for displaying statistics:

**Features:**
- Color-coded statistics display
- Real-time data refresh
- Interactive controls (R: Reset, F5: Refresh, ESC: Return)
- Confirmation dialogs for destructive operations
- Error handling with user feedback
- Responsive layout

**Display Sections:**
- Games played summary
- Score achievements
- Level and row statistics
- Row clearing breakdown
- Time tracking
- Metadata information

### 4. Game Integration

#### GameEngine Updates (`src/Tetris.Core/Models/GameEngine.cs`)
Enhanced with statistics tracking:
- Game start time tracking
- Automatic statistics updates on game completion
- Row clearing statistics updates
- Win/loss tracking
- Time played calculation

#### MainMenuInterface Updates (`src/Tetris.Core/UI/MainMenuInterface.cs`)
Added statistics menu option:
- "Statistics" menu item
- Navigation to statistics interface
- Error handling for statistics display

## Data Persistence

### Storage Format
- **File Format:** JSON
- **Location:** `Statistics/game_statistics.json`
- **Structure:** Human-readable with proper indentation
- **Encoding:** UTF-8

### Sample Data Structure
```json
{
  "totalGamesPlayed": 15,
  "totalGamesCompleted": 12,
  "highestScore": 25000,
  "totalScore": 180000,
  "totalRowsCleared": 450,
  "highestLevel": 8,
  "totalTimePlayedSeconds": 3600.5,
  "singleRowClears": 120,
  "doubleRowClears": 45,
  "tripleRowClears": 15,
  "tetrisRowClears": 8,
  "lastUpdated": "2025-07-03T16:30:45.123Z",
  "createdAt": "2025-07-01T10:00:00.000Z"
}
```

## Features

### Statistics Tracking
- **Automatic:** Statistics are updated automatically during gameplay
- **Real-time:** Updates occur immediately when events happen
- **Persistent:** Data survives application restarts
- **Fault-tolerant:** Continues working even if statistics updates fail

### User Interactions

#### View Statistics
1. Navigate to main menu
2. Select "Statistics" option
3. Browse comprehensive statistics display
4. Use controls for additional actions

#### Reset Statistics
1. Open statistics interface
2. Press 'R' for reset
3. Confirm action in dialog
4. Statistics are cleared and reset to zero

#### Refresh Data
- Press 'F5' to reload statistics from storage
- Useful for viewing updated data

### Performance Metrics

#### Calculated Statistics
- **Average Score:** Total score ÷ Total games played
- **Completion Rate:** (Completed games ÷ Total games) × 100%
- **Average Time:** Total time ÷ Total games played
- **Average Rows:** Total rows cleared ÷ Total games played

#### Time Formatting
- Displays as HH:MM:SS for times under 24 hours
- Shows days for longer play sessions
- Handles very long gaming sessions gracefully

## Integration Points

### Game Events
- **Game Start:** Resets start time tracking
- **Game End:** Updates completion statistics
- **Row Clearing:** Updates row clearing statistics
- **Score Updates:** Tracked through game completion

### UI Navigation
- **Main Menu:** Statistics option available
- **Statistics View:** Return to menu option
- **Error Handling:** Graceful fallback to menu

## Error Handling

### Robust Design
- **File System Errors:** Graceful handling of I/O failures
- **Corrupted Data:** Starts fresh if data is invalid
- **Missing Files:** Creates new statistics automatically
- **Service Failures:** Game continues even if statistics fail

### User Feedback
- **Success Messages:** Confirmation for reset operations
- **Error Messages:** Clear error descriptions
- **Recovery:** Automatic recovery from most error conditions

## Technical Details

### Architecture
- **Clean Architecture:** Separated concerns with clear layer boundaries
- **SOLID Principles:** Single responsibility, dependency injection
- **Async Operations:** Non-blocking I/O operations
- **Event-Driven:** Loose coupling through events

### Performance
- **Lazy Loading:** Statistics loaded only when needed
- **Caching:** In-memory caching for current session
- **Efficient Updates:** Only saves when data changes
- **Background Operations:** Non-blocking statistics updates

## Usage Examples

### Programming Interface
```csharp
// Get statistics
var stats = await statisticsService.GetStatisticsAsync();

// Update game completion
await statisticsService.UpdateGameStatisticsAsync(
    finalScore: 15000,
    finalLevel: 5,
    rowsCleared: 25,
    gameTimeSeconds: 300.5,
    isCompleted: true);

// Update row clearing
await statisticsService.UpdateRowClearStatisticsAsync(4); // Tetris

// Reset all statistics
await statisticsService.ResetStatisticsAsync();
```

### User Interface
1. **Access:** Main Menu → Statistics
2. **Navigation:** Arrow keys, Enter to select
3. **Actions:** R (Reset), F5 (Refresh), ESC (Return)
4. **Confirmation:** Y/N dialogs for destructive actions

## Future Enhancements

### Potential Additions
- Export statistics to CSV/Excel
- Statistics graphs and charts
- Achievement system based on statistics
- Cloud storage synchronization
- Comparative statistics (best week/month)
- Performance trends over time

### Data Extensions
- Game mode specific statistics
- Difficulty level breakdown
- Session-based tracking
- Streak counters (consecutive wins)
- Speed metrics (pieces per minute)

The statistics system provides comprehensive tracking and display of game performance metrics, enhancing the player experience with detailed feedback about their Tetris gameplay history and achievements.

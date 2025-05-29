# SQL Data Model for Tetris Application

This document outlines the SQL data model for the Tetris application, designed to store information about users, their settings, saved game states, gameplay history, and statistics.

## Database Schema Diagram

```
┌───────────────────┐       ┌───────────────────┐       ┌───────────────────┐
│      Users        │       │   UserSettings    │       │   GameStates      │
├───────────────────┤       ├───────────────────┤       ├───────────────────┤
│ UserId (PK)       │━━━━━━━┫ UserId (FK)       │       │ GameStateId (PK)  │
│ Username          │       │ SettingsId (PK)   │       │ UserId (FK)       │
│ Email             │       │ ControlSettings   │       │ BoardState        │
│ PasswordHash      │       │ SoundEnabled      │       │ CurrentScore      │
│ PasswordSalt      │       │ MusicEnabled      │       │ CurrentLevel      │
│ RegistrationDate  │       │ ColorTheme        │       │ LinesCleared      │
│ LastLoginDate     │       │ CreatedAt         │       │ NextTetromino     │
│ IsActive          │       │ UpdatedAt         │       │ SaveDate          │
└───────────────────┘       └───────────────────┘       │ GameMode          │
        │                                               │ DifficultyLevel   │
        │                                               └───────────────────┘
        │                                                       ┃
        │                                                       ┃
┌───────────────────┐                               ┌───────────────────────┐
│ GameStatistics    │                               │    GameHistories      │
├───────────────────┤                               ├───────────────────────┤
│ StatisticsId (PK) │                               │ GameHistoryId (PK)    │
│ UserId (FK)       │                               │ UserId (FK)           │
│ HighestScore      │                               │ GameDate              │
│ TotalGamesPlayed  │                               │ Duration              │
│ WinCount          │                               │ Score                 │
│ LoseCount         │                               │ Level                 │
│ TotalTimePlayed   │                               │ LinesCleared          │
│ AverageScore      │                               │ GameMode              │
│ TotalLinesCleared │                               │ DifficultyLevel       │
│ UpdatedAt         │                               │ TetrominosUsed        │
└───────────────────┘                               └───────────────────────┘
                                                             ┃
                                                             ┃
                                               ┌─────────────────────────────┐
                                               │    GameHistoryDetails       │
                                               ├─────────────────────────────┤
                                               │ DetailId (PK)               │
                                               │ GameHistoryId (FK)          │
                                               │ EventType                   │
                                               │ EventTime                   │
                                               │ EventData                   │
                                               └─────────────────────────────┘
```

## SQL Schema Definitions

### Users Table

```sql
CREATE TABLE Users (
    UserId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(128) NOT NULL,
    PasswordSalt NVARCHAR(128) NOT NULL,
    RegistrationDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    LastLoginDate DATETIME NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT CK_Email_Format CHECK (Email LIKE '%_@_%._%')
);

CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Users_Email ON Users(Email);
```

### UserSettings Table

```sql
CREATE TABLE UserSettings (
    SettingsId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    ControlSettings NVARCHAR(MAX) NOT NULL, -- JSON format for key mappings
    SoundEnabled BIT NOT NULL DEFAULT 1,
    MusicEnabled BIT NOT NULL DEFAULT 1,
    ColorTheme NVARCHAR(50) NOT NULL DEFAULT 'Default',
    CreatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_UserSettings_Users FOREIGN KEY (UserId) 
        REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE INDEX IX_UserSettings_UserId ON UserSettings(UserId);
```

### GameStates Table

```sql
CREATE TABLE GameStates (
    GameStateId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    BoardState NVARCHAR(MAX) NOT NULL, -- JSON representation of the board
    CurrentScore INT NOT NULL DEFAULT 0,
    CurrentLevel INT NOT NULL DEFAULT 1,
    LinesCleared INT NOT NULL DEFAULT 0,
    NextTetromino NVARCHAR(50) NULL, -- The type of the next tetromino
    SaveDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    GameMode NVARCHAR(50) NOT NULL DEFAULT 'Classic',
    DifficultyLevel NVARCHAR(20) NOT NULL DEFAULT 'Medium',
    
    CONSTRAINT FK_GameStates_Users FOREIGN KEY (UserId) 
        REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE INDEX IX_GameStates_UserId ON GameStates(UserId);
CREATE INDEX IX_GameStates_SaveDate ON GameStates(SaveDate);
```

### GameStatistics Table

```sql
CREATE TABLE GameStatistics (
    StatisticsId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    HighestScore INT NOT NULL DEFAULT 0,
    TotalGamesPlayed INT NOT NULL DEFAULT 0,
    WinCount INT NOT NULL DEFAULT 0,
    LoseCount INT NOT NULL DEFAULT 0,
    TotalTimePlayed BIGINT NOT NULL DEFAULT 0, -- In seconds
    AverageScore FLOAT NOT NULL DEFAULT 0.0,
    TotalLinesCleared INT NOT NULL DEFAULT 0,
    UpdatedAt DATETIME NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT FK_GameStatistics_Users FOREIGN KEY (UserId) 
        REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE INDEX IX_GameStatistics_UserId ON GameStatistics(UserId);
CREATE INDEX IX_GameStatistics_HighestScore ON GameStatistics(HighestScore);
```

### GameHistories Table

```sql
CREATE TABLE GameHistories (
    GameHistoryId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    GameDate DATETIME NOT NULL DEFAULT GETUTCDATE(),
    Duration INT NOT NULL, -- In seconds
    Score INT NOT NULL DEFAULT 0,
    Level INT NOT NULL DEFAULT 1,
    LinesCleared INT NOT NULL DEFAULT 0,
    GameMode NVARCHAR(50) NOT NULL DEFAULT 'Classic',
    DifficultyLevel NVARCHAR(20) NOT NULL DEFAULT 'Medium',
    TetrominosUsed INT NOT NULL DEFAULT 0, -- Total number of tetrominos used in the game
    
    CONSTRAINT FK_GameHistories_Users FOREIGN KEY (UserId) 
        REFERENCES Users(UserId) ON DELETE CASCADE
);

CREATE INDEX IX_GameHistories_UserId ON GameHistories(UserId);
CREATE INDEX IX_GameHistories_GameDate ON GameHistories(GameDate);
CREATE INDEX IX_GameHistories_Score ON GameHistories(Score);
```

### GameHistoryDetails Table

```sql
CREATE TABLE GameHistoryDetails (
    DetailId UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    GameHistoryId UNIQUEIDENTIFIER NOT NULL,
    EventType NVARCHAR(50) NOT NULL, -- e.g., 'LineCleared', 'LevelUp', 'GameOver'
    EventTime DATETIME NOT NULL DEFAULT GETUTCDATE(),
    EventData NVARCHAR(MAX) NULL, -- JSON data specific to the event type
    
    CONSTRAINT FK_GameHistoryDetails_GameHistories FOREIGN KEY (GameHistoryId) 
        REFERENCES GameHistories(GameHistoryId) ON DELETE CASCADE
);

CREATE INDEX IX_GameHistoryDetails_GameHistoryId ON GameHistoryDetails(GameHistoryId);
CREATE INDEX IX_GameHistoryDetails_EventType ON GameHistoryDetails(EventType);
```

## Example Queries

### Get User Statistics

```sql
SELECT 
    u.Username,
    gs.HighestScore,
    gs.TotalGamesPlayed,
    gs.WinCount,
    gs.LoseCount,
    gs.TotalTimePlayed,
    gs.AverageScore,
    gs.TotalLinesCleared
FROM Users u
JOIN GameStatistics gs ON u.UserId = gs.UserId
WHERE u.UserId = @UserId;
```

### Get Top 10 Players by Highest Score

```sql
SELECT TOP 10
    u.Username,
    gs.HighestScore,
    gs.TotalGamesPlayed
FROM Users u
JOIN GameStatistics gs ON u.UserId = gs.UserId
ORDER BY gs.HighestScore DESC;
```

### Get Saved Games for a User

```sql
SELECT 
    GameStateId,
    CurrentScore,
    CurrentLevel,
    LinesCleared,
    SaveDate,
    GameMode,
    DifficultyLevel
FROM GameStates
WHERE UserId = @UserId
ORDER BY SaveDate DESC;
```

### Get Game History for a User

```sql
SELECT 
    gh.GameDate,
    gh.Duration,
    gh.Score,
    gh.Level,
    gh.LinesCleared,
    gh.GameMode,
    gh.DifficultyLevel
FROM GameHistories gh
WHERE gh.UserId = @UserId
ORDER BY gh.GameDate DESC;
```

### Get Game Event Details for a Specific Game

```sql
SELECT 
    ghd.EventType,
    ghd.EventTime,
    ghd.EventData
FROM GameHistoryDetails ghd
JOIN GameHistories gh ON ghd.GameHistoryId = gh.GameHistoryId
WHERE gh.GameHistoryId = @GameHistoryId
ORDER BY ghd.EventTime;
```

### Update User Settings

```sql
UPDATE UserSettings
SET 
    ControlSettings = @ControlSettings,
    SoundEnabled = @SoundEnabled,
    MusicEnabled = @MusicEnabled,
    ColorTheme = @ColorTheme,
    UpdatedAt = GETUTCDATE()
WHERE UserId = @UserId;
```

### Save Game State

```sql
INSERT INTO GameStates (UserId, BoardState, CurrentScore, CurrentLevel, LinesCleared, NextTetromino, GameMode, DifficultyLevel)
VALUES (@UserId, @BoardState, @CurrentScore, @CurrentLevel, @LinesCleared, @NextTetromino, @GameMode, @DifficultyLevel);
```

### Record Game Completion

```sql
-- Insert game history record
DECLARE @GameHistoryId UNIQUEIDENTIFIER = NEWID();

INSERT INTO GameHistories (GameHistoryId, UserId, Duration, Score, Level, LinesCleared, GameMode, DifficultyLevel, TetrominosUsed)
VALUES (@GameHistoryId, @UserId, @Duration, @Score, @Level, @LinesCleared, @GameMode, @DifficultyLevel, @TetrominosUsed);

-- Update user statistics
UPDATE GameStatistics
SET 
    HighestScore = CASE WHEN @Score > HighestScore THEN @Score ELSE HighestScore END,
    TotalGamesPlayed = TotalGamesPlayed + 1,
    WinCount = CASE WHEN @GameWon = 1 THEN WinCount + 1 ELSE WinCount END,
    LoseCount = CASE WHEN @GameWon = 0 THEN LoseCount + 1 ELSE LoseCount END,
    TotalTimePlayed = TotalTimePlayed + @Duration,
    AverageScore = ((AverageScore * TotalGamesPlayed) + @Score) / (TotalGamesPlayed + 1),
    TotalLinesCleared = TotalLinesCleared + @LinesCleared,
    UpdatedAt = GETUTCDATE()
WHERE UserId = @UserId;
```

## Database Indexes and Optimization

The data model includes indexes on frequently queried columns to optimize performance:

1. Indexes on foreign keys for efficient joins
2. Indexes on common search fields like Username and Email
3. Indexes on sorting fields like SaveDate and Score for faster ordering operations

## Data Integrity Rules

1. Users must have unique usernames and email addresses
2. Email addresses must follow a valid format
3. When a user is deleted, all related data (settings, states, statistics, and history) is cascaded
4. Default values are provided for most fields to ensure data consistency
5. Timestamps are used to track creation and update times
6. Foreign key constraints maintain referential integrity

## Considerations for Scaling

1. **Partitioning**: For high-traffic applications, consider partitioning GameHistories and GameHistoryDetails tables by date ranges
2. **Archiving**: Implement an archiving strategy for older game history data
3. **JSON Storage**: BoardState is stored as JSON for flexibility but consider indexing specific properties within JSON if frequently queried
4. **Read/Write Split**: For high-traffic scenarios, consider implementing read replicas
5. **Backup Strategy**: Regular backups with point-in-time recovery capability

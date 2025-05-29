-- Tetris Game Data Model
-- This SQL script defines the database schema for the Tetris web application
-- Based on requirements from feature-requirement-en.md

-- Enable foreign key constraints and other features
PRAGMA foreign_keys = ON;

-- Users table to store user profiles
CREATE TABLE Users (
    UserID INTEGER PRIMARY KEY AUTOINCREMENT,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(128) NOT NULL,
    Email NVARCHAR(100) UNIQUE,
    DateCreated DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    LastLogin DATETIME
);

-- UserSettings table to store user preferences
CREATE TABLE UserSettings (
    SettingID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NOT NULL,
    ControlScheme NVARCHAR(50) DEFAULT 'Default', -- US04-03-01: Customizable controls
    SoundEffectsEnabled BOOLEAN DEFAULT 1,        -- US04-03-02: Sound effects toggle
    MusicEnabled BOOLEAN DEFAULT 1,               -- US04-03-02: Music toggle
    ColorTheme NVARCHAR(50) DEFAULT 'Classic',    -- US04-03-03: Customizable theme
    DateModified DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
);

-- DifficultyLevels table to define different game difficulties
CREATE TABLE DifficultyLevels (
    DifficultyID INTEGER PRIMARY KEY AUTOINCREMENT,
    DifficultyName NVARCHAR(50) NOT NULL UNIQUE,  -- US03-01-01: Selectable difficulty
    InitialSpeed INTEGER NOT NULL,                -- US03-01-02: Speed based on difficulty
    SpeedIncrement FLOAT NOT NULL,                -- US01-03-02: Increasing speed
    ScoreMultiplier FLOAT NOT NULL,               -- US03-01-02: Scoring affected by difficulty
    Description NVARCHAR(200)
);

-- GameModes table to define different game modes
CREATE TABLE GameModes (
    ModeID INTEGER PRIMARY KEY AUTOINCREMENT,
    ModeName NVARCHAR(50) NOT NULL UNIQUE,        -- US03-02-01/02/03: Different game modes
    Description NVARCHAR(200),
    TimeLimit INTEGER NULL,                       -- US03-02-02: Time-based mode
    RowClearTarget INTEGER NULL                   -- US03-02-03: Challenge mode
);

-- SavedGames table to store game states
CREATE TABLE SavedGames (
    SaveID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NOT NULL,
    GameName NVARCHAR(100) NOT NULL,              -- US04-01-01: Save game state
    DifficultyID INTEGER NOT NULL,
    ModeID INTEGER NOT NULL,
    BoardState NVARCHAR(MAX) NOT NULL,            -- Serialized board state
    CurrentScore INTEGER NOT NULL DEFAULT 0,      -- US02-02-01: Current score
    LinesCleared INTEGER NOT NULL DEFAULT 0,      -- US02-02-03: Lines cleared
    NextPiece NVARCHAR(10) NOT NULL,              -- US01-02-03: Next piece preview
    DateSaved DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (DifficultyID) REFERENCES DifficultyLevels(DifficultyID),
    FOREIGN KEY (ModeID) REFERENCES GameModes(ModeID)
);

-- GameHistory table to store completed game results
CREATE TABLE GameHistory (
    GameID INTEGER PRIMARY KEY AUTOINCREMENT,
    UserID INTEGER NOT NULL,
    DifficultyID INTEGER NOT NULL,
    ModeID INTEGER NOT NULL,
    FinalScore INTEGER NOT NULL,                  -- US01-05-02: Points for clearing rows
    LinesCleared INTEGER NOT NULL,
    SingleLinesCleared INTEGER NOT NULL DEFAULT 0,
    DoubleLinesCleared INTEGER NOT NULL DEFAULT 0,
    TripleLinesCleared INTEGER NOT NULL DEFAULT 0,
    TetrisCleared INTEGER NOT NULL DEFAULT 0,     -- US01-05-03: Multiple line clears
    GameDuration INTEGER NOT NULL,                -- Duration in seconds
    DatePlayed DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE,
    FOREIGN KEY (DifficultyID) REFERENCES DifficultyLevels(DifficultyID),
    FOREIGN KEY (ModeID) REFERENCES GameModes(ModeID)
);

-- UserStatistics view to aggregate player statistics
CREATE VIEW UserStatistics AS
SELECT 
    u.UserID,
    u.Username,
    COUNT(gh.GameID) AS GamesPlayed,
    MAX(gh.FinalScore) AS HighestScore,          -- US04-02-01: Highest score
    AVG(gh.FinalScore) AS AverageScore,          -- US04-02-01: Average score
    SUM(gh.LinesCleared) AS TotalLinesCleared,
    SUM(gh.SingleLinesCleared) AS TotalSingleLines,
    SUM(gh.DoubleLinesCleared) AS TotalDoubleLines,
    SUM(gh.TripleLinesCleared) AS TotalTripleLines,
    SUM(gh.TetrisCleared) AS TotalTetrises,
    AVG(gh.GameDuration) AS AverageDuration
FROM 
    Users u
LEFT JOIN 
    GameHistory gh ON u.UserID = gh.UserID
GROUP BY 
    u.UserID, u.Username;

-- Initial data for DifficultyLevels
INSERT INTO DifficultyLevels (DifficultyName, InitialSpeed, SpeedIncrement, ScoreMultiplier, Description)
VALUES 
    ('Easy', 1000, 0.05, 1.0, 'Slow falling speed with normal scoring'),
    ('Medium', 750, 0.1, 1.5, 'Moderate falling speed with bonus scoring'),
    ('Hard', 500, 0.15, 2.0, 'Fast falling speed with double scoring');

-- Initial data for GameModes
INSERT INTO GameModes (ModeName, Description, TimeLimit, RowClearTarget)
VALUES 
    ('Classic', 'Standard Tetris gameplay', NULL, NULL),
    ('Timed', 'Score as many points as possible in a limited time', 180, NULL),  -- 3 minutes
    ('Challenge', 'Clear a specific number of rows as fast as possible', NULL, 40);

-- Create index for performance optimization
CREATE INDEX idx_game_history_user ON GameHistory(UserID);
CREATE INDEX idx_saved_games_user ON SavedGames(UserID);

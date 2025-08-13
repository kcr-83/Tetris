-- ============================================================================
-- Tetris Web Application - Database Schema
-- ============================================================================
-- This script creates the database schema for the Tetris web application
-- supporting user management, game sessions, statistics, and settings.
-- Based on feature requirements from feature-requirement.md
-- ============================================================================

-- Create database (uncomment if needed)
-- CREATE DATABASE TetrisWeb;
-- USE TetrisWeb;

-- ============================================================================
-- USERS AND AUTHENTICATION
-- ============================================================================

-- Users table for authentication and user management
-- Supports: EP05 User Management (US05-01-01, US05-01-02, US05-01-03)
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(255) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Salt NVARCHAR(255) NOT NULL,
    DisplayName NVARCHAR(100) NULL,
    IsGuest BIT NOT NULL DEFAULT 0,
    IsActive BIT NOT NULL DEFAULT 1,
    EmailConfirmed BIT NOT NULL DEFAULT 0,
    LastLoginAt DATETIME2 NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    -- Indexes for performance
    INDEX IX_Users_Username (Username),
    INDEX IX_Users_Email (Email),
    INDEX IX_Users_IsActive (IsActive),
    INDEX IX_Users_LastLoginAt (LastLoginAt)
);

-- User sessions for tracking login sessions and SignalR connections
-- Supports: SignalR connection management (US02-02-03)
CREATE TABLE UserSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    SessionToken NVARCHAR(255) NOT NULL UNIQUE,
    SignalRConnectionId NVARCHAR(255) NULL,
    IpAddress NVARCHAR(45) NULL,
    UserAgent NVARCHAR(500) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastActivityAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    
    -- Indexes for performance
    INDEX IX_UserSessions_UserId (UserId),
    INDEX IX_UserSessions_SessionToken (SessionToken),
    INDEX IX_UserSessions_SignalRConnectionId (SignalRConnectionId),
    INDEX IX_UserSessions_IsActive (IsActive),
    INDEX IX_UserSessions_ExpiresAt (ExpiresAt)
);

-- ============================================================================
-- USER SETTINGS AND PREFERENCES
-- ============================================================================

-- User settings for game customization and preferences
-- Supports: EP06 Settings and Customization (US06-01-01 through US06-03-04)
CREATE TABLE UserSettings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    
    -- Game Controls (US06-01-01)
    KeyMoveLeft NVARCHAR(20) NOT NULL DEFAULT 'ArrowLeft',
    KeyMoveRight NVARCHAR(20) NOT NULL DEFAULT 'ArrowRight',
    KeyRotateClockwise NVARCHAR(20) NOT NULL DEFAULT 'ArrowUp',
    KeyRotateCounterClockwise NVARCHAR(20) NOT NULL DEFAULT 'z',
    KeySoftDrop NVARCHAR(20) NOT NULL DEFAULT 'ArrowDown',
    KeyHardDrop NVARCHAR(20) NOT NULL DEFAULT 'Space',
    KeyHold NVARCHAR(20) NOT NULL DEFAULT 'c',
    KeyPause NVARCHAR(20) NOT NULL DEFAULT 'p',
    
    -- Game Settings (US06-01-02, US06-01-03)
    DefaultDifficulty NVARCHAR(20) NOT NULL DEFAULT 'Medium',
    AutoSaveEnabled BIT NOT NULL DEFAULT 1,
    ShowGhostPiece BIT NOT NULL DEFAULT 1,
    ShowGridLines BIT NOT NULL DEFAULT 1,
    EnableAnimations BIT NOT NULL DEFAULT 1,
    EnableParticleEffects BIT NOT NULL DEFAULT 1,
    
    -- Audio Settings (US06-01-04)
    SoundEffectsEnabled BIT NOT NULL DEFAULT 1,
    MusicEnabled BIT NOT NULL DEFAULT 1,
    SoundEffectsVolume DECIMAL(3,2) NOT NULL DEFAULT 0.70,
    MusicVolume DECIMAL(3,2) NOT NULL DEFAULT 0.50,
    
    -- Visual Customization (US06-02-01 through US06-02-04)
    ThemeName NVARCHAR(50) NOT NULL DEFAULT 'Classic',
    BoardWidth INT NOT NULL DEFAULT 10,
    BoardHeight INT NOT NULL DEFAULT 20,
    CellSize INT NOT NULL DEFAULT 30,
    TetrominoColorScheme NVARCHAR(50) NOT NULL DEFAULT 'Classic',
    
    -- Accessibility (US06-03-01, US06-03-02)
    HighContrastMode BIT NOT NULL DEFAULT 0,
    KeyRepeatDelay INT NOT NULL DEFAULT 100,
    KeyRepeatRate INT NOT NULL DEFAULT 50,
    ScreenReaderEnabled BIT NOT NULL DEFAULT 0,
    
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    
    -- Ensure one settings record per user
    UNIQUE (UserId),
    
    -- Indexes for performance
    INDEX IX_UserSettings_UserId (UserId)
);

-- ============================================================================
-- GAME SESSIONS AND STATE
-- ============================================================================

-- Game sessions for active and saved games
-- Supports: EP05 Game State Persistence (US05-02-01 through US05-02-04)
CREATE TABLE GameSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    
    -- Game Metadata
    GameMode NVARCHAR(20) NOT NULL, -- Classic, Timed, Challenge
    DifficultyLevel NVARCHAR(20) NOT NULL, -- Easy, Medium, Hard
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active', -- Active, Paused, Completed, Abandoned
    
    -- Game State (serialized from Tetris.Core models)
    BoardState NVARCHAR(MAX) NOT NULL, -- JSON serialized board state
    CurrentPieceState NVARCHAR(MAX) NULL, -- JSON serialized current piece
    NextPieceState NVARCHAR(MAX) NULL, -- JSON serialized next piece
    HeldPieceState NVARCHAR(MAX) NULL, -- JSON serialized held piece
    
    -- Game Statistics
    Score BIGINT NOT NULL DEFAULT 0,
    Level INT NOT NULL DEFAULT 1,
    LinesCleared INT NOT NULL DEFAULT 0,
    PiecesPlaced INT NOT NULL DEFAULT 0,
    SingleClears INT NOT NULL DEFAULT 0,
    DoubleClears INT NOT NULL DEFAULT 0,
    TripleClears INT NOT NULL DEFAULT 0,
    TetrisClears INT NOT NULL DEFAULT 0,
    
    -- Time Tracking
    GameStartedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    GameEndedAt DATETIME2 NULL,
    PlayTimeSeconds INT NOT NULL DEFAULT 0,
    PausedTimeSeconds INT NOT NULL DEFAULT 0,
    
    -- Challenge Mode Specific (US04-03-01, US04-03-02)
    ChallengeType NVARCHAR(50) NULL,
    ChallengeGoal INT NULL,
    ChallengeProgress INT NULL DEFAULT 0,
    
    -- Timed Mode Specific (US04-02-01, US04-02-02)
    TimeLimitSeconds INT NULL,
    TimeRemainingSeconds INT NULL,
    
    -- Auto-save support (US05-02-02)
    AutoSaved BIT NOT NULL DEFAULT 0,
    LastSavedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    
    -- Indexes for performance
    INDEX IX_GameSessions_UserId (UserId),
    INDEX IX_GameSessions_Status (Status),
    INDEX IX_GameSessions_GameMode (GameMode),
    INDEX IX_GameSessions_Score (Score DESC),
    INDEX IX_GameSessions_CreatedAt (CreatedAt DESC),
    INDEX IX_GameSessions_GameEndedAt (GameEndedAt DESC)
);

-- ============================================================================
-- STATISTICS AND ACHIEVEMENTS
-- ============================================================================

-- User statistics aggregated across all games
-- Supports: EP05 Statistics and Leaderboards (US05-03-01, US05-03-02)
CREATE TABLE UserStatistics (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    
    -- Overall Statistics
    TotalGamesPlayed INT NOT NULL DEFAULT 0,
    TotalGamesCompleted INT NOT NULL DEFAULT 0,
    TotalPlayTimeSeconds BIGINT NOT NULL DEFAULT 0,
    
    -- Scoring Statistics
    HighScore BIGINT NOT NULL DEFAULT 0,
    TotalScore BIGINT NOT NULL DEFAULT 0,
    AverageScore DECIMAL(10,2) NOT NULL DEFAULT 0,
    
    -- Line Clearing Statistics
    TotalLinesCleared INT NOT NULL DEFAULT 0,
    TotalSingleClears INT NOT NULL DEFAULT 0,
    TotalDoubleClears INT NOT NULL DEFAULT 0,
    TotalTripleClears INT NOT NULL DEFAULT 0,
    TotalTetrisClears INT NOT NULL DEFAULT 0,
    
    -- Piece Statistics
    TotalPiecesPlaced INT NOT NULL DEFAULT 0,
    PiecesPerMinute DECIMAL(5,2) NOT NULL DEFAULT 0,
    
    -- Level Statistics
    HighestLevelReached INT NOT NULL DEFAULT 1,
    AverageLevelReached DECIMAL(5,2) NOT NULL DEFAULT 1,
    
    -- Mode-specific Statistics
    ClassicModeGamesPlayed INT NOT NULL DEFAULT 0,
    TimedModeGamesPlayed INT NOT NULL DEFAULT 0,
    ChallengeModeGamesPlayed INT NOT NULL DEFAULT 0,
    
    -- Best Game Records
    BestGameScore BIGINT NOT NULL DEFAULT 0,
    BestGameLevel INT NOT NULL DEFAULT 1,
    BestGameLines INT NOT NULL DEFAULT 0,
    BestGameDuration INT NOT NULL DEFAULT 0,
    
    -- Streak Statistics
    CurrentWinStreak INT NOT NULL DEFAULT 0,
    LongestWinStreak INT NOT NULL DEFAULT 0,
    
    LastUpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    
    -- Ensure one statistics record per user
    UNIQUE (UserId),
    
    -- Indexes for leaderboards
    INDEX IX_UserStatistics_HighScore (HighScore DESC),
    INDEX IX_UserStatistics_TotalScore (TotalScore DESC),
    INDEX IX_UserStatistics_HighestLevel (HighestLevelReached DESC),
    INDEX IX_UserStatistics_TotalLines (TotalLinesCleared DESC),
    INDEX IX_UserStatistics_PiecesPerMinute (PiecesPerMinute DESC)
);

-- Daily leaderboards for competitive play
-- Supports: EP05 Statistics and Leaderboards (US05-03-03, US05-03-04)
CREATE TABLE DailyLeaderboards (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    GameSessionId UNIQUEIDENTIFIER NOT NULL,
    
    LeaderboardDate DATE NOT NULL,
    GameMode NVARCHAR(20) NOT NULL,
    
    -- Leaderboard Metrics
    BestScore BIGINT NOT NULL,
    BestLevel INT NOT NULL,
    BestLines INT NOT NULL,
    TotalGamesPlayed INT NOT NULL DEFAULT 1,
    
    -- Ranking (calculated by triggers or stored procedures)
    ScoreRank INT NULL,
    LevelRank INT NULL,
    LinesRank INT NULL,
    
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (GameSessionId) REFERENCES GameSessions(Id) ON DELETE CASCADE,
    
    -- Ensure one record per user per day per game mode
    UNIQUE (UserId, LeaderboardDate, GameMode),
    
    -- Indexes for leaderboard queries
    INDEX IX_DailyLeaderboards_Date_Mode (LeaderboardDate, GameMode),
    INDEX IX_DailyLeaderboards_Score (LeaderboardDate, GameMode, BestScore DESC),
    INDEX IX_DailyLeaderboards_Level (LeaderboardDate, GameMode, BestLevel DESC),
    INDEX IX_DailyLeaderboards_Lines (LeaderboardDate, GameMode, BestLines DESC)
);

-- ============================================================================
-- ACHIEVEMENTS AND CHALLENGES
-- ============================================================================

-- Achievement definitions
-- Supports: Challenge Mode achievements (US04-03-03, US04-03-04)
CREATE TABLE Achievements (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL UNIQUE,
    Description NVARCHAR(500) NOT NULL,
    Category NVARCHAR(50) NOT NULL, -- Scoring, Lines, Speed, Special
    
    -- Achievement Criteria
    RequirementType NVARCHAR(50) NOT NULL, -- Score, Lines, Level, Games, Streak
    RequirementValue INT NOT NULL,
    RequirementGameMode NVARCHAR(20) NULL, -- NULL = any mode
    
    -- Achievement Properties
    Points INT NOT NULL DEFAULT 0,
    IsSecret BIT NOT NULL DEFAULT 0,
    IconUrl NVARCHAR(255) NULL,
    BadgeColor NVARCHAR(20) NULL,
    
    -- Unlock Requirements
    PrerequisiteAchievementId UNIQUEIDENTIFIER NULL,
    
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (PrerequisiteAchievementId) REFERENCES Achievements(Id),
    
    -- Indexes
    INDEX IX_Achievements_Category (Category),
    INDEX IX_Achievements_RequirementType (RequirementType)
);

-- User achievements tracking
CREATE TABLE UserAchievements (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NOT NULL,
    AchievementId UNIQUEIDENTIFIER NOT NULL,
    GameSessionId UNIQUEIDENTIFIER NULL, -- Game where achievement was earned
    
    Progress INT NOT NULL DEFAULT 0,
    IsCompleted BIT NOT NULL DEFAULT 0,
    CompletedAt DATETIME2 NULL,
    NotifiedAt DATETIME2 NULL,
    
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE,
    FOREIGN KEY (AchievementId) REFERENCES Achievements(Id) ON DELETE CASCADE,
    FOREIGN KEY (GameSessionId) REFERENCES GameSessions(Id),
    
    -- Ensure one record per user per achievement
    UNIQUE (UserId, AchievementId),
    
    -- Indexes
    INDEX IX_UserAchievements_UserId (UserId),
    INDEX IX_UserAchievements_Completed (IsCompleted, CompletedAt),
    INDEX IX_UserAchievements_Progress (Progress DESC)
);

-- ============================================================================
-- SYSTEM TABLES
-- ============================================================================

-- Application logs for monitoring and debugging
-- Supports: Performance monitoring (US07-02-04)
CREATE TABLE ApplicationLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    UserId UNIQUEIDENTIFIER NULL,
    SessionId UNIQUEIDENTIFIER NULL,
    
    LogLevel NVARCHAR(20) NOT NULL, -- Debug, Info, Warning, Error, Critical
    Category NVARCHAR(100) NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    Exception NVARCHAR(MAX) NULL,
    
    -- Context Information
    RequestPath NVARCHAR(500) NULL,
    UserAgent NVARCHAR(500) NULL,
    IpAddress NVARCHAR(45) NULL,
    
    -- Performance Metrics
    ResponseTimeMs INT NULL,
    MemoryUsageMb DECIMAL(10,2) NULL,
    
    Timestamp DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (SessionId) REFERENCES UserSessions(Id),
    
    -- Indexes for log queries
    INDEX IX_ApplicationLogs_Timestamp (Timestamp DESC),
    INDEX IX_ApplicationLogs_Level (LogLevel, Timestamp DESC),
    INDEX IX_ApplicationLogs_Category (Category, Timestamp DESC),
    INDEX IX_ApplicationLogs_UserId (UserId, Timestamp DESC)
);

-- System configuration settings
CREATE TABLE SystemSettings (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SettingKey NVARCHAR(100) NOT NULL UNIQUE,
    SettingValue NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(500) NULL,
    IsSystemLevel BIT NOT NULL DEFAULT 1,
    IsPublic BIT NOT NULL DEFAULT 0,
    
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    
    -- Indexes
    INDEX IX_SystemSettings_Key (SettingKey),
    INDEX IX_SystemSettings_IsPublic (IsPublic)
);

-- ============================================================================
-- VIEWS FOR COMMON QUERIES
-- ============================================================================

-- User profile view with statistics
CREATE VIEW vw_UserProfiles AS
SELECT 
    u.Id,
    u.Username,
    u.DisplayName,
    u.IsGuest,
    u.CreatedAt,
    u.LastLoginAt,
    
    -- Statistics
    ISNULL(s.TotalGamesPlayed, 0) AS TotalGamesPlayed,
    ISNULL(s.HighScore, 0) AS HighScore,
    ISNULL(s.TotalLinesCleared, 0) AS TotalLinesCleared,
    ISNULL(s.HighestLevelReached, 1) AS HighestLevelReached,
    ISNULL(s.TotalPlayTimeSeconds, 0) AS TotalPlayTimeSeconds,
    
    -- Rankings (calculated)
    ROW_NUMBER() OVER (ORDER BY ISNULL(s.HighScore, 0) DESC) AS ScoreRank,
    ROW_NUMBER() OVER (ORDER BY ISNULL(s.TotalLinesCleared, 0) DESC) AS LinesRank,
    ROW_NUMBER() OVER (ORDER BY ISNULL(s.HighestLevelReached, 1) DESC) AS LevelRank
    
FROM Users u
LEFT JOIN UserStatistics s ON u.Id = s.UserId
WHERE u.IsActive = 1 AND u.IsGuest = 0;

-- Active game sessions view
CREATE VIEW vw_ActiveGameSessions AS
SELECT 
    gs.Id,
    gs.UserId,
    u.Username,
    gs.GameMode,
    gs.DifficultyLevel,
    gs.Status,
    gs.Score,
    gs.Level,
    gs.LinesCleared,
    gs.PlayTimeSeconds,
    gs.GameStartedAt,
    gs.LastSavedAt,
    
    -- Calculate current session duration
    DATEDIFF(SECOND, gs.GameStartedAt, GETUTCDATE()) - gs.PausedTimeSeconds AS CurrentSessionSeconds
    
FROM GameSessions gs
INNER JOIN Users u ON gs.UserId = u.Id
WHERE gs.Status IN ('Active', 'Paused')
  AND u.IsActive = 1;

-- Daily leaderboard view
CREATE VIEW vw_TodayLeaderboard AS
SELECT 
    dl.UserId,
    u.Username,
    u.DisplayName,
    dl.GameMode,
    dl.BestScore,
    dl.BestLevel,
    dl.BestLines,
    dl.TotalGamesPlayed,
    
    -- Rankings
    ROW_NUMBER() OVER (PARTITION BY dl.GameMode ORDER BY dl.BestScore DESC) AS ScoreRank,
    ROW_NUMBER() OVER (PARTITION BY dl.GameMode ORDER BY dl.BestLevel DESC) AS LevelRank,
    ROW_NUMBER() OVER (PARTITION BY dl.GameMode ORDER BY dl.BestLines DESC) AS LinesRank
    
FROM DailyLeaderboards dl
INNER JOIN Users u ON dl.UserId = u.Id
WHERE dl.LeaderboardDate = CAST(GETUTCDATE() AS DATE)
  AND u.IsActive = 1
  AND u.IsGuest = 0;

-- ============================================================================
-- SAMPLE DATA INSERTS
-- ============================================================================

-- Insert default system settings
INSERT INTO SystemSettings (SettingKey, SettingValue, Description, IsPublic) VALUES
('MaxConcurrentGames', '1000', 'Maximum number of concurrent game sessions', 1),
('SessionTimeoutMinutes', '30', 'User session timeout in minutes', 1),
('LeaderboardUpdateIntervalMinutes', '5', 'How often to update leaderboard rankings', 0),
('DefaultGameMode', 'Classic', 'Default game mode for new users', 1),
('EnableGuestPlay', 'true', 'Allow guest users to play without registration', 1),
('MaintenanceMode', 'false', 'System maintenance mode flag', 1);

-- Insert sample achievements
INSERT INTO Achievements (Name, Description, Category, RequirementType, RequirementValue, Points) VALUES
('First Steps', 'Complete your first game', 'General', 'Games', 1, 10),
('Line Clearer', 'Clear 100 lines total', 'Lines', 'Lines', 100, 25),
('Tetris Master', 'Clear 4 lines at once (Tetris)', 'Lines', 'TetrisClears', 1, 50),
('Speed Demon', 'Reach level 10', 'Level', 'Level', 10, 75),
('High Scorer', 'Score 100,000 points in a single game', 'Scoring', 'Score', 100000, 100),
('Marathon Runner', 'Play for 1 hour total', 'General', 'PlayTime', 3600, 30),
('Persistent Player', 'Play 50 games', 'General', 'Games', 50, 40),
('Line Legend', 'Clear 1000 lines total', 'Lines', 'Lines', 1000, 150);

-- ============================================================================
-- STORED PROCEDURES AND FUNCTIONS
-- ============================================================================

-- Procedure to update user statistics after game completion
CREATE PROCEDURE sp_UpdateUserStatistics
    @UserId UNIQUEIDENTIFIER,
    @GameSessionId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Score BIGINT, @Level INT, @Lines INT, @PlayTime INT;
    DECLARE @Singles INT, @Doubles INT, @Triples INT, @Tetrises INT;
    DECLARE @GameMode NVARCHAR(20);
    
    -- Get game session data
    SELECT 
        @Score = Score,
        @Level = Level,
        @Lines = LinesCleared,
        @PlayTime = PlayTimeSeconds,
        @Singles = SingleClears,
        @Doubles = DoubleClears,
        @Triples = TripleClears,
        @Tetrises = TetrisClears,
        @GameMode = GameMode
    FROM GameSessions 
    WHERE Id = @GameSessionId AND UserId = @UserId;
    
    -- Update or insert user statistics
    MERGE UserStatistics AS target
    USING (SELECT @UserId AS UserId) AS source
    ON target.UserId = source.UserId
    WHEN MATCHED THEN
        UPDATE SET
            TotalGamesPlayed = TotalGamesPlayed + 1,
            TotalGamesCompleted = TotalGamesCompleted + 1,
            TotalPlayTimeSeconds = TotalPlayTimeSeconds + @PlayTime,
            HighScore = CASE WHEN @Score > HighScore THEN @Score ELSE HighScore END,
            TotalScore = TotalScore + @Score,
            AverageScore = (TotalScore + @Score) / (TotalGamesCompleted + 1.0),
            TotalLinesCleared = TotalLinesCleared + @Lines,
            TotalSingleClears = TotalSingleClears + @Singles,
            TotalDoubleClears = TotalDoubleClears + @Doubles,
            TotalTripleClears = TotalTripleClears + @Triples,
            TotalTetrisClears = TotalTetrisClears + @Tetrises,
            HighestLevelReached = CASE WHEN @Level > HighestLevelReached THEN @Level ELSE HighestLevelReached END,
            BestGameScore = CASE WHEN @Score > BestGameScore THEN @Score ELSE BestGameScore END,
            BestGameLevel = CASE WHEN @Level > BestGameLevel THEN @Level ELSE BestGameLevel END,
            BestGameLines = CASE WHEN @Lines > BestGameLines THEN @Lines ELSE BestGameLines END,
            BestGameDuration = CASE WHEN @PlayTime > BestGameDuration THEN @PlayTime ELSE BestGameDuration END,
            ClassicModeGamesPlayed = ClassicModeGamesPlayed + CASE WHEN @GameMode = 'Classic' THEN 1 ELSE 0 END,
            TimedModeGamesPlayed = TimedModeGamesPlayed + CASE WHEN @GameMode = 'Timed' THEN 1 ELSE 0 END,
            ChallengeModeGamesPlayed = ChallengeModeGamesPlayed + CASE WHEN @GameMode = 'Challenge' THEN 1 ELSE 0 END,
            LastUpdatedAt = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT (
            UserId, TotalGamesPlayed, TotalGamesCompleted, TotalPlayTimeSeconds,
            HighScore, TotalScore, AverageScore, TotalLinesCleared,
            TotalSingleClears, TotalDoubleClears, TotalTripleClears, TotalTetrisClears,
            HighestLevelReached, BestGameScore, BestGameLevel, BestGameLines, BestGameDuration,
            ClassicModeGamesPlayed, TimedModeGamesPlayed, ChallengeModeGamesPlayed
        )
        VALUES (
            @UserId, 1, 1, @PlayTime,
            @Score, @Score, @Score, @Lines,
            @Singles, @Doubles, @Triples, @Tetrises,
            @Level, @Score, @Level, @Lines, @PlayTime,
            CASE WHEN @GameMode = 'Classic' THEN 1 ELSE 0 END,
            CASE WHEN @GameMode = 'Timed' THEN 1 ELSE 0 END,
            CASE WHEN @GameMode = 'Challenge' THEN 1 ELSE 0 END
        );
    
    -- Update daily leaderboard
    EXEC sp_UpdateDailyLeaderboard @UserId, @GameSessionId;
END;

-- Procedure to update daily leaderboard
CREATE PROCEDURE sp_UpdateDailyLeaderboard
    @UserId UNIQUEIDENTIFIER,
    @GameSessionId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Today DATE = CAST(GETUTCDATE() AS DATE);
    DECLARE @Score BIGINT, @Level INT, @Lines INT, @GameMode NVARCHAR(20);
    
    -- Get game session data
    SELECT @Score = Score, @Level = Level, @Lines = LinesCleared, @GameMode = GameMode
    FROM GameSessions 
    WHERE Id = @GameSessionId AND UserId = @UserId;
    
    -- Update or insert daily leaderboard entry
    MERGE DailyLeaderboards AS target
    USING (SELECT @UserId AS UserId, @Today AS LeaderboardDate, @GameMode AS GameMode) AS source
    ON target.UserId = source.UserId 
       AND target.LeaderboardDate = source.LeaderboardDate
       AND target.GameMode = source.GameMode
    WHEN MATCHED THEN
        UPDATE SET
            BestScore = CASE WHEN @Score > BestScore THEN @Score ELSE BestScore END,
            BestLevel = CASE WHEN @Level > BestLevel THEN @Level ELSE BestLevel END,
            BestLines = CASE WHEN @Lines > BestLines THEN @Lines ELSE BestLines END,
            TotalGamesPlayed = TotalGamesPlayed + 1,
            GameSessionId = CASE WHEN @Score > BestScore THEN @GameSessionId ELSE GameSessionId END,
            UpdatedAt = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT (UserId, GameSessionId, LeaderboardDate, GameMode, BestScore, BestLevel, BestLines)
        VALUES (@UserId, @GameSessionId, @Today, @GameMode, @Score, @Level, @Lines);
END;

-- Function to get user rank by score
CREATE FUNCTION fn_GetUserScoreRank(@UserId UNIQUEIDENTIFIER)
RETURNS INT
AS
BEGIN
    DECLARE @Rank INT;
    
    SELECT @Rank = Rank
    FROM (
        SELECT UserId, ROW_NUMBER() OVER (ORDER BY HighScore DESC) AS Rank
        FROM UserStatistics
        WHERE HighScore > 0
    ) ranked
    WHERE UserId = @UserId;
    
    RETURN ISNULL(@Rank, 0);
END;

-- ============================================================================
-- INDEXES FOR PERFORMANCE OPTIMIZATION
-- ============================================================================

-- Additional composite indexes for common query patterns
CREATE INDEX IX_GameSessions_UserMode_Score ON GameSessions (UserId, GameMode, Score DESC);
CREATE INDEX IX_GameSessions_Status_Updated ON GameSessions (Status, UpdatedAt DESC);
CREATE INDEX IX_UserSessions_User_Active ON UserSessions (UserId, IsActive, LastActivityAt DESC);
CREATE INDEX IX_ApplicationLogs_Level_Time ON ApplicationLogs (LogLevel, Timestamp DESC);

-- ============================================================================
-- TRIGGERS FOR DATA INTEGRITY
-- ============================================================================

-- Trigger to update timestamps on UserSettings changes
CREATE TRIGGER tr_UserSettings_UpdateTimestamp
ON UserSettings
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE UserSettings 
    SET UpdatedAt = GETUTCDATE()
    FROM UserSettings us
    INNER JOIN inserted i ON us.Id = i.Id;
END;

-- Trigger to clean up expired sessions
CREATE TRIGGER tr_UserSessions_CleanupExpired
ON UserSessions
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE UserSessions 
    SET IsActive = 0
    WHERE ExpiresAt < GETUTCDATE() AND IsActive = 1;
END;

-- ============================================================================
-- SECURITY AND CONSTRAINTS
-- ============================================================================

-- Add check constraints for data validation
ALTER TABLE UserSettings ADD CONSTRAINT CK_UserSettings_Volume_Range 
    CHECK (SoundEffectsVolume BETWEEN 0 AND 1 AND MusicVolume BETWEEN 0 AND 1);

ALTER TABLE GameSessions ADD CONSTRAINT CK_GameSessions_Score_NonNegative 
    CHECK (Score >= 0);

ALTER TABLE GameSessions ADD CONSTRAINT CK_GameSessions_Level_Positive 
    CHECK (Level > 0);

ALTER TABLE UserStatistics ADD CONSTRAINT CK_UserStatistics_NonNegative 
    CHECK (TotalGamesPlayed >= 0 AND HighScore >= 0 AND TotalScore >= 0);

-- ============================================================================
-- COMMENTS FOR DOCUMENTATION
-- ============================================================================

EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Main users table supporting both registered and guest users for the Tetris web application',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'Users';

EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Stores active and saved game sessions with complete game state for resume functionality',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'GameSessions';

EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'User settings and preferences including controls, audio, visual, and accessibility options',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'UserSettings';

EXEC sp_addextendedproperty 
    @name = N'MS_Description',
    @value = N'Aggregated user statistics across all game sessions for profile and leaderboard display',
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE', @level1name = N'UserStatistics';

-- ============================================================================
-- END OF SCHEMA
-- ============================================================================

PRINT 'Tetris Web Application database schema created successfully.';
PRINT 'Schema includes support for:';
PRINT '- User management and authentication';
PRINT '- Game session persistence and resume functionality';
PRINT '- Comprehensive statistics and leaderboards';
PRINT '- User settings and customization';
PRINT '- Achievement system';
PRINT '- Performance monitoring and logging';
PRINT '';
PRINT 'Total tables created: 11';
PRINT 'Total views created: 3';
PRINT 'Total stored procedures created: 2';
PRINT 'Total functions created: 1';
PRINT 'Total triggers created: 2';

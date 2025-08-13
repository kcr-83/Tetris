# Tetris Web Application - Class Diagrams

## Overview

This document contains UML class diagrams generated from the database schema defined in `data_model.sql`. The diagrams represent the main entities and their relationships in the Tetris web application.

## User Management Domain

### Core User Entities

```mermaid
classDiagram
    class User {
        +Guid Id
        +String Username
        +String Email
        +String PasswordHash
        +String Salt
        +String DisplayName
        +Boolean IsGuest
        +Boolean IsActive
        +Boolean EmailConfirmed
        +DateTime LastLoginAt
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +login()
        +logout()
        +updateProfile()
        +changePassword()
        +confirmEmail()
    }

    class UserSession {
        +Guid Id
        +Guid UserId
        +String SessionToken
        +String SignalRConnectionId
        +String IpAddress
        +String UserAgent
        +Boolean IsActive
        +DateTime ExpiresAt
        +DateTime CreatedAt
        +DateTime LastActivityAt
        +createSession()
        +extendSession()
        +invalidateSession()
        +updateActivity()
    }

    class UserSettings {
        +Guid Id
        +Guid UserId
        +String KeyMoveLeft
        +String KeyMoveRight
        +String KeyRotateClockwise
        +String KeyRotateCounterClockwise
        +String KeySoftDrop
        +String KeyHardDrop
        +String KeyHold
        +String KeyPause
        +String DefaultDifficulty
        +Boolean AutoSaveEnabled
        +Boolean ShowGhostPiece
        +Boolean ShowGridLines
        +Boolean EnableAnimations
        +Boolean EnableParticleEffects
        +Boolean SoundEffectsEnabled
        +Boolean MusicEnabled
        +Decimal SoundEffectsVolume
        +Decimal MusicVolume
        +String ThemeName
        +Integer BoardWidth
        +Integer BoardHeight
        +Integer CellSize
        +String TetrominoColorScheme
        +Boolean HighContrastMode
        +Integer KeyRepeatDelay
        +Integer KeyRepeatRate
        +Boolean ScreenReaderEnabled
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +updateControls()
        +updateAudioSettings()
        +updateVisualSettings()
        +updateAccessibilitySettings()
        +resetToDefaults()
    }

    User ||--o{ UserSession : has
    User ||--|| UserSettings : configures
```

## Game Management Domain

### Game Session and Statistics

```mermaid
classDiagram
    class GameSession {
        +Guid Id
        +Guid UserId
        +String GameMode
        +String DifficultyLevel
        +String Status
        +String BoardState
        +String CurrentPieceState
        +String NextPieceState
        +String HeldPieceState
        +Long Score
        +Integer Level
        +Integer LinesCleared
        +Integer PiecesPlaced
        +Integer SingleClears
        +Integer DoubleClears
        +Integer TripleClears
        +Integer TetrisClears
        +DateTime GameStartedAt
        +DateTime GameEndedAt
        +Integer PlayTimeSeconds
        +Integer PausedTimeSeconds
        +String ChallengeType
        +Integer ChallengeGoal
        +Integer ChallengeProgress
        +Integer TimeLimitSeconds
        +Integer TimeRemainingSeconds
        +Boolean AutoSaved
        +DateTime LastSavedAt
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +startGame()
        +pauseGame()
        +resumeGame()
        +endGame()
        +saveGame()
        +updateScore()
        +clearLines()
        +placePiece()
    }

    class UserStatistics {
        +Guid Id
        +Guid UserId
        +Integer TotalGamesPlayed
        +Integer TotalGamesCompleted
        +Long TotalPlayTimeSeconds
        +Long HighScore
        +Long TotalScore
        +Decimal AverageScore
        +Integer TotalLinesCleared
        +Integer TotalSingleClears
        +Integer TotalDoubleClears
        +Integer TotalTripleClears
        +Integer TotalTetrisClears
        +Integer TotalPiecesPlaced
        +Decimal PiecesPerMinute
        +Integer HighestLevelReached
        +Decimal AverageLevelReached
        +Integer ClassicModeGamesPlayed
        +Integer TimedModeGamesPlayed
        +Integer ChallengeModeGamesPlayed
        +Long BestGameScore
        +Integer BestGameLevel
        +Integer BestGameLines
        +Integer BestGameDuration
        +Integer CurrentWinStreak
        +Integer LongestWinStreak
        +DateTime LastUpdatedAt
        +DateTime CreatedAt
        +updateFromGame()
        +calculateAverages()
        +updateStreaks()
        +resetStatistics()
    }

    class DailyLeaderboard {
        +Guid Id
        +Guid UserId
        +Guid GameSessionId
        +Date LeaderboardDate
        +String GameMode
        +Long BestScore
        +Integer BestLevel
        +Integer BestLines
        +Integer TotalGamesPlayed
        +Integer ScoreRank
        +Integer LevelRank
        +Integer LinesRank
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +updateRankings()
        +calculateRank()
        +getTopPlayers()
    }

    User ||--o{ GameSession : plays
    User ||--|| UserStatistics : tracks
    User ||--o{ DailyLeaderboard : competes
    GameSession ||--o| DailyLeaderboard : generates
```

## Achievement System Domain

### Achievements and User Progress

```mermaid
classDiagram
    class Achievement {
        +Guid Id
        +String Name
        +String Description
        +String Category
        +String RequirementType
        +Integer RequirementValue
        +String RequirementGameMode
        +Integer Points
        +Boolean IsSecret
        +String IconUrl
        +String BadgeColor
        +Guid PrerequisiteAchievementId
        +DateTime CreatedAt
        +checkRequirement()
        +calculateProgress()
        +isUnlocked()
    }

    class UserAchievement {
        +Guid Id
        +Guid UserId
        +Guid AchievementId
        +Guid GameSessionId
        +Integer Progress
        +Boolean IsCompleted
        +DateTime CompletedAt
        +DateTime NotifiedAt
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +updateProgress()
        +complete()
        +notify()
        +calculateProgressPercentage()
    }

    User ||--o{ UserAchievement : earns
    Achievement ||--o{ UserAchievement : tracks
    Achievement ||--o| Achievement : prerequisite
    GameSession ||--o{ UserAchievement : triggers
```

## System and Monitoring Domain

### System Configuration and Logging

```mermaid
classDiagram
    class SystemSettings {
        +Guid Id
        +String SettingKey
        +String SettingValue
        +String Description
        +Boolean IsSystemLevel
        +Boolean IsPublic
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +getValue()
        +setValue()
        +isEditable()
        +getPublicSettings()
    }

    class ApplicationLog {
        +Guid Id
        +Guid UserId
        +Guid SessionId
        +String LogLevel
        +String Category
        +String Message
        +String Exception
        +String RequestPath
        +String UserAgent
        +String IpAddress
        +Integer ResponseTimeMs
        +Decimal MemoryUsageMb
        +DateTime Timestamp
        +logError()
        +logWarning()
        +logInfo()
        +logDebug()
        +getLogsByLevel()
        +getLogsByUser()
    }

    User ||--o{ ApplicationLog : generates
    UserSession ||--o{ ApplicationLog : produces
```

## Complete Domain Model

### Entity Relationships Overview

```mermaid
classDiagram
    class User {
        +Guid Id
        +String Username
        +String Email
        +Boolean IsGuest
        +Boolean IsActive
    }

    class UserSession {
        +Guid Id
        +String SessionToken
        +Boolean IsActive
    }

    class UserSettings {
        +Guid Id
        +String DefaultDifficulty
        +Boolean AutoSaveEnabled
    }

    class GameSession {
        +Guid Id
        +String GameMode
        +String Status
        +Long Score
        +Integer Level
    }

    class UserStatistics {
        +Guid Id
        +Long HighScore
        +Integer TotalGamesPlayed
    }

    class Achievement {
        +Guid Id
        +String Name
        +String Category
        +Integer Points
    }

    class UserAchievement {
        +Guid Id
        +Integer Progress
        +Boolean IsCompleted
    }

    class DailyLeaderboard {
        +Guid Id
        +Date LeaderboardDate
        +Long BestScore
        +Integer ScoreRank
    }

    User ||--o{ UserSession : "1..n"
    User ||--|| UserSettings : "1..1"
    User ||--o{ GameSession : "1..n"
    User ||--|| UserStatistics : "1..1"
    User ||--o{ UserAchievement : "1..n"
    User ||--o{ DailyLeaderboard : "1..n"
    
    Achievement ||--o{ UserAchievement : "1..n"
    GameSession ||--o| DailyLeaderboard : "1..0..1"
    GameSession ||--o{ UserAchievement : "0..1..n"
```

---

## Missing Business Entities

Based on your request for "invoices, change requests and refund requests," it appears you may want to extend this system with additional business functionality. Here are suggested class diagrams for those entities:

### Proposed Invoice Management Domain

```mermaid
classDiagram
    class Invoice {
        +Guid Id
        +Guid UserId
        +String InvoiceNumber
        +DateTime InvoiceDate
        +DateTime DueDate
        +Decimal Amount
        +Decimal TaxAmount
        +Decimal TotalAmount
        +String Currency
        +String Status
        +String Description
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +generateInvoice()
        +markAsPaid()
        +markAsOverdue()
        +sendReminder()
        +calculateTax()
    }

    class InvoiceLineItem {
        +Guid Id
        +Guid InvoiceId
        +String Description
        +Integer Quantity
        +Decimal UnitPrice
        +Decimal LineTotal
        +String ProductCode
        +addLineItem()
        +updateQuantity()
        +calculateTotal()
    }

    class Payment {
        +Guid Id
        +Guid InvoiceId
        +Decimal Amount
        +DateTime PaymentDate
        +String PaymentMethod
        +String TransactionId
        +String Status
        +processPayment()
        +refundPayment()
        +verifyPayment()
    }

    User ||--o{ Invoice : "owns"
    Invoice ||--o{ InvoiceLineItem : "contains"
    Invoice ||--o{ Payment : "receives"
```

### Proposed Change Request Management Domain

```mermaid
classDiagram
    class ChangeRequest {
        +Guid Id
        +Guid RequesterId
        +Guid AssigneeId
        +String Title
        +String Description
        +String Category
        +String Priority
        +String Status
        +DateTime RequestedDate
        +DateTime DueDate
        +DateTime CompletedDate
        +String ImpactAssessment
        +Decimal EstimatedCost
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +submitRequest()
        +approveRequest()
        +rejectRequest()
        +assignTo()
        +updateStatus()
        +addComment()
    }

    class ChangeRequestComment {
        +Guid Id
        +Guid ChangeRequestId
        +Guid UserId
        +String Comment
        +DateTime CreatedAt
        +addComment()
        +editComment()
        +deleteComment()
    }

    class ChangeRequestApproval {
        +Guid Id
        +Guid ChangeRequestId
        +Guid ApproverId
        +String Decision
        +String Comments
        +DateTime ApprovalDate
        +approve()
        +reject()
        +requestMoreInfo()
    }

    User ||--o{ ChangeRequest : "creates"
    User ||--o{ ChangeRequest : "assigned"
    ChangeRequest ||--o{ ChangeRequestComment : "has"
    ChangeRequest ||--o{ ChangeRequestApproval : "requires"
    User ||--o{ ChangeRequestComment : "writes"
    User ||--o{ ChangeRequestApproval : "provides"
```

### Proposed Refund Request Management Domain

```mermaid
classDiagram
    class RefundRequest {
        +Guid Id
        +Guid UserId
        +Guid InvoiceId
        +Guid PaymentId
        +String RefundReason
        +String Description
        +Decimal RequestedAmount
        +Decimal ApprovedAmount
        +String Status
        +DateTime RequestDate
        +DateTime ProcessedDate
        +String ProcessorNotes
        +String RefundMethod
        +String TransactionId
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +submitRefund()
        +approveRefund()
        +rejectRefund()
        +processRefund()
        +calculateRefundAmount()
    }

    class RefundApproval {
        +Guid Id
        +Guid RefundRequestId
        +Guid ApproverId
        +String Decision
        +Decimal ApprovedAmount
        +String ApprovalNotes
        +DateTime ApprovalDate
        +approve()
        +reject()
        +adjustAmount()
    }

    User ||--o{ RefundRequest : "requests"
    Invoice ||--o{ RefundRequest : "refunded"
    Payment ||--o{ RefundRequest : "reversed"
    RefundRequest ||--|| RefundApproval : "requires"
    User ||--o{ RefundApproval : "approves"
```

---

## Notes

1. **Current Schema**: The existing schema focuses on Tetris game functionality and doesn't include invoice, change request, or refund entities.

2. **Proposed Extensions**: The additional class diagrams show how you might extend the system to include business management functionality.

3. **Data Types**: I've used appropriate .NET data types (Guid, String, DateTime, Decimal, etc.) that would map to the SQL schema.

4. **Relationships**: The diagrams show proper cardinality relationships between entities using UML notation.

5. **Methods**: Each class includes relevant business methods that would be implemented in the actual application layer.

If you'd like me to modify the schema to include these business entities or adjust the class diagrams to match a different business domain, please let me know!

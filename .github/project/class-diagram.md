# Class Diagram for Tetris Game

```mermaid
classDiagram
    class User {
        +int UserID
        +String Username
        +String PasswordHash
        +String Email
        +DateTime DateCreated
        +DateTime LastLogin
    }
    
    class UserSettings {
        +int SettingID
        +int UserID
        +String ControlScheme
        +bool SoundEffectsEnabled
        +bool MusicEnabled
        +String ColorTheme
        +DateTime DateModified
    }
    
    class DifficultyLevel {
        +int DifficultyID
        +String DifficultyName
        +int InitialSpeed
        +float SpeedIncrement
        +float ScoreMultiplier
        +String Description
    }
    
    class GameMode {
        +int ModeID
        +String ModeName
        +String Description
        +int TimeLimit
        +int RowClearTarget
    }
    
    class SavedGame {
        +int SaveID
        +int UserID
        +String GameName
        +int DifficultyID
        +int ModeID
        +String BoardState
        +int CurrentScore
        +int LinesCleared
        +String NextPiece
        +DateTime DateSaved
    }
    
    class GameHistory {
        +int GameID
        +int UserID
        +int DifficultyID
        +int ModeID
        +int FinalScore
        +int LinesCleared
        +int SingleLinesCleared
        +int DoubleLinesCleared
        +int TripleLinesCleared
        +int TetrisCleared
        +int GameDuration
        +DateTime DatePlayed
    }
    
    class UserStatistics {
        +int UserID
        +String Username
        +int GamesPlayed
        +int HighestScore
        +float AverageScore
        +int TotalLinesCleared
        +int TotalSingleLines
        +int TotalDoubleLines
        +int TotalTripleLines
        +int TotalTetrises
        +float AverageDuration
    }
    
    User "1" -- "1" UserSettings : has
    User "1" -- "0..*" SavedGame : owns
    User "1" -- "0..*" GameHistory : has
    User "1" -- "0..1" UserStatistics : has
    
    SavedGame "0..*" -- "1" DifficultyLevel : uses
    SavedGame "0..*" -- "1" GameMode : uses
    
    GameHistory "0..*" -- "1" DifficultyLevel : uses
    GameHistory "0..*" -- "1" GameMode : uses
```

## Extended Model with Invoice, Change Request and Refund Request

Since the original data model does not include invoices, change requests, or refund requests, here's an extended model incorporating these entities:

```mermaid
classDiagram
    class User {
        +int UserID
        +String Username
        +String PasswordHash
        +String Email
        +DateTime DateCreated
        +DateTime LastLogin
        +createAccount()
        +login()
        +updateProfile()
    }
    
    class UserSettings {
        +int SettingID
        +int UserID
        +String ControlScheme
        +bool SoundEffectsEnabled
        +bool MusicEnabled
        +String ColorTheme
        +DateTime DateModified
        +updateSettings()
    }
    
    class Subscription {
        +int SubscriptionID
        +int UserID
        +String SubscriptionType
        +DateTime StartDate
        +DateTime ExpiryDate
        +bool IsActive
        +activate()
        +deactivate()
        +renew()
    }
    
    class Invoice {
        +int InvoiceID
        +int UserID
        +int SubscriptionID
        +float Amount
        +String Currency
        +DateTime IssuedDate
        +DateTime DueDate
        +String PaymentStatus
        +String InvoiceNumber
        +generateInvoice()
        +markAsPaid()
        +sendReminder()
    }
    
    class Payment {
        +int PaymentID
        +int InvoiceID
        +int UserID
        +float Amount
        +String PaymentMethod
        +String TransactionID
        +DateTime PaymentDate
        +String Status
        +processPayment()
        +validatePayment()
    }
    
    class ChangeRequest {
        +int RequestID
        +int UserID
        +int SubscriptionID
        +String RequestType
        +String CurrentPlan
        +String RequestedPlan
        +DateTime RequestDate
        +String Status
        +String Comments
        +submitRequest()
        +approve()
        +reject()
    }
    
    class RefundRequest {
        +int RefundID
        +int UserID
        +int PaymentID
        +float RefundAmount
        +String Reason
        +DateTime RequestDate
        +String Status
        +String Comments
        +submitRequest()
        +approve()
        +reject()
        +processRefund()
    }
    
    class GameHistory {
        +int GameID
        +int UserID
        +int DifficultyID
        +int ModeID
        +int FinalScore
        +int LinesCleared
        +DateTime DatePlayed
        +saveResult()
    }
    
    User "1" -- "1" UserSettings : has
    User "1" -- "0..*" GameHistory : plays
    User "1" -- "0..*" Subscription : subscribes to
    User "1" -- "0..*" Invoice : receives
    User "1" -- "0..*" Payment : makes
    User "1" -- "0..*" ChangeRequest : submits
    User "1" -- "0..*" RefundRequest : requests
    
    Subscription "1" -- "0..*" Invoice : generates
    Subscription "1" -- "0..*" ChangeRequest : subject of
    
    Invoice "1" -- "0..*" Payment : paid through
    Payment "1" -- "0..*" RefundRequest : linked to
```

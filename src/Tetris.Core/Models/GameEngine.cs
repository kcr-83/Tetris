using System;
using System.Drawing;
using System.Timers;
using System.Collections.Generic;
using Timer = System.Timers.Timer;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Manages the game state and mechanics of a Tetris game, including the falling of blocks.
    /// </summary>
    public class GameEngine : IDisposable
    {
        #region Constants        /// <summary>
        /// The default initial delay between automatic block movements in milliseconds.
        /// This is overridden by difficulty settings.
        /// </summary>
        private const double DefaultInitialFallDelay = 1000;

        /// <summary>
        /// The default minimum delay between automatic block movements in milliseconds.
        /// This is overridden by difficulty settings.
        /// </summary>
        private const double DefaultMinFallDelay = 100;

        /// <summary>
        /// The default amount of delay reduction per level in milliseconds.
        /// This is overridden by difficulty settings.
        /// </summary>
        private const double DefaultDelayReductionPerLevel = 50;

        /// <summary>
        /// The number of rows that need to be cleared to advance to the next level.
        /// </summary>
        private const int RowsPerLevel = 10;

        /// <summary>
        /// The delay when player is accelerating the fall (Fast Drop) in milliseconds.
        /// </summary>
        private const double FastDropDelay = 50;

        /// <summary>
        /// Base score for clearing a single row.
        /// </summary>
        private const int SingleRowScore = 100;

        /// <summary>
        /// Base score for clearing two rows at once.
        /// </summary>
        private const int DoubleRowScore = 300;

        /// <summary>
        /// Base score for clearing three rows at once.
        /// </summary>
        private const int TripleRowScore = 500;

        /// <summary>
        /// Base score for clearing four rows at once (Tetris).
        /// </summary>
        private const int TetrisScore = 800;

        #endregion

        #region Fields

        private readonly Timer _fallTimer;
        private readonly Timer _gameTimer;  // Timer for Timed mode
        private double _currentFallDelay;
        private bool _isFastDropActive;
        private bool _isGamePaused;
        private bool _isGameOver;
        private bool _isGameWon;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the game board.
        /// </summary>
        public Board Board { get; private set; }

        /// <summary>
        /// Gets the current Tetromino piece that's falling.
        /// </summary>
        public Tetromino CurrentPiece { get; private set; } = null!;

        /// <summary>
        /// Gets the next Tetromino piece to be played.
        /// </summary>
        public Tetromino NextPiece { get; private set; } = null!;

    /// <summary>
    /// Gets the current game level, which affects the falling speed.
    /// </summary>
    public int Level { get; private set; }
    
    /// <summary>
    /// Gets the difficulty level of the game.
    /// </summary>
    public DifficultyLevel Difficulty { get; private set; }
    
    /// <summary>
    /// Gets the current game mode.
    /// </summary>
    public GameMode GameMode { get; private set; }
    
    /// <summary>
    /// Gets the remaining time in seconds for Timed mode.
    /// </summary>
    public int RemainingTimeSeconds { get; private set; }
    
    /// <summary>
    /// Gets the target rows to clear for Challenge mode.
    /// </summary>
    public int TargetRowsToClear { get; private set; }
    
    /// <summary>
    /// Gets the total rows cleared in the current game.
    /// </summary>
    public int TotalRowsCleared { get; private set; }

    /// <summary>
    /// Gets the current game score.
    /// </summary>
    public int Score { get; private set; }

        /// <summary>
        /// Gets the number of single row clears achieved.
        /// </summary>
        public int SingleRowsCleared { get; private set; }

        /// <summary>
        /// Gets the number of double row clears achieved.
        /// </summary>
        public int DoubleRowsCleared { get; private set; }

        /// <summary>
        /// Gets the number of triple row clears achieved.
        /// </summary>
        public int TripleRowsCleared { get; private set; }

        /// <summary>
        /// Gets the number of tetris (4 rows at once) clears achieved.
        /// </summary>
        public int TetrisCleared { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the game is currently paused.
        /// </summary>
        public bool IsPaused => _isGamePaused;

        /// <summary>
        /// Gets a value indicating whether the game is over.
        /// </summary>
        public bool IsGameOver => _isGameOver;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the game board state changes.
        /// </summary>
        public event EventHandler? BoardUpdated;

        /// <summary>
        /// Occurs when the score changes.
        /// </summary>
        public event EventHandler? ScoreUpdated;        /// <summary>
        /// Occurs when the level increases.
        /// </summary>
        public event EventHandler<LevelIncreasedEventArgs>? LevelIncreased;

        /// <summary>
        /// Occurs when rows are cleared.
        /// </summary>
        public event EventHandler<RowsClearedEventArgs>? RowsCleared;

        /// <summary>
        /// Occurs when the game ends, providing game statistics.
        /// </summary>
        public event EventHandler<GameOverEventArgs>? GameOver;
        
        /// <summary>
        /// Occurs when the remaining time changes in Timed mode.
        /// </summary>
        public event EventHandler? RemainingTimeChanged;
        
        /// <summary>
        /// Occurs when the game is won in Challenge mode.
        /// </summary>
        public event EventHandler? GameWon;

        #endregion

        #region Constructor        /// <summary>
        /// Initializes a new instance of the GameEngine class with default medium difficulty.
        /// </summary>
        public GameEngine() : this(DifficultyLevel.Medium)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the GameEngine class with the specified difficulty level.
        /// </summary>
        /// <param name="difficulty">The difficulty level for the game.</param>
        public GameEngine(DifficultyLevel difficulty)
        {
            Board = new Board();
            Difficulty = difficulty;
            GameMode = GameMode.Classic; // Default to classic mode
            
            double initialDelay = DifficultySettings.GetInitialFallDelay(difficulty);
            _fallTimer = new Timer(initialDelay);
            _fallTimer.Elapsed += FallTimer_Elapsed;
            
            _gameTimer = new Timer(1000); // 1 second interval for game timer
            _gameTimer.Elapsed += GameTimer_Elapsed;
            
            _currentFallDelay = initialDelay;
            Level = 1;
            Score = 0;
            SingleRowsCleared = 0;
            DoubleRowsCleared = 0;
            TripleRowsCleared = 0;
            TetrisCleared = 0;
            TotalRowsCleared = 0;
            RemainingTimeSeconds = 0;
            TargetRowsToClear = 0;
            _isFastDropActive = false;
            _isGamePaused = false;
            _isGameOver = false;
            _isGameWon = false;

            // Initialize with random pieces
            CurrentPiece = TetrominoFactory.CreateRandomTetromino();
            NextPiece = TetrominoFactory.CreateRandomTetromino();
        }

        #endregion

        #region Game Control Methods        /// <summary>
        /// Starts a new game with the current difficulty and game mode.
        /// </summary>
        public void StartNewGame()
        {
            StartNewGame(Difficulty, GameMode);
        }
        
        /// <summary>
        /// Starts a new game with a specific difficulty.
        /// </summary>
        /// <param name="difficulty">The difficulty level for the game.</param>
        public void StartNewGame(DifficultyLevel difficulty)
        {
            StartNewGame(difficulty, GameMode);
        }

        /// <summary>
        /// Starts a new game with a specific difficulty and game mode.
        /// </summary>
        /// <param name="difficulty">The difficulty level for the game.</param>
        /// <param name="gameMode">The game mode to use.</param>
        public void StartNewGame(DifficultyLevel difficulty, GameMode gameMode)
        {
            // Update difficulty level and game mode
            Difficulty = difficulty;
            GameMode = gameMode;
            
            // Reset game state
            Board.Clear();
            Level = 1;
            Score = 0;
            SingleRowsCleared = 0;
            DoubleRowsCleared = 0;
            TripleRowsCleared = 0;
            TetrisCleared = 0;
            TotalRowsCleared = 0;
            _isGameOver = false;
            _isGamePaused = false;
            _isFastDropActive = false;
            _isGameWon = false;
            
            // Stop all timers
            _fallTimer.Stop();
            _gameTimer.Stop();
            
            // Set initial fall delay based on difficulty
            _currentFallDelay = DifficultySettings.GetInitialFallDelay(difficulty);
            _fallTimer.Interval = _currentFallDelay;
            
            // Configure game mode specific settings
            switch (gameMode)
            {
                case GameMode.Timed:
                    RemainingTimeSeconds = GameModeSettings.GetTimedModeSeconds(difficulty);
                    _gameTimer.Start();
                    break;
                    
                case GameMode.Challenge:
                    TargetRowsToClear = GameModeSettings.GetChallengeRowsTarget(difficulty);
                    break;
                    
                case GameMode.Classic:
                default:
                    // Nothing special for classic mode
                    break;
            }
            
            // Create new pieces
            CurrentPiece = TetrominoFactory.CreateRandomTetromino();
            NextPiece = TetrominoFactory.CreateRandomTetromino();

            // Start the fall timer
            _fallTimer.Start();

            // Notify listeners
            OnBoardUpdated();
            OnScoreUpdated();
        }

        /// <summary>
        /// Pauses the game.
        /// </summary>
        public void PauseGame()
        {
            if (_isGameOver)
                return;

            _isGamePaused = true;
            _fallTimer.Stop();
        }

        /// <summary>
        /// Resumes the game from a paused state.
        /// </summary>
        public void ResumeGame()
        {
            if (_isGameOver || !_isGamePaused)
                return;

            _isGamePaused = false;
            _fallTimer.Start();
        }

        /// <summary>
        /// Activates fast drop mode, accelerating the falling of the current piece.
        /// </summary>
        public void ActivateFastDrop()
        {
            if (_isGameOver || _isGamePaused || _isFastDropActive)
                return;

            _isFastDropActive = true;
            _fallTimer.Stop();
            _fallTimer.Interval = FastDropDelay;
            _fallTimer.Start();
        }

        /// <summary>
        /// Deactivates fast drop mode, returning to normal falling speed.
        /// </summary>
        public void DeactivateFastDrop()
        {
            if (_isGameOver || _isGamePaused || !_isFastDropActive)
                return;

            _isFastDropActive = false;
            _fallTimer.Stop();
            _fallTimer.Interval = _currentFallDelay;
            _fallTimer.Start();
        }

        #endregion

        #region Game Mechanics Methods        /// <summary>
        /// Handles the timer elapsed event, causing the current piece to fall.
        /// </summary>
        private void FallTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_isGamePaused || _isGameOver)
                return;

            MovePieceDown();
        }

        /// <summary>
        /// Moves the current piece down one step.
        /// If it can't move down anymore, locks it in place and spawns a new piece.
        /// </summary>
        /// <returns>True if the piece was moved down, false otherwise.</returns>
        private bool MovePieceDown()
        {
            // Try to move the piece down
            Point[] newPositions = CurrentPiece.GetPositionsAfterMove(0, 1);

            // Check for collisions
            bool canMoveDown = !Board.CheckCollision(ToTuples(newPositions));

            if (canMoveDown)
            {
                // Move the piece down
                CurrentPiece.Move(0, 1);
                OnBoardUpdated();
                return true;
            }
            else
            {
                // Lock the piece in place and spawn a new one
                LockCurrentPiece();
                return false;
            }
        }

        /// <summary>
        /// Moves the current piece to the left if possible.
        /// </summary>
        /// <returns>True if the piece was moved, false otherwise.</returns>
        public bool MovePieceLeft()
        {
            if (_isGameOver || _isGamePaused)
                return false;

            // Try to move the piece left
            Point[] newPositions = CurrentPiece.GetPositionsAfterMove(-1, 0);

            // Check for collisions
            if (!Board.CheckCollision(ToTuples(newPositions)))
            {
                // Move the piece left
                CurrentPiece.Move(-1, 0);
                OnBoardUpdated();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Moves the current piece to the right if possible.
        /// </summary>
        /// <returns>True if the piece was moved, false otherwise.</returns>
        public bool MovePieceRight()
        {
            if (_isGameOver || _isGamePaused)
                return false;

            // Try to move the piece right
            Point[] newPositions = CurrentPiece.GetPositionsAfterMove(1, 0);

            // Check for collisions
            if (!Board.CheckCollision(ToTuples(newPositions)))
            {
                // Move the piece right
                CurrentPiece.Move(1, 0);
                OnBoardUpdated();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the current piece clockwise if possible.
        /// </summary>
        /// <returns>True if the piece was rotated, false otherwise.</returns>
        public bool RotatePieceClockwise()
        {
            if (_isGameOver || _isGamePaused)
                return false;

            // Try to rotate the piece
            Point[] newPositions = CurrentPiece.GetPositionsAfterClockwiseRotation();

            // Check for collisions
            if (!Board.CheckCollision(ToTuples(newPositions)))
            {
                // Rotate the piece
                CurrentPiece.RotateClockwise();
                OnBoardUpdated();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rotates the current piece counter-clockwise if possible.
        /// </summary>
        /// <returns>True if the piece was rotated, false otherwise.</returns>
        public bool RotatePieceCounterClockwise()
        {
            if (_isGameOver || _isGamePaused)
                return false;

            // Try to rotate the piece
            Point[] newPositions = CurrentPiece.GetPositionsAfterCounterClockwiseRotation();

            // Check for collisions
            if (!Board.CheckCollision(ToTuples(newPositions)))
            {
                // Rotate the piece
                CurrentPiece.RotateCounterClockwise();
                OnBoardUpdated();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Performs a hard drop, moving the current piece down as far as possible in one go.
        /// </summary>
        public void HardDrop()
        {
            if (_isGameOver || _isGamePaused)
                return;

            // Move down until collision
            while (MovePieceDown()) { }
        }

        /// <summary>
        /// Locks the current piece in place, checks for completed rows, and spawns a new piece.
        /// </summary>
        private void LockCurrentPiece()
        {
            // Add the current piece to the board
            Point[] positions = CurrentPiece.GetAbsolutePositions();
            Board.AddBlocks(ToTuples(positions), CurrentPiece.Id);            // Check for completed rows and update score
            int[] clearedRowIndices = Board.RemoveFullRowsWithIndices();
            int rowsCleared = clearedRowIndices.Length;
            if (rowsCleared > 0)
            {
                UpdateScore(rowsCleared, clearedRowIndices);
                CheckForLevelUp();
            }// Check if game is over
            if (Board.IsGameOver())
            {
                EndGame(GameOverReason.BoardFull);
                return;
            } // Spawn new piece
            CurrentPiece = NextPiece;
            NextPiece = TetrominoFactory.CreateRandomTetromino();            // Check if the new piece can be placed on the board
            Point[] newPositions = CurrentPiece.GetAbsolutePositions();
            if (Board.CheckCollision(ToTuples(newPositions)))
            {
                // Can't place new piece, game over
                EndGame(GameOverReason.NoSpaceForNewPiece);
                return;
            }

            OnBoardUpdated();
        }        /// <summary>
                 /// Updates the game score based on the number of rows cleared.
                 /// </summary>
                 /// <param name="rowsCleared">The number of rows cleared.</param>
                 /// <param name="clearedRowIndices">The indices of the cleared rows.</param>
        private void UpdateScore(int rowsCleared, int[] clearedRowIndices)
        {
            // Basic scoring system:
            // 1 row = 100 points
            // 2 rows = 300 points
            // 3 rows = 500 points
            // 4 rows (Tetris) = 800 points
            // All scores are multiplied by the current level
            int baseScore = 0;
            switch (rowsCleared)
            {
                case 1:
                    baseScore = SingleRowScore;
                    SingleRowsCleared++;
                    break;
                case 2:
                    baseScore = DoubleRowScore;
                    DoubleRowsCleared++;
                    break;
                case 3:
                    baseScore = TripleRowScore;
                    TripleRowsCleared++;
                    break;
                case 4:
                    baseScore = TetrisScore;
                    TetrisCleared++;
                    break;
            }            // Apply difficulty multiplier to the base score
            double difficultyMultiplier = DifficultySettings.GetScoreMultiplier(Difficulty);
            int scoreGained = (int)(baseScore * Level * difficultyMultiplier);
            Score += scoreGained;

            // Raise events
            OnScoreUpdated();
            OnRowsCleared(rowsCleared, scoreGained, clearedRowIndices);
        }

        /// <summary>
        /// Checks if the player has cleared enough rows to advance to the next level.
        /// </summary>
        private void CheckForLevelUp()
        {
            int totalRowsCleared = Board.RowsCleared;
            int newLevel = (totalRowsCleared / RowsPerLevel) + 1;

            if (newLevel > Level)
            {
                int oldLevel = Level;
                Level = newLevel;
                UpdateFallSpeed();
                OnLevelIncreased(oldLevel, newLevel);
            }
        }        /// <summary>
        /// Updates the falling speed based on the current level and difficulty.
        /// </summary>
        private void UpdateFallSpeed()
        {
            // Get difficulty-specific values
            double initialDelay = DifficultySettings.GetInitialFallDelay(Difficulty);
            double delayReduction = DifficultySettings.GetDelayReductionPerLevel(Difficulty);
            double minDelay = DifficultySettings.GetMinimumFallDelay(Difficulty);
            
            // Calculate new fall delay based on level and difficulty
            _currentFallDelay = Math.Max(
                initialDelay - (Level - 1) * delayReduction,
                minDelay
            );

            // Update timer if fast drop is not active
            if (!_isFastDropActive)
            {
                _fallTimer.Stop();
                _fallTimer.Interval = _currentFallDelay;
                _fallTimer.Start();
            }
        }/// <summary>
                 /// Ends the game.
                 /// </summary>
                 /// <param name="reason">The reason why the game ended.</param>
        private void EndGame(GameOverReason reason = GameOverReason.BoardFull)
        {
            _isGameOver = true;
            _fallTimer.Stop();
            OnGameOver(reason);
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Converts an array of Points to an enumerable of (int X, int Y) tuples.
        /// </summary>
        private IEnumerable<(int X, int Y)> ToTuples(Point[] points)
        {
            foreach (var point in points)
            {
                yield return (point.X, point.Y);
            }
        }

        /// <summary>
        /// Gets the total number of rows cleared across all types of clears.
        /// </summary>
        /// <returns>The total number of rows cleared.</returns>
        public int GetTotalRowsCleared()
        {
            return SingleRowsCleared + (DoubleRowsCleared * 2) + (TripleRowsCleared * 3) + (TetrisCleared * 4);
        }

        /// <summary>
        /// Gets statistics about the different types of line clears achieved during the game.
        /// </summary>
        /// <returns>A dictionary containing the count of each type of line clear.</returns>
        public Dictionary<string, int> GetLineStatistics()
        {
            return new Dictionary<string, int>
            {
                { "Singles", SingleRowsCleared },
                { "Doubles", DoubleRowsCleared },
                { "Triples", TripleRowsCleared },
                { "Tetris", TetrisCleared }
            };
        }

        #endregion

        #region Event Invokers

        /// <summary>
        /// Raises the BoardUpdated event.
        /// </summary>
        protected virtual void OnBoardUpdated()
        {
            BoardUpdated?.Invoke(this, EventArgs.Empty);
        }        /// <summary>
                 /// Raises the ScoreUpdated event.
                 /// </summary>
        protected virtual void OnScoreUpdated()
        {
            ScoreUpdated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Raises the RowsCleared event.
        /// </summary>
        /// <param name="rowsCleared">The number of rows cleared.</param>
        /// <param name="scoreGained">The score gained from clearing the rows.</param>
        /// <param name="clearedRowIndices">The indices of the rows that were cleared.</param>
        protected virtual void OnRowsCleared(int rowsCleared, int scoreGained, int[] clearedRowIndices)
        {
            TotalRowsCleared += rowsCleared;
            RowsCleared?.Invoke(this, new RowsClearedEventArgs(rowsCleared, scoreGained, clearedRowIndices));
            
            // Check if Challenge mode goal has been reached
            if (GameMode == GameMode.Challenge && TotalRowsCleared >= TargetRowsToClear && !_isGameOver && !_isGameWon)
            {
                _isGameWon = true;
                _fallTimer.Stop();
                OnGameWon();
            }
        }/// <summary>
         /// Raises the LevelIncreased event.
         /// </summary>
         /// <param name="oldLevel">The previous level.</param>
         /// <param name="newLevel">The new level reached.</param>
        protected virtual void OnLevelIncreased(int oldLevel, int newLevel)
        {
            LevelIncreased?.Invoke(this, new LevelIncreasedEventArgs(oldLevel, newLevel));
        }/// <summary>
         /// Raises the GameOver event with game statistics.
         /// </summary>
         /// <param name="reason">The reason why the game ended.</param>
        protected virtual void OnGameOver(GameOverReason reason)
        {
            var eventArgs = new GameOverEventArgs(
                Score,
                Level,
                GetTotalRowsCleared(),
                GetLineStatistics(),
                reason
            );

            GameOver?.Invoke(this, eventArgs);
        }
        
        /// <summary>
        /// Raises the RemainingTimeChanged event.
        /// </summary>
        protected virtual void OnRemainingTimeChanged()
        {
            RemainingTimeChanged?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Raises the GameWon event.
        /// </summary>
        protected virtual void OnGameWon()
        {
            GameWon?.Invoke(this, EventArgs.Empty);
        }
        
        /// <summary>
        /// Handles the game timer tick for timed mode.
        /// </summary>
        private void GameTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (_isGamePaused || _isGameOver || _isGameWon)
            {
                return;
            }

            if (GameMode == GameMode.Timed)
            {
                RemainingTimeSeconds--;
                OnRemainingTimeChanged();
                
                if (RemainingTimeSeconds <= 0)
                {
                    _fallTimer.Stop();
                    _gameTimer.Stop();
                    _isGameOver = true;
                    OnGameOver(GameOverReason.TimeUp);
                }
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases all resources used by the GameEngine.
        /// </summary>
        public void Dispose()
        {
            _fallTimer.Stop();
            _fallTimer.Elapsed -= FallTimer_Elapsed;
            _fallTimer.Dispose();
            
            _gameTimer.Stop();
            _gameTimer.Elapsed -= GameTimer_Elapsed;
            _gameTimer.Dispose();
        }

        #endregion

        #region Save and Load Methods

        /// <summary>
        /// Creates a GameState object from the current game state.
        /// </summary>
        /// <returns>A GameState object containing the current game state.</returns>
        public GameState CreateGameState()
        {
            var gameState = new GameState
            {
                // Copy board state
                BoardGrid = (int?[,])Board.Grid.Clone(),
                BoardRowsCleared = Board.RowsCleared,
                
                // Copy tetromino states
                CurrentPiece = CurrentPiece != null ? TetrominoState.FromTetromino(CurrentPiece) : null,
                NextPiece = NextPiece != null ? TetrominoState.FromTetromino(NextPiece) : null,
                
                // Copy game statistics
                Level = Level,
                Difficulty = Difficulty,
                GameMode = GameMode,
                RemainingTimeSeconds = RemainingTimeSeconds,
                TargetRowsToClear = TargetRowsToClear,
                TotalRowsCleared = TotalRowsCleared,
                Score = Score,
                SingleRowsCleared = SingleRowsCleared,
                DoubleRowsCleared = DoubleRowsCleared,
                TripleRowsCleared = TripleRowsCleared,
                TetrisCleared = TetrisCleared,
                
                // Copy game state flags
                IsPaused = _isGamePaused,
                CurrentFallDelay = _currentFallDelay,
                IsFastDropActive = _isFastDropActive
            };

            return gameState;
        }

        /// <summary>
        /// Loads a game state and restores the game to that state.
        /// </summary>
        /// <param name="gameState">The game state to load.</param>
        /// <exception cref="ArgumentNullException">Thrown when gameState is null.</exception>
        public void LoadGameState(GameState gameState)
        {
            if (gameState == null)
                throw new ArgumentNullException(nameof(gameState));

            // Stop current timers
            _fallTimer.Stop();
            _gameTimer.Stop();

            // Restore board state
            Board.LoadState(gameState.BoardGrid, gameState.BoardRowsCleared);        // Restore tetromino pieces
        CurrentPiece = gameState.CurrentPiece != null ? TetrominoFactory.CreateFromState(gameState.CurrentPiece) : TetrominoFactory.CreateRandomTetromino();
        NextPiece = gameState.NextPiece != null ? TetrominoFactory.CreateFromState(gameState.NextPiece) : TetrominoFactory.CreateRandomTetromino();

            // Restore game statistics
            Level = gameState.Level;
            Difficulty = gameState.Difficulty;
            GameMode = gameState.GameMode;
            RemainingTimeSeconds = gameState.RemainingTimeSeconds;
            TargetRowsToClear = gameState.TargetRowsToClear;
            TotalRowsCleared = gameState.TotalRowsCleared;
            Score = gameState.Score;
            SingleRowsCleared = gameState.SingleRowsCleared;
            DoubleRowsCleared = gameState.DoubleRowsCleared;
            TripleRowsCleared = gameState.TripleRowsCleared;
            TetrisCleared = gameState.TetrisCleared;

            // Restore game state flags
            _isGamePaused = gameState.IsPaused;
            _currentFallDelay = gameState.CurrentFallDelay;
            _isFastDropActive = gameState.IsFastDropActive;
            _isGameOver = false;
            _isGameWon = false;

            // Configure timers based on restored state
            _fallTimer.Interval = _currentFallDelay;
            
            // Start appropriate timers based on game mode
            if (GameMode == GameMode.Timed && RemainingTimeSeconds > 0)
            {
                _gameTimer.Start();
            }

            // Start fall timer if game is not paused and not over
            if (!_isGamePaused && !_isGameOver)
            {
                _fallTimer.Start();
            }

            // Notify UI of state changes
            OnBoardUpdated();
            OnScoreUpdated();
            OnRemainingTimeChanged();
        }

        #endregion
    }
}

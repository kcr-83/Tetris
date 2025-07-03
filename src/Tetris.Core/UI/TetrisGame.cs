using System;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Main entry point for the Tetris game application that integrates the UI components with the game engine.
    /// </summary>
    public class TetrisGame : IDisposable
    {
        #region Fields
        private readonly GameEngine _gameEngine;
        private readonly MainMenuInterface _mainMenu;
        private readonly GameOverDisplay _gameOverDisplay;
        private readonly GameplayInterface _gameplayInterface;
        private GameplayInterfaceComplete? _gameplayInterfaceComplete;
        private bool _isGameActive;
        private bool _isPaused;
        private bool _isExiting;
        private bool _useResponsiveInterface;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the TetrisGame class.
        /// </summary>
        public TetrisGame()
        {
            _gameEngine = new GameEngine();
            _mainMenu = new MainMenuInterface(_gameEngine);
            _gameOverDisplay = new GameOverDisplay(_gameEngine);
            _gameplayInterface = new GameplayInterface(_gameEngine);
            _gameplayInterfaceComplete = new GameplayInterfaceComplete(_gameEngine);
            _useResponsiveInterface = true; // Enable responsive interface by default

            // Set up event handlers
            _mainMenu.NewGameRequested += OnNewGameRequested;
            _mainMenu.ExitRequested += OnExitRequested;
            _gameOverDisplay.NewGameRequested += OnNewGameRequested;
            _gameOverDisplay.ReturnToMenuRequested += OnReturnToMenuRequested;
            _gameEngine.GameOver += OnGameOver;
            _gameEngine.LevelIncreased += OnLevelIncreased;
            _gameEngine.RowsCleared += OnRowsCleared;
            _gameEngine.GameWon += OnGameWon;
            _gameEngine.RemainingTimeChanged += OnRemainingTimeChanged;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs the Tetris game application.
        /// </summary>
        /// <returns>A task that completes when the application exits.</returns>
        public async Task RunAsync()
        {
            // Show the main menu
            await _mainMenu.ShowAsync();

            // Main application loop
            while (!_isExiting)
            {
                if (_isGameActive)
                {
                    await RunGameLoopAsync();
                }
                else
                {
                    await _mainMenu.ShowAsync();
                }
            }

            // Clean up before exiting
            Console.Clear();
            Console.WriteLine("Thank you for playing Tetris!");
            await Task.Delay(1500);
        }

        /// <summary>
        /// Releases resources used by the TetrisGame.
        /// </summary>
        public void Dispose()
        {
            _gameEngine?.Dispose();
        }

        #endregion

        #region Private Methods        /// <summary>
        /// Handles the game loop when a game is active.
        /// </summary>
        private async Task RunGameLoopAsync()
        {
            Console.Clear();
            Console.CursorVisible = false;

            // Reset state for new game
            _isPaused = false;

            // Initialize the game interface
            _gameplayInterface.Initialize();

            // Display initial board
            DisplayGame();

            // Cache window size values to detect changes
            int lastWindowWidth = Console.WindowWidth;
            int lastWindowHeight = Console.WindowHeight;

            // Game loop continues until game is over or player exits
            while (_isGameActive && !_isExiting)
            {
                // Check for window resize
                if (
                    Console.WindowWidth != lastWindowWidth
                    || Console.WindowHeight != lastWindowHeight
                )
                {
                    lastWindowWidth = Console.WindowWidth;
                    lastWindowHeight = Console.WindowHeight;
                    _gameplayInterface.HandleResize();
                }

                // Only process input and update display if game is not paused
                if (!_isPaused)
                {
                    // Process all available input to prevent input lag
                    while (Console.KeyAvailable)
                    {
                        ProcessGameInput();
                    }

                    DisplayGame();
                }
                else
                {
                    // When paused, only check for unpause or exit
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.P)
                        {
                            _isPaused = false;
                            DisplayGame(); // Refresh to remove pause message
                        }
                        else if (key.Key == ConsoleKey.Escape)
                        {
                            _isGameActive = false;
                            break;
                        }
                    }
                }

                // Adaptive delay based on level for smoother gameplay at higher levels
                int frameDelay = Math.Max(10, 50 - (_gameEngine.Level * 2));
                await Task.Delay(frameDelay);
            }
        }

        /// <summary>
        /// Processes user input during gameplay.
        /// </summary>
        private void ProcessGameInput()
        {
            var key = Console.ReadKey(true);
            bool fastDropActive = false;

            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    // Quick successive moves for responsive controls
                    _gameEngine.MovePieceLeft();
                    break;

                case ConsoleKey.RightArrow:
                    // Quick successive moves for responsive controls
                    _gameEngine.MovePieceRight();
                    break;

                case ConsoleKey.UpArrow:
                    // Support for multiple rotations in quick succession
                    _gameEngine.RotatePieceClockwise();
                    break;

                case ConsoleKey.DownArrow:
                    _gameEngine.ActivateFastDrop();
                    fastDropActive = true;
                    break;

                case ConsoleKey.Spacebar:
                    _gameEngine.HardDrop();
                    break;

                case ConsoleKey.P:
                    TogglePause();
                    break;

                case ConsoleKey.Escape:
                    PromptExitToMenu();
                    break;

                // Additional keys for improved controls
                case ConsoleKey.Z: // Alternative rotation (counterclockwise)
                    // If implemented in the game engine
                    // _gameEngine.RotatePieceCounterclockwise();
                    break;

                case ConsoleKey.C: // Hold piece functionality if implemented
                    // If implemented in the game engine
                    // _gameEngine.HoldCurrentPiece();
                    break;
            }

            // Release fast drop if down arrow is released and no other keys maintain it
            if (!fastDropActive)
            {
                _gameEngine.DeactivateFastDrop();
            }
        }

        /// <summary>
        /// Displays the current game state using the GameplayInterface.
        /// </summary>
        private void DisplayGame()
        {
            // Use the GameplayInterface to render the game
            _gameplayInterface.Render();

            // Show pause overlay if game is paused
            if (_isPaused)
            {
                _gameplayInterface.ShowPauseOverlay();
            }
        } 
        // The drawing methods are now handled by the GameplayInterface class        /// <summary>

        /// Toggles the pause state of the game.
        /// </summary>
        private void TogglePause()
        {
            _isPaused = !_isPaused;

            if (_isPaused)
            {
                // The pause overlay will be shown by the DisplayGame method
                DisplayGame();
            }
            else
            {
                // Refresh the screen without pause overlay
                DisplayGame();
            }
        }

        // Pause overlay is now handled by the GameplayInterface class        /// <summary>
        /// Prompts the user if they want to exit to the main menu.
        /// </summary>
        private void PromptExitToMenu()
        {
            _isPaused = true;
            DisplayGame(); // Show pause overlay first

            int centerX = Console.WindowWidth / 2 - 15;
            int centerY = Console.WindowHeight / 2 - 2;

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.SetCursorPosition(centerX, centerY);
            Console.WriteLine("┌───────────────────────────┐");
            Console.SetCursorPosition(centerX, centerY + 1);
            Console.WriteLine("│  Exit to main menu? (Y/N) │");
            Console.SetCursorPosition(centerX, centerY + 2);
            Console.WriteLine("└───────────────────────────┘");

            Console.ForegroundColor = ConsoleColor.White;

            bool answered = false;
            while (!answered)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Y)
                {
                    _isGameActive = false;
                    answered = true;
                }
                else if (key.Key == ConsoleKey.N)
                {
                    _isPaused = false;
                    answered = true;
                    DisplayGame(); // Refresh the screen
                }
            }
        } // Tetromino coloring is now handled by the GameplayInterface class
        #endregion

        #region Event Handlers
        /// <summary>
        /// Handles the NewGameRequested event from the main menu or game over display.
        /// </summary>
        private void OnNewGameRequested(object? sender, EventArgs e)
        {
            if (e is GameModeSelectionEventArgs gameModeArgs)
            {
                _isGameActive = true;
                _gameEngine.StartNewGame(gameModeArgs.SelectedDifficulty, gameModeArgs.SelectedGameMode);
            }
            else
            {
                // Fallback to Medium difficulty and Classic mode if event args are not of expected type
                _isGameActive = true;
                _gameEngine.StartNewGame(DifficultyLevel.Medium, GameMode.Classic);
            }
        }

        /// <summary>
        /// Handles the ExitRequested event from the main menu.
        /// </summary>
        private void OnExitRequested(object? sender, EventArgs e)
        {
            _isExiting = true;
        }

        /// <summary>
        /// Handles the GameOver event from the game engine.
        /// </summary>
        private void OnGameOver(object? sender, GameOverEventArgs e)
        {
            _isGameActive = false;

            // Let the game over display show its message
            // Control will return to the main menu when the player presses a key
        }

        /// <summary>
        /// Handles the ReturnToMenuRequested event from the game over display.
        /// </summary>
        private void OnReturnToMenuRequested(object? sender, EventArgs e)
        {
            _isGameActive = false;
        }

        /// <summary>
        /// Handles the LevelIncreased event from the game engine.
        /// </summary>
        private async void OnLevelIncreased(object? sender, LevelIncreasedEventArgs e)
        {
            if (_isGameActive && !_isPaused)
            {
                await _gameplayInterface.ShowLevelUpAnimationAsync(e.NewLevel);
            }
        }

        /// <summary>
        /// Handles the RowsCleared event from the game engine.
        /// </summary>
        private async void OnRowsCleared(object? sender, RowsClearedEventArgs e)
        {
            if (_isGameActive && !_isPaused)
            {
                await _gameplayInterface.ShowRowsClearedAnimationAsync(
                    e.RowsCleared,
                    e.ScoreGained
                );
            }
        }

        /// <summary>
        /// Handles the GameWon event from the game engine.
        /// </summary>
        private async void OnGameWon(object? sender, EventArgs e)
        {
            // Game is active until animations complete
            
            // Show win animation first
            await _gameplayInterface.ShowWinAnimationAsync();
            
            // Then show win message
            _gameplayInterface.ShowWinOverlay();
            
            // Mark game as no longer active
            _isGameActive = false;
            
            // Game will transition to game over display with victory message
        }

        /// <summary>
        /// Handles the RemainingTimeChanged event from the game engine.
        /// </summary>
        private void OnRemainingTimeChanged(object? sender, EventArgs e)
        {
            if (_isGameActive && !_isPaused)
            {
                _gameplayInterface.UpdateTimerDisplay(_gameEngine.RemainingTimeSeconds);
            }
        }

        #endregion

        #region Responsive Interface Methods

        /// <summary>
        /// Handles the game loop when a game is active, using the enhanced responsive interface.
        /// </summary>
        private async Task RunGameLoopWithResponsiveInterfaceAsync()
        {
            Console.Clear();
            Console.CursorVisible = false;

            // Create the responsive interface on demand
            if (_gameplayInterfaceComplete == null)
            {
                _gameplayInterfaceComplete = new GameplayInterfaceComplete(_gameEngine);
            }

            // Reset state for new game
            _isPaused = false;

            // Initialize the game interface with double buffering enabled
            _gameplayInterfaceComplete.EnableDoubleBuffering = true;
            _gameplayInterfaceComplete.Initialize();

            // Display initial board
            _gameplayInterfaceComplete.Render();

            // Variables for controlling timing
            const int targetFrameRate = 60;
            const int targetFrameTime = 1000 / targetFrameRate;
            DateTime lastFrameTime = DateTime.Now;
            DateTime lastGameUpdate = DateTime.Now;
            int gameUpdateInterval = 50; // Start with 20 updates per second

            // Game loop continues until game is over or player exits
            while (_isGameActive && !_isExiting)
            {
                // Start frame timing
                DateTime frameStartTime = DateTime.Now;

                // Update game update interval based on level (faster updates at higher levels)
                gameUpdateInterval = Math.Max(10, 50 - (_gameEngine.Level * 2));

                // Check for window resize
                _gameplayInterfaceComplete.CheckForResize();

                // Only process input and update display if game is not paused
                if (!_isPaused)
                {
                    // Process all available input with priority for responsiveness
                    int maxInputProcessingPerFrame = 10; // Prevent input flood
                    int inputsProcessed = 0;

                    while (Console.KeyAvailable && inputsProcessed < maxInputProcessingPerFrame)
                    {
                        var key = Console.ReadKey(true);

                        // Use the enhanced input processing
                        bool fastDropActive = _gameplayInterfaceComplete.ProcessGameInput(key);

                        // Handle special keys like pause and exit
                        HandleSpecialKeys(key);

                        // Ensure fast drop state is consistent
                        if (!fastDropActive)
                        {
                            _gameEngine.DeactivateFastDrop();
                        }

                        inputsProcessed++;
                    }

                    // Update game state at appropriate intervals
                    TimeSpan timeSinceLastUpdate = DateTime.Now - lastGameUpdate;
                    if (timeSinceLastUpdate.TotalMilliseconds > gameUpdateInterval)
                    {
                        // Update game state
                        _gameEngine.Update();

                        // Render the game with the current state
                        _gameplayInterfaceComplete.Render();
                        lastGameUpdate = DateTime.Now;
                    }
                }
                else
                {
                    // When paused, only check for unpause or exit commands
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        HandleSpecialKeys(key);
                    }

                    // Show pause overlay
                    _gameplayInterfaceComplete.ShowPauseOverlay();
                }

                // Calculate frame timing and delay to maintain target framerate
                TimeSpan frameTime = DateTime.Now - frameStartTime;
                int delayTime = Math.Max(1, targetFrameTime - (int)frameTime.TotalMilliseconds);

                // Use adaptive frame timing for smoother experience
                await Task.Delay(delayTime);
            }
        }

        /// <summary>
        /// Handles special keys like pause and exit.
        /// </summary>
        private void HandleSpecialKeys(ConsoleKeyInfo key)
        {
            if (key.Key == ConsoleKey.P)
            {
                _isPaused = !_isPaused;
                if (!_isPaused)
                {
                    // Refresh the screen when unpaused
                    if (_useResponsiveInterface && _gameplayInterfaceComplete != null)
                    {
                        _gameplayInterfaceComplete.Render();
                    }
                    else
                    {
                        _gameplayInterface.Render();
                    }
                }
            }
            else if (key.Key == ConsoleKey.Escape)
            {
                if (_isPaused)
                {
                    PromptExitToMenu();
                }
                else
                {
                    _isPaused = true;
                    if (_useResponsiveInterface && _gameplayInterfaceComplete != null)
                    {
                        _gameplayInterfaceComplete.ShowPauseOverlay();
                    }
                    else
                    {
                        _gameplayInterface.ShowPauseOverlay();
                    }
                }
            }
        }

        #endregion
    }
}

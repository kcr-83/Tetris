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
        private bool _isGameActive;
        private bool _isPaused;
        private bool _isExiting;

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

            // Set up event handlers
            _mainMenu.NewGameRequested += OnNewGameRequested;
            _mainMenu.ExitRequested += OnExitRequested;
            _gameOverDisplay.NewGameRequested += OnNewGameRequested;
            _gameOverDisplay.ReturnToMenuRequested += OnReturnToMenuRequested;
            _gameEngine.GameOver += OnGameOver;
            _gameEngine.LevelIncreased += OnLevelIncreased;
            _gameEngine.RowsCleared += OnRowsCleared;
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

        #region Private Methods

        /// <summary>
        /// Handles the game loop when a game is active.
        /// </summary>
        private async Task RunGameLoopAsync()
        {
            Console.Clear();
            Console.CursorVisible = false;

            // Reset state for new game
            _isPaused = false;

            // Display initial board
            DisplayGame();

            // Game loop continues until game is over or player exits
            while (_isGameActive && !_isExiting)
            {
                // Only process input and update display if game is not paused
                if (!_isPaused)
                {
                    if (Console.KeyAvailable)
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

                await Task.Delay(50); // Short delay to prevent CPU overuse
            }
        }

        /// <summary>
        /// Processes user input during gameplay.
        /// </summary>
        private void ProcessGameInput()
        {
            var key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    _gameEngine.MovePieceLeft();
                    break;

                case ConsoleKey.RightArrow:
                    _gameEngine.MovePieceRight();
                    break;

                case ConsoleKey.UpArrow:
                    _gameEngine.RotatePieceClockwise();
                    break;

                case ConsoleKey.DownArrow:
                    _gameEngine.ActivateFastDrop();
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
            }

            // Release fast drop if down arrow is released
            if (key.Key != ConsoleKey.DownArrow)
            {
                _gameEngine.DeactivateFastDrop();
            }
        }        /// <summary>
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
        }        // The drawing methods are now handled by the GameplayInterface class        /// <summary>
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
        }// Pause overlay is now handled by the GameplayInterface class        /// <summary>
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
        }        // Tetromino coloring is now handled by the GameplayInterface class

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the NewGameRequested event from the main menu.
        /// </summary>
        private void OnNewGameRequested(object? sender, EventArgs e)
        {
            _isGameActive = true;
            _gameEngine.StartNewGame();
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
        }        /// <summary>
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
                await _gameplayInterface.ShowRowsClearedAnimationAsync(e.RowsCleared, e.ScoreGained);
            }
        }

        #endregion
    }
}

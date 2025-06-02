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
            
            // Set up event handlers
            _mainMenu.NewGameRequested += OnNewGameRequested;
            _mainMenu.ExitRequested += OnExitRequested;
            _gameOverDisplay.NewGameRequested += OnNewGameRequested;
            _gameOverDisplay.ReturnToMenuRequested += OnReturnToMenuRequested;
            _gameEngine.GameOver += OnGameOver;
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
        }
        
        /// <summary>
        /// Displays the current game state.
        /// </summary>
        private void DisplayGame()
        {
            Console.Clear();
            
            // Display game info
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"TETRIS - Level: {_gameEngine.Level} | Score: {_gameEngine.Score}");
            Console.WriteLine($"Lines: {_gameEngine.Board.RowsCleared}");
            Console.WriteLine();
            
            // Create a copy of the board with the current piece added
            var boardWithPiece = new Board(_gameEngine.Board);
            var positions = _gameEngine.CurrentPiece.GetAbsolutePositions();
            
            foreach (var position in positions)
            {
                if (boardWithPiece.IsWithinBounds(position.X, position.Y))
                {
                    boardWithPiece.AddBlock(position.X, position.Y, _gameEngine.CurrentPiece.Id);
                }
            }
            
            // Display the board
            DrawBoard(boardWithPiece);
            
            // Display next piece
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
            Console.WriteLine("Next Piece:");
            DrawNextPiece(_gameEngine.NextPiece);
            
            // Display controls
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine();
            Console.WriteLine("Controls: ←→↑↓, Space=Drop, P=Pause, Esc=Menu");
            
            // Display pause message if paused
            if (_isPaused)
            {
                DisplayPauseOverlay();
            }
        }
        
        /// <summary>
        /// Draws the game board with colored blocks.
        /// </summary>
        /// <param name="board">The board to draw.</param>
        private void DrawBoard(Board board)
        {
            Console.WriteLine("┌" + new string('─', Board.Width * 2) + "┐");
            
            for (int y = 0; y < Board.Height; y++)
            {
                Console.Write("│");
                
                for (int x = 0; x < Board.Width; x++)
                {
                    var cell = board.Grid[x, y];
                    
                    if (cell.HasValue)
                    {
                        // Set color based on the tetromino type
                        SetTetrominoColor(cell.Value);
                        Console.Write("██");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("··");
                    }
                }
                
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("│");
            }
            
            Console.WriteLine("└" + new string('─', Board.Width * 2) + "┘");
        }
          /// <summary>
        /// Draws the next piece preview.
        /// </summary>
        /// <param name="nextPiece">The next piece to display.</param>
        private void DrawNextPiece(Tetromino nextPiece)
        {
            // Get the piece's shape in its default orientation
            var positions = nextPiece.Blocks;
            
            // Determine the bounding box
            int minX = positions.Min(p => p.X);
            int minY = positions.Min(p => p.Y);
            int maxX = positions.Max(p => p.X);
            int maxY = positions.Max(p => p.Y);
            
            int width = maxX - minX + 1;
            int height = maxY - minY + 1;
            
            // Create a 2D array to represent the piece
            bool[,] pieceShape = new bool[width, height];
            
            // Fill in the shape
            foreach (var pos in positions)
            {
                pieceShape[pos.X - minX, pos.Y - minY] = true;
            }
            
            // Draw the piece
            SetTetrominoColor(nextPiece.Id);
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (pieceShape[x, y])
                    {
                        Console.Write("██");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
                
                Console.WriteLine();
            }
            
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        /// <summary>
        /// Toggles the pause state of the game.
        /// </summary>
        private void TogglePause()
        {
            _isPaused = !_isPaused;
            
            if (_isPaused)
            {
                DisplayPauseOverlay();
            }
        }
        
        /// <summary>
        /// Displays the pause overlay.
        /// </summary>
        private void DisplayPauseOverlay()
        {
            int centerX = Console.WindowWidth / 2 - 12;
            int centerY = Console.WindowHeight / 2 - 2;
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            // Draw pause message box
            Console.SetCursorPosition(centerX, centerY);
            Console.WriteLine("┌─────────────────────┐");
            Console.SetCursorPosition(centerX, centerY + 1);
            Console.WriteLine("│       PAUSED        │");
            Console.SetCursorPosition(centerX, centerY + 2);
            Console.WriteLine("│ Press P to continue │");
            Console.SetCursorPosition(centerX, centerY + 3);
            Console.WriteLine("│ Press ESC for menu  │");
            Console.SetCursorPosition(centerX, centerY + 4);
            Console.WriteLine("└─────────────────────┘");
            
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        /// <summary>
        /// Prompts the user if they want to exit to the main menu.
        /// </summary>
        private void PromptExitToMenu()
        {
            _isPaused = true;
            
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
        }
        
        /// <summary>
        /// Sets the console foreground color based on the tetromino ID.
        /// </summary>
        /// <param name="tetrominoId">The ID of the tetromino.</param>
        private void SetTetrominoColor(int tetrominoId)
        {
            // Set colors based on traditional Tetris colors
            switch (tetrominoId)
            {
                case 1: // I piece
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case 2: // J piece
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case 3: // L piece
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case 4: // O piece
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 5: // S piece
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case 6: // T piece
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case 7: // Z piece
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }
        
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
        }
        
        /// <summary>
        /// Handles the ReturnToMenuRequested event from the game over display.
        /// </summary>
        private void OnReturnToMenuRequested(object? sender, EventArgs e)
        {
            _isGameActive = false;
        }
        
        #endregion
    }
}

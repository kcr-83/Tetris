using System;
using Tetris.Core.Models;

namespace Tetris.Core.Tests
{
    /// <summary>
    /// Demonstrates usage of the TetrominoController class.
    /// </summary>
    public class TetrominoControllerDemo
    {
        private GameEngine _gameEngine;
        private TetrominoController _controller;

        /// <summary>
        /// Initializes a new instance of the TetrominoControllerDemo class.
        /// </summary>
        public TetrominoControllerDemo()
        {
            // Initialize the game engine
            _gameEngine = new GameEngine();
            
            // Create the controller
            _controller = new TetrominoController(_gameEngine);
            
            // Set up event handlers
            _gameEngine.BoardUpdated += (s, e) => RenderBoard();
            _gameEngine.ScoreUpdated += (s, e) => Console.WriteLine($"Score: {_gameEngine.Score}, Level: {_gameEngine.Level}");
            _gameEngine.GameOver += (s, e) => Console.WriteLine("Game Over!");
        }        /// <summary>
        /// Renders the current state of the game board to the console.
        /// </summary>
        private void RenderBoard()
        {
            Console.Clear();
            
            // Get the board with the current piece
            var grid = _controller.GetBoardWithCurrentPiece();
            
            // Get the hard drop preview positions
            var previewGrid = _controller.GetHardDropPreview();
            
            // Render the board
            for (int y = 0; y < Board.Height; y++)
            {
                Console.Write("|");
                for (int x = 0; x < Board.Width; x++)
                {
                    if (grid[x, y] == null)
                    {
                        // Show drop preview in a dimmer representation
                        if (previewGrid[x, y] != null && grid[x, y] == null)
                        {
                            Console.Write("·");
                        }
                        else
                        {
                            Console.Write(" ");
                        }
                    }
                    else
                    {
                        // Convert piece ID to a character representation
                        char pieceChar = grid[x, y] switch
                        {
                            1 => 'I',
                            2 => 'J',
                            3 => 'L',
                            4 => 'O',
                            5 => 'S',
                            6 => 'T',
                            7 => 'Z',
                            _ => '#'
                        };
                        Console.Write(pieceChar);
                    }
                }
                Console.WriteLine("|");
            }
            Console.WriteLine(new string('-', Board.Width + 2));
            
            // Display next piece
            Console.WriteLine($"Next piece: {_gameEngine.NextPiece.Name}");
        }

        /// <summary>
        /// Starts a new demo game and processes user input.
        /// </summary>
        public void StartDemo()
        {
            // Start a new game
            _gameEngine.StartNewGame();
            
            // Display controls
            Console.WriteLine("Tetris Controls:");
            Console.WriteLine("A/Left Arrow: Move Left");
            Console.WriteLine("D/Right Arrow: Move Right");
            Console.WriteLine("W/Up Arrow: Rotate Clockwise");
            Console.WriteLine("Q: Rotate Counter-Clockwise");
            Console.WriteLine("S/Down Arrow: Soft Drop (hold)");
            Console.WriteLine("Space: Hard Drop");
            Console.WriteLine("P: Pause/Resume");
            Console.WriteLine("Esc: Quit");
            Console.WriteLine();
            
            Console.WriteLine("Press any key to start...");
            Console.ReadKey(true);
            
            // Main game loop
            bool isRunning = true;
            while (isRunning && !_gameEngine.IsGameOver)
            {
                if (Console.KeyAvailable)
                {
                    // Process key input
                    var key = Console.ReadKey(true).Key;
                    
                    switch (key)
                    {
                        case ConsoleKey.LeftArrow:
                        case ConsoleKey.A:
                            _controller.ProcessInput(TetrominoController.TetrisInput.MoveLeft);
                            break;
                        case ConsoleKey.RightArrow:
                        case ConsoleKey.D:
                            _controller.ProcessInput(TetrominoController.TetrisInput.MoveRight);
                            break;
                        case ConsoleKey.UpArrow:
                        case ConsoleKey.W:
                            _controller.ProcessInput(TetrominoController.TetrisInput.RotateClockwise);
                            break;
                        case ConsoleKey.Q:
                            _controller.ProcessInput(TetrominoController.TetrisInput.RotateCounterClockwise);
                            break;
                        case ConsoleKey.DownArrow:
                        case ConsoleKey.S:
                            _controller.ProcessInput(TetrominoController.TetrisInput.SoftDropStart);
                            break;
                        case ConsoleKey.Spacebar:
                            _controller.ProcessInput(TetrominoController.TetrisInput.HardDrop);
                            break;
                        case ConsoleKey.P:
                            if (_gameEngine.IsPaused)
                            {
                                _gameEngine.ResumeGame();
                            }
                            else
                            {
                                _gameEngine.PauseGame();
                            }
                            break;
                        case ConsoleKey.Escape:
                            isRunning = false;
                            break;
                    }
                }
                
                // Handle key release for soft drop
                if (_controller.IsSoftDropActive && !Console.KeyAvailable)
                {
                    _controller.ProcessInput(TetrominoController.TetrisInput.SoftDropEnd);
                }
                
                // Small delay to prevent CPU overuse
                System.Threading.Thread.Sleep(50);
            }
        }        /// <summary>
        /// Example of programmatic control of the Tetromino.
        /// </summary>
        public void DemonstrateControls()
        {
            // Start a new game
            _gameEngine.StartNewGame();
            
            Console.WriteLine("Demonstrating programmatic Tetromino controls...");
            System.Threading.Thread.Sleep(1000);
            
            // Move left a few times
            for (int i = 0; i < 3; i++)
            {
                _controller.MoveLeft();
                RenderBoard();
                System.Threading.Thread.Sleep(300);
            }
            
            // Move right a few times
            for (int i = 0; i < 5; i++)
            {
                _controller.MoveRight();
                RenderBoard();
                System.Threading.Thread.Sleep(300);
            }
            
            // Rotate clockwise
            _controller.RotateClockwise();
            RenderBoard();
            System.Threading.Thread.Sleep(500);
            
            // Rotate counter-clockwise
            _controller.RotateCounterClockwise();
            RenderBoard();
            System.Threading.Thread.Sleep(500);
            
            // Activate soft drop for a moment
            _controller.ToggleSoftDrop(true);
            System.Threading.Thread.Sleep(1000);
            _controller.ToggleSoftDrop(false);
            RenderBoard();
            System.Threading.Thread.Sleep(500);
            
            // Finally, perform a hard drop
            _controller.HardDrop();
            RenderBoard();
            
            Console.WriteLine("Demo complete! Press any key to exit.");
            Console.ReadKey(true);
        }

        /// <summary>
        /// Main entry point for the demo.
        /// </summary>
        public static void Main()
        {
            var demo = new TetrominoControllerDemo();
            
            // Choose which demo to run
            Console.WriteLine("Select demo type:");
            Console.WriteLine("1. Interactive demo with keyboard controls");
            Console.WriteLine("2. Automatic demonstration of controls");
            
            var key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.D1 || key == ConsoleKey.NumPad1)
            {
                demo.StartDemo();
            }
            else
            {
                demo.DemonstrateControls();
            }
        }
    }
}

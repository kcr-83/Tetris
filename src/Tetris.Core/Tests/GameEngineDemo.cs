using System;
using System.Threading;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.Tests
{
    /// <summary>
    /// A simple console application for testing the Tetris GameEngine.
    /// </summary>
    public static class GameEngineDemo
    {
        /// <summary>
        /// Main entry point for the demo.
        /// </summary>
        public static async Task RunDemo()
        {
            Console.WriteLine("=== Tetris Game Engine Demo ===");
            Console.WriteLine("This demo shows how the falling mechanics work");
            Console.WriteLine();

            // Create a new game engine
            using (var game = new GameEngine())
            {
                // Listen to events
                game.BoardUpdated += (s, e) => DisplayBoard(game);
                game.ScoreUpdated += (s, e) => Console.WriteLine($"Score: {game.Score}");
                game.LevelIncreased += (s, e) => Console.WriteLine($"Level Up! Now at level {game.Level}");
                game.GameOver += (s, e) => Console.WriteLine("Game Over!");

                // Start a new game
                game.StartNewGame();
                
                // Display initial state
                Console.WriteLine("Initial board state:");
                DisplayBoard(game);

                // Demo sequence to demonstrate the falling mechanics
                Console.WriteLine("\nDemonstrating normal falling speed...");
                await Task.Delay(3000); // Let the piece fall at normal speed for 3 seconds
                
                Console.WriteLine("\nDemonstrating player-initiated fast drop...");
                game.ActivateFastDrop();
                await Task.Delay(2000); // Fast drop for 2 seconds
                game.DeactivateFastDrop();
                
                Console.WriteLine("\nReturned to normal falling speed");
                await Task.Delay(2000); // Back to normal speed
                
                Console.WriteLine("\nRotating piece...");
                game.RotatePieceClockwise();
                await Task.Delay(1000);
                
                Console.WriteLine("\nMoving left...");
                for (int i = 0; i < 3; i++)
                {
                    game.MovePieceLeft();
                    await Task.Delay(200);
                }
                
                Console.WriteLine("\nMoving right...");
                for (int i = 0; i < 4; i++)
                {
                    game.MovePieceRight();
                    await Task.Delay(200);
                }
                
                Console.WriteLine("\nPerforming hard drop...");
                game.HardDrop();
                await Task.Delay(1000);
                
                // Let a few more pieces fall to demonstrate speed increases with levels
                Console.WriteLine("\nContinuing play to demonstrate level progression...");
                await Task.Delay(10000); // Let the game run for 10 more seconds
                
                Console.WriteLine("\nDemo complete!");
            }
        }

        /// <summary>
        /// Displays the current board state to the console.
        /// </summary>
        private static void DisplayBoard(GameEngine game)
        {
            Console.Clear();
            
            // Display game info
            Console.WriteLine($"Level: {game.Level} | Score: {game.Score} | Rows: {game.Board.RowsCleared}");
            Console.WriteLine();
            
            // Make a copy of the board to show current piece position
            var boardWithPiece = new Board(game.Board);
            
            // Add the current piece to the display board (not actually adding to the game board)
            var positions = game.CurrentPiece.GetAbsolutePositions();
            foreach (var position in positions)
            {
                if (boardWithPiece.IsWithinBounds(position.X, position.Y))
                {
                    boardWithPiece.AddBlock(position.X, position.Y, game.CurrentPiece.Id);
                }
            }
            
            // Display the board
            for (int y = 0; y < Board.Height; y++)
            {
                Console.Write("|");
                for (int x = 0; x < Board.Width; x++)
                {
                    var blockType = boardWithPiece.Grid[x, y];
                    if (blockType.HasValue)
                    {
                        Console.Write("#");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine("|");
            }
            Console.WriteLine(new string('-', Board.Width + 2));
            
            // Display next piece info
            Console.WriteLine("Next piece: " + game.NextPiece.Name);
            
            // Instructions
            Console.WriteLine("\nControls (not active in this demo):");
            Console.WriteLine("← → : Move left/right");
            Console.WriteLine("↑   : Rotate clockwise");
            Console.WriteLine("↓   : Fast drop (hold)");
            Console.WriteLine("Space: Hard drop");
        }

        /// <summary>
        /// Entry point for running just this demo.
        /// </summary>
        public static void Main(string[] args)
        {
            RunDemo().GetAwaiter().GetResult();
        }
    }
}

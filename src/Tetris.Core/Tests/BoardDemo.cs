using System;
using Tetris.Core.Models;

namespace Tetris.Core.Tests
{
    /// <summary>
    /// A simple demonstration of the Board class functionality.
    /// </summary>
    public class BoardDemo
    {
        public static void Main()
        {
            Console.WriteLine("Tetris Board Demonstration");
            Console.WriteLine("=========================");

            // Create a new board
            var board = new Board();
            Console.WriteLine("Empty board:");
            Console.WriteLine(board);

            // Add some blocks to form a pattern
            Console.WriteLine("Adding blocks to the board...");

            // Add blocks to form a partial row at the bottom
            for (int x = 0; x < 8; x++)
            {
                board.AddBlock(x, Board.Height - 1, 1);
            }

            // Add a column of blocks
            for (int y = Board.Height - 5; y < Board.Height; y++)
            {
                board.AddBlock(8, y, 2);
            }

            // Add a complete row
            for (int x = 0; x < Board.Width; x++)
            {
                board.AddBlock(x, Board.Height - 2, 3);
            }

            Console.WriteLine("Board after adding blocks:");
            Console.WriteLine(board);

            // Check for collisions
            var willCollide = board.CheckCollision(
                new[] { (8, Board.Height - 1), (9, Board.Height - 1) }
            );
            Console.WriteLine(
                $"Will adding blocks at positions (8,{Board.Height - 1}) and (9,{Board.Height - 1}) cause a collision? {willCollide}"
            );

            // Remove full rows
            int rowsRemoved = board.RemoveFullRows();
            Console.WriteLine($"Removed {rowsRemoved} full rows");
            Console.WriteLine("Board after removing full rows:");
            Console.WriteLine(board);

            // Check if game is over
            bool isGameOver = board.IsGameOver();
            Console.WriteLine($"Is game over? {isGameOver}");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}

using System;
using System.Drawing;
using System.Text;
using Tetris.Core.Models;

namespace Tetris.Core.Tests;

/// <summary>
/// Demonstrates the functionality of the Tetromino classes.
/// </summary>
public class TetrominoDemo
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Tetromino Class Hierarchy Demonstration");
        Console.WriteLine("======================================");

        // Create all tetromino types
        Tetromino[] tetrominos = new Tetromino[]
        {
            new ITetromino(),
            new JTetromino(),
            new LTetromino(),
            new OTetromino(),
            new STetromino(),
            new TTetromino(),
            new ZTetromino()
        };

        // Display information about each tetromino
        foreach (Tetromino tetromino in tetrominos)
        {
            Console.WriteLine($"\nTetromino Type: {tetromino.Name}");
            Console.WriteLine($"ID: {tetromino.Id}");
            Console.WriteLine($"Color: {tetromino.Color.Name}");

            // Demonstrate rotation states
            for (int rotation = 0; rotation < 4; rotation++)
            {
                // Set rotation state
                for (int i = 0; i < rotation; i++)
                {
                    tetromino.RotateClockwise();
                }

                Console.WriteLine($"\nRotation State {rotation} (Degrees: {rotation * 90}):");
                DisplayTetrominoShape(tetromino);

                // Reset rotation for next demonstration
                tetromino.Reset();
            }
        }

        // Demonstrate using the TetrominoFactory
        Console.WriteLine("\nTetrominoFactory Demonstration:");
        Console.WriteLine("------------------------------");

        // Create a specific tetromino
        Tetromino specificTetromino = TetrominoFactory.CreateTetromino(
            TetrominoFactory.TetrominoType.T
        );
        Console.WriteLine($"Created specific tetromino: {specificTetromino.Name}");

        // Create a random tetromino
        Tetromino randomTetromino = TetrominoFactory.CreateRandomTetromino();
        Console.WriteLine($"Created random tetromino: {randomTetromino.Name}");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Displays a visual representation of a tetromino in the console.
    /// </summary>
    /// <param name="tetromino">The tetromino to display.</param>
    private static void DisplayTetrominoShape(Tetromino tetromino)
    {
        // Create a 4x4 grid to display the tetromino
        char[,] grid = new char[4, 4];
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                grid[y, x] = '.';
            }
        }

        // Place the tetromino blocks on the grid
        foreach (Point block in tetromino.Blocks)
        {
            if (block.X >= 0 && block.X < 4 && block.Y >= 0 && block.Y < 4)
            {
                grid[block.Y, block.X] = '#';
            }
        }

        // Display the grid
        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < 4; y++)
        {
            for (int x = 0; x < 4; x++)
            {
                sb.Append(grid[y, x]).Append(' ');
            }
            sb.AppendLine();
        }

        Console.WriteLine(sb.ToString());
    }
}

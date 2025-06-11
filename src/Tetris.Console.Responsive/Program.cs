using System;
using Tetris.Core.UI;

namespace Tetris.Console.Responsive
{
    /// <summary>
    /// Program entry point for the Tetris game with responsive interface.
    /// </summary>
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // Set console properties for optimal game display
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            System.Console.Title = "Responsive Tetris";
            System.Console.CursorVisible = false;

            // Create game instance with responsive interface enabled
            using (var game = new TetrisGame().EnableResponsiveInterface())
            {
                await game.RunAsync();
            }
        }
    }
}

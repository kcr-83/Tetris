using System;
using System.Threading.Tasks;
using Tetris.Core.UI;

namespace Tetris.Core.Tests
{
    /// <summary>
    /// A demo application that showcases the Tetris game with the main menu interface.
    /// </summary>
    public static class MainMenuDemo
    {
        /// <summary>
        /// Main entry point for the demo.
        /// </summary>
        public static async Task RunDemo()
        {
            Console.WriteLine("=== Tetris Game with Menu Interface Demo ===");
            Console.WriteLine("This demo shows the main menu and game integration");
            Console.WriteLine();
            Console.WriteLine("Press any key to start...");
            Console.ReadKey(true);

            // Create and run the Tetris game
            using (var game = new TetrisGame())
            {
                await game.RunAsync();
            }
        }
        
        /// <summary>
        /// Application entry point for the example.
        /// </summary>
        public static void Main()
        {
            RunDemo().GetAwaiter().GetResult();
        }
    }
}

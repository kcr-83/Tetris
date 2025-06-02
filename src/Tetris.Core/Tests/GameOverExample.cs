using System;
using Tetris.Core.Models;
using Tetris.Core.UI;

namespace Tetris.Core.Tests
{
    /// <summary>
    /// Example showing how to handle game over events in a Tetris game.
    /// </summary>
    public class GameOverExample
    {
        private GameEngine _gameEngine;
        private GameOverDisplay _gameOverDisplay;
        private bool _gameRunning;

        /// <summary>
        /// Sets up the game and initializes event handling.
        /// </summary>
        public void Run()
        {
            // Create the game engine
            _gameEngine = new GameEngine();
            
            // Create the game over display
            _gameOverDisplay = new GameOverDisplay(_gameEngine);
            
            // Subscribe to game over event to know when to stop the game loop
            _gameEngine.GameOver += (sender, e) => 
            {
                _gameRunning = false;
                
                // Display restart instructions (will be handled by GameOverDisplay)
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey(true);
            };
            
            // Start a new game
            StartNewGame();
            
            // Clean up resources
            _gameEngine.Dispose();
        }
        
        /// <summary>
        /// Starts a new game and sets up the game loop.
        /// </summary>
        private void StartNewGame()
        {
            _gameRunning = true;
            _gameEngine.StartNewGame();
            
            // Game loop would go here in a real implementation
            // This is a simplified example
            
            // For testing purposes, we'll simulate the game until game over
            SimulateGameUntilGameOver();
        }
        
        /// <summary>
        /// Simulates gameplay until game over for demonstration purposes.
        /// </summary>
        private void SimulateGameUntilGameOver()
        {
            // Fill the board to force a game over
            // In a real game, this would happen naturally during gameplay
            
            // Force a game over by calling EndGame directly for demonstration
            // In a real game, this would happen when blocks stack to the top
            typeof(GameEngine).GetMethod("EndGame", 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Instance)?.Invoke(_gameEngine, 
                    new object[] { GameOverReason.BoardFull });
        }
        
        /// <summary>
        /// Application entry point for the example.
        /// </summary>
        public static void Main()
        {
            var example = new GameOverExample();
            example.Run();
        }
    }
}

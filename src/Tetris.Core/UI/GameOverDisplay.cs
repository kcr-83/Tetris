using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Handles displaying the game over message and statistics when a game ends.
    /// </summary>
    public class GameOverDisplay : IDisposable
    {
        #region Fields
        
        private readonly GameEngine _gameEngine;
        private bool _isShowing;
        
        #endregion
        
        #region Events
        
        /// <summary>
        /// Event raised when the user wants to return to the main menu.
        /// </summary>
        public event EventHandler? ReturnToMenuRequested;
          /// <summary>
        /// Event raised when the user wants to start a new game.
        /// </summary>
        public event EventHandler<GameModeSelectionEventArgs>? NewGameRequested;
        
        #endregion
        
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GameOverDisplay class.
        /// </summary>
        /// <param name="gameEngine">The game engine to monitor for game over events.</param>
        public GameOverDisplay(GameEngine gameEngine)
        {
            _gameEngine = gameEngine ?? throw new ArgumentNullException(nameof(gameEngine));
            _gameEngine.GameOver += OnGameOver;
        }
        
        #endregion
        
        #region Public Methods
        
        /// <summary>
        /// Handles the GameOver event by displaying a message with game statistics.
        /// </summary>
        private async void OnGameOver(object? sender, GameOverEventArgs e)
        {
            await DisplayGameOverMessage(e);
        }

        /// <summary>
        /// Displays a game over message with the final score and statistics.
        /// </summary>
        /// <param name="gameOverData">The game over event arguments containing game statistics.</param>
        public async Task DisplayGameOverMessage(GameOverEventArgs gameOverData)
        {
            _isShowing = true;
            
            var messageBuilder = new StringBuilder();
            
            messageBuilder.AppendLine("*********************************");
            messageBuilder.AppendLine("*           GAME OVER           *");
            messageBuilder.AppendLine("*********************************");
            messageBuilder.AppendLine();
            
            // Display reason for game over
            switch (gameOverData.Reason)
            {
                case GameOverReason.BoardFull:
                    messageBuilder.AppendLine("The board is full! You've reached the top!");
                    break;
                case GameOverReason.NoSpaceForNewPiece:
                    messageBuilder.AppendLine("No space to place a new block!");
                    break;
                case GameOverReason.PlayerEnded:
                    messageBuilder.AppendLine("Game ended by player.");
                    break;
            }
            
            messageBuilder.AppendLine();
            messageBuilder.AppendLine($"Final Score: {gameOverData.FinalScore}");
            messageBuilder.AppendLine($"Level Reached: {gameOverData.FinalLevel}");
            messageBuilder.AppendLine($"Total Rows Cleared: {gameOverData.TotalRowsCleared}");
            messageBuilder.AppendLine();
            
            // Display line clear statistics
            messageBuilder.AppendLine("Line Clear Statistics:");
            foreach (var stat in gameOverData.LineStatistics.Where(s => s.Value > 0))
            {
                messageBuilder.AppendLine($"  {stat.Key}: {stat.Value}");
            }
            
            messageBuilder.AppendLine();
            messageBuilder.AppendLine("Press [Enter] to start a new game");
            messageBuilder.AppendLine("Press [Esc] to return to main menu");
            
            // Display the message with animation
            await DisplayMessageWithAnimationAsync(messageBuilder.ToString());
            
            // Wait for user input
            await WaitForUserResponseAsync();
        }
        
        /// <summary>
        /// Releases resources used by the GameOverDisplay.
        /// </summary>
        public void Dispose()
        {
            if (_gameEngine != null)
            {
                _gameEngine.GameOver -= OnGameOver;
            }
        }
        
        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Displays a message with a typewriter animation effect.
        /// </summary>
        /// <param name="message">The message to display.</param>
        private async Task DisplayMessageWithAnimationAsync(string message)
        {
            // Clear the console
            Console.Clear();
            Console.CursorVisible = false;
            
            // Calculate the center position for the message
            var lines = message.Split(Environment.NewLine);
            int maxWidth = lines.Max(l => l.Length);
            int startX = Math.Max(0, (Console.WindowWidth - maxWidth) / 2);
            int startY = Math.Max(0, (Console.WindowHeight - lines.Length) / 2);
            
            // Display each line with animation
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                Console.SetCursorPosition(startX, startY + lineIndex);
                
                // Change color based on content
                if (lines[lineIndex].Contains("GAME OVER"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (lines[lineIndex].Contains("Final Score:"))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else if (lines[lineIndex].Contains("Level Reached:"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (lines[lineIndex].Contains("Line Clear Statistics:"))
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                else if (lines[lineIndex].Contains("Press"))
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                
                // For the title lines, display them instantly
                if (lines[lineIndex].Contains("*"))
                {
                    Console.WriteLine(lines[lineIndex]);
                    continue;
                }
                
                // For other lines, use typewriter effect
                foreach (char c in lines[lineIndex])
                {
                    Console.Write(c);
                    await Task.Delay(10); // Adjust speed as needed
                }
                
                Console.WriteLine();
            }
            
            Console.ForegroundColor = ConsoleColor.White;
        }
        
        /// <summary>
        /// Waits for the user to press Enter or Escape.
        /// </summary>
        private async Task WaitForUserResponseAsync()
        {
            while (_isShowing)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                      if (key.Key == ConsoleKey.Enter)
                    {
                        _isShowing = false;
                        
                        // Show difficulty selection dialog
                        var difficultyDialog = new DifficultySelectionDialog();
                        var difficulty = await difficultyDialog.ShowAsync();
                        
                        // Show game mode selection dialog
                        var gameModeDialog = new GameModeSelectionDialog(difficulty);
                        var gameMode = await gameModeDialog.ShowAsync();
                        
                        // Pass the selected difficulty and game mode as event args
                        NewGameRequested?.Invoke(this, new GameModeSelectionEventArgs(gameMode, difficulty));
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        _isShowing = false;
                        ReturnToMenuRequested?.Invoke(this, EventArgs.Empty);
                    }
                }
                
                await Task.Delay(50); // Short delay to prevent CPU overuse
            }
        }
        
        #endregion
    }
}

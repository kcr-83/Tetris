using System;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Dialog for selecting game mode before starting a new game.
    /// </summary>
    public class GameModeSelectionDialog
    {
        private readonly string _title = "SELECT GAME MODE";
        private readonly GameMode[] _gameModes = new[] 
        { 
            GameMode.Classic, 
            GameMode.Timed, 
            GameMode.Challenge 
        };
        private int _selectedIndex = 0; // Default to Classic
        
        private readonly DifficultyLevel _selectedDifficulty;
        
        /// <summary>
        /// Initializes a new instance of the GameModeSelectionDialog class.
        /// </summary>
        /// <param name="difficulty">The selected difficulty level.</param>
        public GameModeSelectionDialog(DifficultyLevel difficulty)
        {
            _selectedDifficulty = difficulty;
        }
        
        /// <summary>
        /// Shows the game mode selection dialog and returns the selected game mode.
        /// </summary>
        /// <returns>The selected game mode.</returns>
        public async Task<GameMode> ShowAsync()
        {
            Console.Clear();
            Console.CursorVisible = false;
            
            // Store original colors to restore later
            ConsoleColor originalForeground = Console.ForegroundColor;
            ConsoleColor originalBackground = Console.BackgroundColor;
            
            // Set menu colors
            Console.BackgroundColor = ConsoleColor.Black;
            
            bool selectionMade = false;
            
            while (!selectionMade)
            {
                RenderDialog();
                
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            _selectedIndex = Math.Max(0, _selectedIndex - 1);
                            break;
                            
                        case ConsoleKey.DownArrow:
                            _selectedIndex = Math.Min(_gameModes.Length - 1, _selectedIndex + 1);
                            break;
                            
                        case ConsoleKey.Enter:
                            selectionMade = true;
                            break;
                            
                        case ConsoleKey.Escape:
                            // Default to Classic if user cancels
                            _selectedIndex = 0;
                            selectionMade = true;
                            break;
                    }
                }
                
                await Task.Delay(50); // Prevent CPU overuse
            }
            
            // Restore console colors
            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;
            
            return _gameModes[_selectedIndex];
        }
        
        /// <summary>
        /// Renders the game mode selection dialog.
        /// </summary>
        private void RenderDialog()
        {
            Console.Clear();
            
            // Center the title
            int titleX = Console.WindowWidth / 2 - _title.Length / 2;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(titleX, 2);
            Console.WriteLine(_title);
            
            // Show the difficulty level
            Console.ForegroundColor = ConsoleColor.Yellow;
            string difficultyText = $"Difficulty: {_selectedDifficulty}";
            int difficultyX = Console.WindowWidth / 2 - difficultyText.Length / 2;
            Console.SetCursorPosition(difficultyX, 4);
            Console.WriteLine(difficultyText);
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(Console.WindowWidth / 2 - 25, 6);
            Console.WriteLine("Select a game mode:");
            
            // Draw options
            int startY = 8;
            for (int i = 0; i < _gameModes.Length; i++)
            {
                if (_selectedIndex == i)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 24, startY + i * 2);
                    Console.Write("> ");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.SetCursorPosition(Console.WindowWidth / 2 - 24, startY + i * 2);
                    Console.Write("  ");
                }
                
                string modeName = $"{_gameModes[i]}";
                Console.Write(modeName);
                
                // Draw description
                Console.SetCursorPosition(Console.WindowWidth / 2 - 10, startY + i * 2);
                
                switch (_gameModes[i])
                {
                    case GameMode.Classic:
                        Console.WriteLine("- Play until game over with increasing difficulty");
                        break;
                    case GameMode.Timed:
                        int seconds = GameModeSettings.GetTimedModeSeconds(_selectedDifficulty);
                        Console.WriteLine($"- Score as many points as possible in {seconds} seconds");
                        break;
                    case GameMode.Challenge:
                        int rows = GameModeSettings.GetChallengeRowsTarget(_selectedDifficulty);
                        Console.WriteLine($"- Clear {rows} rows to win");
                        break;
                }
            }
            
            // Draw instructions
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.SetCursorPosition(Console.WindowWidth / 2 - 25, startY + _gameModes.Length * 2 + 2);
            Console.WriteLine("Use arrow keys to navigate and ENTER to select");
        }
    }
}

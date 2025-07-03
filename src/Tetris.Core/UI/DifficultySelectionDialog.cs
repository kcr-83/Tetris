using System;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.UI
{
    /// <summary>
    /// Dialog for selecting game difficulty level before starting a new game.
    /// </summary>
    public class DifficultySelectionDialog
    {
        private readonly string _title = "SELECT DIFFICULTY";
        private readonly DifficultyLevel[] _difficultyLevels = new[] 
        { 
            DifficultyLevel.Easy, 
            DifficultyLevel.Medium, 
            DifficultyLevel.Hard 
        };
        private int _selectedIndex = 1; // Default to Medium
        
        /// <summary>
        /// Shows the difficulty selection dialog and returns the selected difficulty.
        /// </summary>
        /// <returns>The selected difficulty level.</returns>
        public async Task<DifficultyLevel> ShowAsync()
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
                            _selectedIndex = Math.Min(_difficultyLevels.Length - 1, _selectedIndex + 1);
                            break;
                        
                        case ConsoleKey.Enter:
                            selectionMade = true;
                            break;
                        
                        case ConsoleKey.Escape:
                            // Return default (Medium) on escape
                            Console.ForegroundColor = originalForeground;
                            Console.BackgroundColor = originalBackground;
                            return DifficultyLevel.Medium;
                    }
                }
                
                await Task.Delay(50); // Prevent high CPU usage
            }
            
            // Restore original console colors
            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;
            
            return _difficultyLevels[_selectedIndex];
        }
        
        /// <summary>
        /// Renders the difficulty selection dialog.
        /// </summary>
        private void RenderDialog()
        {
            Console.Clear();
            
            // Calculate positions for centered display
            int windowWidth = Console.WindowWidth;
            int windowHeight = Console.WindowHeight;
            
            int dialogWidth = 50;
            int dialogHeight = 10 + _difficultyLevels.Length;
            
            int startX = Math.Max(0, (windowWidth - dialogWidth) / 2);
            int startY = Math.Max(0, (windowHeight - dialogHeight) / 2);
            
            // Draw title
            Console.ForegroundColor = ConsoleColor.Cyan;
            string titleText = $"=== {_title} ===";
            Console.SetCursorPosition(startX + (dialogWidth - titleText.Length) / 2, startY);
            Console.WriteLine(titleText);
            Console.WriteLine();
            
            // Draw instructions
            Console.ForegroundColor = ConsoleColor.White;
            string instructionsText = "Choose your difficulty level:";
            Console.SetCursorPosition(startX + (dialogWidth - instructionsText.Length) / 2, startY + 2);
            Console.WriteLine(instructionsText);
            Console.WriteLine();
            
            // Draw difficulty options
            for (int i = 0; i < _difficultyLevels.Length; i++)
            {
                DifficultyLevel difficulty = _difficultyLevels[i];
                string difficultyName = DifficultySettings.GetDisplayName(difficulty);
                string difficultyDescription = DifficultySettings.GetDescription(difficulty);
                
                // Set color based on selection
                if (i == _selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                
                // Draw selection indicator and difficulty name
                Console.SetCursorPosition(startX + 5, startY + 4 + i * 2);
                Console.Write(i == _selectedIndex ? "> " : "  ");
                Console.Write($"{difficultyName}");
                
                // Draw description
                Console.SetCursorPosition(startX + 5, startY + 5 + i * 2);
                Console.Write($"  {difficultyDescription}");
            }
            
            // Draw footer
            Console.ForegroundColor = ConsoleColor.Gray;
            string footerText = "Use UP/DOWN arrows to select, ENTER to confirm";
            Console.SetCursorPosition(startX + (dialogWidth - footerText.Length) / 2, startY + dialogHeight - 2);
            Console.WriteLine(footerText);
        }
    }
}

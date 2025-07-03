using System;
using System.Threading.Tasks;

namespace Tetris.Core.UI;

/// <summary>
/// Dialog for saving the current game state.
/// </summary>
public class SaveGameDialog
{
    private readonly string _title = "SAVE GAME";
    private string _saveName = string.Empty;
    private bool _dialogActive = true;

    /// <summary>
    /// Shows the save game dialog and returns the entered save name.
    /// </summary>
    /// <returns>The save name entered by the user, or null if cancelled.</returns>
    public async Task<string?> ShowAsync()
    {
        Console.Clear();
        Console.CursorVisible = true;
        
        // Store original colors to restore later
        ConsoleColor originalForeground = Console.ForegroundColor;
        ConsoleColor originalBackground = Console.BackgroundColor;
        
        // Set dialog colors
        Console.BackgroundColor = ConsoleColor.Black;
        
        _dialogActive = true;
        _saveName = string.Empty;
        
        while (_dialogActive)
        {
            RenderDialog();
            
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                
                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    if (!string.IsNullOrWhiteSpace(_saveName) && _saveName.Length >= 1)
                    {
                        _dialogActive = false;
                        break;
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    _saveName = string.Empty;
                    _dialogActive = false;
                    break;
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (_saveName.Length > 0)
                    {
                        _saveName = _saveName[..^1];
                    }
                }
                else if (char.IsLetterOrDigit(keyInfo.KeyChar) || 
                         char.IsWhiteSpace(keyInfo.KeyChar) || 
                         keyInfo.KeyChar == '-' || 
                         keyInfo.KeyChar == '_')
                {
                    if (_saveName.Length < 50) // Limit save name length
                    {
                        _saveName += keyInfo.KeyChar;
                    }
                }
            }
            
            await Task.Delay(50); // Prevent CPU overuse
        }
        
        // Restore console settings
        Console.CursorVisible = false;
        Console.ForegroundColor = originalForeground;
        Console.BackgroundColor = originalBackground;
        
        return string.IsNullOrWhiteSpace(_saveName) ? null : _saveName.Trim();
    }
    
    /// <summary>
    /// Renders the save game dialog.
    /// </summary>
    private void RenderDialog()
    {
        Console.Clear();
        
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        
        // Center the dialog
        int dialogWidth = 50;
        int dialogHeight = 10;
        int startX = (windowWidth - dialogWidth) / 2;
        int startY = (windowHeight - dialogHeight) / 2;
        
        // Draw title
        Console.ForegroundColor = ConsoleColor.Cyan;
        int titleX = startX + (dialogWidth - _title.Length) / 2;
        Console.SetCursorPosition(titleX, startY);
        Console.WriteLine(_title);
        
        // Draw dialog box
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(startX, startY + 2);
        Console.WriteLine("╔" + new string('═', dialogWidth - 2) + "╗");
        
        for (int i = 0; i < 5; i++)
        {
            Console.SetCursorPosition(startX, startY + 3 + i);
            Console.WriteLine("║" + new string(' ', dialogWidth - 2) + "║");
        }
        
        Console.SetCursorPosition(startX, startY + 8);
        Console.WriteLine("╚" + new string('═', dialogWidth - 2) + "╝");
        
        // Draw prompt
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetCursorPosition(startX + 2, startY + 4);
        Console.WriteLine("Enter save name:");
        
        // Draw input field
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(startX + 2, startY + 5);
        Console.Write("[" + _saveName.PadRight(44) + "]");
        
        // Draw instructions
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.SetCursorPosition(startX + 2, startY + 7);
        Console.WriteLine("ENTER to save, ESC to cancel");
        
        // Position cursor for input
        Console.SetCursorPosition(startX + 3 + _saveName.Length, startY + 5);
    }
}

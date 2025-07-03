using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tetris.Core.Services;

namespace Tetris.Core.UI;

/// <summary>
/// Dialog for loading saved games.
/// </summary>
public class LoadGameDialog
{
    private readonly string _title = "LOAD GAME";
    private readonly GameSaveService _saveService;
    private List<SaveFileInfo> _saveFiles = new();
    private int _selectedIndex = 0;
    
    /// <summary>
    /// Initializes a new instance of the LoadGameDialog class.
    /// </summary>
    public LoadGameDialog()
    {
        _saveService = new GameSaveService();
    }
    
    /// <summary>
    /// Shows the load game dialog and returns the selected save file info.
    /// </summary>
    /// <returns>The selected save file info, or null if cancelled.</returns>
    public async Task<SaveFileInfo?> ShowAsync()
    {
        Console.Clear();
        Console.CursorVisible = false;
        
        // Store original colors to restore later
        ConsoleColor originalForeground = Console.ForegroundColor;
        ConsoleColor originalBackground = Console.BackgroundColor;
        
        // Set dialog colors
        Console.BackgroundColor = ConsoleColor.Black;
        
        // Load save files
        try
        {
            _saveFiles = await _saveService.GetSaveFilesAsync();
        }
        catch (Exception ex)
        {
            ShowError($"Failed to load save files: {ex.Message}");
            return null;
        }
        
        if (_saveFiles.Count == 0)
        {
            ShowError("No saved games found.");
            return null;
        }
        
        _selectedIndex = 0;
        bool dialogActive = true;
        SaveFileInfo? selectedSave = null;
        
        while (dialogActive)
        {
            RenderDialog();
            
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(true);
                
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        _selectedIndex = Math.Max(0, _selectedIndex - 1);
                        break;
                        
                    case ConsoleKey.DownArrow:
                        _selectedIndex = Math.Min(_saveFiles.Count - 1, _selectedIndex + 1);
                        break;
                        
                    case ConsoleKey.Enter:
                        selectedSave = _saveFiles[_selectedIndex];
                        dialogActive = false;
                        break;
                        
                    case ConsoleKey.Delete:
                        if (_saveFiles.Count > 0)
                        {
                            await DeleteSelectedSave();
                            if (_saveFiles.Count == 0)
                            {
                                dialogActive = false;
                            }
                            else if (_selectedIndex >= _saveFiles.Count)
                            {
                                _selectedIndex = _saveFiles.Count - 1;
                            }
                        }
                        break;
                        
                    case ConsoleKey.Escape:
                        dialogActive = false;
                        break;
                }
            }
            
            await Task.Delay(50); // Prevent CPU overuse
        }
        
        // Restore console settings
        Console.ForegroundColor = originalForeground;
        Console.BackgroundColor = originalBackground;
        
        return selectedSave;
    }
    
    /// <summary>
    /// Renders the load game dialog.
    /// </summary>
    private void RenderDialog()
    {
        Console.Clear();
        
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        
        // Draw title
        Console.ForegroundColor = ConsoleColor.Cyan;
        int titleX = (windowWidth - _title.Length) / 2;
        Console.SetCursorPosition(titleX, 2);
        Console.WriteLine(_title);
        
        // Draw header
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetCursorPosition(5, 4);
        Console.WriteLine("Save Name".PadRight(25) + "Date".PadRight(20) + "Mode".PadRight(12) + "Score".PadRight(10) + "Level");
        
        Console.SetCursorPosition(5, 5);
        Console.WriteLine(new string('─', windowWidth - 10));
        
        // Draw save files
        int startY = 6;
        int maxDisplayItems = Math.Min(_saveFiles.Count, windowHeight - 12);
        int scrollOffset = Math.Max(0, _selectedIndex - maxDisplayItems + 1);
        
        for (int i = 0; i < maxDisplayItems; i++)
        {
            int saveIndex = i + scrollOffset;
            if (saveIndex >= _saveFiles.Count) break;
            
            SaveFileInfo save = _saveFiles[saveIndex];
            bool isSelected = saveIndex == _selectedIndex;
            
            Console.ForegroundColor = isSelected ? ConsoleColor.Yellow : ConsoleColor.White;
            Console.BackgroundColor = isSelected ? ConsoleColor.DarkBlue : ConsoleColor.Black;
            
            Console.SetCursorPosition(5, startY + i);
            
            string saveName = save.SaveName.Length > 22 ? save.SaveName[..19] + "..." : save.SaveName;
            string saveDate = save.SavedAt.ToString("yyyy-MM-dd HH:mm");
            string gameMode = save.GameMode.Length > 9 ? save.GameMode[..6] + "..." : save.GameMode;
            
            string line = saveName.PadRight(25) + 
                         saveDate.PadRight(20) + 
                         gameMode.PadRight(12) + 
                         save.Score.ToString().PadRight(10) + 
                         save.Level.ToString();
            
            Console.WriteLine(line.PadRight(windowWidth - 10));
        }
        
        // Reset background
        Console.BackgroundColor = ConsoleColor.Black;
        
        // Draw instructions
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.SetCursorPosition(5, windowHeight - 4);
        Console.WriteLine("↑↓ Navigate  ENTER Load  DEL Delete  ESC Cancel");
        
        // Show selection info
        if (_saveFiles.Count > 0 && _selectedIndex < _saveFiles.Count)
        {
            SaveFileInfo selectedSave = _saveFiles[_selectedIndex];
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.SetCursorPosition(5, windowHeight - 2);
            Console.WriteLine($"Selected: {selectedSave.SaveName} ({selectedSave.Difficulty})");
        }
    }
    
    /// <summary>
    /// Deletes the currently selected save file.
    /// </summary>
    private async Task DeleteSelectedSave()
    {
        if (_selectedIndex < 0 || _selectedIndex >= _saveFiles.Count)
            return;
            
        SaveFileInfo saveToDelete = _saveFiles[_selectedIndex];
        
        // Show confirmation
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(5, Console.WindowHeight - 6);
        Console.WriteLine($"Delete '{saveToDelete.SaveName}'? (Y/N)".PadRight(Console.WindowWidth - 10));
        
        var key = Console.ReadKey(true);
        if (key.Key == ConsoleKey.Y)
        {
            try
            {
                await _saveService.DeleteSaveAsync(saveToDelete.SaveName);
                _saveFiles.RemoveAt(_selectedIndex);
            }
            catch (Exception ex)
            {
                ShowError($"Failed to delete save: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Shows an error message and waits for user input.
    /// </summary>
    /// <param name="message">The error message to display.</param>
    private static void ShowError(string message)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        
        Console.SetCursorPosition((windowWidth - "ERROR".Length) / 2, windowHeight / 2 - 2);
        Console.WriteLine("ERROR");
        
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition((windowWidth - message.Length) / 2, windowHeight / 2);
        Console.WriteLine(message);
        
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.SetCursorPosition((windowWidth - "Press any key to continue...".Length) / 2, windowHeight / 2 + 2);
        Console.WriteLine("Press any key to continue...");
        
        Console.ReadKey(true);
    }
}

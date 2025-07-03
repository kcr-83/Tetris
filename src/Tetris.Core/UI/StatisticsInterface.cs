using System;
using System.Threading.Tasks;
using Tetris.Core.Models;
using Tetris.Core.Services;

namespace Tetris.Core.UI;

/// <summary>
/// Interface for displaying game statistics.
/// </summary>
public class StatisticsInterface
{
    #region Fields

    private readonly GameStatisticsService _statisticsService;
    private bool _isShowing;
    private GameStatistics _currentStatistics;

    #endregion

    #region Events

    /// <summary>
    /// Event raised when the user wants to return to the main menu.
    /// </summary>
    public event EventHandler? ReturnToMenuRequested;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the StatisticsInterface class.
    /// </summary>
    public StatisticsInterface()
    {
        _statisticsService = new GameStatisticsService();
        _currentStatistics = new GameStatistics();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Shows the statistics interface.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ShowAsync()
    {
        try
        {
            _currentStatistics = await _statisticsService.GetStatisticsAsync();
            _isShowing = true;

            Console.Clear();
            Console.CursorVisible = false;

            while (_isShowing)
            {
                RenderStatistics();
                
                if (Console.KeyAvailable)
                {
                    await HandleInputAsync();
                }

                await Task.Delay(50);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Error loading statistics: {ex.Message}");
            await Task.Delay(2000);
            ReturnToMenuRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Renders the statistics display.
    /// </summary>
    private void RenderStatistics()
    {
        Console.Clear();
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;

        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;

        // Title
        Console.ForegroundColor = ConsoleColor.Cyan;
        string title = "TETRIS STATISTICS";
        int titleX = (windowWidth - title.Length) / 2;
        Console.SetCursorPosition(titleX, 2);
        Console.WriteLine(title);

        // Draw border
        Console.ForegroundColor = ConsoleColor.Gray;
        int borderWidth = Math.Min(60, windowWidth - 4);
        int borderX = (windowWidth - borderWidth) / 2;
        int startY = 4;

        Console.SetCursorPosition(borderX, startY);
        Console.WriteLine("╔" + new string('═', borderWidth - 2) + "╗");

        // Main statistics
        Console.ForegroundColor = ConsoleColor.Yellow;
        int contentX = borderX + 2;
        int currentY = startY + 2;

        // Games played section
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Games Played: {_currentStatistics.TotalGamesPlayed}");
        
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Games Completed: {_currentStatistics.TotalGamesCompleted}");
        
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Completion Rate: {_currentStatistics.CompletionRate:F1}%");
        
        currentY++;

        // Score section
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Highest Score: {_currentStatistics.HighestScore:N0}");
        
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Total Score: {_currentStatistics.TotalScore:N0}");
        
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Average Score: {_currentStatistics.AverageScore:F0}");
        
        currentY++;

        // Level and rows section
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Highest Level: {_currentStatistics.HighestLevel}");
        
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Total Rows Cleared: {_currentStatistics.TotalRowsCleared:N0}");
        
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Average Rows per Game: {_currentStatistics.AverageRowsPerGame:F1}");
        
        currentY++;

        // Row clearing statistics
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Single Line Clears: {_currentStatistics.SingleRowClears}");
        
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Double Line Clears: {_currentStatistics.DoubleRowClears}");
        
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Triple Line Clears: {_currentStatistics.TripleRowClears}");
        
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Tetris (4 Lines): {_currentStatistics.TetrisRowClears}");
        
        currentY++;

        // Time statistics
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Total Time Played: {_currentStatistics.FormattedTotalTime}");
        
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Average Time per Game: {TimeSpan.FromSeconds(_currentStatistics.AverageTimePerGame):mm\\:ss}");
        
        currentY++;

        // Timestamps
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.SetCursorPosition(contentX, currentY++);
        Console.WriteLine($"Last Updated: {_currentStatistics.LastUpdated:yyyy-MM-dd HH:mm:ss}");

        // Close border
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.SetCursorPosition(borderX, currentY + 1);
        Console.WriteLine("╚" + new string('═', borderWidth - 2) + "╝");

        // Instructions
        Console.ForegroundColor = ConsoleColor.White;
        int instructionsY = currentY + 3;
        
        Console.SetCursorPosition(contentX, instructionsY);
        Console.WriteLine("R: Reset Statistics");
        Console.SetCursorPosition(contentX, instructionsY + 1);
        Console.WriteLine("ESC: Return to Menu");
        Console.SetCursorPosition(contentX, instructionsY + 2);
        Console.WriteLine("F5: Refresh Statistics");
    }

    /// <summary>
    /// Handles user input for the statistics interface.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task HandleInputAsync()
    {
        var key = Console.ReadKey(true);

        switch (key.Key)
        {
            case ConsoleKey.Escape:
                _isShowing = false;
                ReturnToMenuRequested?.Invoke(this, EventArgs.Empty);
                break;

            case ConsoleKey.R:
                await HandleResetStatisticsAsync();
                break;

            case ConsoleKey.F5:
                await RefreshStatisticsAsync();
                break;
        }
    }

    /// <summary>
    /// Handles resetting the statistics with confirmation.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task HandleResetStatisticsAsync()
    {
        if (await ShowConfirmationDialog("Are you sure you want to reset all statistics?"))
        {
            try
            {
                await _statisticsService.ResetStatisticsAsync();
                _currentStatistics = await _statisticsService.GetStatisticsAsync();
                ShowSuccessMessage("Statistics reset successfully!");
                await Task.Delay(1500);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Failed to reset statistics: {ex.Message}");
                await Task.Delay(2000);
            }
        }
    }

    /// <summary>
    /// Refreshes the statistics from storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task RefreshStatisticsAsync()
    {
        try
        {
            _currentStatistics = await _statisticsService.GetStatisticsAsync();
            ShowSuccessMessage("Statistics refreshed!");
            await Task.Delay(1000);
        }
        catch (Exception ex)
        {
            ShowErrorMessage($"Failed to refresh statistics: {ex.Message}");
            await Task.Delay(2000);
        }
    }

    /// <summary>
    /// Shows a confirmation dialog.
    /// </summary>
    /// <param name="message">The confirmation message.</param>
    /// <returns>A task that returns true if confirmed, false otherwise.</returns>
    private async Task<bool> ShowConfirmationDialog(string message)
    {
        Console.Clear();
        
        int windowWidth = Console.WindowWidth;
        int windowHeight = Console.WindowHeight;
        
        // Center the dialog
        int dialogWidth = Math.Min(50, windowWidth - 4);
        int dialogHeight = 8;
        int dialogX = (windowWidth - dialogWidth) / 2;
        int dialogY = (windowHeight - dialogHeight) / 2;

        // Draw dialog box
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.SetCursorPosition(dialogX, dialogY);
        Console.WriteLine("╔" + new string('═', dialogWidth - 2) + "╗");
        
        for (int i = 1; i < dialogHeight - 1; i++)
        {
            Console.SetCursorPosition(dialogX, dialogY + i);
            Console.WriteLine("║" + new string(' ', dialogWidth - 2) + "║");
        }
        
        Console.SetCursorPosition(dialogX, dialogY + dialogHeight - 1);
        Console.WriteLine("╚" + new string('═', dialogWidth - 2) + "╝");

        // Message
        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(dialogX + 2, dialogY + 2);
        Console.WriteLine("CONFIRMATION");
        
        Console.SetCursorPosition(dialogX + 2, dialogY + 4);
        Console.WriteLine(message);
        
        Console.SetCursorPosition(dialogX + 2, dialogY + 6);
        Console.WriteLine("Y: Yes    N: No");

        // Wait for input
        while (true)
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Y)
            {
                return true;
            }
            else if (key.Key == ConsoleKey.N || key.Key == ConsoleKey.Escape)
            {
                return false;
            }
            
            await Task.Delay(50);
        }
    }

    /// <summary>
    /// Shows a success message.
    /// </summary>
    /// <param name="message">The success message.</param>
    private void ShowSuccessMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.SetCursorPosition(0, Console.WindowHeight - 2);
        Console.WriteLine($"✓ {message}");
        Console.ResetColor();
    }

    /// <summary>
    /// Shows an error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    private void ShowErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(0, Console.WindowHeight - 2);
        Console.WriteLine($"✗ {message}");
        Console.ResetColor();
    }

    #endregion
}

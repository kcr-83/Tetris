using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.Services;

/// <summary>
/// Service for managing game statistics with file-based persistence.
/// </summary>
public class GameStatisticsService : IGameStatisticsService
{
    #region Constants

    private const string StatisticsDirectory = "Statistics";
    private const string StatisticsFileName = "game_statistics.json";

    #endregion

    #region Fields

    private GameStatistics _statistics;
    private readonly string _statisticsFilePath;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the GameStatisticsService class.
    /// </summary>
    public GameStatisticsService()
    {
        _statisticsFilePath = Path.Combine(StatisticsDirectory, StatisticsFileName);
        _statistics = new GameStatistics();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets the current game statistics.
    /// </summary>
    /// <returns>A task that returns the current game statistics.</returns>
    public async Task<GameStatistics> GetStatisticsAsync()
    {
        await LoadStatisticsAsync();
        return _statistics;
    }

    /// <summary>
    /// Updates the statistics with data from a completed game.
    /// </summary>
    /// <param name="finalScore">The final score of the game.</param>
    /// <param name="finalLevel">The final level reached.</param>
    /// <param name="rowsCleared">The number of rows cleared in the game.</param>
    /// <param name="gameTimeSeconds">The time played in seconds.</param>
    /// <param name="isCompleted">Whether the game was completed or abandoned.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateGameStatisticsAsync(int finalScore, int finalLevel, int rowsCleared, double gameTimeSeconds, bool isCompleted)
    {
        await LoadStatisticsAsync();
        _statistics.UpdateFromGame(finalScore, finalLevel, rowsCleared, gameTimeSeconds, isCompleted);
        await SaveStatisticsAsync();
    }

    /// <summary>
    /// Updates the row clearing statistics.
    /// </summary>
    /// <param name="rowsCleared">The number of rows cleared in a single action.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateRowClearStatisticsAsync(int rowsCleared)
    {
        await LoadStatisticsAsync();
        _statistics.UpdateRowClearStats(rowsCleared);
        await SaveStatisticsAsync();
    }

    /// <summary>
    /// Resets all statistics to their initial values.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ResetStatisticsAsync()
    {
        _statistics.Reset();
        await SaveStatisticsAsync();
    }

    /// <summary>
    /// Saves the current statistics to persistent storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SaveStatisticsAsync()
    {
        try
        {
            EnsureDirectoryExists();

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string json = JsonSerializer.Serialize(_statistics, options);
            await File.WriteAllTextAsync(_statisticsFilePath, json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save statistics: {ex.Message}", ex);
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Loads the statistics from persistent storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task LoadStatisticsAsync()
    {
        try
        {
            if (!File.Exists(_statisticsFilePath))
            {
                _statistics = new GameStatistics();
                return;
            }

            string json = await File.ReadAllTextAsync(_statisticsFilePath);
            
            if (string.IsNullOrWhiteSpace(json))
            {
                _statistics = new GameStatistics();
                return;
            }

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            _statistics = JsonSerializer.Deserialize<GameStatistics>(json, options) ?? new GameStatistics();
        }
        catch (Exception ex)
        {
            // If loading fails, start with fresh statistics
            _statistics = new GameStatistics();
            throw new InvalidOperationException($"Failed to load statistics: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Ensures the statistics directory exists.
    /// </summary>
    private void EnsureDirectoryExists()
    {
        string? directory = Path.GetDirectoryName(_statisticsFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    #endregion
}

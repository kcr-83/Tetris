using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.Services;

/// <summary>
/// Interface for managing game statistics.
/// </summary>
public interface IGameStatisticsService
{
    /// <summary>
    /// Gets the current game statistics.
    /// </summary>
    /// <returns>A task that returns the current game statistics.</returns>
    Task<GameStatistics> GetStatisticsAsync();

    /// <summary>
    /// Updates the statistics with data from a completed game.
    /// </summary>
    /// <param name="finalScore">The final score of the game.</param>
    /// <param name="finalLevel">The final level reached.</param>
    /// <param name="rowsCleared">The number of rows cleared in the game.</param>
    /// <param name="gameTimeSeconds">The time played in seconds.</param>
    /// <param name="isCompleted">Whether the game was completed or abandoned.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateGameStatisticsAsync(int finalScore, int finalLevel, int rowsCleared, double gameTimeSeconds, bool isCompleted);

    /// <summary>
    /// Updates the row clearing statistics.
    /// </summary>
    /// <param name="rowsCleared">The number of rows cleared in a single action.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateRowClearStatisticsAsync(int rowsCleared);

    /// <summary>
    /// Resets all statistics to their initial values.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ResetStatisticsAsync();

    /// <summary>
    /// Saves the current statistics to persistent storage.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveStatisticsAsync();
}

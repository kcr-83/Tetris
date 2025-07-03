using System;
using System.Text.Json.Serialization;

namespace Tetris.Core.Models;

/// <summary>
/// Represents comprehensive game statistics for a Tetris player.
/// </summary>
public class GameStatistics
{
    /// <summary>
    /// Gets or sets the total number of games played.
    /// </summary>
    public int TotalGamesPlayed { get; set; }

    /// <summary>
    /// Gets or sets the total number of games completed (not abandoned).
    /// </summary>
    public int TotalGamesCompleted { get; set; }

    /// <summary>
    /// Gets or sets the highest score achieved.
    /// </summary>
    public int HighestScore { get; set; }

    /// <summary>
    /// Gets or sets the total cumulative score across all games.
    /// </summary>
    public long TotalScore { get; set; }

    /// <summary>
    /// Gets or sets the total number of rows cleared across all games.
    /// </summary>
    public int TotalRowsCleared { get; set; }

    /// <summary>
    /// Gets or sets the highest level reached.
    /// </summary>
    public int HighestLevel { get; set; }

    /// <summary>
    /// Gets or sets the total time played in seconds.
    /// </summary>
    public double TotalTimePlayedSeconds { get; set; }

    /// <summary>
    /// Gets or sets the number of single row clears.
    /// </summary>
    public int SingleRowClears { get; set; }

    /// <summary>
    /// Gets or sets the number of double row clears.
    /// </summary>
    public int DoubleRowClears { get; set; }

    /// <summary>
    /// Gets or sets the number of triple row clears.
    /// </summary>
    public int TripleRowClears { get; set; }

    /// <summary>
    /// Gets or sets the number of Tetris (quad) row clears.
    /// </summary>
    public int TetrisRowClears { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the statistics were last updated.
    /// </summary>
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the statistics were created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the average score per game.
    /// </summary>
    [JsonIgnore]
    public double AverageScore => TotalGamesPlayed > 0 ? (double)TotalScore / TotalGamesPlayed : 0.0;

    /// <summary>
    /// Gets the average time per game in seconds.
    /// </summary>
    [JsonIgnore]
    public double AverageTimePerGame => TotalGamesPlayed > 0 ? TotalTimePlayedSeconds / TotalGamesPlayed : 0.0;

    /// <summary>
    /// Gets the completion rate as a percentage.
    /// </summary>
    [JsonIgnore]
    public double CompletionRate => TotalGamesPlayed > 0 ? (double)TotalGamesCompleted / TotalGamesPlayed * 100 : 0.0;

    /// <summary>
    /// Gets the average rows cleared per game.
    /// </summary>
    [JsonIgnore]
    public double AverageRowsPerGame => TotalGamesPlayed > 0 ? (double)TotalRowsCleared / TotalGamesPlayed : 0.0;

    /// <summary>
    /// Gets the total time played formatted as a string.
    /// </summary>
    [JsonIgnore]
    public string FormattedTotalTime
    {
        get
        {
            var timeSpan = TimeSpan.FromSeconds(TotalTimePlayedSeconds);
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays}d {timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
            else
                return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}";
        }
    }

    /// <summary>
    /// Initializes a new instance of the GameStatistics class.
    /// </summary>
    public GameStatistics()
    {
        CreatedAt = DateTime.Now;
        LastUpdated = DateTime.Now;
    }

    /// <summary>
    /// Updates the statistics with data from a completed game.
    /// </summary>
    /// <param name="finalScore">The final score of the game.</param>
    /// <param name="finalLevel">The final level reached.</param>
    /// <param name="rowsCleared">The number of rows cleared in the game.</param>
    /// <param name="gameTimeSeconds">The time played in seconds.</param>
    /// <param name="isCompleted">Whether the game was completed or abandoned.</param>
    public void UpdateFromGame(int finalScore, int finalLevel, int rowsCleared, double gameTimeSeconds, bool isCompleted)
    {
        TotalGamesPlayed++;
        
        if (isCompleted)
        {
            TotalGamesCompleted++;
        }

        TotalScore += finalScore;
        TotalRowsCleared += rowsCleared;
        TotalTimePlayedSeconds += gameTimeSeconds;

        if (finalScore > HighestScore)
        {
            HighestScore = finalScore;
        }

        if (finalLevel > HighestLevel)
        {
            HighestLevel = finalLevel;
        }

        LastUpdated = DateTime.Now;
    }

    /// <summary>
    /// Updates the row clearing statistics.
    /// </summary>
    /// <param name="rowsCleared">The number of rows cleared in a single action.</param>
    public void UpdateRowClearStats(int rowsCleared)
    {
        switch (rowsCleared)
        {
            case 1:
                SingleRowClears++;
                break;
            case 2:
                DoubleRowClears++;
                break;
            case 3:
                TripleRowClears++;
                break;
            case 4:
                TetrisRowClears++;
                break;
        }

        LastUpdated = DateTime.Now;
    }

    /// <summary>
    /// Resets all statistics to their initial values.
    /// </summary>
    public void Reset()
    {
        TotalGamesPlayed = 0;
        TotalGamesCompleted = 0;
        HighestScore = 0;
        TotalScore = 0;
        TotalRowsCleared = 0;
        HighestLevel = 0;
        TotalTimePlayedSeconds = 0;
        SingleRowClears = 0;
        DoubleRowClears = 0;
        TripleRowClears = 0;
        TetrisRowClears = 0;
        LastUpdated = DateTime.Now;
    }
}

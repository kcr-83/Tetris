using System;
using System.Drawing;
using System.Text.Json.Serialization;

namespace Tetris.Core.Models;

/// <summary>
/// Represents the complete state of a Tetris game that can be saved and loaded.
/// </summary>
public class GameState
{
    /// <summary>
    /// Gets or sets the save metadata.
    /// </summary>
    public SaveMetadata Metadata { get; set; } = new();

    /// <summary>
    /// Gets or sets the board state as a 2D array.
    /// </summary>
    public int?[,] BoardGrid { get; set; } = new int?[Board.Width, Board.Height];

    /// <summary>
    /// Gets or sets the number of rows cleared on the board.
    /// </summary>
    public int BoardRowsCleared { get; set; }

    /// <summary>
    /// Gets or sets the current tetromino piece data.
    /// </summary>
    public TetrominoState? CurrentPiece { get; set; }

    /// <summary>
    /// Gets or sets the next tetromino piece data.
    /// </summary>
    public TetrominoState? NextPiece { get; set; }

    /// <summary>
    /// Gets or sets the current game level.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the difficulty level.
    /// </summary>
    public DifficultyLevel Difficulty { get; set; }

    /// <summary>
    /// Gets or sets the game mode.
    /// </summary>
    public GameMode GameMode { get; set; }

    /// <summary>
    /// Gets or sets the remaining time in seconds for Timed mode.
    /// </summary>
    public int RemainingTimeSeconds { get; set; }

    /// <summary>
    /// Gets or sets the target rows to clear for Challenge mode.
    /// </summary>
    public int TargetRowsToClear { get; set; }

    /// <summary>
    /// Gets or sets the total rows cleared in the game.
    /// </summary>
    public int TotalRowsCleared { get; set; }

    /// <summary>
    /// Gets or sets the current score.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the number of single row clears.
    /// </summary>
    public int SingleRowsCleared { get; set; }

    /// <summary>
    /// Gets or sets the number of double row clears.
    /// </summary>
    public int DoubleRowsCleared { get; set; }

    /// <summary>
    /// Gets or sets the number of triple row clears.
    /// </summary>
    public int TripleRowsCleared { get; set; }

    /// <summary>
    /// Gets or sets the number of tetris (4 rows) clears.
    /// </summary>
    public int TetrisCleared { get; set; }

    /// <summary>
    /// Gets or sets whether the game was paused when saved.
    /// </summary>
    public bool IsPaused { get; set; }

    /// <summary>
    /// Gets or sets the current fall delay in milliseconds.
    /// </summary>
    public double CurrentFallDelay { get; set; }

    /// <summary>
    /// Gets or sets whether fast drop was active when saved.
    /// </summary>
    public bool IsFastDropActive { get; set; }
}

/// <summary>
/// Contains metadata about a saved game.
/// </summary>
public class SaveMetadata
{
    /// <summary>
    /// Gets or sets the date and time when the game was saved.
    /// </summary>
    public DateTime SavedAt { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the version of the save file format.
    /// </summary>
    public string Version { get; set; } = "1.0";

    /// <summary>
    /// Gets or sets a user-friendly name for the save file.
    /// </summary>
    public string SaveName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional description of the save.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the game mode as a string for display purposes.
    /// </summary>
    public string GameModeDisplay { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the difficulty as a string for display purposes.
    /// </summary>
    public string DifficultyDisplay { get; set; } = string.Empty;
}

/// <summary>
/// Represents the state of a tetromino piece for serialization.
/// </summary>
public class TetrominoState
{
    /// <summary>
    /// Gets or sets the tetromino type identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the tetromino.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the position of the tetromino.
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Gets or sets the rotation state.
    /// </summary>
    public int RotationState { get; set; }

    /// <summary>
    /// Gets or sets the color as an ARGB integer value.
    /// </summary>
    public int ColorArgb { get; set; }

    /// <summary>
    /// Creates a TetrominoState from a Tetromino instance.
    /// </summary>
    /// <param name="tetromino">The tetromino to convert.</param>
    /// <returns>The tetromino state.</returns>
    public static TetrominoState FromTetromino(Tetromino tetromino)
    {
        return new TetrominoState
        {
            Id = tetromino.Id,
            Name = tetromino.Name,
            Position = tetromino.Position,
            RotationState = tetromino.RotationState,
            ColorArgb = tetromino.Color.ToArgb()
        };
    }

    /// <summary>
    /// Gets the color from the stored ARGB value.
    /// </summary>
    /// <returns>The color.</returns>
    [JsonIgnore]
    public Color Color => Color.FromArgb(ColorArgb);
}

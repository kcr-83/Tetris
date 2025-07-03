using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tetris.Core.Models;

namespace Tetris.Core.Services;

/// <summary>
/// Service for saving and loading Tetris game states.
/// </summary>
public class GameSaveService
{
    #region Constants

    private const string SaveDirectory = "Saves";
    private const string SaveExtension = ".tetris";
    private const string QuickSaveFileName = "quicksave.tetris";

    #endregion

    #region Fields

    private readonly JsonSerializerOptions _jsonOptions;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the GameSaveService class.
    /// </summary>
    public GameSaveService()
    {
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = false
        };

        EnsureSaveDirectoryExists();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Saves the game state to a file with the specified name.
    /// </summary>
    /// <param name="gameState">The game state to save.</param>
    /// <param name="saveName">The name for the save file.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when gameState or saveName is null.</exception>
    /// <exception cref="ArgumentException">Thrown when saveName is empty or contains invalid characters.</exception>
    public async Task SaveGameAsync(GameState gameState, string saveName)
    {
        if (gameState == null)
            throw new ArgumentNullException(nameof(gameState));

        if (string.IsNullOrWhiteSpace(saveName))
            throw new ArgumentException("Save name cannot be null or empty.", nameof(saveName));

        ValidateSaveName(saveName);

        // Update metadata
        gameState.Metadata.SaveName = saveName;
        gameState.Metadata.SavedAt = DateTime.Now;
        gameState.Metadata.GameModeDisplay = gameState.GameMode.ToString();
        gameState.Metadata.DifficultyDisplay = DifficultySettings.GetDisplayName(gameState.Difficulty);

        string fileName = GetSafeFileName(saveName) + SaveExtension;
        string filePath = Path.Combine(SaveDirectory, fileName);

        try
        {
            string jsonContent = JsonSerializer.Serialize(gameState, _jsonOptions);
            await File.WriteAllTextAsync(filePath, jsonContent);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save game to '{filePath}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Performs a quick save of the game state.
    /// </summary>
    /// <param name="gameState">The game state to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task QuickSaveAsync(GameState gameState)
    {
        if (gameState == null)
            throw new ArgumentNullException(nameof(gameState));

        gameState.Metadata.SaveName = "Quick Save";
        gameState.Metadata.SavedAt = DateTime.Now;
        gameState.Metadata.Description = $"Quick save from {gameState.GameMode} mode";

        string filePath = Path.Combine(SaveDirectory, QuickSaveFileName);

        try
        {
            string jsonContent = JsonSerializer.Serialize(gameState, _jsonOptions);
            await File.WriteAllTextAsync(filePath, jsonContent);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to quick save game: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Loads a game state from the specified file.
    /// </summary>
    /// <param name="saveName">The name of the save file to load.</param>
    /// <returns>The loaded game state.</returns>
    /// <exception cref="ArgumentException">Thrown when saveName is invalid.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the save file doesn't exist.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the save file is corrupted.</exception>
    public async Task<GameState> LoadGameAsync(string saveName)
    {
        if (string.IsNullOrWhiteSpace(saveName))
            throw new ArgumentException("Save name cannot be null or empty.", nameof(saveName));

        string fileName = GetSafeFileName(saveName) + SaveExtension;
        string filePath = Path.Combine(SaveDirectory, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Save file '{saveName}' not found.");

        try
        {
            string jsonContent = await File.ReadAllTextAsync(filePath);
            GameState? gameState = JsonSerializer.Deserialize<GameState>(jsonContent, _jsonOptions);

            if (gameState == null)
                throw new InvalidOperationException("Failed to deserialize game state.");

            ValidateGameState(gameState);
            return gameState;
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Save file '{saveName}' is corrupted or in an invalid format: {ex.Message}", ex);
        }
        catch (Exception ex) when (!(ex is InvalidOperationException))
        {
            throw new InvalidOperationException($"Failed to load game from '{saveName}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Loads the quick save game state.
    /// </summary>
    /// <returns>The loaded game state, or null if no quick save exists.</returns>
    public async Task<GameState?> LoadQuickSaveAsync()
    {
        string filePath = Path.Combine(SaveDirectory, QuickSaveFileName);

        if (!File.Exists(filePath))
            return null;

        try
        {
            string jsonContent = await File.ReadAllTextAsync(filePath);
            GameState? gameState = JsonSerializer.Deserialize<GameState>(jsonContent, _jsonOptions);

            if (gameState != null)
            {
                ValidateGameState(gameState);
            }

            return gameState;
        }
        catch (Exception)
        {
            // If quick save is corrupted, return null rather than throwing
            return null;
        }
    }

    /// <summary>
    /// Gets a list of all available save files.
    /// </summary>
    /// <returns>A list of save file information.</returns>
    public async Task<List<SaveFileInfo>> GetSaveFilesAsync()
    {
        var saveFiles = new List<SaveFileInfo>();

        if (!Directory.Exists(SaveDirectory))
            return saveFiles;

        try
        {
            string[] files = Directory.GetFiles(SaveDirectory, "*" + SaveExtension);

            foreach (string filePath in files)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);
                    
                    // Skip quick save in the normal list
                    if (fileName.Equals("quicksave", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string jsonContent = await File.ReadAllTextAsync(filePath);
                    GameState? gameState = JsonSerializer.Deserialize<GameState>(jsonContent, _jsonOptions);

                    if (gameState?.Metadata != null)
                    {
                        var fileInfo = new FileInfo(filePath);
                        saveFiles.Add(new SaveFileInfo
                        {
                            SaveName = gameState.Metadata.SaveName,
                            SavedAt = gameState.Metadata.SavedAt,
                            GameMode = gameState.Metadata.GameModeDisplay,
                            Difficulty = gameState.Metadata.DifficultyDisplay,
                            Score = gameState.Score,
                            Level = gameState.Level,
                            FilePath = filePath,
                            FileSize = fileInfo.Length
                        });
                    }
                }
                catch
                {
                    // Skip corrupted files
                    continue;
                }
            }

            return saveFiles.OrderByDescending(s => s.SavedAt).ToList();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to retrieve save files: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Deletes a save file.
    /// </summary>
    /// <param name="saveName">The name of the save file to delete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task DeleteSaveAsync(string saveName)
    {
        if (string.IsNullOrWhiteSpace(saveName))
            throw new ArgumentException("Save name cannot be null or empty.", nameof(saveName));

        string fileName = GetSafeFileName(saveName) + SaveExtension;
        string filePath = Path.Combine(SaveDirectory, fileName);

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Save file '{saveName}' not found.");

        try
        {
            await Task.Run(() => File.Delete(filePath));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to delete save file '{saveName}': {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Checks if a quick save exists.
    /// </summary>
    /// <returns>True if a quick save exists, false otherwise.</returns>
    public bool QuickSaveExists()
    {
        string filePath = Path.Combine(SaveDirectory, QuickSaveFileName);
        return File.Exists(filePath);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Ensures the save directory exists.
    /// </summary>
    private static void EnsureSaveDirectoryExists()
    {
        if (!Directory.Exists(SaveDirectory))
        {
            Directory.CreateDirectory(SaveDirectory);
        }
    }

    /// <summary>
    /// Validates the save name for invalid characters.
    /// </summary>
    /// <param name="saveName">The save name to validate.</param>
    /// <exception cref="ArgumentException">Thrown when the save name contains invalid characters.</exception>
    private static void ValidateSaveName(string saveName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        if (saveName.Any(c => invalidChars.Contains(c)))
        {
            throw new ArgumentException("Save name contains invalid characters.", nameof(saveName));
        }

        if (saveName.Length > 100)
        {
            throw new ArgumentException("Save name is too long. Maximum length is 100 characters.", nameof(saveName));
        }
    }

    /// <summary>
    /// Gets a safe file name from the save name.
    /// </summary>
    /// <param name="saveName">The save name.</param>
    /// <returns>A safe file name.</returns>
    private static string GetSafeFileName(string saveName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        string safeFileName = new string(saveName.Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
        return safeFileName.Trim();
    }

    /// <summary>
    /// Validates the loaded game state for integrity.
    /// </summary>
    /// <param name="gameState">The game state to validate.</param>
    /// <exception cref="InvalidOperationException">Thrown when the game state is invalid.</exception>
    private static void ValidateGameState(GameState gameState)
    {
        if (gameState.BoardGrid.GetLength(0) != Board.Width || gameState.BoardGrid.GetLength(1) != Board.Height)
        {
            throw new InvalidOperationException("Invalid board dimensions in save file.");
        }

        if (gameState.Score < 0 || gameState.Level < 1)
        {
            throw new InvalidOperationException("Invalid game statistics in save file.");
        }

        // Additional validation can be added here
    }

    #endregion
}

/// <summary>
/// Information about a save file.
/// </summary>
public class SaveFileInfo
{
    /// <summary>
    /// Gets or sets the name of the save.
    /// </summary>
    public string SaveName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the game was saved.
    /// </summary>
    public DateTime SavedAt { get; set; }

    /// <summary>
    /// Gets or sets the game mode.
    /// </summary>
    public string GameMode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the difficulty level.
    /// </summary>
    public string Difficulty { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the score at save time.
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Gets or sets the level at save time.
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the file path.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes.
    /// </summary>
    public long FileSize { get; set; }
}

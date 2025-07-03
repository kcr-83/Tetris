using System;

namespace Tetris.Core.Models;

/// <summary>
/// Event arguments for when a game is loaded from a save file.
/// </summary>
public class GameLoadedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the loaded game state.
    /// </summary>
    public GameState GameState { get; }

    /// <summary>
    /// Initializes a new instance of the GameLoadedEventArgs class.
    /// </summary>
    /// <param name="gameState">The loaded game state.</param>
    /// <exception cref="ArgumentNullException">Thrown when gameState is null.</exception>
    public GameLoadedEventArgs(GameState gameState)
    {
        GameState = gameState ?? throw new ArgumentNullException(nameof(gameState));
    }
}

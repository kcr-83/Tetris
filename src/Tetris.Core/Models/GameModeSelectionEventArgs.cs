using System;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Event arguments that contain the selected game mode.
    /// </summary>
    public class GameModeSelectionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the selected game mode.
        /// </summary>
        public GameMode SelectedGameMode { get; }
        
        /// <summary>
        /// Gets the selected difficulty level.
        /// </summary>
        public DifficultyLevel SelectedDifficulty { get; }
        
        /// <summary>
        /// Initializes a new instance of the GameModeSelectionEventArgs class.
        /// </summary>
        /// <param name="selectedGameMode">The selected game mode.</param>
        /// <param name="selectedDifficulty">The selected difficulty level.</param>
        public GameModeSelectionEventArgs(GameMode selectedGameMode, DifficultyLevel selectedDifficulty)
        {
            SelectedGameMode = selectedGameMode;
            SelectedDifficulty = selectedDifficulty;
        }
    }
}

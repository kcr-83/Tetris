using System;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Event arguments that contain the selected difficulty level.
    /// </summary>
    public class DifficultySelectionEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the selected difficulty level.
        /// </summary>
        public DifficultyLevel SelectedDifficulty { get; }
        
        /// <summary>
        /// Initializes a new instance of the DifficultySelectionEventArgs class.
        /// </summary>
        /// <param name="selectedDifficulty">The selected difficulty level.</param>
        public DifficultySelectionEventArgs(DifficultyLevel selectedDifficulty)
        {
            SelectedDifficulty = selectedDifficulty;
        }
    }
}

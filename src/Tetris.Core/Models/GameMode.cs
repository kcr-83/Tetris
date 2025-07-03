using System;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Represents the different game modes available in Tetris.
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// Classic mode - play until game over with increasing difficulty.
        /// </summary>
        Classic = 0,
        
        /// <summary>
        /// Timed mode - score as many points as possible in a given time.
        /// </summary>
        Timed = 1,
        
        /// <summary>
        /// Challenge mode - clear a specific number of rows to win.
        /// </summary>
        Challenge = 2
    }
    
    /// <summary>
    /// Contains settings and utility methods for game modes.
    /// </summary>
    public static class GameModeSettings
    {
        /// <summary>
        /// Gets the time limit in seconds for the timed mode.
        /// </summary>
        /// <param name="difficultyLevel">The difficulty level.</param>
        /// <returns>The time limit in seconds.</returns>
        public static int GetTimedModeSeconds(DifficultyLevel difficultyLevel)
        {
            return difficultyLevel switch
            {
                DifficultyLevel.Easy => 180,    // 3 minutes
                DifficultyLevel.Medium => 120,  // 2 minutes
                DifficultyLevel.Hard => 90,     // 1.5 minutes
                _ => 120                        // Default to 2 minutes
            };
        }
        
        /// <summary>
        /// Gets the number of rows to clear for the challenge mode.
        /// </summary>
        /// <param name="difficultyLevel">The difficulty level.</param>
        /// <returns>The number of rows to clear.</returns>
        public static int GetChallengeRowsTarget(DifficultyLevel difficultyLevel)
        {
            return difficultyLevel switch
            {
                DifficultyLevel.Easy => 20,     // 20 rows
                DifficultyLevel.Medium => 40,   // 40 rows
                DifficultyLevel.Hard => 60,     // 60 rows
                _ => 40                         // Default to 40 rows
            };
        }
    }
}

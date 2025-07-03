using System;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Represents the difficulty level of the Tetris game, affecting falling speed and scoring.
    /// </summary>
    public enum DifficultyLevel
    {
        /// <summary>
        /// Easy difficulty - slower falling speed and standard scoring.
        /// </summary>
        Easy = 0,
        
        /// <summary>
        /// Medium difficulty - standard falling speed and scoring.
        /// </summary>
        Medium = 1,
        
        /// <summary>
        /// Hard difficulty - faster falling speed and increased scoring.
        /// </summary>
        Hard = 2
    }
    
    /// <summary>
    /// Contains settings and utility methods for game difficulty levels.
    /// </summary>
    public static class DifficultySettings
    {
        /// <summary>
        /// Gets the initial fall delay for the specified difficulty level in milliseconds.
        /// </summary>
        /// <param name="difficultyLevel">The difficulty level.</param>
        /// <returns>The initial fall delay in milliseconds.</returns>
        public static double GetInitialFallDelay(DifficultyLevel difficultyLevel)
        {
            return difficultyLevel switch
            {
                DifficultyLevel.Easy => 1200.0, // Slower than normal
                DifficultyLevel.Medium => 1000.0, // Standard
                DifficultyLevel.Hard => 800.0, // Faster
                _ => 1000.0 // Default to Medium
            };
        }
        
        /// <summary>
        /// Gets the delay reduction per level for the specified difficulty level in milliseconds.
        /// </summary>
        /// <param name="difficultyLevel">The difficulty level.</param>
        /// <returns>The delay reduction per level in milliseconds.</returns>
        public static double GetDelayReductionPerLevel(DifficultyLevel difficultyLevel)
        {
            return difficultyLevel switch
            {
                DifficultyLevel.Easy => 40.0, // Less reduction per level
                DifficultyLevel.Medium => 50.0, // Standard
                DifficultyLevel.Hard => 60.0, // More reduction per level
                _ => 50.0 // Default to Medium
            };
        }
        
        /// <summary>
        /// Gets the minimum fall delay for the specified difficulty level in milliseconds.
        /// </summary>
        /// <param name="difficultyLevel">The difficulty level.</param>
        /// <returns>The minimum fall delay in milliseconds.</returns>
        public static double GetMinimumFallDelay(DifficultyLevel difficultyLevel)
        {
            return difficultyLevel switch
            {
                DifficultyLevel.Easy => 150.0, // Not as fast at higher levels
                DifficultyLevel.Medium => 100.0, // Standard
                DifficultyLevel.Hard => 80.0, // Very fast at higher levels
                _ => 100.0 // Default to Medium
            };
        }
        
        /// <summary>
        /// Gets the score multiplier for the specified difficulty level.
        /// </summary>
        /// <param name="difficultyLevel">The difficulty level.</param>
        /// <returns>The score multiplier.</returns>
        public static double GetScoreMultiplier(DifficultyLevel difficultyLevel)
        {
            return difficultyLevel switch
            {
                DifficultyLevel.Easy => 1.0, // Standard scoring
                DifficultyLevel.Medium => 1.5, // 50% bonus
                DifficultyLevel.Hard => 2.0, // Double scoring
                _ => 1.0 // Default to Easy
            };
        }
        
        /// <summary>
        /// Gets the display name for the specified difficulty level.
        /// </summary>
        /// <param name="difficultyLevel">The difficulty level.</param>
        /// <returns>The display name.</returns>
        public static string GetDisplayName(DifficultyLevel difficultyLevel)
        {
            return difficultyLevel switch
            {
                DifficultyLevel.Easy => "Easy",
                DifficultyLevel.Medium => "Medium",
                DifficultyLevel.Hard => "Hard",
                _ => "Unknown"
            };
        }
        
        /// <summary>
        /// Gets the description for the specified difficulty level.
        /// </summary>
        /// <param name="difficultyLevel">The difficulty level.</param>
        /// <returns>The description.</returns>
        public static string GetDescription(DifficultyLevel difficultyLevel)
        {
            return difficultyLevel switch
            {
                DifficultyLevel.Easy => "Slower falling speed, standard scoring",
                DifficultyLevel.Medium => "Standard falling speed and scoring",
                DifficultyLevel.Hard => "Faster falling speed, double scoring",
                _ => "Unknown difficulty level"
            };
        }
    }
}

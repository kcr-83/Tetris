using System;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Provides event data for when the level increases in a Tetris game.
    /// </summary>
    public class LevelIncreasedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the new level that was reached.
        /// </summary>
        public int NewLevel { get; }
        
        /// <summary>
        /// Gets the previous level before the increase.
        /// </summary>
        public int OldLevel { get; }
        
        /// <summary>
        /// Initializes a new instance of the LevelIncreasedEventArgs class.
        /// </summary>
        /// <param name="oldLevel">The previous level before the increase.</param>
        /// <param name="newLevel">The new level that was reached.</param>
        public LevelIncreasedEventArgs(int oldLevel, int newLevel)
        {
            OldLevel = oldLevel;
            NewLevel = newLevel;
        }
    }
    
    /// <summary>
    /// Provides event data for when rows are cleared in a Tetris game.
    /// </summary>
    public class RowsClearedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the number of rows that were cleared.
        /// </summary>
        public int RowsCleared { get; }
        
        /// <summary>
        /// Gets the score gained from clearing the rows.
        /// </summary>
        public int ScoreGained { get; }
        
        /// <summary>
        /// Gets the y-coordinates of the rows that were cleared.
        /// </summary>
        public int[] ClearedRowIndices { get; }
        
        /// <summary>
        /// Initializes a new instance of the RowsClearedEventArgs class.
        /// </summary>
        /// <param name="rowsCleared">The number of rows that were cleared.</param>
        /// <param name="scoreGained">The score gained from clearing the rows.</param>
        /// <param name="clearedRowIndices">The y-coordinates of the rows that were cleared.</param>
        public RowsClearedEventArgs(int rowsCleared, int scoreGained, int[] clearedRowIndices)
        {
            RowsCleared = rowsCleared;
            ScoreGained = scoreGained;
            ClearedRowIndices = clearedRowIndices;
        }
    }
}

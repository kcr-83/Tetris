using System;
using System.Collections.Generic;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Event arguments for the GameOver event containing game statistics.
    /// </summary>
    public class GameOverEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the final score achieved in the game.
        /// </summary>
        public int FinalScore { get; }

        /// <summary>
        /// Gets the level reached in the game.
        /// </summary>
        public int FinalLevel { get; }

        /// <summary>
        /// Gets the total number of rows cleared in the game.
        /// </summary>
        public int TotalRowsCleared { get; }

        /// <summary>
        /// Gets the statistics about different types of line clears.
        /// </summary>
        public Dictionary<string, int> LineStatistics { get; }

        /// <summary>
        /// Gets the reason why the game ended.
        /// </summary>
        public GameOverReason Reason { get; }

        /// <summary>
        /// Initializes a new instance of the GameOverEventArgs class.
        /// </summary>
        /// <param name="finalScore">The final score achieved in the game.</param>
        /// <param name="finalLevel">The level reached in the game.</param>
        /// <param name="totalRowsCleared">The total number of rows cleared.</param>
        /// <param name="lineStatistics">Statistics about different types of line clears.</param>
        /// <param name="reason">The reason why the game ended.</param>
        public GameOverEventArgs(
            int finalScore,
            int finalLevel,
            int totalRowsCleared,
            Dictionary<string, int> lineStatistics,
            GameOverReason reason)
        {
            FinalScore = finalScore;
            FinalLevel = finalLevel;
            TotalRowsCleared = totalRowsCleared;
            LineStatistics = lineStatistics;
            Reason = reason;
        }
    }

    /// <summary>
    /// Represents the reason why the game ended.
    /// </summary>
    public enum GameOverReason
    {
        /// <summary>
        /// The board has reached the top.
        /// </summary>
        BoardFull,

        /// <summary>
        /// A new piece cannot be placed on the board.
        /// </summary>
        NoSpaceForNewPiece,

        /// <summary>
        /// The game was manually ended by the player.
        /// </summary>
        PlayerEnded
    }
}

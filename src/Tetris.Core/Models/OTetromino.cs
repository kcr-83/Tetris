using System.Drawing;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Represents the O-shaped (square) Tetromino piece.
    /// </summary>
    public class OTetromino : Tetromino
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier for the O Tetromino.
        /// </summary>
        public override int Id => 4;

        /// <summary>
        /// Gets the color of the O Tetromino (Yellow).
        /// </summary>
        public override Color Color => Color.Yellow;

        /// <summary>
        /// Gets the name of the O Tetromino.
        /// </summary>
        public override string Name => "O";

        /// <summary>
        /// Gets the blocks that make up the O Tetromino.
        /// Note: The O Tetromino always has the same shape regardless of rotation.
        /// </summary>
        public override Point[] Blocks
        {
            get
            {
                // The O tetromino is the same in all rotation states
                return new Point[] { new(1, 0), new(2, 0), new(1, 1), new(2, 1) };
            }
        }

        #endregion

        /// <summary>
        /// Creates a deep copy of this O Tetromino.
        /// </summary>
        /// <returns>A new instance of the O Tetromino with the same properties.</returns>
        public override Tetromino Clone()
        {
            OTetromino clone = new OTetromino
            {
                Position = this.Position,
                RotationState = this.RotationState
            };
            return clone;
        }
    }
}

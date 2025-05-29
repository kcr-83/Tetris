using System.Drawing;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Represents the I-shaped Tetromino piece.
    /// </summary>
    public class ITetromino : Tetromino
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier for the I Tetromino.
        /// </summary>
        public override int Id => 1;

        /// <summary>
        /// Gets the color of the I Tetromino (Cyan).
        /// </summary>
        public override Color Color => Color.Cyan;

        /// <summary>
        /// Gets the name of the I Tetromino.
        /// </summary>
        public override string Name => "I";

        /// <summary>
        /// Gets the blocks that make up the I Tetromino in its current rotation state.
        /// </summary>
        public override Point[] Blocks
        {
            get
            {
                return RotationState switch
                {
                    0 => new Point[] { new(0, 1), new(1, 1), new(2, 1), new(3, 1) },
                    1 => new Point[] { new(2, 0), new(2, 1), new(2, 2), new(2, 3) },
                    2 => new Point[] { new(0, 2), new(1, 2), new(2, 2), new(3, 2) },
                    3 => new Point[] { new(1, 0), new(1, 1), new(1, 2), new(1, 3) },
                    _ => new Point[] { new(0, 1), new(1, 1), new(2, 1), new(3, 1) }
                };
            }
        }

        #endregion

        /// <summary>
        /// Creates a deep copy of this I Tetromino.
        /// </summary>
        /// <returns>A new instance of the I Tetromino with the same properties.</returns>
        public override Tetromino Clone()
        {
            ITetromino clone = new ITetromino
            {
                Position = this.Position,
                RotationState = this.RotationState
            };
            return clone;
        }
    }
}

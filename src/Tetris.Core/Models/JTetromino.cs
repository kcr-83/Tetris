using System.Drawing;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Represents the J-shaped Tetromino piece.
    /// </summary>
    public class JTetromino : Tetromino
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier for the J Tetromino.
        /// </summary>
        public override int Id => 2;

        /// <summary>
        /// Gets the color of the J Tetromino (Blue).
        /// </summary>
        public override Color Color => Color.Blue;

        /// <summary>
        /// Gets the name of the J Tetromino.
        /// </summary>
        public override string Name => "J";

        /// <summary>
        /// Gets the blocks that make up the J Tetromino in its current rotation state.
        /// </summary>
        public override Point[] Blocks
        {
            get
            {
                return RotationState switch
                {
                    0 => new Point[] { new(0, 0), new(0, 1), new(1, 1), new(2, 1) },
                    1 => new Point[] { new(1, 0), new(2, 0), new(1, 1), new(1, 2) },
                    2 => new Point[] { new(0, 1), new(1, 1), new(2, 1), new(2, 2) },
                    3 => new Point[] { new(1, 0), new(1, 1), new(0, 2), new(1, 2) },
                    _ => new Point[] { new(0, 0), new(0, 1), new(1, 1), new(2, 1) }
                };
            }
        }

        #endregion

        /// <summary>
        /// Creates a deep copy of this J Tetromino.
        /// </summary>
        /// <returns>A new instance of the J Tetromino with the same properties.</returns>
        public override Tetromino Clone()
        {
            JTetromino clone = new JTetromino
            {
                Position = this.Position,
                RotationState = this.RotationState
            };
            return clone;
        }
    }
}

using System.Drawing;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Represents the S-shaped Tetromino piece.
    /// </summary>
    public class STetromino : Tetromino
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier for the S Tetromino.
        /// </summary>
        public override int Id => 5;

        /// <summary>
        /// Gets the color of the S Tetromino (Green).
        /// </summary>
        public override Color Color => Color.Green;

        /// <summary>
        /// Gets the name of the S Tetromino.
        /// </summary>
        public override string Name => "S";

        /// <summary>
        /// Gets the blocks that make up the S Tetromino in its current rotation state.
        /// </summary>
        public override Point[] Blocks
        {
            get
            {
                return RotationState switch
                {
                    0 => new Point[] { new(1, 0), new(2, 0), new(0, 1), new(1, 1) },
                    1 => new Point[] { new(1, 0), new(1, 1), new(2, 1), new(2, 2) },
                    2 => new Point[] { new(1, 1), new(2, 1), new(0, 2), new(1, 2) },
                    3 => new Point[] { new(0, 0), new(0, 1), new(1, 1), new(1, 2) },
                    _ => new Point[] { new(1, 0), new(2, 0), new(0, 1), new(1, 1) }
                };
            }
        }

        #endregion

        /// <summary>
        /// Creates a deep copy of this S Tetromino.
        /// </summary>
        /// <returns>A new instance of the S Tetromino with the same properties.</returns>
        public override Tetromino Clone()
        {
            STetromino clone = new STetromino
            {
                Position = this.Position,
                RotationState = this.RotationState
            };
            return clone;
        }
    }
}

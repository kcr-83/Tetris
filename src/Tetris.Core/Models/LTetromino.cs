using System.Drawing;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Represents the L-shaped Tetromino piece.
    /// </summary>
    public class LTetromino : Tetromino
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier for the L Tetromino.
        /// </summary>
        public override int Id => 3;

        /// <summary>
        /// Gets the color of the L Tetromino (Orange).
        /// </summary>
        public override Color Color => Color.Orange;

        /// <summary>
        /// Gets the name of the L Tetromino.
        /// </summary>
        public override string Name => "L";

        /// <summary>
        /// Gets the blocks that make up the L Tetromino in its current rotation state.
        /// </summary>
        public override Point[] Blocks
        {
            get
            {
                return RotationState switch
                {
                    0 => new Point[] { new(2, 0), new(0, 1), new(1, 1), new(2, 1) },
                    1 => new Point[] { new(1, 0), new(1, 1), new(1, 2), new(2, 2) },
                    2 => new Point[] { new(0, 1), new(1, 1), new(2, 1), new(0, 2) },
                    3 => new Point[] { new(0, 0), new(1, 0), new(1, 1), new(1, 2) },
                    _ => new Point[] { new(2, 0), new(0, 1), new(1, 1), new(2, 1) }
                };
            }
        }

        #endregion

        /// <summary>
        /// Creates a deep copy of this L Tetromino.
        /// </summary>
        /// <returns>A new instance of the L Tetromino with the same properties.</returns>
        public override Tetromino Clone()
        {
            LTetromino clone = new LTetromino
            {
                Position = this.Position,
                RotationState = this.RotationState
            };
            return clone;
        }
    }
}

using System.Drawing;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Represents the Z-shaped Tetromino piece.
    /// </summary>
    public class ZTetromino : Tetromino
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier for the Z Tetromino.
        /// </summary>
        public override int Id => 7;

        /// <summary>
        /// Gets the color of the Z Tetromino (Red).
        /// </summary>
        public override Color Color => Color.Red;

        /// <summary>
        /// Gets the name of the Z Tetromino.
        /// </summary>
        public override string Name => "Z";

        /// <summary>
        /// Gets the blocks that make up the Z Tetromino in its current rotation state.
        /// </summary>
        public override Point[] Blocks
        {
            get
            {
                return RotationState switch
                {
                    0 => new Point[] { new(0, 0), new(1, 0), new(1, 1), new(2, 1) },
                    1 => new Point[] { new(2, 0), new(1, 1), new(2, 1), new(1, 2) },
                    2 => new Point[] { new(0, 1), new(1, 1), new(1, 2), new(2, 2) },
                    3 => new Point[] { new(1, 0), new(0, 1), new(1, 1), new(0, 2) },
                    _ => new Point[] { new(0, 0), new(1, 0), new(1, 1), new(2, 1) }
                };
            }
        }

        #endregion

        /// <summary>
        /// Creates a deep copy of this Z Tetromino.
        /// </summary>
        /// <returns>A new instance of the Z Tetromino with the same properties.</returns>
        public override Tetromino Clone()
        {
            ZTetromino clone = new ZTetromino
            {
                Position = this.Position,
                RotationState = this.RotationState
            };
            return clone;
        }
    }
}

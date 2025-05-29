using System.Drawing;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Represents the T-shaped Tetromino piece.
    /// </summary>
    public class TTetromino : Tetromino
    {
        #region Properties

        /// <summary>
        /// Gets the unique identifier for the T Tetromino.
        /// </summary>
        public override int Id => 6;

        /// <summary>
        /// Gets the color of the T Tetromino (Purple).
        /// </summary>
        public override Color Color => Color.Purple;

        /// <summary>
        /// Gets the name of the T Tetromino.
        /// </summary>
        public override string Name => "T";

        /// <summary>
        /// Gets the blocks that make up the T Tetromino in its current rotation state.
        /// </summary>
        public override Point[] Blocks
        {
            get
            {
                return RotationState switch
                {
                    0 => new Point[] { new(1, 0), new(0, 1), new(1, 1), new(2, 1) },
                    1 => new Point[] { new(1, 0), new(1, 1), new(2, 1), new(1, 2) },
                    2 => new Point[] { new(0, 1), new(1, 1), new(2, 1), new(1, 2) },
                    3 => new Point[] { new(1, 0), new(0, 1), new(1, 1), new(1, 2) },
                    _ => new Point[] { new(1, 0), new(0, 1), new(1, 1), new(2, 1) }
                };
            }
        }

        #endregion

        /// <summary>
        /// Creates a deep copy of this T Tetromino.
        /// </summary>
        /// <returns>A new instance of the T Tetromino with the same properties.</returns>
        public override Tetromino Clone()
        {
            TTetromino clone = new TTetromino
            {
                Position = this.Position,
                RotationState = this.RotationState
            };
            return clone;
        }
    }
}

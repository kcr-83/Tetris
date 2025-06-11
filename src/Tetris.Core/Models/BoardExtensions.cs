using System.Drawing;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Extension methods for the Board class.
    /// </summary>
    public static class BoardExtensions
    {
        /// <summary>
        /// Checks if a tetromino can move down on the board.
        /// </summary>
        /// <param name="board">The game board.</param>
        /// <param name="tetromino">The tetromino to check.</param>
        /// <returns>True if the tetromino can move down, false otherwise.</returns>
        public static bool CanTetrominoMoveDown(this Board board, Tetromino tetromino)
        {
            // Create a position one row down
            var newPosition = new Point(tetromino.Position.X, tetromino.Position.Y + 1);
            
            // Check if this position would be valid
            var blocks = tetromino.Blocks;
            foreach (var block in blocks)
            {
                int x = newPosition.X + block.X;
                int y = newPosition.Y + block.Y;
                
                // Check if the new position is out of bounds or collides with existing blocks
                if (y >= Board.Height || x < 0 || x >= Board.Width || 
                    (board.IsWithinBounds(x, y) && board.Grid[x, y].HasValue && board.Grid[x, y] != -1))
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Adds a block to the board at the specified position.
        /// </summary>
        /// <param name="board">The game board.</param>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        /// <param name="tetrominoId">The ID of the tetromino.</param>
        /// <returns>True if the block was added successfully, false otherwise.</returns>
        public static bool AddBlock(this Board board, int x, int y, int tetrominoId)
        {
            if (!board.IsWithinBounds(x, y))
            {
                return false;
            }
            
            board.Grid[x, y] = tetrominoId;
            return true;
        }
    }
}

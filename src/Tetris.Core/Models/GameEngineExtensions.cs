using System;
using System.Drawing;
using System.Linq;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Extension methods for GameEngine to support the responsive interface.
    /// </summary>
    public static class GameEngineExtensions
    {
        /// <summary>
        /// Determines if a tetromino can move in a specified direction.
        /// </summary>
        /// <param name="gameEngine">The game engine instance.</param>
        /// <param name="piece">The tetromino to check.</param>
        /// <param name="deltaX">The horizontal movement.</param>
        /// <param name="deltaY">The vertical movement.</param>
        /// <returns>True if the move is valid, false otherwise.</returns>
        public static bool CanPieceMove(this GameEngine gameEngine, Tetromino piece, int deltaX, int deltaY)
        {
            // Create a new position for the piece
            Point newPosition = new Point(piece.Position.X + deltaX, piece.Position.Y + deltaY);
            
            // Check if the new position would be valid
            foreach (var block in piece.Blocks)
            {
                int newX = newPosition.X + block.X;
                int newY = newPosition.Y + block.Y;
                
                // Check boundaries
                if (newX < 0 || newX >= Board.Width || newY < 0 || newY >= Board.Height)
                {
                    return false;
                }
                
                // Check collision with existing pieces
                if (gameEngine.Board.Grid[newX, newY].HasValue)
                {
                    return false;
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Checks if a specific position on the board is valid (in bounds and empty).
        /// </summary>
        /// <param name="gameEngine">The game engine instance.</param>
        /// <param name="x">The x-coordinate to check.</param>
        /// <param name="y">The y-coordinate to check.</param>
        /// <returns>True if the position is valid, false otherwise.</returns>
        public static bool IsValidPosition(this GameEngine gameEngine, int x, int y)
        {
            // Check boundaries
            if (x < 0 || x >= Board.Width || y < 0 || y >= Board.Height)
            {
                return false;
            }
            
            // Check if the cell is empty
            return !gameEngine.Board.Grid[x, y].HasValue;
        }
          /// <summary>
        /// Updates the game state, moving the current piece down if necessary.
        /// </summary>
        /// <param name="gameEngine">The game engine instance.</param>
        public static void Update(this GameEngine gameEngine)
        {
            // Simulate the auto-drop behavior that would happen during game loop
            if (gameEngine.CurrentPiece != null)
            {
                // Since MovePieceDown is inaccessible, we'll use reflection to call it
                var movePieceDownMethod = gameEngine.GetType().GetMethod("MovePieceDown", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                // Try to move the piece down
                if (CanPieceMove(gameEngine, gameEngine.CurrentPiece, 0, 1) && movePieceDownMethod != null)
                {
                    movePieceDownMethod.Invoke(gameEngine, null);
                }
            }
        }
    }
}

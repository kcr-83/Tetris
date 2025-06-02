using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Provides a unified interface for controlling Tetromino pieces in a Tetris game.
    /// </summary>
    public class TetrominoController
    {
        private readonly GameEngine _gameEngine;
        private bool _isSoftDropActive;

        /// <summary>
        /// Initializes a new instance of the TetrominoController class.
        /// </summary>
        /// <param name="gameEngine">The game engine to control.</param>
        public TetrominoController(GameEngine gameEngine)
        {
            _gameEngine = gameEngine ?? throw new ArgumentNullException(nameof(gameEngine));
            _isSoftDropActive = false;
        }

        /// <summary>
        /// Gets a value indicating whether soft drop is currently active.
        /// </summary>
        public bool IsSoftDropActive => _isSoftDropActive;

        /// <summary>
        /// Moves the current tetromino piece to the left if possible.
        /// </summary>
        /// <returns>True if the piece was moved, false otherwise.</returns>
        public bool MoveLeft()
        {
            return _gameEngine.MovePieceLeft();
        }

        /// <summary>
        /// Moves the current tetromino piece to the right if possible.
        /// </summary>
        /// <returns>True if the piece was moved, false otherwise.</returns>
        public bool MoveRight()
        {
            return _gameEngine.MovePieceRight();
        }

        /// <summary>
        /// Rotates the current tetromino piece clockwise if possible.
        /// </summary>
        /// <returns>True if the piece was rotated, false otherwise.</returns>
        public bool RotateClockwise()
        {
            return _gameEngine.RotatePieceClockwise();
        }

        /// <summary>
        /// Rotates the current tetromino piece counter-clockwise if possible.
        /// </summary>
        /// <returns>True if the piece was rotated, false otherwise.</returns>
        public bool RotateCounterClockwise()
        {
            return _gameEngine.RotatePieceCounterClockwise();
        }

        /// <summary>
        /// Toggles the soft drop feature (faster falling).
        /// </summary>
        /// <param name="activate">True to activate soft drop, false to deactivate.</param>
        public void ToggleSoftDrop(bool activate)
        {
            if (activate && !_isSoftDropActive)
            {
                _gameEngine.ActivateFastDrop();
                _isSoftDropActive = true;
            }
            else if (!activate && _isSoftDropActive)
            {
                _gameEngine.DeactivateFastDrop();
                _isSoftDropActive = false;
            }
        }

        /// <summary>
        /// Performs a hard drop, moving the current piece down as far as possible in one go.
        /// </summary>
        public void HardDrop()
        {
            _gameEngine.HardDrop();
        }

        /// <summary>
        /// Enum representing the different types of inputs for Tetris.
        /// </summary>
        public enum TetrisInput
        {
            /// <summary>Move the piece left</summary>
            MoveLeft,
            
            /// <summary>Move the piece right</summary>
            MoveRight,
            
            /// <summary>Rotate the piece clockwise</summary>
            RotateClockwise,
            
            /// <summary>Rotate the piece counter-clockwise</summary>
            RotateCounterClockwise,
            
            /// <summary>Start soft drop (accelerated fall)</summary>
            SoftDropStart,
            
            /// <summary>End soft drop</summary>
            SoftDropEnd,
            
            /// <summary>Hard drop (immediate placement)</summary>
            HardDrop
        }

        /// <summary>
        /// Processes the given input and performs the corresponding action.
        /// </summary>
        /// <param name="input">The input to process.</param>
        /// <returns>True if the input resulted in a change, false otherwise.</returns>
        public bool ProcessInput(TetrisInput input)
        {
            switch (input)
            {
                case TetrisInput.MoveLeft:
                    return MoveLeft();
                case TetrisInput.MoveRight:
                    return MoveRight();
                case TetrisInput.RotateClockwise:
                    return RotateClockwise();
                case TetrisInput.RotateCounterClockwise:
                    return RotateCounterClockwise();
                case TetrisInput.SoftDropStart:
                    ToggleSoftDrop(true);
                    return true;
                case TetrisInput.SoftDropEnd:
                    ToggleSoftDrop(false);
                    return true;
                case TetrisInput.HardDrop:
                    HardDrop();
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the current state of the game board, including the current falling piece.
        /// </summary>
        /// <returns>A 2D array representing the game board with the current piece included.</returns>
        public int?[,] GetBoardWithCurrentPiece()
        {
            // Create a copy of the current board
            var board = new Board(_gameEngine.Board);
            
            // Get current piece positions
            Point[] positions = _gameEngine.CurrentPiece.GetAbsolutePositions();
            
            // Convert to tuples
            var tuples = positions.Select(p => (p.X, p.Y));
            
            // Add the current piece to the board
            board.AddBlocks(tuples, _gameEngine.CurrentPiece.Id);
            
            return board.Grid;
        }

        /// <summary>
        /// Gets a preview of the hard drop position for the current piece.
        /// </summary>
        /// <returns>A 2D array representing the game board with the current piece at its drop position.</returns>
        public int?[,] GetHardDropPreview()
        {
            // Get current piece and create a clone for simulation
            var currentPiece = _gameEngine.CurrentPiece;
            var tempPiece = currentPiece.Clone();
            
            // Track the drop position
            int dropY = currentPiece.Position.Y;
            bool canMove = true;
            
            // Move down until collision
            while (canMove)
            {
                // Get positions after moving down
                Point[] newPositions = tempPiece.GetPositionsAfterMove(0, 1);
                
                // Convert to tuples
                var tuples = newPositions.Select(p => (p.X, p.Y));
                
                // Check for collision
                if (!_gameEngine.Board.CheckCollision(tuples))
                {
                    // Move down one step
                    tempPiece.Move(0, 1);
                    dropY++;
                }
                else
                {
                    // Can't move further
                    canMove = false;
                }
            }
            
            // Create a copy of the current board
            var board = new Board(_gameEngine.Board);
            
            // Set the piece at the drop position
            tempPiece.Position = new Point(currentPiece.Position.X, dropY);
            
            // Get the drop position and add to board
            Point[] positions = tempPiece.GetAbsolutePositions();
            var dropTuples = positions.Select(p => (p.X, p.Y));
            board.AddBlocks(dropTuples, currentPiece.Id);
            
            return board.Grid;
        }
    }
}

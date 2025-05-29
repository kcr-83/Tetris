using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tetris.Core.Models;

/// <summary>
/// Represents an abstract base class for all Tetromino pieces in Tetris.
/// </summary>
public abstract class Tetromino
{
    #region Properties

    /// <summary>
    /// Gets the unique identifier for the Tetromino type.
    /// </summary>
    public abstract int Id { get; }

    /// <summary>
    /// Gets the color of the Tetromino.
    /// </summary>
    public abstract Color Color { get; }

    /// <summary>
    /// Gets the name of the Tetromino piece (I, J, L, O, S, T, Z).
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the current position of the Tetromino on the board (top-left corner).
    /// </summary>
    public Point Position { get; set; }

    /// <summary>
    /// Gets the current rotation state of the Tetromino (0, 1, 2, or 3 representing 0, 90, 180, 270 degrees).
    /// </summary>
    public int RotationState { get; set; }

    /// <summary>
    /// Gets the blocks that make up the Tetromino in its current rotation state.
    /// Each point is relative to the Position property.
    /// </summary>
    public abstract Point[] Blocks { get; }

    #endregion

    #region Constructor

    /// <summary>
    /// Initializes a new instance of the Tetromino class.
    /// </summary>
    protected Tetromino()
    {
        Reset();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Resets the Tetromino to its initial position and rotation.
    /// </summary>
    public void Reset()
    {
        // Start position: centered at the top of the board
        Position = new Point(Board.Width / 2 - 2, 0);
        RotationState = 0;
    }

    /// <summary>
    /// Moves the Tetromino in the specified direction.
    /// </summary>
    /// <param name="x">The x-direction to move (-1 for left, 1 for right).</param>
    /// <param name="y">The y-direction to move (typically 1 for down).</param>
    public void Move(int x, int y)
    {
        Position = new Point(Position.X + x, Position.Y + y);
    }

    /// <summary>
    /// Rotates the Tetromino clockwise.
    /// </summary>
    public void RotateClockwise()
    {
        RotationState = (RotationState + 1) % 4;
    }

    /// <summary>
    /// Rotates the Tetromino counter-clockwise.
    /// </summary>
    public void RotateCounterClockwise()
    {
        RotationState = (RotationState + 3) % 4; // Adding 3 is equivalent to subtracting 1 with modulo 4
    }

    /// <summary>
    /// Gets the absolute positions of all blocks in the Tetromino on the board.
    /// </summary>
    /// <returns>An array of points representing the positions of all blocks.</returns>
    public Point[] GetAbsolutePositions()
    {
        Point[] blocks = Blocks;
        Point[] positions = new Point[blocks.Length];

        for (int i = 0; i < blocks.Length; i++)
        {
            positions[i] = new Point(Position.X + blocks[i].X, Position.Y + blocks[i].Y);
        }

        return positions;
    }

    /// <summary>
    /// Gets the absolute positions the Tetromino would have after moving.
    /// </summary>
    /// <param name="moveX">The x-direction to move.</param>
    /// <param name="moveY">The y-direction to move.</param>
    /// <returns>An array of points representing the positions after moving.</returns>
    public Point[] GetPositionsAfterMove(int moveX, int moveY)
    {
        Point[] blocks = Blocks;
        Point[] positions = new Point[blocks.Length];

        for (int i = 0; i < blocks.Length; i++)
        {
            positions[i] = new Point(
                Position.X + blocks[i].X + moveX,
                Position.Y + blocks[i].Y + moveY
            );
        }

        return positions;
    }

    /// <summary>
    /// Gets the absolute positions the Tetromino would have after rotating clockwise.
    /// </summary>
    /// <returns>An array of points representing the positions after rotating clockwise.</returns>
    public Point[] GetPositionsAfterClockwiseRotation()
    {
        // Save current rotation state
        int originalRotation = RotationState;

        // Temporarily rotate clockwise
        RotateClockwise();

        // Get positions in this new rotation
        Point[] positions = GetAbsolutePositions();

        // Restore original rotation
        RotationState = originalRotation;

        return positions;
    }

    /// <summary>
    /// Gets the absolute positions the Tetromino would have after rotating counter-clockwise.
    /// </summary>
    /// <returns>An array of points representing the positions after rotating counter-clockwise.</returns>
    public Point[] GetPositionsAfterCounterClockwiseRotation()
    {
        // Save current rotation state
        int originalRotation = RotationState;

        // Temporarily rotate counter-clockwise
        RotateCounterClockwise();

        // Get positions in this new rotation
        Point[] positions = GetAbsolutePositions();

        // Restore original rotation
        RotationState = originalRotation;

        return positions;
    }

    /// <summary>
    /// Creates a deep copy of this Tetromino.
    /// </summary>
    /// <returns>A new instance of the same Tetromino with the same properties.</returns>
    public abstract Tetromino Clone();

    #endregion
}

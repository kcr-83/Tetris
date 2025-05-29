using System;
using System.Collections.Generic;
using System.Linq;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Represents a Tetris game board with dimensions of 10x20.
    /// Provides functionality for adding blocks, checking collisions, and removing full rows.
    /// </summary>
    public class Board
    {
        #region Constants

        /// <summary>
        /// The width of the Tetris board (10 cells).
        /// </summary>
        public const int Width = 10;

        /// <summary>
        /// The height of the Tetris board (20 cells).
        /// </summary>
        public const int Height = 20;

        #endregion

        #region Properties

        /// <summary>
        /// The grid representation of the board.
        /// A null value indicates an empty cell. Otherwise, the cell contains a block of a specific color.
        /// </summary>
        public int?[,] Grid { get; private set; }

        /// <summary>
        /// Gets the number of rows cleared since the board was created.
        /// </summary>
        public int RowsCleared { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Tetris board.
        /// </summary>
        public Board()
        {
            Grid = new int?[Width, Height];
            RowsCleared = 0;
        }

        /// <summary>
        /// Creates a deep copy of the board.
        /// </summary>
        /// <param name="board">The board to copy from.</param>
        public Board(Board board)
        {
            Grid = new int?[Width, Height];
            RowsCleared = board.RowsCleared;

            // Copy all cells from the original board
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Grid[x, y] = board.Grid[x, y];
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Clears the board by setting all cells to empty.
        /// </summary>
        public void Clear()
        {
            Grid = new int?[Width, Height];
            RowsCleared = 0;
        }

        /// <summary>
        /// Checks if the specified position is within the board boundaries.
        /// </summary>
        /// <param name="x">The x-coordinate to check.</param>
        /// <param name="y">The y-coordinate to check.</param>
        /// <returns>True if the position is within bounds, otherwise false.</returns>
        public bool IsWithinBounds(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        /// <summary>
        /// Checks if the cell at the specified position is empty.
        /// </summary>
        /// <param name="x">The x-coordinate to check.</param>
        /// <param name="y">The y-coordinate to check.</param>
        /// <returns>True if the cell is empty or outside the board, otherwise false.</returns>
        public bool IsCellEmpty(int x, int y)
        {
            // If outside the board bounds, consider it as not empty
            if (!IsWithinBounds(x, y))
                return false;

            // Return true if the cell is null (empty)
            return Grid[x, y] == null;
        }

        /// <summary>
        /// Adds a block to the specified position on the board.
        /// </summary>
        /// <param name="x">The x-coordinate where to add the block.</param>
        /// <param name="y">The y-coordinate where to add the block.</param>
        /// <param name="blockType">The type/color of the block to add.</param>
        /// <returns>True if the block was successfully added, otherwise false.</returns>
        public bool AddBlock(int x, int y, int blockType)
        {
            // Check if the position is valid
            if (!IsWithinBounds(x, y) || !IsCellEmpty(x, y))
                return false;

            // Add the block
            Grid[x, y] = blockType;
            return true;
        }

        /// <summary>
        /// Adds multiple blocks to the board at once.
        /// </summary>
        /// <param name="positions">The positions where to add blocks.</param>
        /// <param name="blockType">The type/color of the blocks to add.</param>
        /// <returns>True if all blocks were successfully added, otherwise false.</returns>
        public bool AddBlocks(IEnumerable<(int X, int Y)> positions, int blockType)
        {
            // Make a copy of the current board state
            var tempBoard = new Board(this);

            // Try to add each block
            foreach (var (x, y) in positions)
            {
                if (!tempBoard.AddBlock(x, y, blockType))
                {
                    return false;
                }
            }

            // If all blocks were added successfully, update the current board
            Grid = tempBoard.Grid;
            return true;
        }

        /// <summary>
        /// Checks if adding blocks at the specified positions would result in a collision.
        /// </summary>
        /// <param name="positions">The positions to check.</param>
        /// <returns>True if a collision would occur, otherwise false.</returns>
        public bool CheckCollision(IEnumerable<(int X, int Y)> positions)
        {
            foreach (var (x, y) in positions)
            {
                if (!IsWithinBounds(x, y) || !IsCellEmpty(x, y))
                {
                    return true; // Collision detected
                }
            }
            return false; // No collision
        }

        /// <summary>
        /// Checks if a row is full (all cells are filled with blocks).
        /// </summary>
        /// <param name="row">The row to check.</param>
        /// <returns>True if the row is full, otherwise false.</returns>
        public bool IsRowFull(int row)
        {
            if (row < 0 || row >= Height)
                return false;

            for (int x = 0; x < Width; x++)
            {
                if (Grid[x, row] == null)
                {
                    return false; // Empty cell found, row is not full
                }
            }
            return true; // Row is full
        }

        /// <summary>
        /// Removes a row from the board and shifts all rows above it down.
        /// </summary>
        /// <param name="row">The row to remove.</param>
        public void RemoveRow(int row)
        {
            if (row < 0 || row >= Height)
                return;

            // Shift all rows above the cleared row down by one
            for (int y = row; y > 0; y--)
            {
                for (int x = 0; x < Width; x++)
                {
                    Grid[x, y] = Grid[x, y - 1];
                }
            }

            // Clear the top row
            for (int x = 0; x < Width; x++)
            {
                Grid[x, 0] = null;
            }

            RowsCleared++;
        }

        /// <summary>
        /// Removes all full rows from the board and returns the number of rows removed.
        /// </summary>
        /// <returns>The number of rows removed.</returns>
        public int RemoveFullRows()
        {
            int rowsRemoved = 0;

            // Check each row from bottom to top
            for (int y = Height - 1; y >= 0; y--)
            {
                if (IsRowFull(y))
                {
                    RemoveRow(y);
                    rowsRemoved++;
                    y++; // Check the same row again as rows have shifted down
                }
            }

            return rowsRemoved;
        }

        /// <summary>
        /// Checks if the game is over (blocks have reached the top of the board).
        /// </summary>
        /// <returns>True if the game is over, otherwise false.</returns>
        public bool IsGameOver()
        {
            // If any block is in the top row, the game is over
            for (int x = 0; x < Width; x++)
            {
                if (Grid[x, 0] != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a string representation of the board for debugging purposes.
        /// </summary>
        /// <returns>A string representation of the board.</returns>
        public override string ToString()
        {
            var result = new System.Text.StringBuilder();

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    result.Append(Grid[x, y] == null ? ". " : "# ");
                }
                result.AppendLine();
            }

            return result.ToString();
        }

        #endregion
    }
}

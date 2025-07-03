using System;
using System.Collections.Generic;
using System.Drawing;

namespace Tetris.Core.Models;

/// <summary>
/// Factory class for creating Tetromino instances.
/// </summary>
public static class TetrominoFactory
{
    private static readonly Random _random = new Random();

    /// <summary>
    /// The available Tetromino types.
    /// </summary>
    public enum TetrominoType
    {
        I,
        J,
        L,
        O,
        S,
        T,
        Z
    }

    /// <summary>
    /// Creates a new Tetromino of the specified type.
    /// </summary>
    /// <param name="type">The type of Tetromino to create.</param>
    /// <returns>A new Tetromino instance.</returns>
    public static Tetromino CreateTetromino(TetrominoType type)
    {
        return type switch
        {
            TetrominoType.I => new ITetromino(),
            TetrominoType.J => new JTetromino(),
            TetrominoType.L => new LTetromino(),
            TetrominoType.O => new OTetromino(),
            TetrominoType.S => new STetromino(),
            TetrominoType.T => new TTetromino(),
            TetrominoType.Z => new ZTetromino(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid Tetromino type")
        };
    }

    /// <summary>
    /// Creates a random Tetromino.
    /// </summary>
    /// <returns>A random Tetromino instance.</returns>
    public static Tetromino CreateRandomTetromino()
    {
        Array values = Enum.GetValues(typeof(TetrominoType));
        TetrominoType randomType = (TetrominoType)(values.GetValue(_random.Next(values.Length)) ?? TetrominoType.I);
        return CreateTetromino(randomType);
    }

    /// <summary>
    /// Creates a Tetromino from saved state data.
    /// </summary>
    /// <param name="tetrominoState">The saved tetromino state.</param>
    /// <returns>A Tetromino instance restored from the saved state.</returns>
    /// <exception cref="ArgumentNullException">Thrown when tetrominoState is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the tetromino ID is invalid.</exception>
    public static Tetromino CreateFromState(TetrominoState tetrominoState)
    {
        if (tetrominoState == null)
            throw new ArgumentNullException(nameof(tetrominoState));

        Tetromino tetromino = tetrominoState.Id switch
        {
            1 => new ITetromino(),
            2 => new JTetromino(),
            3 => new LTetromino(),
            4 => new OTetromino(),
            5 => new STetromino(),
            6 => new TTetromino(),
            7 => new ZTetromino(),
            _ => throw new ArgumentException($"Invalid tetromino ID: {tetrominoState.Id}", nameof(tetrominoState))
        };

        // Restore position and rotation state
        tetromino.Position = tetrominoState.Position;
        tetromino.RotationState = tetrominoState.RotationState;

        return tetromino;
    }

    /// <summary>
    /// Creates a Tetromino by its ID.
    /// </summary>
    /// <param name="id">The tetromino ID.</param>
    /// <returns>A new Tetromino instance.</returns>
    /// <exception cref="ArgumentException">Thrown when the ID is invalid.</exception>
    public static Tetromino CreateById(int id)
    {
        return id switch
        {
            1 => new ITetromino(),
            2 => new JTetromino(),
            3 => new LTetromino(),
            4 => new OTetromino(),
            5 => new STetromino(),
            6 => new TTetromino(),
            7 => new ZTetromino(),
            _ => throw new ArgumentException($"Invalid tetromino ID: {id}", nameof(id))
        };
    }
}

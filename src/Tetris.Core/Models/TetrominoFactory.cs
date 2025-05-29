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
        TetrominoType randomType = (TetrominoType)values.GetValue(_random.Next(values.Length));
        return CreateTetromino(randomType);
    }
}

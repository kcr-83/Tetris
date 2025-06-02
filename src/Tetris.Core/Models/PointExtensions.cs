using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace Tetris.Core.Models
{
    /// <summary>
    /// Extension methods for Point and tuple conversions used in the Tetris game.
    /// </summary>
    public static class PointExtensions
    {
        /// <summary>
        /// Converts an array of Points to an enumerable of (int X, int Y) tuples.
        /// </summary>
        /// <param name="points">The array of points to convert.</param>
        /// <returns>An enumerable of (int X, int Y) tuples.</returns>
        public static IEnumerable<(int X, int Y)> ToTuples(this Point[] points)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));
                
            return points.Select(p => (p.X, p.Y));
        }
        
        /// <summary>
        /// Converts an enumerable of (int X, int Y) tuples to an array of Points.
        /// </summary>
        /// <param name="tuples">The tuples to convert.</param>
        /// <returns>An array of Points.</returns>
        public static Point[] ToPoints(this IEnumerable<(int X, int Y)> tuples)
        {
            if (tuples == null)
                throw new ArgumentNullException(nameof(tuples));
                
            return tuples.Select(t => new Point(t.X, t.Y)).ToArray();
        }
    }
}

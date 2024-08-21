using System;
using System.Globalization;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FlaxEngine
{
    /// <summary>
    /// Represents a two dimensional mathematical vector, 
    /// but both values are integer.
    /// </summary>
    [Unmanaged]
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public unsafe partial struct Vector2Int
    {
        private static readonly string _formatString = "X:{0:F2} Y:{1:F2}";

        /// <summary>
        /// The X Component.
        /// </summary>
        public int X;
        /// <summary>
        /// The Y Component.
        /// </summary>
        public int Y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Int" /> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Vector2Int(int value)
        {
            X = value;
            Y = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Int" /> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Vector2Int(float value)
        {
            X = (int)value;
            Y = (int)value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Int" /> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Vector2Int(double value)
        {
            X = (int)value;
            Y = (int)value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Int" /> struct.
        /// </summary>
        /// <param name="x">Initial value for the X component of the vector.</param>
        /// <param name="y">Initial value for the Y component of the vector.</param>
        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector2Int" /> struct.
        /// </summary>
        /// <param name="vector">The Vector2 value to intial.</param>
        public Vector2Int(Vector2 vector)
        {
            X = (int)vector.X;
            Y = (int)vector.Y;
        }

        #region Static Read-Only

        /// <summary>
        /// The size of the <see cref="Vector2Int" /> type, in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector2Int));

        /// <summary>
        /// A <see cref="Vector2Int" /> with all of its components set to zero.
        /// </summary>
        public static readonly Vector2Int Zero = new(0, 0);

        /// <summary>
        /// The X unit <see cref="Vector2Int" /> (1, 0).
        /// </summary>
        public static readonly Vector2Int UnitX = new(1, 0);

        /// <summary>
        /// The X unit <see cref="Vector2Int" /> (0, 1).
        /// </summary>
        public static readonly Vector2Int UnitY = new(0, 1);

        /// <summary>
        /// A <see cref="Vector2Int" /> with all of its components set to one.
        /// </summary>
        public static readonly Vector2Int One = new(1, 1);

        /// <summary>
        /// A <see cref="Vector2Int" /> with all of its components set to <see cref="int.MinValue"/>
        /// </summary>
        public static readonly Vector2Int Minimum = new(int.MinValue);

        /// <summary>
        /// A <see cref="Vector2Int" /> with all of its components set to <see cref="int.MaxValue"/>
        /// </summary>
        public static readonly Vector2Int Maximum = new(int.MaxValue);

        /// <summary>
        /// A <see cref="Vector2Int" /> with its X components set to <see cref="int.MinValue"/>
        /// </summary>
        public static readonly Vector2Int MinimumX = new(int.MinValue, 0);

        /// <summary>
        /// A <see cref="Vector2Int" /> with its Y components set to <see cref="int.MinValue"/>
        /// </summary>
        public static readonly Vector2Int MinimumY = new(0, int.MinValue);

        /// <summary>
        /// A <see cref="Vector2Int" /> with its X components set to <see cref="int.MaxValue"/>
        /// </summary>
        public static readonly Vector2Int MaximumX = new(int.MaxValue, 0);

        /// <summary>
        /// A <see cref="Vector2Int" /> with its Y components set to <see cref="int.MaxValue"/>
        /// </summary>
        public static readonly Vector2Int MaximumY = new(0, int.MaxValue);

        #endregion

        #region Operators
        public static Vector2Int operator +(Vector2Int left, Vector2Int right)
        {
            return new Vector2Int(left.X + right.X, left.Y + right.Y);
        }
        #endregion
    }
}

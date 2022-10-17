#region License
/*This file is part of Satsuma Graph Library
Copyright © 2013 Balázs Szalkai

This software is provided 'as-is', without any express or implied
warranty. In no event will the authors be held liable for any damages
arising from the use of this software.

Permission is granted to anyone to use this software for any purpose,
including commercial applications, and to alter it and redistribute it
freely, subject to the following restrictions:

   1. The origin of this software must not be misrepresented; you must not
   claim that you wrote the original software. If you use this software
   in a product, an acknowledgment in the product documentation would be
   appreciated but is not required.

   2. Altered source versions must be plainly marked as such, and must not be
   misrepresented as being the original software.

   3. This notice may not be removed or altered from any source
   distribution.

Updated by Unchase © 2022*/
#endregion

using System.Drawing;
using System.Globalization;

// ReSharper disable StructCanBeMadeReadOnly

namespace Unchase.Satsuma.Drawing
{
    /// <summary>
    /// An immutable point whose coordinates are double.
    /// </summary>
    public struct PointD : 
        IEquatable<PointD>
    {
        /// <summary>
        /// X.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Y.
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Initialize <see cref="PointD"/>.
        /// </summary>
        /// <param name="x">X.</param>
        /// <param name="y">Y.</param>
        public PointD(double x, double y)
            : this()
        {
            X = x;
            Y = y;
        }

        /// <inheritdoc />
        public bool Equals(PointD other) => X.Equals(other.X) && Y.Equals(other.Y);

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is not PointD d)
            {
                return false;
            }

            return Equals(d);
        }

        /// <summary>
        /// == operator.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        public static bool operator ==(PointD a, PointD b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// != operator.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        public static bool operator !=(PointD a, PointD b)
        {
            return !(a == b);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return (X.GetHashCode() * 17) + Y.GetHashCode();
        }

        /// <summary>
        /// Get formatted string.
        /// </summary>
        /// <param name="provider"><see cref="IFormatProvider"/>.</param>
        public string ToString(IFormatProvider provider)
        {
            return string.Format(provider, "({0} {1})", X, Y);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns the vector sum of two points.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        public static PointD operator +(PointD a, PointD b)
        {
            return new(a.X + b.X, a.Y + b.Y);
        }

        /// <summary>
        /// Returns the vector sum of two points.
        /// </summary>
        /// <remarks>
        /// Added for CLS compliancy.
        /// </remarks>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        public static PointD Add(PointD a, PointD b)
        {
            return a + b;
        }

        /// <param name="self">The self point.</param>
        public static explicit operator PointF(PointD self)
        {
            return new PointF((float)self.X, (float)self.Y);
        }

        /// <remarks>
        /// Added for CLS compliancy.
        /// </remarks>
        /// <param name="self"></param>
        /// <returns></returns>
        public static PointF ToPointF(PointD self)
        {
            return (PointF)self;
        }

        /// <summary>
        /// Returns the Euclidean distance from another point.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public double Distance(PointD other)
        {
            return Math.Sqrt((X - other.X) * (X - other.X) + (Y - other.Y) * (Y - other.Y));
        }
    }
}
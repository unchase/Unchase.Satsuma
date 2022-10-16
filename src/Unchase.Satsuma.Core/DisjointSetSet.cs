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
   distribution.*/
# endregion

namespace Unchase.Satsuma.Core
{
    /// <summary>
    /// Represents a set in the DisjointSet data structure.
    /// </summary>
    /// <remarks>
    /// The purpose is to ensure type safety by distinguishing between sets and their representatives.
    /// </remarks>
    /// <typeparam name="T">Type of element.</typeparam>
    public readonly struct DisjointSetSet<T> :
        IEquatable<DisjointSetSet<T>>
            where T : IEquatable<T>
    {
        /// <summary>
        /// Representative.
        /// </summary>
        public T Representative { get; }

        /// <summary>
        /// Initialize <see cref="DisjointSetSet{T}"/>.
        /// </summary>
        /// <param name="representative">Representative</param>
        public DisjointSetSet(T representative)
            : this()
        {
            Representative = representative;
        }

        /// <inheritdoc />
        public bool Equals(DisjointSetSet<T> other)
        {
            return Representative.Equals(other.Representative);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is DisjointSetSet<T> set)
            {
                return Equals(set);
            }

            return false;
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        public static bool operator ==(DisjointSetSet<T> a, DisjointSetSet<T> b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="a">The first operand.</param>
        /// <param name="b">The second operand.</param>
        public static bool operator !=(DisjointSetSet<T> a, DisjointSetSet<T> b)
        {
            return !(a == b);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Representative.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "[DisjointSetSet:" + Representative + "]";
        }
    }
}
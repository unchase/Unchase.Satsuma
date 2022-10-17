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
# endregion

using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Core
{
    /// <summary>
    /// Represents a graph arc, consisting of a wrapped <see cref="Id"/>.
    /// </summary>
    /// <remarks>
    /// Arcs can be either directed or undirected. Undirected arcs are called edges.
    /// Endpoints and directedness of an arc are not stored in this object, but rather they can be queried
    /// using methods of the containing graph (see <see cref="IArcLookup"/>).
    /// </remarks>
    public readonly struct Arc : 
        IEquatable<Arc>
    {
        /// <summary>
        /// The integer which uniquely identifies the arc within its containing graph.
        /// </summary>
        /// <remarks>
        /// Arcs belonging to different graph objects may have the same Id.
        /// </remarks>
        public long Id { get; }

        /// <summary>
        /// Initialize <see cref="Arc"/>.
        /// </summary>
        /// <param name="id">Supplied id</param>
        public Arc(long id)
            : this()
        {
            Id = id;
        }

        /// <summary>
        /// A special arc value, denoting an invalid arc.
        /// </summary>
        /// <remarks>
        /// This is the default value for the Arc type.
        /// </remarks>
        public static Arc Invalid => new(0);

        /// <inheritdoc />
        public bool Equals(Arc other)
        {
            return Id == other.Id;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is Arc arc)
            {
                return Equals(arc);
            }

            return false;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "|" + Id;
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="a">The first arc.</param>
        /// <param name="b">The second arc.</param>
        public static bool operator ==(Arc a, Arc b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="a">The first arc.</param>
        /// <param name="b">The second arc.</param>
        public static bool operator !=(Arc a, Arc b)
        {
            return !(a == b);
        }
    }
}
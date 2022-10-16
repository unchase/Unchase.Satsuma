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
    /// Represents a graph node, consisting of a wrapped #Id.
    /// </summary>
    public readonly struct Node : 
        IEquatable<Node>
    {
        /// <summary>
        /// The integer which uniquely identifies the node within its containing graph.
        /// </summary>
        /// <remarks>
        /// Nodes belonging to different graph objects may have the same Id.
        /// </remarks>
        public long Id { get; }

        /// <summary>
        /// Initialize <see cref="Node"/>.
        /// </summary>
        /// <param name="id">Supplied id</param>
        public Node(long id)
            : this()
        {
            Id = id;
        }

        /// <summary>
        /// A special node value, denoting an invalid node.
        /// </summary>
        /// <remarks>
        /// This is the default value for the Node type.
        /// </remarks>
        public static Node Invalid => new(0);

        /// <inheritdoc />
        public bool Equals(Node other)
        {
            return Id == other.Id;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is Node node)
            {
                return Equals(node);
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
            return "#" + Id;
        }

        /// <summary>
        /// Operator ==.
        /// </summary>
        /// <param name="a">The first node.</param>
        /// <param name="b">The second node.</param>
        public static bool operator ==(Node a, Node b)
        {
            return a.Equals(b);
        }

        /// <summary>
        /// Operator !=.
        /// </summary>
        /// <param name="a">The first node.</param>
        /// <param name="b">The second node.</param>
        /// <returns></returns>
        public static bool operator !=(Node a, Node b)
        {
            return !(a == b);
        }
    }
}
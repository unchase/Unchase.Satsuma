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

namespace Unchase.Satsuma.Core
{
    /// <inheritdoc cref="ArcProperties{TProperty}"/>
    public class ArcProperties :
        ArcProperties<object>
    {
        /// <summary>
        /// Initialize <see cref="ArcProperties"/>.
        /// </summary>
        /// <param name="u">U node.</param>
        /// <param name="v">V node.</param>
        /// <param name="isEdge">Is edge.</param>
        /// <param name="properties">The arc properties.</param>
        public ArcProperties(
            Node u,
            Node v,
            bool isEdge,
            Dictionary<string, object>? properties = default)
                : base(u, v, isEdge, properties)
        {
        }
    }

    /// <summary>
    /// The arc properties.
    /// </summary>
    /// <typeparam name="TProperty">The type of stored arc properties.</typeparam>
    public class ArcProperties<TProperty>
    {
        /// <summary>
        /// The first node.
        /// </summary>
        public Node U { get; }

        /// <summary>
        /// The second node.
        /// </summary>
        public Node V { get; }

        /// <summary>
        /// The arc is edge.
        /// </summary>
        public bool IsEdge { get; }

        /// <summary>
        /// The arc properties.
        /// </summary>
        public Dictionary<string, TProperty>? Properties { get; }

        /// <summary>
        /// Initialize <see cref="ArcProperties{TProperty}"/>.
        /// </summary>
        /// <param name="u">U node.</param>
        /// <param name="v">V node.</param>
        /// <param name="isEdge">Is edge.</param>
        /// <param name="properties">The arc properties.</param>
        public ArcProperties(
            Node u,
            Node v,
            bool isEdge,
            Dictionary<string, TProperty>? properties = default)
        {
            U = u;
            V = v;
            IsEdge = isEdge;
            Properties = properties;
        }
	}
}
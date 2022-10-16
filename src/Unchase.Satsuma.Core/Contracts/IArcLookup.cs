﻿#region License
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

using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Core.Contracts
{
    /// <summary>
    /// A graph which can provide information about its arcs.
    /// </summary>
    /// <remarks>
    /// <seealso cref="ArcLookupExtensions"/>.
    /// </remarks>
    public interface IArcLookup
    {
        /// <summary>
        /// Get the first node of an arc.
        /// </summary>
        /// <remarks>
        /// Directed arcs point from U to V.
        /// </remarks>
        /// <param name="arc">Arc.</param>
        /// <returns>Returns the first node of an arc.</returns>
        public Node U(Arc arc);

        /// <summary>
        /// Get the second node of an arc.
        /// </summary>
        /// <remarks>
        /// Directed arcs point from U to V.
        /// </remarks>
        /// <param name="arc">Arc.</param>
        /// <returns>Returns the second node of an arc.</returns>
        public Node V(Arc arc);

        /// <summary>
        /// The arc is edge.
        /// </summary>
        /// <param name="arc">Arc.</param>
        /// <returns>Returns whether the arc is undirected (true) or directed (false).</returns>
        public bool IsEdge(Arc arc);
    }
}
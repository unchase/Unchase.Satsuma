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

using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Adapters.Abstractions
{
    /// <summary>
    /// Interface to a read-only matching.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A matching is a subgraph without loop arcs
    /// where the degree of each node of the containing graph is at most 1.
    /// </para>
    /// <para>The node set of a matching consists of those nodes whose degree is 1 in the matching.</para>
    /// </remarks>
    public interface IMatching : 
        IGraph
    {
        /// <summary>
        /// The underlying graph, i.e. the graph containing the matching.
        /// </summary>
        public IGraph Graph { get; }

        /// <summary>
        /// Gets the matching arc which contains the given node.
        /// </summary>
        /// <remarks>
        /// Equivalent to <tt>Arcs(node).FirstOrDefault()</tt>, but should be faster.
        /// </remarks>
        /// <param name="node">A node of <see cref="Graph"/>.</param>
        /// <returns>Returns the arc which matches the given node, or <see cref="Arc.Invalid"/> if the node is unmatched.</returns>
        public Arc MatchedArc(Node node);
    }
}
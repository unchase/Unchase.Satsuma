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

namespace Unchase.Satsuma.Core.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IArcLookup"/>.
    /// </summary>
    public static class ArcLookupExtensions
    {
        /// <summary>
        /// Converts an arc to a readable string representation by looking up its nodes.
        /// </summary>
        /// <param name="graph"><see cref="IArcLookup"/>.</param>
        /// <param name="arc">An arc belonging to the graph, or Arc.Invalid.</param>
        public static string ArcToString(
            this IArcLookup graph, 
            Arc arc)
        {
            if (arc == Arc.Invalid)
            {
                return "Arc.Invalid";
            }

            return graph.U(arc) + (graph.IsEdge(arc) ? "<-->" : "--->") + graph.V(arc);
        }

        /// <summary>
        /// Returns U(arc) if it is different from the given node, or 
        /// V(arc) if U(arc) equals to the given node.
        /// </summary>
        /// <remarks>
        /// If the given node is on the given arc, then this function returns the other node of the arc.
        /// </remarks>
        /// <param name="graph"><see cref="IArcLookup"/>.</param>
        /// <param name="arc">An arc belonging to the graph.</param>
        /// <param name="node">An arbitrary node, may even be Node.Invalid.</param>
        public static Node Other(
            this IArcLookup graph, 
            Arc arc, 
            Node node)
        {
            var u = graph.U(arc);
            return u != node 
                ? u
                : graph.V(arc);
        }

        /// <summary>
        /// Get the two nodes of an arc.
        /// </summary>
        /// <param name="graph"><see cref="IArcLookup"/>.</param>
        /// <param name="arc">An arc belonging to the graph.</param>
        /// <param name="allowDuplicates">
        /// If true, then the resulting array always contains two items, even if the arc connects a node with itself.
        /// If false, then the resulting array contains only one node if the arc is a loop.
        /// </param>
        /// <returns>Returns the two nodes of an arc.</returns>
        public static Node[] Nodes(
            this IArcLookup graph, 
            Arc arc, 
            bool allowDuplicates = true)
        {
            var u = graph.U(arc);
            var v = graph.V(arc);
            if (!allowDuplicates && u == v)
            {
                return new[] { u };
            }

            return new[] { u, v };
        }
    }
}
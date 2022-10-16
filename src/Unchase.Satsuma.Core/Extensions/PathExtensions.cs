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

using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;

namespace Unchase.Satsuma.Core.Extensions
{
    /// <summary>
    /// Extension methods to <see cref="IPath"/>.
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        /// Returns true if FirstNode equals LastNode and the path has at least one arc.
        /// </summary>
        /// <param name="path"><see cref="IPath"/>.</param>
        public static bool IsCycle(this IPath path)
        {
            return path.FirstNode == path.LastNode && path.ArcCount() > 0;
        }

        /// <summary>
        /// Get the successor of a node in the path.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="Node.Invalid"/> if the node is not on the path or has no successor.
        /// If the path is a cycle, then each node has a successor.
        /// </remarks>
        /// <param name="path"><see cref="IPath"/>.</param>
        /// <param name="node">The node.</param>
        /// <returns>Returns the successor of a node in the path.</returns>
        public static Node NextNode(this IPath path, Node node)
        {
            var arc = path.NextArc(node);
            if (arc == Arc.Invalid)
            {
                return Node.Invalid;
            }

            return path.Other(arc, node);
        }

        /// <summary>
        /// Get the predecessor of a node in the path.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="Node.Invalid"/> if the node is not on the path or has no predecessor.
        /// If the path is a cycle, then each node has a predecessor.
        /// </remarks>
        /// <param name="path"><see cref="IPath"/>.</param>
        /// <param name="node">The node.</param>
        /// <returns>Returns the predecessor of a node in the path.</returns>
        public static Node PrevNode(this IPath path, Node node)
        {
            var arc = path.PrevArc(node);
            if (arc == Arc.Invalid)
            {
                return Node.Invalid;
            }

            return path.Other(arc, node);
        }

        /// <summary>
        /// Implements IGraph.Arcs for paths.
        /// </summary>
        /// <param name="path"><see cref="IPath"/>.</param>
        /// <param name="u">U node.</param>
        /// <param name="filter"><see cref="ArcFilter"/>.</param>
        public static IEnumerable<Arc> ArcsHelper(this IPath path, Node u, ArcFilter filter)
        {
            Arc arc1 = path.PrevArc(u), arc2 = path.NextArc(u);
            if (arc1 == arc2) 
            {
                arc2 = Arc.Invalid; // avoid duplicates
            }

            for (var i = 0; i < 2; i++)
            {
                var arc = (i == 0 ? arc1 : arc2);
                if (arc == Arc.Invalid)
                {
                    continue;
                }

                switch (filter)
                {
                    case ArcFilter.All: 
                        yield return arc; 
                        break;
                    case ArcFilter.Edge:
                        if (path.IsEdge(arc))
                        {
                            yield return arc;
                        } 
                        break;
                    case ArcFilter.Forward:
                        if (path.IsEdge(arc) || path.U(arc) == u)
                        {
                            yield return arc;
                        } 
                        break;
                    case ArcFilter.Backward:
                        if (path.IsEdge(arc) || path.V(arc) == u)
                        {
                            yield return arc;
                        } 
                        break;
                }
            }
        }
    }
}
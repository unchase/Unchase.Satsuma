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

using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Algorithms.Connectivity
{
    /// <summary>
    /// Finds the bridges and 2-edge-connected components in a graph.
    /// </summary>
    /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
    public sealed class BiEdgeConnectedComponents<TNodeProperty, TArcProperty>
    {
        /// <summary>
        /// The input graph.
        /// </summary>
        public IGraph<TNodeProperty, TArcProperty> Graph { get; }

        /// <summary>
        /// The number of 2-edge-connected components in the graph.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// The 2-edge-connected components of the graph.
        /// </summary>
        /// <remarks>
        /// Null if <see cref="BiEdgeConnectedComponentsFlags.CreateComponents"/> was not set during construction.
        /// </remarks>
        public List<HashSet<Node>>? Components { get; }

        /// <summary>
        /// The bridges of the graph.
        /// </summary>
        /// <remarks>
        /// Null if <see cref="BiEdgeConnectedComponentsFlags.CreateBridges"/> was not set during construction.
        /// </remarks>
        public HashSet<Arc>? Bridges { get; }

        /// <summary>
        /// Initialize <see cref="BiEdgeConnectedComponents{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="flags"><see cref="BiEdgeConnectedComponentsFlags"/>.</param>
        public BiEdgeConnectedComponents(
            IGraph<TNodeProperty, TArcProperty> graph, 
            BiEdgeConnectedComponentsFlags flags = 0)
        {
            Graph = graph;
            var dfs = new BridgeDfs<TNodeProperty, TArcProperty>(graph);
            dfs.Run();

            Count = dfs.ComponentCount;
            if (0 != (flags & BiEdgeConnectedComponentsFlags.CreateBridges))
            {
                Bridges = dfs.Bridges;
            }

            if (0 != (flags & BiEdgeConnectedComponentsFlags.CreateComponents))
            {
                var withoutBridges = new Subgraph<TNodeProperty, TArcProperty>(graph);
                foreach (var arc in dfs.Bridges ?? new())
                {
                    withoutBridges.Enable(arc, false);
                }

                Components = 
                    new ConnectedComponents<TNodeProperty, TArcProperty>(withoutBridges, ConnectedComponentsFlags.CreateComponents).Components;
            }
        }
    }
}
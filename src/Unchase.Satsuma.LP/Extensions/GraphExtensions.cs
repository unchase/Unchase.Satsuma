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
using Unchase.Satsuma.LP.Contracts;

namespace Unchase.Satsuma.LP.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IGraph{TNodeProperty,TArcProperty}"/> and <see cref="IGraph"/>.
    /// </summary>
    public static class GraphExtensions
    {
        #region OptimalSubgraph

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="OptimalSubgraph{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <returns>Returns <see cref="OptimalSubgraph{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static OptimalSubgraph<TNodeProperty, TArcProperty> ToOptimalSubgraph<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph)
        {
            return new(graph);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="OptimalSubgraph"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <returns>Returns <see cref="OptimalSubgraph"/> algorithm.</returns>
        public static OptimalSubgraph ToOptimalSubgraph(
            this IGraph graph)
        {
            return new(graph);
        }

        #endregion OptimalSubgraph

        #region MinimumVertexCover

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="MinimumVertexCover{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="solver"><see cref="ISolver"/>.</param>
        /// <param name="nodeCost">A finite cost function on the nodes of Graph.</param>
        /// <param name="arcWeight">A finite weight function on the arcs of Graph.</param>
        /// <param name="relaxed">If true, each node can be chosen with a fractional weight.</param>
        /// <returns>Returns <see cref="MinimumVertexCover{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static MinimumVertexCover<TNodeProperty, TArcProperty> ToMinimumVertexCover<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            ISolver solver,
            Func<Node, double>? nodeCost = null,
            Func<Arc, double>? arcWeight = null,
            bool relaxed = false)
        {
            return new(solver, graph, nodeCost, arcWeight, relaxed);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="MinimumVertexCover"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="solver"><see cref="ISolver"/>.</param>
        /// <param name="nodeCost">A finite cost function on the nodes of Graph.</param>
        /// <param name="arcWeight">A finite weight function on the arcs of Graph.</param>
        /// <param name="relaxed">If true, each node can be chosen with a fractional weight.</param>
        /// <returns>Returns <see cref="MinimumVertexCover"/> algorithm.</returns>
        public static MinimumVertexCover ToMinimumVertexCover(
            this IGraph graph,
            ISolver solver,
            Func<Node, double>? nodeCost = null,
            Func<Arc, double>? arcWeight = null,
            bool relaxed = false)
        {
            return new(solver, graph, nodeCost, arcWeight, relaxed);
        }

        #endregion MinimumVertexCover

        #region MaximumStableSet

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="MaximumStableSet{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="solver"><see cref="ISolver"/>.</param>
        /// <param name="weight">A finite weight function on the nodes of Graph.</param>
        /// <returns>Returns <see cref="MaximumStableSet{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static MaximumStableSet<TNodeProperty, TArcProperty> ToMaximumStableSet<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            ISolver solver,
            Func<Node, double>? weight = null)
        {
            return new(solver, graph, weight);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="MaximumStableSet"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="solver"><see cref="ISolver"/>.</param>
        /// <param name="weight">A finite weight function on the nodes of Graph.</param>
        /// <returns>Returns <see cref="MaximumStableSet"/> algorithm.</returns>
        public static MaximumStableSet ToMaximumStableSet(
            this IGraph graph,
            ISolver solver,
            Func<Node, double>? weight = null)
        {
            return new(solver, graph, weight);
        }

        #endregion MaximumStableSet

        #region LpMaximumMatching

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="LpMaximumMatching{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="solver"><see cref="ISolver"/>.</param>
        /// <returns>Returns <see cref="LpMaximumMatching{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static LpMaximumMatching<TNodeProperty, TArcProperty> ToLpMaximumMatching<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            ISolver solver)
        {
            return new(solver, graph);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="LpMaximumMatching"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="solver"><see cref="ISolver"/>.</param>
        /// <returns>Returns <see cref="LpMaximumMatching"/> algorithm.</returns>
        public static LpMaximumMatching ToLpMaximumMatching(
            this IGraph graph,
            ISolver solver)
        {
            return new(solver, graph);
        }

        #endregion LpMaximumMatching

        #region LpMinimumCostMatching

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="LpMinimumCostMatching{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="solver"><see cref="ISolver"/>.</param>
        /// <param name="cost">A finite cost function on the arcs of Graph.</param>
        /// <param name="minimumMatchingSize">Minimum constraint on the size (number of arcs) of the returned matching.</param>
        /// <param name="maximumMatchingSize">Maximum constraint on the size (number of arcs) of the returned matching.</param>
        /// <returns>Returns <see cref="LpMinimumCostMatching{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static LpMinimumCostMatching<TNodeProperty, TArcProperty> ToLpMinimumCostMatching<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            ISolver solver,
            Func<Arc, double> cost,
            int minimumMatchingSize = 0,
            int maximumMatchingSize = int.MaxValue)
        {
            return new(solver, graph, cost, minimumMatchingSize, maximumMatchingSize);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="LpMinimumCostMatching"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="solver"><see cref="ISolver"/>.</param>
        /// <param name="cost">A finite cost function on the arcs of Graph.</param>
        /// <param name="minimumMatchingSize">Minimum constraint on the size (number of arcs) of the returned matching.</param>
        /// <param name="maximumMatchingSize">Maximum constraint on the size (number of arcs) of the returned matching.</param>
        /// <returns>Returns <see cref="LpMinimumCostMatching"/> algorithm.</returns>
        public static LpMinimumCostMatching ToLpMinimumCostMatching(
            this IGraph graph,
            ISolver solver,
            Func<Arc, double> cost,
            int minimumMatchingSize = 0,
            int maximumMatchingSize = int.MaxValue)
        {
            return new(solver, graph, cost, minimumMatchingSize, maximumMatchingSize);
        }

        #endregion LpMinimumCostMatching
    }
}
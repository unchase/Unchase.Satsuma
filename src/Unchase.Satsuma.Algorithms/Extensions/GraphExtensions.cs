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

using Unchase.Satsuma.Algorithms.Connectivity;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Algorithms.SpanningForest;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Algorithms.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IGraph{TNodeProperty,TArcProperty}"/> and <see cref="IGraph"/>.
    /// </summary>
    public static class GraphExtensions
    {
        #region AStar

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="AStar{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="cost">A non-negative arc cost function.</param>
        /// <param name="heuristic">The A* heuristic function.</param>
        /// <returns>Returns <see cref="AStar{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static AStar<TNodeProperty, TArcProperty> ToAStar<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Func<Arc, double> cost,
            Func<Node, double> heuristic)
        {
            return new(graph, cost, heuristic);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="AStar"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="cost">A non-negative arc cost function.</param>
        /// <param name="heuristic">The A* heuristic function.</param>
        /// <returns>Returns <see cref="AStar"/> algorithm.</returns>
        public static AStar ToAStar(
            this IGraph graph,
            Func<Arc, double> cost,
            Func<Node, double> heuristic)
        {
            return new(graph, cost, heuristic);
        }

        #endregion AStar

        #region BellmanFord

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="BellmanFord{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="cost">The arc cost function. Each value must be finite or positive infinity.</param>
        /// <param name="sources">The source nodes.</param>
        /// <returns>Returns <see cref="BellmanFord{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static BellmanFord<TNodeProperty, TArcProperty> ToBellmanFord<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Func<Arc, double> cost,
            IEnumerable<Node> sources)
        {
            return new(graph, cost, sources);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="BellmanFord"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="cost">The arc cost function. Each value must be finite or positive infinity.</param>
        /// <param name="sources">The source nodes.</param>
        /// <returns>Returns <see cref="BellmanFord"/> algorithm.</returns>
        public static BellmanFord ToBellmanFord(
            this IGraph graph,
            Func<Arc, double> cost,
            IEnumerable<Node> sources)
        {
            return new(graph, cost, sources);
        }

        #endregion BellmanFord

        #region Bfs

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="Bfs{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <returns>Returns <see cref="Bfs{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static Bfs<TNodeProperty, TArcProperty> ToBfs<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph)
        {
            return new(graph);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="Bfs"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <returns>Returns <see cref="Bfs"/> algorithm.</returns>
        public static Bfs ToBfs(
            this IGraph graph)
        {
            return new(graph);
        }

        #endregion Bfs

        #region Dijkstra

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="Dijkstra{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="cost">The arc cost function.</param>
        /// <param name="mode">The path cost calculation mode.</param>
        /// <returns>Returns <see cref="Dijkstra{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static Dijkstra<TNodeProperty, TArcProperty> ToDijkstra<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Func<Arc, double> cost,
            DijkstraMode mode)
        {
            return new(graph, cost, mode);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="Dijkstra"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="cost">The arc cost function.</param>
        /// <param name="mode">The path cost calculation mode.</param>
        /// <returns>Returns <see cref="Dijkstra"/> algorithm.</returns>
        public static Dijkstra ToDijkstra(
            this IGraph graph,
            Func<Arc, double> cost,
            DijkstraMode mode)
        {
            return new(graph, cost, mode);
        }

        #endregion Dijkstra

        #region Preflow

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="Preflow{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="capacity">The arc capacity function.</param>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>Returns <see cref="Preflow{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static Preflow<TNodeProperty, TArcProperty> ToPreflow<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Func<Arc, double> capacity,
            Node source,
            Node target)
        {
            return new(graph, capacity, source, target);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="Preflow"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="capacity">The arc capacity function.</param>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>Returns <see cref="Preflow"/> algorithm.</returns>
        public static Preflow ToPreflow(
            this IGraph graph,
            Func<Arc, double> capacity,
            Node source,
            Node target)
        {
            return new(graph, capacity, source, target);
        }

        #endregion Preflow

        #region IntegerPreflow

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="IntegerPreflow{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="capacity">The arc capacity function.</param>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>Returns <see cref="IntegerPreflow{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static IntegerPreflow<TNodeProperty, TArcProperty> ToIntegerPreflow<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Func<Arc, long> capacity,
            Node source,
            Node target)
        {
            return new(graph, capacity, source, target);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="IntegerPreflow"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="capacity">The arc capacity function.</param>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>Returns <see cref="IntegerPreflow"/> algorithm.</returns>
        public static IntegerPreflow ToIntegerPreflow(
            this IGraph graph,
            Func<Arc, long> capacity,
            Node source,
            Node target)
        {
            return new(graph, capacity, source, target);
        }

        #endregion IntegerPreflow

        #region NetworkSimplex

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="NetworkSimplex{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="lowerBound">The lower bound for the circulation.</param>
        /// <param name="upperBound">The upper bound for the circulation.</param>
        /// <param name="supply">The desired difference of outgoing and incoming flow for a node. Must be finite.</param>
        /// <param name="cost">The cost of sending a unit of circulation through an arc. Must be finite.</param>
        /// <returns>Returns <see cref="NetworkSimplex{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static NetworkSimplex<TNodeProperty, TArcProperty> ToNetworkSimplex<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Func<Arc, long>? lowerBound = null,
            Func<Arc, long>? upperBound = null,
            Func<Node, long>? supply = null,
            Func<Arc, double>? cost = null)
        {
            return new(graph, lowerBound, upperBound, supply, cost);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="NetworkSimplex"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="lowerBound">The lower bound for the circulation.</param>
        /// <param name="upperBound">The upper bound for the circulation.</param>
        /// <param name="supply">The desired difference of outgoing and incoming flow for a node. Must be finite.</param>
        /// <param name="cost">The cost of sending a unit of circulation through an arc. Must be finite.</param>
        /// <returns>Returns <see cref="NetworkSimplex"/> algorithm.</returns>
        public static NetworkSimplex ToNetworkSimplex(
            this IGraph graph,
            Func<Arc, long>? lowerBound = null,
            Func<Arc, long>? upperBound = null,
            Func<Node, long>? supply = null,
            Func<Arc, double>? cost = null)
        {
            return new(graph, lowerBound, upperBound, supply, cost);
        }

        #endregion NetworkSimplex

        #region BipartiteMinimumCostMatching

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="BipartiteMinimumCostMatching{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="isRed">Describes a bipartition of Graph by dividing its nodes into red and blue ones.</param>
        /// <param name="cost">A finite cost function on the arcs of Graph.</param>
        /// <param name="minimumMatchingSize">Minimum constraint on the size (number of arcs) of the returned matching.</param>
        /// <param name="maximumMatchingSize">Maximum constraint on the size (number of arcs) of the returned matching.</param>
        /// <returns>Returns <see cref="BipartiteMinimumCostMatching{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static BipartiteMinimumCostMatching<TNodeProperty, TArcProperty> ToBipartiteMinimumCostMatching<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Func<Node, bool> isRed,
            Func<Arc, double> cost,
            int minimumMatchingSize = 0,
            int maximumMatchingSize = int.MaxValue)
        {
            return new(graph, isRed, cost, minimumMatchingSize, maximumMatchingSize);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="BipartiteMinimumCostMatching"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="isRed">Describes a bipartition of Graph by dividing its nodes into red and blue ones.</param>
        /// <param name="cost">A finite cost function on the arcs of Graph.</param>
        /// <param name="minimumMatchingSize">Minimum constraint on the size (number of arcs) of the returned matching.</param>
        /// <param name="maximumMatchingSize">Maximum constraint on the size (number of arcs) of the returned matching.</param>
        /// <returns>Returns <see cref="BipartiteMinimumCostMatching"/> algorithm.</returns>
        public static BipartiteMinimumCostMatching ToBipartiteMinimumCostMatching(
            this IGraph graph,
            Func<Node, bool> isRed,
            Func<Arc, double> cost,
            int minimumMatchingSize = 0,
            int maximumMatchingSize = int.MaxValue)
        {
            return new(graph, isRed, cost, minimumMatchingSize, maximumMatchingSize);
        }

        #endregion BipartiteMinimumCostMatching

        #region BipartiteMaximumMatching

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="BipartiteMaximumMatching{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="isRed">Describes a bipartition of the input graph by dividing its nodes into red and blue ones.</param>
        /// <returns>Returns <see cref="BipartiteMaximumMatching{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static BipartiteMaximumMatching<TNodeProperty, TArcProperty> ToBipartiteMaximumMatching<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Func<Node, bool> isRed)
        {
            return new(graph, isRed);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="BipartiteMaximumMatching"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="isRed">Describes a bipartition of the input graph by dividing its nodes into red and blue ones.</param>
        /// <returns>Returns <see cref="BipartiteMaximumMatching"/> algorithm.</returns>
        public static BipartiteMaximumMatching ToBipartiteMaximumMatching(
            this IGraph graph,
            Func<Node, bool> isRed)
        {
            return new(graph, isRed);
        }

        #endregion BipartiteMaximumMatching

        #region Prim

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="Prim{TCost, TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TCost">The arc cost type.</typeparam>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="cost">An arbitrary function assigning costs to the arcs.</param>
        /// <returns>Returns <see cref="Prim{TCost, TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static Prim<TCost, TNodeProperty, TArcProperty> ToPrim<TCost, TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Func<Arc, TCost> cost)
                where TCost : IComparable<TCost>
        {
            return new(graph, cost);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="Prim{TCost}"/> algorithm.
        /// </summary>
        /// <typeparam name="TCost">The arc cost type.</typeparam>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="cost">An arbitrary function assigning costs to the arcs.</param>
        /// <returns>Returns <see cref="Prim{TCost}"/> algorithm.</returns>
        public static Prim<TCost> ToPrim<TCost>(
            this IGraph graph,
            Func<Arc, TCost> cost)
                where TCost : IComparable<TCost>
        {
            return new(graph, cost);
        }

        #endregion Prim

        #region Kruskal

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="Kruskal{TCost, TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TCost">The arc cost type.</typeparam>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="cost">An arbitrary function assigning costs to the arcs.</param>
        /// <param name="maxDegree">An optional per-node maximum degree constraint on the resulting spanning forest. Can be null.</param>
        /// <returns>Returns <see cref="Kruskal{TCost, TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static Kruskal<TCost, TNodeProperty, TArcProperty> ToKruskal<TCost, TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Func<Arc, TCost> cost,
            Func<Node, int>? maxDegree = null)
                where TCost : IComparable<TCost>
        {
            return new(graph, cost, maxDegree);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="Kruskal{TCost}"/> algorithm.
        /// </summary>
        /// <typeparam name="TCost">The arc cost type.</typeparam>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="cost">An arbitrary function assigning costs to the arcs.</param>
        /// <param name="maxDegree">An optional per-node maximum degree constraint on the resulting spanning forest. Can be null.</param>
        /// <returns>Returns <see cref="Kruskal{TCost}"/> algorithm.</returns>
        public static Kruskal<TCost> ToKruskal<TCost>(
            this IGraph graph,
            Func<Arc, TCost> cost,
            Func<Node, int>? maxDegree = null)
                where TCost : IComparable<TCost>
        {
            return new(graph, cost, maxDegree);
        }

        #endregion Kruskal

        #region TopologicalOrder

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="TopologicalOrder{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="flags"><see cref="TopologicalOrderFlags"/>.</param>
        /// <returns>Returns <see cref="TopologicalOrder{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static TopologicalOrder<TNodeProperty, TArcProperty> ToTopologicalOrder<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            TopologicalOrderFlags flags = 0)
        {
            return new(graph, flags);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="TopologicalOrder"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="flags"><see cref="TopologicalOrderFlags"/>.</param>
        /// <returns>Returns <see cref="TopologicalOrder"/> algorithm.</returns>
        public static TopologicalOrder ToTopologicalOrder(
            this IGraph graph,
            TopologicalOrderFlags flags = 0)
        {
            return new(graph, flags);
        }

        #endregion TopologicalOrder

        #region Bipartition

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="Bipartition{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="flags"><see cref="BipartitionFlags"/>.</param>
        /// <returns>Returns <see cref="Bipartition{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static Bipartition<TNodeProperty, TArcProperty> ToBipartition<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            BipartitionFlags flags = 0)
        {
            return new(graph, flags);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="Bipartition"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="flags"><see cref="BipartitionFlags"/>.</param>
        /// <returns>Returns <see cref="Bipartition"/> algorithm.</returns>
        public static Bipartition ToBipartition(
            this IGraph graph,
            BipartitionFlags flags = 0)
        {
            return new(graph, flags);
        }

        #endregion Bipartition

        #region StrongComponents

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="StrongComponents{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="flags"><see cref="StrongComponentsFlags"/>.</param>
        /// <returns>Returns <see cref="StrongComponents{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static StrongComponents<TNodeProperty, TArcProperty> ToStrongComponents<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            StrongComponentsFlags flags = 0)
        {
            return new(graph, flags);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="StrongComponents"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="flags"><see cref="StrongComponentsFlags"/>.</param>
        /// <returns>Returns <see cref="StrongComponents"/> algorithm.</returns>
        public static StrongComponents ToStrongComponents(
            this IGraph graph,
            StrongComponentsFlags flags = 0)
        {
            return new(graph, flags);
        }

        #endregion StrongComponents

        #region ConnectedComponents

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="ConnectedComponents{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="flags"><see cref="ConnectedComponentsFlags"/>.</param>
        /// <returns>Returns <see cref="ConnectedComponents{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static ConnectedComponents<TNodeProperty, TArcProperty> ToConnectedComponents<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            ConnectedComponentsFlags flags = 0)
        {
            return new(graph, flags);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="ConnectedComponents"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="flags"><see cref="ConnectedComponentsFlags"/>.</param>
        /// <returns>Returns <see cref="ConnectedComponents"/> algorithm.</returns>
        public static ConnectedComponents ToConnectedComponents(
            this IGraph graph,
            ConnectedComponentsFlags flags = 0)
        {
            return new(graph, flags);
        }

        #endregion ConnectedComponents

        #region BiEdgeConnectedComponents

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="BiEdgeConnectedComponents{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="flags"><see cref="BiEdgeConnectedComponentsFlags"/>.</param>
        /// <returns>Returns <see cref="BiEdgeConnectedComponents{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static BiEdgeConnectedComponents<TNodeProperty, TArcProperty> ToBiEdgeConnectedComponents<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            BiEdgeConnectedComponentsFlags flags = 0)
        {
            return new(graph, flags);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="BiEdgeConnectedComponents"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="flags"><see cref="BiEdgeConnectedComponentsFlags"/>.</param>
        /// <returns>Returns <see cref="BiEdgeConnectedComponents"/> algorithm.</returns>
        public static BiEdgeConnectedComponents ToBiEdgeConnectedComponents(
            this IGraph graph,
            BiEdgeConnectedComponentsFlags flags = 0)
        {
            return new(graph, flags);
        }

        #endregion BiEdgeConnectedComponents

        #region BiNodeConnectedComponents

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="BiNodeConnectedComponents{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="flags"><see cref="BiNodeConnectedComponentsFlags"/>.</param>
        /// <returns>Returns <see cref="BiNodeConnectedComponents{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static BiNodeConnectedComponents<TNodeProperty, TArcProperty> ToBiNodeConnectedComponents<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            BiNodeConnectedComponentsFlags flags = 0)
        {
            return new(graph, flags);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="BiNodeConnectedComponents"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="flags"><see cref="BiNodeConnectedComponentsFlags"/>.</param>
        /// <returns>Returns <see cref="BiNodeConnectedComponents"/> algorithm.</returns>
        public static BiNodeConnectedComponents ToBiNodeConnectedComponents(
            this IGraph graph,
            BiNodeConnectedComponentsFlags flags = 0)
        {
            return new(graph, flags);
        }

        #endregion BiNodeConnectedComponents
    }
}
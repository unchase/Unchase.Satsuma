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

using Unchase.Satsuma.Adapters.Contracts;
using Unchase.Satsuma.Adapters.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Adapters.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IGraph{TNodeProperty,TArcProperty}"/> and <see cref="IGraph"/>.
    /// </summary>
    public static class GraphExtensions
    {
        #region Supergraph

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="Supergraph{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <returns>Returns <see cref="Supergraph{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static Supergraph<TNodeProperty, TArcProperty> ToSupergraph<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph)
        {
            return new(graph);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="Supergraph"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <returns>Returns <see cref="Supergraph"/> algorithm.</returns>
        public static Supergraph ToSupergraph(
            this IGraph graph)
        {
            return new(graph);
        }

        #endregion Supergraph

        #region Subgraph

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="Subgraph{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <returns>Returns <see cref="Subgraph{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static Subgraph<TNodeProperty, TArcProperty> ToSubgraph<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph)
        {
            return new(graph);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="Subgraph"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <returns>Returns <see cref="Subgraph"/> algorithm.</returns>
        public static Subgraph ToSubgraph(
            this IGraph graph)
        {
            return new(graph);
        }

        #endregion Subgraph

        #region ContractedGraph

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="ContractedGraph{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <returns>Returns <see cref="ContractedGraph{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static ContractedGraph<TNodeProperty, TArcProperty> ToContractedGraph<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph)
        {
            return new(graph);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="ContractedGraph"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <returns>Returns <see cref="ContractedGraph"/> algorithm.</returns>
        public static ContractedGraph ToContractedGraph(
            this IGraph graph)
        {
            return new(graph);
        }

        #endregion ContractedGraph

        #region ReverseGraph

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="ReverseGraph{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <returns>Returns <see cref="ReverseGraph{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static ReverseGraph<TNodeProperty, TArcProperty> ToReverseGraph<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph)
        {
            return new(graph);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="ReverseGraph"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <returns>Returns <see cref="ReverseGraph"/> algorithm.</returns>
        public static ReverseGraph ToReverseGraph(
            this IGraph graph)
        {
            return new(graph);
        }

        #endregion ReverseGraph

        #region RedirectedGraph

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="RedirectedGraph{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="getDirection">The function which modifies the arc directions.</param>
        /// <returns>Returns <see cref="RedirectedGraph{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static RedirectedGraph<TNodeProperty, TArcProperty> ToRedirectedGraph<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Func<Arc, ArcDirection> getDirection)
        {
            return new(graph, getDirection);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="RedirectedGraph"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="getDirection">The function which modifies the arc directions.</param>
        /// <returns>Returns <see cref="RedirectedGraph"/> algorithm.</returns>
        public static RedirectedGraph ToRedirectedGraph(
            this IGraph graph,
            Func<Arc, ArcDirection> getDirection)
        {
            return new(graph, getDirection);
        }

        #endregion RedirectedGraph

        #region Matching

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="Matching{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <returns>Returns <see cref="Matching{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static Matching<TNodeProperty, TArcProperty> ToMatching<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph)
        {
            return new(graph);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="Matching"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <returns>Returns <see cref="Matching"/> algorithm.</returns>
        public static Matching ToMatching(
            this IGraph graph)
        {
            return new(graph);
        }

        #endregion Matching

        #region UndirectedGraph

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="UndirectedGraph{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <returns>Returns <see cref="UndirectedGraph{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static UndirectedGraph<TNodeProperty, TArcProperty> ToUndirectedGraph<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph)
        {
            return new(graph);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="UndirectedGraph"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <returns>Returns <see cref="UndirectedGraph"/> algorithm.</returns>
        public static UndirectedGraph ToUndirectedGraph(
            this IGraph graph)
        {
            return new(graph);
        }

        #endregion UndirectedGraph

        #region Path

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="Path{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <returns>Returns <see cref="Path{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static Path<TNodeProperty, TArcProperty> ToPath<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph)
        {
            return new(graph);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="Path"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <returns>Returns <see cref="Path"/> algorithm.</returns>
        public static Path ToPath(
            this IGraph graph)
        {
            return new(graph);
        }

        #endregion Path

        #region Nodes

        /// <summary>
        /// Add nodes to the buildable graph and return the graph.
        /// </summary>
        /// <param name="graph"><see cref="IBuildableGraph"/>.</param>
        /// <param name="nodes"><see cref="Node"/> array.</param>
        /// <returns>Returns <see cref="IBuildableGraph"/>.</returns>
        public static TGraph WithNodes<TGraph>(
            this TGraph graph,
            params Node[] nodes)
                where TGraph : IBuildableGraph
        {
            foreach (var node in nodes)
            {
                graph.AddNode(node.Id);
            }
            
            return graph;
        }

        /// <summary>
        /// Add nodes with properties to the buildable graph and return the graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="nodesWithProperties">Array of <see cref="Node"/> with <see cref="NodeProperties{TProperty}"/>.</param>
        /// <returns>Returns the graph.</returns>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        public static TGraph WithNodesWithProperties<TGraph, TNodeProperty, TArcProperty>(
            this TGraph graph,
            params (Node Node, NodeProperties<TNodeProperty> Properties)[] nodesWithProperties)
                where TGraph : IBuildableGraph, IGraph<TNodeProperty, TArcProperty>
        {
            foreach (var nodeWithProperties in nodesWithProperties)
            {
                graph.AddNode(nodeWithProperties.Node.Id);
                graph.WithNodeProperties<TGraph, TNodeProperty, TArcProperty>((nodeWithProperties.Node, nodeWithProperties.Properties));
            }

            return graph;
        }

        /// <summary>
        /// Add nodes with properties to the buildable graph and return the graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="nodesWithProperties">Array of <see cref="Node"/> with <see cref="NodeProperties{TProperty}"/>.</param>
        /// <returns>Returns the graph.</returns>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        public static TGraph WithNodesWithProperties<TGraph, TNodeProperty, TArcProperty>(
            this TGraph graph,
            params (Node Node, string PropertyKey, TNodeProperty PropertyValue)[] nodesWithProperties)
                where TGraph : IBuildableGraph, IGraph<TNodeProperty, TArcProperty>
        {
            foreach (var nodeWithProperties in nodesWithProperties)
            {
                graph.AddNode(nodeWithProperties.Node.Id);
                graph.WithNodeProperties<TGraph, TNodeProperty, TArcProperty>(nodesWithProperties.ToDictionary(x => x.Node, y => new NodeProperties<TNodeProperty>(y.PropertyKey, y.PropertyValue)));
            }

            return graph;
        }

        /// <summary>
        /// Add nodes to the buildable graph and return the graph.
        /// </summary>
        /// <param name="graph"><see cref="IBuildableGraph"/>.</param>
        /// <param name="nodeIds"><see cref="Node"/> identifier array.</param>
        /// <returns>Returns <see cref="IBuildableGraph"/>.</returns>
        public static TGraph WithNodes<TGraph>(
            this TGraph graph,
            params long[] nodeIds)
                where TGraph : IBuildableGraph
        {
            foreach (var nodeId in nodeIds)
            {
                graph.AddNode(nodeId);
            }

            return graph;
        }

        /// <summary>
        /// Add <paramref name="count"/> nodes to the buildable graph and return the graph.
        /// </summary>
        /// <param name="graph"><see cref="IBuildableGraph"/>.</param>
        /// <param name="count">Created <see cref="Node"/> count.</param>
        /// <returns>Returns <see cref="IBuildableGraph"/>.</returns>
        public static TGraph WithNodesCount<TGraph>(
            this TGraph graph,
            uint count)
                where TGraph : IBuildableGraph
        {
            for (var i = 0; i < count; i++)
            {
                graph.AddNode();
            }

            return graph;
        }

        /// <summary>
        /// Add nodes with properties to the buildable graph and return the graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="nodesWithProperties">Array of <see cref="Node"/> with <see cref="NodeProperties{TProperty}"/>.</param>
        /// <returns>Returns the graph.</returns>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        public static TGraph WithNodesWithProperties<TGraph, TNodeProperty, TArcProperty>(
            this TGraph graph,
            params (long NodeId, NodeProperties<TNodeProperty> Properties)[] nodesWithProperties)
                where TGraph : IBuildableGraph, IGraph<TNodeProperty, TArcProperty>
        {
            foreach (var (nodeId, properties) in nodesWithProperties)
            {
                var node = new Node(nodeId);
                graph.AddNode(nodeId);
                graph.WithNodeProperties<TGraph, TNodeProperty, TArcProperty>((node, properties));
            }

            return graph;
        }

        /// <summary>
        /// Add nodes with properties to the buildable graph and return the graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <param name="nodesWithProperties">Array of <see cref="Node"/> with <see cref="NodeProperties{TProperty}"/>.</param>
        /// <returns>Returns the graph.</returns>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        public static TGraph WithNodesWithProperties<TGraph, TNodeProperty, TArcProperty>(
            this TGraph graph,
            params (long NodeId, string PropertyKey, TNodeProperty PropertyValue)[] nodesWithProperties)
                where TGraph : IBuildableGraph, IGraph<TNodeProperty, TArcProperty>
        {
            foreach (var (nodeId, propertyKey, propertyValue) in nodesWithProperties)
            {
                var node = new Node(nodeId);
                graph.AddNode(nodeId);
                graph.WithNodeProperties<TGraph, TNodeProperty, TArcProperty>((node, new(propertyKey, propertyValue)));
            }

            return graph;
        }

        #endregion Nodes

        #region Arcs

        /// <summary>
        /// Add arcs to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <param name="arcs"><see cref="Arc"/> data array.</param>
        /// <returns>Returns the graph.</returns>
        public static TGraph WithArcs<TGraph, TNodeProperty, TArcProperty>(
            this TGraph graph,
            params (Node U, Node V, Directedness Directedness)[] arcs)
                where TGraph : IGraph<TNodeProperty, TArcProperty>, IBuildableGraph, IArcLookup<TArcProperty>
        {
            foreach (var arc in arcs)
            {
                graph.AddArc(arc.U, arc.V, arc.Directedness);
            }

            return graph;
        }

        /// <summary>
        /// Add arcs to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <param name="arcs"><see cref="Arc"/> data array.</param>
        /// <returns>Returns the graph.</returns>
        public static TGraph WithArcs<TGraph, TNodeProperty, TArcProperty>(
            this TGraph graph,
            params (long UId, long VId, Directedness Directedness)[] arcs)
                where TGraph : IGraph<TNodeProperty, TArcProperty>, IBuildableGraph, IArcLookup<TArcProperty>
        {
            foreach (var arc in arcs)
            {
                graph.AddArc(new(arc.UId), new(arc.VId), arc.Directedness);
            }

            return graph;
        }

        /// <summary>
        /// Add arcs to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <param name="arcs"><see cref="Arc"/> data array.</param>
        /// <returns>Returns the graph.</returns>
        public static TGraph WithArcs<TGraph>(
            this TGraph graph,
            params (Node U, Node V, Directedness Directedness)[] arcs)
                where TGraph : IBuildableGraph, IArcLookup<object>
        {
            foreach (var arc in arcs)
            {
                graph.AddArc(arc.U, arc.V, arc.Directedness);
            }

            return graph;
        }

        /// <summary>
        /// Add arcs to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <param name="graph">The graph.</param>
        /// <param name="arcs"><see cref="Arc"/> data array.</param>
        /// <returns>Returns the graph.</returns>
        public static TGraph WithArcs<TGraph>(
            this TGraph graph,
            params (long UId, long VId, Directedness Directedness)[] arcs)
                where TGraph : IBuildableGraph, IArcLookup<object>
        {
            foreach (var arc in arcs)
            {
                graph.AddArc(new(arc.UId), new(arc.VId), arc.Directedness);
            }

            return graph;
        }

        #endregion Arcs
    }
}
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

using Unchase.Satsuma.Adapters.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;

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
    }
}
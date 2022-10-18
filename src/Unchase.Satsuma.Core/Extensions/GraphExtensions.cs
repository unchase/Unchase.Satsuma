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
    /// Extension methods for <see cref="IGraph{TNodeProperty, TArcProperty}"/> and <see cref="IGraph"/>.
    /// </summary>
    public static class GraphExtensions
    {
        #region Properties

        #region NodeProperties

        /// <summary>
        /// Add node properties to the graph.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="nodeProperties">Node properties dictionary.</param>
        public static void AddNodeProperties<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Dictionary<Node, NodeProperties<TNodeProperty>> nodeProperties)
        {
            foreach (var node in nodeProperties.Keys)
            {
                if (graph.NodePropertiesDictionary.ContainsKey(node))
                {
                    var theNodeProperties = graph.NodePropertiesDictionary[node].Properties;
                    foreach (var property in nodeProperties[node].Properties ?? new())
                    {
                        if (theNodeProperties?.ContainsKey(property.Key) == true)
                        {
                            theNodeProperties[property.Key] = property.Value;
                        }
                        else
                        {
                            theNodeProperties?.Add(property.Key, property.Value);
                        }
                    }
                }
                else
                {
                    graph.NodePropertiesDictionary.Add(node, nodeProperties[node]);
                }
            }
        }

        /// <summary>
        /// Add node properties to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="nodeProperties">Node properties dictionary.</param>
        /// <returns>Returns <see cref="IGraph{TNodeProperty, TArcProperty}"/></returns>
        public static TGraph WithNodeProperties<TGraph, TNodeProperty, TArcProperty>(
            this TGraph graph,
            Dictionary<Node, NodeProperties<TNodeProperty>> nodeProperties)
                where TGraph : IGraph<TNodeProperty, TArcProperty>
        {
            graph.AddNodeProperties(nodeProperties);
            return graph;
        }

        /// <summary>
        /// Add node properties to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="nodeProperties">Node properties dictionary.</param>
        /// <returns>Returns <see cref="IGraph{TNodeProperty, TArcProperty}"/></returns>
        public static TGraph WithNodeProperties<TGraph, TNodeProperty, TArcProperty>(
            this TGraph graph,
            params (Node Node, NodeProperties<TNodeProperty> Properties)[] nodeProperties)
                where TGraph : IGraph<TNodeProperty, TArcProperty>
        {
            graph.AddNodeProperties(nodeProperties.ToDictionary(x => x.Node, y => y.Properties));
            return graph;
        }

        /// <summary>
        /// Add node properties to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="nodeProperties">Node properties dictionary.</param>
        /// <returns>Returns <see cref="IGraph{TNodeProperty, TArcProperty}"/></returns>
        public static TGraph WithNodeProperties<TGraph, TNodeProperty, TArcProperty>(
            this TGraph graph,
            params (long NodeId, NodeProperties<TNodeProperty> Properties)[] nodeProperties)
                where TGraph : IGraph<TNodeProperty, TArcProperty>
        {
            graph.AddNodeProperties(nodeProperties.ToDictionary(x => new Node(x.NodeId), y => y.Properties));
            return graph;
        }

        /// <summary>
        /// Add node properties to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="nodeProperties">Node properties dictionary.</param>
        /// <returns>Returns <see cref="IGraph{TNodeProperty, TArcProperty}"/></returns>
        public static TGraph WithNodeProperties<TGraph, TNodeProperty, TArcProperty>(
            this TGraph graph,
            params (long NodeId, string NodePropertyKey, TNodeProperty NodePropertyValue)[] nodeProperties)
                where TGraph : IGraph<TNodeProperty, TArcProperty>
        {
            graph.AddNodeProperties(nodeProperties.ToDictionary(x => new Node(x.NodeId), y => new NodeProperties<TNodeProperty>(y.NodePropertyKey, y.NodePropertyValue)));
            return graph;
        }

        /// <summary>
        /// Add node properties to the graph.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="nodeProperties">Node properties dictionary.</param>
        public static void AddNodeProperties(
            this IGraph graph,
            Dictionary<Node, NodeProperties<object>> nodeProperties)
        {
            foreach (var node in nodeProperties.Keys)
            {
                if (graph.NodePropertiesDictionary.ContainsKey(node))
                {
                    var theNodeProperties = graph.NodePropertiesDictionary[node].Properties;
                    foreach (var property in nodeProperties[node].Properties ?? new())
                    {
                        if (theNodeProperties?.ContainsKey(property.Key) == true)
                        {
                            theNodeProperties[property.Key] = property.Value;
                        }
                        else
                        {
                            theNodeProperties?.Add(property.Key, property.Value);
                        }
                    }
                }
                else
                {
                    graph.NodePropertiesDictionary.Add(node, nodeProperties[node]);
                }
            }
        }

        /// <summary>
        /// Add node properties to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="nodeProperties">Node properties dictionary.</param>
        /// <returns>Returns <see cref="IGraph"/></returns>
        public static TGraph WithNodeProperties<TGraph>(
            this TGraph graph,
            Dictionary<Node, NodeProperties<object>> nodeProperties)
                where TGraph : IGraph<object, object>
        {
            graph.AddNodeProperties(nodeProperties);
            return graph;
        }

        /// <summary>
        /// Add node properties to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="nodeProperties">Node properties array.</param>
        /// <returns>Returns <see cref="IGraph"/></returns>
        public static TGraph WithNodeProperties<TGraph>(
            this TGraph graph,
            params (Node Node, NodeProperties<object> NodeProperties)[] nodeProperties)
                where TGraph : IGraph<object, object>
        {
            graph.AddNodeProperties(nodeProperties.ToDictionary(x => x.Node, y => y.NodeProperties));
            return graph;
        }

        /// <summary>
        /// Add node properties to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="nodeProperties">Node properties array.</param>
        /// <returns>Returns <see cref="IGraph"/></returns>
        public static TGraph WithNodeProperties<TGraph>(
            this TGraph graph,
            params (Node Node, string NodePropertyKey, object NodePropertyValue)[] nodeProperties)
                where TGraph : IGraph<object, object>
        {
            graph.AddNodeProperties(nodeProperties.ToDictionary(x => x.Node, y => new NodeProperties<object>(y.NodePropertyKey, y.NodePropertyValue)));
            return graph;
        }

        /// <summary>
        /// Add node properties to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="nodeProperties">Node properties array.</param>
        /// <returns>Returns <see cref="IGraph"/></returns>
        public static TGraph WithNodeProperties<TGraph>(
            this TGraph graph,
            params (long NodeId, NodeProperties<object> NodeProperties)[] nodeProperties)
                where TGraph : IGraph<object, object>
        {
            graph.AddNodeProperties(nodeProperties.ToDictionary(x => new Node(x.NodeId), y => y.NodeProperties));
            return graph;
        }

        /// <summary>
        /// Add node properties to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="nodeProperties">Node properties array.</param>
        /// <returns>Returns <see cref="IGraph"/></returns>
        public static TGraph WithNodeProperties<TGraph>(
            this TGraph graph,
            params (long NodeId, string NodePropertyKey, object NodePropertyValue)[] nodeProperties)
                where TGraph : IGraph<object, object>
        {
            graph.AddNodeProperties(nodeProperties.ToDictionary(x => new Node(x.NodeId), y => new NodeProperties<object>(y.NodePropertyKey, y.NodePropertyValue)));
            return graph;
        }

        #endregion NodeProperties

        #region ArcProperties

        /// <summary>
        /// Add arc properties to the graph.
        /// </summary>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="arcProperties">Arc properties dictionary.</param>
        public static void AddArcProperties<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph,
            Dictionary<Arc, ArcProperties<TArcProperty>> arcProperties)
        {
            foreach (var arc in arcProperties.Keys)
            {
                if (graph.ArcPropertiesDictionary.ContainsKey(arc))
                {
                    graph.ArcPropertiesDictionary[arc] = arcProperties[arc];
                }
                else
                {
                    graph.ArcPropertiesDictionary.Add(arc, arcProperties[arc]);
                }
            }
        }

        /// <summary>
        /// Add arc properties to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <typeparam name="TNodeProperty">The type of node property.</typeparam>
        /// <typeparam name="TArcProperty">The type of arc property.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="arcProperties">Arc properties dictionary.</param>
        /// <returns>Returns <see cref="IGraph{TNodeProperty, TArcProperty}"/></returns>
        public static TGraph WithArcProperties<TGraph, TNodeProperty, TArcProperty>(
            this TGraph graph,
            Dictionary<Arc, ArcProperties<TArcProperty>> arcProperties)
                where TGraph : IGraph<TNodeProperty, TArcProperty>
        {
            graph.AddArcProperties(arcProperties);
            return graph;
        }

        /// <summary>
        /// Add arc properties to the graph.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="arcProperties">Arc properties dictionary.</param>
        public static void AddArcProperties(
            this IGraph graph,
            Dictionary<Arc, ArcProperties<object>> arcProperties)
        {
            foreach (var arc in arcProperties.Keys)
            {
                if (graph.ArcPropertiesDictionary.ContainsKey(arc))
                {
                    graph.ArcPropertiesDictionary[arc] = arcProperties[arc];
                }
                else
                {
                    graph.ArcPropertiesDictionary.Add(arc, arcProperties[arc]);
                }
            }
        }

        /// <summary>
        /// Add arc properties to the graph and return the graph.
        /// </summary>
        /// <typeparam name="TGraph">The type of graph.</typeparam>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="arcProperties">Arc properties dictionary.</param>
        /// <returns>Returns <see cref="IGraph"/></returns>
        public static TGraph WithArcProperties<TGraph>(
            this TGraph graph,
            Dictionary<Arc, ArcProperties<object>> arcProperties)
                where TGraph : IGraph
        {
            graph.AddArcProperties(arcProperties);
            return graph;
        }

        #endregion ArcProperties

        #endregion Properties
    }
}
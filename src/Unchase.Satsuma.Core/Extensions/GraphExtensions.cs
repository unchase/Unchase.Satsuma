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

        /// <summary>
        /// Add node properties to the graph.
        /// </summary>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="nodeProperties">Node properties dictionary.</param>
        public static void AddNodeProperties<TNodeProperty, TArcProperty>(this IGraph<TNodeProperty, TArcProperty> graph, Dictionary<Node, NodeProperties<TNodeProperty>> nodeProperties)
        {
            foreach (var node in nodeProperties.Keys)
            {
                if (graph.NodePropertiesDictionary.ContainsKey(node))
                {
                    graph.NodePropertiesDictionary[node] = nodeProperties[node];
                }
                else
                {
                    graph.NodePropertiesDictionary.Add(node, nodeProperties[node]);
                }
            }
        }

        /// <summary>
        /// Add node properties to the graph.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="nodeProperties">Node properties dictionary.</param>
        public static void AddNodeProperties(this IGraph graph, Dictionary<Node, NodeProperties<object>> nodeProperties)
        {
            foreach (var node in nodeProperties.Keys)
            {
                if (graph.NodePropertiesDictionary.ContainsKey(node))
                {
                    graph.NodePropertiesDictionary[node] = nodeProperties[node];
                }
                else
                {
                    graph.NodePropertiesDictionary.Add(node, nodeProperties[node]);
                }
            }
        }

        /// <summary>
        /// Add arc properties to the graph.
        /// </summary>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="arcProperties">Arc properties dictionary.</param>
        public static void AddArcProperties<TNodeProperty, TArcProperty>(this IGraph<TNodeProperty, TArcProperty> graph, Dictionary<Arc, ArcProperties<TArcProperty>> arcProperties)
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
        /// Add arc properties to the graph.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="arcProperties">Arc properties dictionary.</param>
        public static void AddArcProperties(this IGraph graph, Dictionary<Arc, ArcProperties<object>> arcProperties)
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

        #endregion Properties
    }
}
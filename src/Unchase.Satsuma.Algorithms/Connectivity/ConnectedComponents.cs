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

using Unchase.Satsuma.Algorithms.Abstractions;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Algorithms.Connectivity
{
    /// <inheritdoc cref="ConnectedComponents{TNodeProperty, TArcProperty}"/>
    public sealed class ConnectedComponents :
        ConnectedComponents<object, object>
    {
        /// <summary>
        /// Initialize <see cref="ConnectedComponents"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="flags"><see cref="ConnectedComponentsFlags"/>.</param>
        public ConnectedComponents(
            IGraph graph,
            ConnectedComponentsFlags flags = 0)
                : base(graph, flags)
        {
        }
    }

    /// <summary>
    /// Finds the connected components of a graph.
    /// </summary>
    /// <remarks>
    /// <para>Example:</para>
    /// <para>
    /// <code>
    /// var g = new CustomGraph();
    /// for (int i = 0; i &lt; 5; i++) g.AddNode();
    /// var components = new ConnectedComponents(g, ConnectedComponents.Flags.CreateComponents);
    /// Console.WriteLine("Number of components: " + components.Count); // should print 5
    /// Console.WriteLine("Components:");
    /// foreach (var component in components.Components)
    /// 	Console.WriteLine(string.Join(" ", component));
    /// </code>
    /// </para>
    /// </remarks>
    /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
    public class ConnectedComponents<TNodeProperty, TArcProperty>
    {
        /// <summary>
        /// The input graph.
        /// </summary>
        public IGraph<TNodeProperty, TArcProperty> Graph { get; }

        /// <summary>
        /// The number of connected components in the graph.
        /// </summary>
        public int Count { get; private set; }

        /// <summary>
        /// The connected components of the graph.
        /// </summary>
        /// <remarks>
        /// Null if <see cref="ConnectedComponentsFlags.CreateComponents"/> was not set during construction.
        /// </remarks>
        public List<HashSet<Node>>? Components { get; }

        private class MyDfs : 
            Dfs<TNodeProperty, TArcProperty>
        {
            /// <summary>
            /// Parent <see cref="ConnectedComponents{TNodeProperty, TArcProperty}"/>.
            /// </summary>
            private readonly ConnectedComponents<TNodeProperty, TArcProperty> _parent;

            /// <summary>
            /// Initialize <see cref="MyDfs"/>.
            /// </summary>
            /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
            /// <param name="parent">Parent <see cref="ConnectedComponents{TNodeProperty, TArcProperty}"/>.</param>
            public MyDfs(
                IGraph<TNodeProperty, TArcProperty> graph,
                ConnectedComponents<TNodeProperty, TArcProperty> parent)
                    : base(graph)
            {
                _parent = parent;
            }

            /// <inheritdoc />
            protected override void Start(out Direction direction)
            {
                direction = Direction.Undirected;
            }

            /// <inheritdoc />
            protected override bool NodeEnter(Node node, Arc arc)
            {
                if (arc == Arc.Invalid)
                {
                    _parent.Count++;
                    _parent.Components?.Add(new() { node });
                }
                else
                {
                    _parent.Components?[_parent.Count - 1].Add(node);
                }

                return true;
            }
        }

        /// <summary>
        /// Initialize <see cref="ConnectedComponents{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="flags"><see cref="ConnectedComponentsFlags"/>.</param>
        public ConnectedComponents(
            IGraph<TNodeProperty, TArcProperty> graph, 
            ConnectedComponentsFlags flags = 0)
        {
            Graph = graph;
            if (0 != (flags & ConnectedComponentsFlags.CreateComponents))
            {
                Components = new();
            }

            new MyDfs(graph, this).Run();
        }
    }
}
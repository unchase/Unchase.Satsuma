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
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Algorithms.Connectivity
{
    /// <summary>
    /// Decides whether a digraph is acyclic and finds a topological order of its nodes.
    /// </summary>
    /// <remarks>
    /// Edges count as 2-cycles.
    /// </remarks>
    /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
    public sealed class TopologicalOrder<TNodeProperty, TArcProperty>
    {
        /// <summary>
        /// The input graph.
        /// </summary>
        public IGraph<TNodeProperty, TArcProperty> Graph { get; }

        /// <summary>
        /// True if the digraph has no cycles.
        /// </summary>
        public bool Acyclic { get; private set; }

        /// <summary>
        /// An order of the nodes where each arc points forward.
        /// </summary>
        /// <remarks>
        /// <para>Null if <see cref="TopologicalOrderFlags.CreateOrder"/> was not set during construction.</para>
        /// <para>Otherwise, empty if the digraph has a cycle.</para>
        /// </remarks>
        public List<Node>? Order { get; }

        private class MyDfs : 
            Dfs<TNodeProperty, TArcProperty>
        {
            private readonly TopologicalOrder<TNodeProperty, TArcProperty> _parent;
            private HashSet<Node> _exited = new();

            /// <summary>
            /// Initialize <see cref="MyDfs"/>.
            /// </summary>
            /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
            /// <param name="parent"><see cref="TopologicalOrder{TNodeProperty, TArcProperty}"/>.</param>
            public MyDfs(
                IGraph<TNodeProperty, TArcProperty> graph, 
                TopologicalOrder<TNodeProperty, TArcProperty> parent) 
                    : base(graph)
            {
                _parent = parent;
            }

            /// <inheritdoc />
            protected override void Start(out Direction direction)
            {
                direction = Direction.Forward;
                _parent.Acyclic = true;
                _exited = new();
            }

            /// <inheritdoc />
            protected override bool NodeEnter(Node node, Arc arc)
            {
                if (arc != Arc.Invalid && Graph.IsEdge(arc))
                {
                    _parent.Acyclic = false;
                    return false;
                }

                return true;
            }

            /// <inheritdoc />
            protected override bool NodeExit(Node node, Arc arc)
            {
                _parent.Order?.Add(node);
                _exited.Add(node);
                return true;
            }

            /// <inheritdoc />
            protected override bool BackArc(Node node, Arc arc)
            {
                var other = Graph.Other(arc, node);
                if (!_exited.Contains(other))
                {
                    _parent.Acyclic = false;
                    return false;
                }

                return true;
            }

            /// <inheritdoc />
            protected override void StopSearch()
            {
                if (_parent.Order != null)
                {
                    if (_parent.Acyclic) _parent.Order.Reverse();
                    else _parent.Order.Clear();
                }
            }
        }

        /// <summary>
        /// Initialize <see cref="TopologicalOrder{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <param name="flags"><see cref="TopologicalOrderFlags"/>.</param>
        public TopologicalOrder(
            IGraph<TNodeProperty, TArcProperty> graph, 
            TopologicalOrderFlags flags = 0)
        {
            Graph = graph;
            if (0 != (flags & TopologicalOrderFlags.CreateOrder))
            {
                Order = new();
            }

            new MyDfs(graph, this).Run();
        }
    }
}
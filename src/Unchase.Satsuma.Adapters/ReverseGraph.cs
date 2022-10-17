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

using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;

namespace Unchase.Satsuma.Adapters
{
    /// <summary>
    /// Adapter for reversing all arcs of an underlying graph.
    /// </summary>
    /// <remarks>
    /// <para>Node and Arc objects are interchangeable between the adapter and the original graph.</para>
    /// <para>The underlying graph can be freely modified while using this adapter.</para>
    /// </remarks>
    public sealed class ReverseGraph : 
        IGraph
    {
        private readonly IGraph _graph;

        /// <inheritdoc />
        public Dictionary<Node, NodeProperties> NodePropertiesDictionary { get; } = new();

        /// <inheritdoc />
        public Dictionary<Arc, ArcProperties> ArcPropertiesDictionary { get; } = new();

        /// <summary>
        /// Get the opposite of an arc filter.
        /// </summary>
        /// <param name="filter"><see cref="ArcFilter"/>.</param>
        /// <returns>Returns the opposite of an arc filter.</returns>
        public static ArcFilter Reverse(ArcFilter filter)
        {
            if (filter == ArcFilter.Forward)
            {
                return ArcFilter.Backward;
            }

            if (filter == ArcFilter.Backward)
            {
                return ArcFilter.Forward;
            }

            return filter;
        }

        /// <summary>
        /// Initialize <see cref="ReverseGraph"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        public ReverseGraph(
            IGraph graph)
        {
            _graph = graph;
        }

        /// <inheritdoc />
        public Dictionary<string, object>? GetNodeProperties(Node node)
        {
            return NodePropertiesDictionary.TryGetValue(node, out var p)
                ? p.Properties
                : _graph.GetNodeProperties(node);
        }

        /// <inheritdoc />
        public Dictionary<string, object>? GetArcProperties(Arc arc)
        {
            return ArcPropertiesDictionary.TryGetValue(arc, out var p)
                ? p.Properties
                : _graph.GetArcProperties(arc);
        }

        /// <inheritdoc />
        public Node U(Arc arc)
        {
            return _graph.V(arc);
        }

        /// <inheritdoc />
        public Node V(Arc arc)
        {
            return _graph.U(arc);
        }

        /// <inheritdoc />
        public bool IsEdge(Arc arc)
        {
            return _graph.IsEdge(arc);
        }

        /// <inheritdoc />
        public IEnumerable<Node> Nodes()
        {
            return _graph.Nodes();
        }

        /// <inheritdoc />
        public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
        {
            return _graph.Arcs(filter);
        }

        /// <inheritdoc />
        public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
        {
            return _graph.Arcs(u, Reverse(filter));
        }

        /// <inheritdoc />
        public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
        {
            return _graph.Arcs(u, v, Reverse(filter));
        }

        /// <inheritdoc />
        public int NodeCount()
        {
            return _graph.NodeCount();
        }

        /// <inheritdoc />
        public int ArcCount(ArcFilter filter = ArcFilter.All)
        {
            return _graph.ArcCount(filter);
        }

        /// <inheritdoc />
        public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
        {
            return _graph.ArcCount(u, Reverse(filter));
        }

        /// <inheritdoc />
        public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
        {
            return _graph.ArcCount(u, v, Reverse(filter));
        }

        /// <inheritdoc />
        public bool HasNode(Node node)
        {
            return _graph.HasNode(node);
        }

        /// <inheritdoc />
        public bool HasArc(Arc arc)
        {
            return _graph.HasArc(arc);
        }
    }
}
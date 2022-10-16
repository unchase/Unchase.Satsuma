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
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Adapters
{
    /// <summary>
	/// Adapter for hiding/showing nodes/arcs of an underlying graph.
	/// </summary>
	/// <remarks>
	/// <para>Node and Arc objects are interchangeable between the adapter and the original graph.</para>
	/// <para>The underlying graph can be modified while using this adapter,
	/// as long as no nodes/arcs are deleted; and newly added nodes/arcs are explicitly enabled/disabled,
	/// since enabledness of newly added nodes/arcs is undefined.</para>
	/// <para>By default, all nodes and arcs are enabled.</para>
	/// </remarks>
	public sealed class Subgraph : 
        IGraph
	{
		private readonly IGraph _graph;

		private bool _defaultNodeEnabled;
		private readonly HashSet<Node> _nodeExceptions = new();
		private bool _defaultArcEnabled;
		private readonly HashSet<Arc> _arcExceptions = new();
        private readonly Dictionary<Node, NodeProperties>? _nodeProperties;

		/// <summary>
		/// Initialize <see cref="Subgraph"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		/// <param name="nodeProperties">Node properties dictionary.</param>
		public Subgraph(
            IGraph graph,
            Dictionary<Node, NodeProperties>? nodeProperties = default)
		{
			_graph = graph;
			_nodeProperties = _nodeProperties = nodeProperties?
                .Where(x => _graph.HasNode(x.Key))
                .ToDictionary(x => x.Key, y => y.Value);

			EnableAllNodes(true);
			EnableAllArcs(true);
		}

		/// <summary>
		/// Enables/disables all nodes at once.
		/// </summary>
		/// <param name="enabled">True if all nodes should be enabled, false if all nodes should be disabled.</param>
		public void EnableAllNodes(bool enabled)
		{
			_defaultNodeEnabled = enabled;
			_nodeExceptions.Clear();
		}

		/// <summary>
		/// Enables/disables all arcs at once.
		/// </summary>
		/// <param name="enabled">True if all arcs should be enabled, false if all arcs should be disabled.</param>
		public void EnableAllArcs(bool enabled)
		{
			_defaultArcEnabled = enabled;
			_arcExceptions.Clear();
		}

		/// <summary>
		/// Enables/disables a single node.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="enabled">True if the node should be enabled, false if the node should be disabled.</param>
		public void Enable(Node node, bool enabled)
		{
			var exception = (_defaultNodeEnabled != enabled);
            if (exception)
            {
                _nodeExceptions.Add(node);
            }
            else
            {
                _nodeExceptions.Remove(node);
            }
		}

		/// <summary>
		/// Enables/disables a single arc.
		/// </summary>
		/// <param name="arc">The arc.</param>
		/// <param name="enabled">True if the arc should be enabled, \c false if the arc should be disabled.</param>
		public void Enable(Arc arc, bool enabled)
		{
			var exception = (_defaultArcEnabled != enabled);
            if (exception)
            {
                _arcExceptions.Add(arc);
            }
            else
            {
                _arcExceptions.Remove(arc);
            }
		}

		/// <summary>
		/// Queries the enabledness of a node.
		/// </summary>
		/// <param name="node">The node.</param>
        public bool IsEnabled(Node node)
		{
			return _defaultNodeEnabled ^ _nodeExceptions.Contains(node);
		}

		/// <summary>
		/// Queries the enabledness of an arc.
		/// </summary>
		/// <param name="arc">The arc.</param>
        public bool IsEnabled(Arc arc)
		{
			return _defaultArcEnabled ^ _arcExceptions.Contains(arc);
		}

        /// <inheritdoc />
        public Dictionary<string, object>? Properties(Node node)
        {
            if (_nodeProperties == null)
            {
                return null;
            }

            return _nodeProperties.TryGetValue(node, out var p)
                ? p.Properties
                : _graph.Properties(node) ?? null;
        }

		/// <inheritdoc />
		public Node U(Arc arc)
		{
			return _graph.U(arc);
		}

        /// <inheritdoc />
		public Node V(Arc arc)
		{
			return _graph.V(arc);
		}

        /// <inheritdoc />
		public bool IsEdge(Arc arc)
		{
			return _graph.IsEdge(arc);
		}

		private IEnumerable<Node> NodesInternal()
		{
            foreach (var node in _graph.Nodes())
            {
                if (IsEnabled(node))
                {
                    yield return node;
                }
            }
		}

        /// <inheritdoc />
		public IEnumerable<Node> Nodes()
		{
			if (_nodeExceptions.Count == 0)
			{
                if (_defaultNodeEnabled)
                {
                    return _graph.Nodes();
                }

				return Enumerable.Empty<Node>();
			}

			return NodesInternal();
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
            foreach (var arc in _graph.Arcs(filter))
            {
                if (IsEnabled(arc) && IsEnabled(_graph.U(arc)) && IsEnabled(_graph.V(arc)))
                {
                    yield return arc;
                }
            }
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
            if (!IsEnabled(u))
            {
                yield break;
            }

            foreach (var arc in _graph.Arcs(u, filter))
            {
                if (IsEnabled(arc) && IsEnabled(_graph.Other(arc, u)))
                {
                    yield return arc;
                }
            }
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
            if (!IsEnabled(u) || !IsEnabled(v))
            {
                yield break;
            }

			foreach (var arc in _graph.Arcs(u, v, filter))
            {
                if (IsEnabled(arc))
                {
                    yield return arc;
                }
            }
		}

        /// <inheritdoc />
		public int NodeCount()
		{
			return _defaultNodeEnabled ? _graph.NodeCount() - _nodeExceptions.Count : _nodeExceptions.Count;
		}

        /// <inheritdoc />
		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			if (_nodeExceptions.Count == 0 && filter == ArcFilter.All)
            {
                return _defaultNodeEnabled ?
					_defaultArcEnabled ? _graph.ArcCount() - _arcExceptions.Count : _arcExceptions.Count
					: 0;
            }

			return Arcs(filter).Count();
		}

        /// <inheritdoc />
		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			return Arcs(u, filter).Count();
		}

        /// <inheritdoc />
		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return Arcs(u, v, filter).Count();
		}

        /// <inheritdoc />
		public bool HasNode(Node node)
		{
			return _graph.HasNode(node) && IsEnabled(node);
		}

        /// <inheritdoc />
		public bool HasArc(Arc arc)
		{
			return _graph.HasArc(arc) && IsEnabled(arc);
		}
	}
}
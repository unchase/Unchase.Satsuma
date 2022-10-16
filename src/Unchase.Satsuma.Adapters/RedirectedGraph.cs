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
   distribution.*/
#endregion

using Unchase.Satsuma.Adapters.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;

namespace Unchase.Satsuma.Adapters
{
    /// <summary>
	/// Adapter for modifying the direction of some arcs of an underlying graph.
	/// </summary>
	/// <remarks>
	/// <para>Node and Arc objects are interchangeable between the adapter and the original graph.</para>
	/// <para>The underlying graph can be freely modified while using this adapter.</para>
	/// <para>For special cases, consider the UndirectedGraph and ReverseGraph classes for performance.</para>
	/// </remarks>
	public sealed class RedirectedGraph : 
        IGraph
	{
        private readonly IGraph _graph;
		private readonly Func<Arc, ArcDirection> _getDirection;
        private readonly Dictionary<Node, NodeProperties>? _nodeProperties;

		/// <summary>
		/// Creates an adapter over the given graph for redirecting its arcs.
		/// </summary>
		/// <param name="graph">The graph to redirect.</param>
		/// <param name="getDirection">The function which modifies the arc directions.</param>
		/// <param name="nodeProperties">Node properties dictionary.</param>
		public RedirectedGraph(
            IGraph graph,
            Func<Arc, ArcDirection> getDirection,
			Dictionary<Node, NodeProperties>? nodeProperties = default)
		{
			_graph = graph;
			_getDirection = getDirection;
			_nodeProperties = _nodeProperties = nodeProperties?
                .Where(x => _graph.HasNode(x.Key))
                .ToDictionary(x => x.Key, y => y.Value);
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
			return _getDirection(arc) == ArcDirection.Backward 
                ? _graph.V(arc) 
                : _graph.U(arc);
		}

        /// <inheritdoc />
		public Node V(Arc arc)
		{
			return _getDirection(arc) == ArcDirection.Backward 
                ? _graph.U(arc) 
                : _graph.V(arc);
		}

        /// <inheritdoc />
		public bool IsEdge(Arc arc)
		{
			return _getDirection(arc) == ArcDirection.Edge;
		}

        /// <inheritdoc />
		public IEnumerable<Node> Nodes()
		{
			return _graph.Nodes();
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			return filter == ArcFilter.All 
                ? _graph.Arcs() 
                : _graph.Arcs().Where(x => _getDirection(x) == ArcDirection.Edge);
		}

		private IEnumerable<Arc> FilterArcs(Node u, IEnumerable<Arc> arcs, ArcFilter filter)
		{
			switch (filter)
			{
				case ArcFilter.All: 
                    return arcs;
				case ArcFilter.Edge: 
                    return arcs.Where(x => _getDirection(x) == ArcDirection.Edge);
				case ArcFilter.Forward:
					return arcs.Where(x =>
					{
						var dir = _getDirection(x);
						switch (dir)
						{
							case ArcDirection.Forward: 
                                return U(x) == u;
							case ArcDirection.Backward: 
                                return V(x) == u;
							default: 
                                return true;
						}
					});
				default:
					return arcs.Where(x =>
					{
						var dir = _getDirection(x);
						switch (dir)
						{
							case ArcDirection.Forward: 
                                return V(x) == u;
							case ArcDirection.Backward: 
                                return U(x) == u;
							default: 
                                return true;
						}
					});
			}
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			return FilterArcs(u, _graph.Arcs(u), filter);
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return FilterArcs(u, _graph.Arcs(u, v), filter);
		}

        /// <inheritdoc />
		public int NodeCount()
		{
			return _graph.NodeCount();
		}

        /// <inheritdoc />
		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return filter == ArcFilter.All 
                ? _graph.ArcCount() 
                : Arcs(filter).Count();
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
			return _graph.HasNode(node);
		}

        /// <inheritdoc />
		public bool HasArc(Arc arc)
		{
			return _graph.HasArc(arc);
		}
	}
}
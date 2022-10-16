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

using System.Diagnostics;

using Unchase.Satsuma.Adapters.Abstractions;
using Unchase.Satsuma.Adapters.Contracts;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Adapters
{
	/// <summary>
	/// Adapter for adding nodes/arcs to an underlying graph.
	/// </summary>
	/// <remarks>
	/// <para><see cref="Node"/> and <see cref="Arc"/> objects of the original graph are valid in the adapter as well, but the converse is not true.</para>
	/// <para>The underlying graph must NOT be modified while using this adapter.</para>
	/// <para>The adapter is an <see cref="IDestroyableGraph"/>, but only the nodes/arcs added by the adapter can be deleted.</para>
	/// <para>Memory usage: O(n+m), where n is the number of new nodes and m is the number of new arcs.</para>
	/// <para>
	/// The following example demonstrates how nodes and arcs can be added to a(n otherwise immutable) <see cref="CompleteGraph"/>:
	/// <code>
	/// var g = new CompleteGraph(10);
	/// var sg = new Supergraph(g);
	/// var u = sg.AddNode(); // create a new node
	/// var a = sg.AddArc(u, g.GetNode(3), Directedness.Undirected); // add an edge
	/// Console.WriteLine(string.Format("The augmented graph contains {0} nodes and {1} arcs.",
	///		sg.NodeCount(), sg.ArcCount())); // should print 11 and 46, respectively
	/// </code>
	/// </para>
	/// </remarks>
	public class Supergraph : 
        IBuildableGraph, 
        IDestroyableGraph, 
        IGraph
	{
        /// <inheritdoc cref="IdAllocator" />
		private class NodeAllocator : 
            IdAllocator
		{
			public Supergraph? Parent;

			/// <summary>
			/// Initialize <see cref="NodeAllocator"/>.
			/// </summary>
			public NodeAllocator()
            {
            }

			/// <inheritdoc />
            protected override bool IsAllocated(long id)
            {
                return Parent?.HasNode(new(id)) ?? false;
            }
		}

		/// <inheritdoc cref="IdAllocator" />
		private class ArcAllocator : 
            IdAllocator
		{
			public Supergraph? Parent;

			/// <summary>
			/// Initialize <see cref="ArcAllocator"/>.
            /// </summary>
			public ArcAllocator()
            {
            }

            /// <inheritdoc />
			protected override bool IsAllocated(long id)
            {
                return Parent?.HasArc(new(id)) ?? false;
            }
		}

		private class ArcProperties
		{
			/// <summary>
			/// The first node.
			/// </summary>
			public Node U { get; }

			/// <summary>
			/// The second node.
			/// </summary>
			public Node V { get; }

			/// <summary>
			/// The arc is edge.
			/// </summary>
			public bool IsEdge { get; }

			/// <summary>
			/// Initialize <see cref="ArcProperties"/>.
			/// </summary>
			/// <param name="u">U node.</param>
			/// <param name="v">V node.</param>
			/// <param name="isEdge">Is edge.</param>
			public ArcProperties(Node u, Node v, bool isEdge)
			{
				U = u;
				V = v;
				IsEdge = isEdge;
			}
		}

		private readonly IGraph? _graph;

		private readonly NodeAllocator _nodeAllocator;
		private readonly ArcAllocator _arcAllocator;

		private readonly HashSet<Node> _nodes;
		private readonly HashSet<Arc> _arcs;
		private readonly Dictionary<Arc, ArcProperties> _arcProperties;
        private readonly Dictionary<Node, NodeProperties> _nodeProperties;
		private readonly HashSet<Arc> _edges;

		private readonly Dictionary<Node, List<Arc>> _nodeArcsAll;
		private readonly Dictionary<Node, List<Arc>> _nodeArcsEdge;
		private readonly Dictionary<Node, List<Arc>> _nodeArcsForward;
		private readonly Dictionary<Node, List<Arc>> _nodeArcsBackward;

		/// <summary>
		/// Initialize <see cref="Supergraph"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		/// <param name="nodeProperties">Node properties dictionary.</param>
		public Supergraph(
            IGraph? graph,
            Dictionary<Node, NodeProperties>? nodeProperties = default)
		{
			_graph = graph;

			_nodeAllocator = new() { Parent = this };
			_arcAllocator = new() { Parent = this };

			_nodes = new();
			_arcs = new();
			_arcProperties = new();
            _nodeProperties = nodeProperties?
                .Where(x => _graph?.HasNode(x.Key) == true)
                .ToDictionary(x => x.Key, y => y.Value) ?? new();
			_edges = new();

			_nodeArcsAll = new();
			_nodeArcsEdge = new();
			_nodeArcsForward = new();
			_nodeArcsBackward = new();
		}

		/// <summary>
		/// Deletes all nodes and arcs of the adapter.
		/// </summary>
		public void Clear()
		{
			_nodeAllocator.Rewind();
			_arcAllocator.Rewind();

			_nodes.Clear();
			_arcs.Clear();
			_arcProperties.Clear();
            _nodeProperties.Clear();
            _edges.Clear();

			_nodeArcsAll.Clear();
			_nodeArcsEdge.Clear();
			_nodeArcsForward.Clear();
			_nodeArcsBackward.Clear();
		}

		/// <inheritdoc />
		public Node AddNode(Dictionary<string, object>? properties = default)
		{
            if (NodeCount() == int.MaxValue)
            {
                throw new InvalidOperationException("Error: too many nodes!");
            }

			var node = new Node(_nodeAllocator.Allocate());
			if (!_nodeProperties.ContainsKey(node))
            {
                _nodeProperties.Add(node, new(properties));
            }
			_nodes.Add(node);
			return node;
		}

        /// <summary>
        /// Add a new node to the graph with id.
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="properties">Node properties.</param>
        /// <returns>Returns new added node.</returns>
        public Node AddNode(long id, Dictionary<string, object>? properties = default)
		{
            if (NodeCount() == int.MaxValue)
            {
                throw new InvalidOperationException("Error: too many nodes!");
            }

			var node = new Node(id);
			if (!_nodeProperties.ContainsKey(node))
            {
                _nodeProperties.Add(node, new(properties));
            }
			_nodes.Add(node);
			return node;
		}

        /// <inheritdoc />
		public Arc AddArc(Node u, Node v, Directedness directedness)
		{
            if (ArcCount() == int.MaxValue)
            {
                throw new InvalidOperationException("Error: too many arcs!");
            }

#if DEBUG
			// check if u and v are valid nodes of the graph
			Debug.Assert(HasNode(u));
			Debug.Assert(HasNode(v));
#endif

			var a = new Arc(_arcAllocator.Allocate());
			_arcs.Add(a);
			var isEdge = (directedness == Directedness.Undirected);
			_arcProperties[a] = new(u, v, isEdge);

			Utils.MakeEntry(_nodeArcsAll, u).Add(a);
			Utils.MakeEntry(_nodeArcsForward, u).Add(a);
			Utils.MakeEntry(_nodeArcsBackward, v).Add(a);

			if (isEdge)
			{
				_edges.Add(a);
				Utils.MakeEntry(_nodeArcsEdge, u).Add(a);
			}

			if (v != u)
			{
				Utils.MakeEntry(_nodeArcsAll, v).Add(a);
				if (isEdge)
				{
					Utils.MakeEntry(_nodeArcsEdge, v).Add(a);
					Utils.MakeEntry(_nodeArcsForward, v).Add(a);
					Utils.MakeEntry(_nodeArcsBackward, u).Add(a);
				}
			}

			return a;
		}

        /// <inheritdoc />
		public bool DeleteNode(Node node)
		{
            if (!_nodes.Remove(node))
            {
                return false;
            }

            _nodeProperties.Remove(node);

			bool ArcsToRemove(Arc a) => (U(a) == node || V(a) == node);

            // remove arcs from nodeArcs_... of other ends of the arcs going from "node"
			foreach (var otherNode in Nodes())
			{
				if (otherNode != node)
				{
					Utils.RemoveAll(_nodeArcsAll[otherNode], ArcsToRemove);
					Utils.RemoveAll(_nodeArcsEdge[otherNode], ArcsToRemove);
					Utils.RemoveAll(_nodeArcsForward[otherNode], ArcsToRemove);
					Utils.RemoveAll(_nodeArcsBackward[otherNode], ArcsToRemove);
				}
			}

			Utils.RemoveAll(_arcs, ArcsToRemove);
			Utils.RemoveAll(_edges, ArcsToRemove);
			Utils.RemoveAll(_arcProperties, ArcsToRemove);

            _nodeArcsAll.Remove(node);
			_nodeArcsEdge.Remove(node);
			_nodeArcsForward.Remove(node);
			_nodeArcsBackward.Remove(node);

			return true;
		}

        /// <inheritdoc />
		public bool DeleteArc(Arc arc)
		{
			if (!_arcs.Remove(arc)) return false;

			var p = _arcProperties[arc];
			_arcProperties.Remove(arc);

			Utils.RemoveLast(_nodeArcsAll[p.U], arc);
			Utils.RemoveLast(_nodeArcsForward[p.U], arc);
			Utils.RemoveLast(_nodeArcsBackward[p.V], arc);

			if (p.IsEdge)
			{
				_edges.Remove(arc);
				Utils.RemoveLast(_nodeArcsEdge[p.U], arc);
			}

			if (p.V != p.U)
			{
				Utils.RemoveLast(_nodeArcsAll[p.V], arc);
				if (p.IsEdge)
				{
					Utils.RemoveLast(_nodeArcsEdge[p.V], arc);
					Utils.RemoveLast(_nodeArcsForward[p.V], arc);
					Utils.RemoveLast(_nodeArcsBackward[p.U], arc);
				}
			}

			return true;
		}

        /// <inheritdoc />
		public Dictionary<string, object>? Properties(Node node)
        {
            return _nodeProperties.TryGetValue(node, out var p)
                ? p.Properties
                : _graph?.Properties(node) ?? null;
		}

		/// <inheritdoc />
		public Node U(Arc arc)
        {
            return _arcProperties.TryGetValue(arc, out var p) 
                ? p.U 
                : _graph?.U(arc) ?? Node.Invalid;
        }

        /// <inheritdoc />
		public Node V(Arc arc)
        {
            return _arcProperties.TryGetValue(arc, out var p) 
                ? p.V 
                : _graph?.V(arc) ?? Node.Invalid;
        }

        /// <inheritdoc />
		public bool IsEdge(Arc arc)
        {
            return _arcProperties.TryGetValue(arc, out var p) 
                ? p.IsEdge 
                : _graph?.IsEdge(arc) ?? false;
        }

		private HashSet<Arc> ArcsInternal(ArcFilter filter)
		{
			return filter == ArcFilter.All 
                ? _arcs 
                : _edges;
		}

        private List<Arc> ArcsInternal(Node v, ArcFilter filter)
		{
			List<Arc>? result;
			switch (filter)
			{
				case ArcFilter.All: 
                    _nodeArcsAll.TryGetValue(v, out result); 
                    break;
				case ArcFilter.Edge: 
                    _nodeArcsEdge.TryGetValue(v, out result); 
                    break;
				case ArcFilter.Forward: 
                    _nodeArcsForward.TryGetValue(v, out result); 
                    break;
				default: 
                    _nodeArcsBackward.TryGetValue(v, out result); 
                    break;
			}

			return result ?? new();
		}

        /// <inheritdoc />
		public IEnumerable<Node> Nodes()
		{
			return _graph == null 
                ? _nodes 
                : _nodes.Concat(_graph.Nodes());
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			return _graph == null 
                ? ArcsInternal(filter) 
                : ArcsInternal(filter).Concat(_graph.Arcs(filter));
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
            if (_graph == null || _nodes.Contains(u))
            {
                return ArcsInternal(u, filter);
            }

			return ArcsInternal(u, filter).Concat(_graph.Arcs(u, filter));
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
            foreach (var arc in ArcsInternal(u, filter))
            {
                if (this.Other(arc, u) == v)
                {
                    yield return arc;
                }
            }

			if (!(_graph == null || _nodes.Contains(u) || _nodes.Contains(v)))
            {
                foreach (var arc in _graph.Arcs(u, v, filter))
                {
                    yield return arc;
                }
            }
		}

        /// <inheritdoc />
		public int NodeCount()
		{
			return _nodes.Count + (_graph?.NodeCount() ?? 0);
		}

        /// <inheritdoc />
		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return ArcsInternal(filter).Count + (_graph?.ArcCount(filter) ?? 0);
		}

        /// <inheritdoc />
		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			return ArcsInternal(u, filter).Count + (_graph == null || _nodes.Contains(u) ? 0 : _graph.ArcCount(u, filter));
		}

        /// <inheritdoc />
		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			var result = 0;
            foreach (var arc in ArcsInternal(u, filter))
            {
                if (this.Other(arc, u) == v)
                {
                    result++;
                }
            }

			return result + (_graph == null || _nodes.Contains(u) || _nodes.Contains(v) ? 0 : _graph.ArcCount(u, v, filter));
		}

        /// <inheritdoc />
		public bool HasNode(Node node)
		{
			return _nodes.Contains(node) || (_graph != null && _graph.HasNode(node));
		}

        /// <inheritdoc />
		public bool HasArc(Arc arc)
		{
			return _arcs.Contains(arc) || (_graph != null && _graph.HasArc(arc));
		}
	}
}
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
	/// Adapter for identifying some nodes of an underlying graph.
	/// </summary>
	/// <remarks>
	/// <para>Uses a <see cref="DisjointSet{T}"/> to keep track of node equivalence classes.</para>
	/// <para>
	/// <see cref="Node"/> and <see cref="Arc"/> objects are interchangeable between the adapter and the original graph,
	/// though some nodes of the underlying graph represent the same node in the adapter.
	/// </para>
	/// <para> The underlying graph can be modified while using this adapter, as long as none of its nodes are deleted.</para>
	/// </remarks>
	public sealed class ContractedGraph : 
        IGraph
	{
		private readonly IGraph _graph;
		private readonly DisjointSet<Node> _nodeGroups;
        private int _unionCount;

        /// <inheritdoc />
        public Dictionary<Node, NodeProperties> NodePropertiesDictionary { get; } = new();

        /// <inheritdoc />
        public Dictionary<Arc, ArcProperties> ArcPropertiesDictionary { get; } = new();

		/// <summary>
		/// Initialize <see cref="ContractedGraph"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
        public ContractedGraph(
            IGraph graph)
		{
			_graph = graph;
			_nodeGroups = new();
            Reset();
		}

		/// <summary>
		/// Undoes all mergings.
		/// </summary>
		public void Reset()
		{
			_nodeGroups.Clear();
            NodePropertiesDictionary.Clear();
            ArcPropertiesDictionary.Clear();
			_unionCount = 0;
		}

		/// <summary>
		/// Gets the node of the contracted graph which contains the given original node as a child.
		/// </summary>
		/// <param name="n">A node of the original graph (this includes nodes of the adapter).</param>
		/// <returns>Returns the merged node which contains the given node.</returns>
		public Node GetRepresentative(Node n)
		{
			return _nodeGroups.WhereIs(n).Representative;
		}

		/// <summary>
		/// Gets the nodes which are contracted with the given node.
		/// </summary>
		/// <param name="n">A node of the original graph (this includes nodes of the adapter).</param>
		/// <returns>Returns the nodes in the same equivalence class (at least 1).</returns>
		public IEnumerable<Node> GetChildren(Node n)
		{
			return _nodeGroups.Elements(_nodeGroups.WhereIs(n));
		}

		/// <summary>
		/// Identifies two nodes so they become one node.
		/// </summary>
		/// <param name="u">A node of the original graph (this includes nodes of the adapter).</param>
		/// <param name="v">Another node of the original graph (this includes nodes of the adapter).</param>
		/// <returns>Returns the object representing the merged node.</returns>
		public Node Merge(Node u, Node v)
		{
			var x = _nodeGroups.WhereIs(u);
			var y = _nodeGroups.WhereIs(v);
            if (x.Equals(y))
            {
                return x.Representative;
            }

			_unionCount++;
			return _nodeGroups.Union(x, y).Representative;
		}

		/// <summary>
		/// Contracts an arc into a node.
		/// </summary>
		/// <param name="arc">An arc of the original graph (or, equivalently, one of the adapter).</param>
		/// <returns>Returns the node resulting from the contracted arc.</returns>
		public Node Contract(Arc arc)
		{
			return Merge(_graph.U(arc), _graph.V(arc));
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
			return GetRepresentative(_graph.U(arc));
		}

        /// <inheritdoc />
		public Node V(Arc arc)
		{
			return GetRepresentative(_graph.V(arc));
		}

        /// <inheritdoc />
		public bool IsEdge(Arc arc)
		{
			return _graph.IsEdge(arc);
		}

        /// <inheritdoc />
		public IEnumerable<Node> Nodes()
		{
            foreach (var node in _graph.Nodes())
            {
                if (GetRepresentative(node) == node)
                {
                    yield return node;
                }
            }
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			return _graph.Arcs(filter);
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			foreach (var node in GetChildren(u))
			{
				foreach (var arc in _graph.Arcs(node, filter))
				{
					var loop = (U(arc) == V(arc));

					// we should avoid outputting an arc twice
                    if (!loop || !(filter == ArcFilter.All || IsEdge(arc)) || _graph.U(arc) == node)
                    {
                        yield return arc;
                    }
				}
			}
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
            foreach (var arc in Arcs(u, filter))
            {
                if (this.Other(arc, u) == v)
                {
                    yield return arc;
                }
            }
		}

        /// <inheritdoc />
		public int NodeCount()
		{
			return _graph.NodeCount() - _unionCount;
		}

        /// <inheritdoc />
		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return _graph.ArcCount(filter);
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
			return node == GetRepresentative(node);
		}

		/// <inheritdoc />
		public bool HasArc(Arc arc)
		{
			return _graph.HasArc(arc);
		}
    }
}
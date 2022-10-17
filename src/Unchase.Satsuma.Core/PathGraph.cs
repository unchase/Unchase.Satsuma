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

using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Core
{
    /// <summary>
	/// A path or cycle graph on a given number of nodes.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Not to be confused with <see cref="Path"/>. <see cref="Path"/> is an adapter which stores a path or cycle of some other graph,
	/// while <see cref="PathGraph"/> is a standalone graph (a "graph constant").
	/// </para>
	/// <para>Memory usage: O(1).</para>
	/// <para>This type is thread safe.</para>
	/// </remarks>
	public sealed class PathGraph : 
        IPath
	{
		private readonly int _nodeCount;
		private readonly bool _isCycle, _directed;

        /// <inheritdoc />
        public Dictionary<Node, NodeProperties> NodePropertiesDictionary { get; } = new();

        /// <inheritdoc />
        public Dictionary<Arc, ArcProperties> ArcPropertiesDictionary { get; } = new();

		/// <summary>
		/// The first node.
		/// </summary>
		public Node FirstNode => _nodeCount > 0 
            ? new(1) 
            : Node.Invalid;

		/// <summary>
		/// The last node.
		/// </summary>
        public Node LastNode => _nodeCount > 0 
            ? new(_isCycle ? 1 : _nodeCount) 
            : Node.Invalid;

		/// <summary>
		/// Initialize <see cref="PathGraph"/>.
		/// </summary>
		/// <param name="nodeCount">Node count.</param>
		/// <param name="topology"><see cref="Topology"/>.</param>
		/// <param name="directedness"><see cref="Directedness"/>.</param>
		public PathGraph(
            int nodeCount, 
            Topology topology, 
            Directedness directedness)
		{
			_nodeCount = nodeCount;
			_isCycle = topology == Topology.Cycle;
			_directed = directedness == Directedness.Directed;
        }

		/// <summary>
		/// Gets a node of the path by its index.
		/// </summary>
		/// <param name="index">An integer between 0 (inclusive) and NodeCount() (exclusive).</param>
        public Node GetNode(int index)
		{
			return new Node(1L + index);
        }

		/// <summary>
		/// Gets the index of a path node.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Returns an integer between 0 (inclusive) and NodeCount() (exclusive).</returns>
		public int GetNodeIndex(Node node)
		{
			return (int)(node.Id - 1);
		}

		/// <inheritdoc />
		public Arc NextArc(Node node)
		{
            if (!_isCycle && node.Id == _nodeCount)
            {
                return Arc.Invalid;
            }

			return new(node.Id);
		}

        /// <inheritdoc />
		public Arc PrevArc(Node node)
		{
            if (node.Id == 1)
            {
                return _isCycle 
                    ? new(_nodeCount) 
                    : Arc.Invalid;
            }

			return new(node.Id - 1);
		}

		/// <inheritdoc />
        public Dictionary<string, object>? GetNodeProperties(Node node)
        {
            return NodePropertiesDictionary.TryGetValue(node, out var p)
                ? p.Properties
                : null;
        }

        /// <inheritdoc />
        public Dictionary<string, object>? GetArcProperties(Arc arc)
        {
            return ArcPropertiesDictionary.TryGetValue(arc, out var p)
                ? p.Properties
                : null;
        }

		/// <inheritdoc />
		public Node U(Arc arc)
		{
			return new(arc.Id);
		}

        /// <inheritdoc />
		public Node V(Arc arc)
		{
			return new(arc.Id == _nodeCount 
                ? 1 
                : arc.Id + 1);
		}

        /// <inheritdoc />
		public bool IsEdge(Arc arc)
		{
			return !_directed;
		}

        /// <inheritdoc />
		public IEnumerable<Node> Nodes()
		{
            for (var i = 1; i <= _nodeCount; i++)
            {
                yield return new(i);
            }
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
            if (_directed && filter == ArcFilter.Edge)
            {
                yield break;
            }

            for (int i = 1, n = ArcCountInternal(); i <= n; i++)
            {
                yield return new(i);
            }
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			return this.ArcsHelper(u, filter);
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return Arcs(u, filter)
                .Where(arc => this.Other(arc, u) == v);
		}

        /// <inheritdoc />
		public int NodeCount()
		{
			return _nodeCount;
		}

		private int ArcCountInternal()
		{
			return _nodeCount == 0 
                ? 0 
                : (_isCycle ? _nodeCount : _nodeCount - 1);
		}

        /// <inheritdoc />
		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return _directed && filter == ArcFilter.Edge 
                ? 0 
                : ArcCountInternal();
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
			return node.Id >= 1 && node.Id <= _nodeCount;
		}

        /// <inheritdoc />
		public bool HasArc(Arc arc)
		{
			return arc.Id >= 1 && arc.Id <= ArcCountInternal();
		}
	}
}
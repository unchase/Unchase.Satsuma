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
	/// Adapter for storing a path of an underlying graph.
	/// </summary>
	/// <remarks>
	/// <para>The Node and Arc set of the adapter is a subset of that of the original graph.</para>
	/// <para>
	/// The underlying graph can be modified while using this adapter,
	/// as long as no path nodes and path arcs are deleted.
	/// </para>
	/// <para>
	/// Example (building a path):
	/// <code>
	/// var g = new CompleteGraph(15);
    /// var p = new Path(g);
    /// var u = g.GetNode(0), v = g.GetNode(1), w = g.GetNode(2);
    /// p.Begin(u);
    /// p.AddLast(g.GetArc(u, v));
    /// p.AddFirst(g.GetArc(w, u));
    /// // now we have the w--u--v path
    /// p.Reverse();
    /// // now we have the v--u--w path
	/// </code>
	/// </para>
	/// <para></para>
	/// </remarks>
	public sealed class Path : 
        IPath, 
        IClearable
	{
		/// The graph containing the path.
		public IGraph Graph { get; }

		/// <summary>
		/// The first node.
		/// </summary>
		public Node FirstNode { get; private set; }

		/// <summary>
		/// The last node.
		/// </summary>
		public Node LastNode { get; private set; }

        /// <inheritdoc />
        public Dictionary<Node, NodeProperties> NodePropertiesDictionary { get; } = new();

        /// <inheritdoc />
        public Dictionary<Arc, ArcProperties> ArcPropertiesDictionary { get; } = new();

		private int _nodeCount;
		private Dictionary<Node, Arc> _nextArc;
		private Dictionary<Node, Arc> _prevArc;
		private readonly HashSet<Arc> _arcs;
        private int _edgeCount;

		/// <summary>
		/// Initialize and empty <see cref="Path"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
        public Path(
            IGraph graph)
		{
			Graph = graph;

			_nextArc = new();
			_prevArc = new();
			_arcs = new();

            Clear();
		}

		/// <summary>
		/// Resets the path to an empty path.
		/// </summary>
		public void Clear()
		{
			FirstNode = Node.Invalid;
			LastNode = Node.Invalid;

			_nodeCount = 0;
			_nextArc.Clear();
			_prevArc.Clear();
			_arcs.Clear();
            NodePropertiesDictionary.Clear();
            ArcPropertiesDictionary.Clear();
			_edgeCount = 0;
		}

		/// <summary>
		/// Makes a one-node path from an empty path.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <exception cref="InvalidOperationException">The path is not empty.</exception>
		public void Begin(Node node)
		{
            if (_nodeCount > 0)
            {
                throw new InvalidOperationException("Path not empty.");
            }

			_nodeCount = 1;
			FirstNode = LastNode = node;
		}

		/// <summary>
		/// Appends an arc to the start of the path.
		/// </summary>
		/// <param name="arc">An arc connecting #FirstNode either with #LastNode or with a node not yet on the path. The arc may point in any direction.</param>
		/// <exception cref="ArgumentException">The arc is not valid or the path is a cycle.</exception>
		public void AddFirst(Arc arc)
		{
			Node u = U(arc), v = V(arc);
			var newNode = (u == FirstNode ? v : u);
            if ((u != FirstNode && v != FirstNode) || _nextArc.ContainsKey(newNode) || _prevArc.ContainsKey(FirstNode))
            {
                throw new ArgumentException("Arc not valid or path is a cycle.");
            }

            if (newNode != LastNode)
            {
                _nodeCount++;
            }

			_nextArc[newNode] = arc;
			_prevArc[FirstNode] = arc;
			if (!_arcs.Contains(arc))
			{
				_arcs.Add(arc);
                if (IsEdge(arc))
                {
                    _edgeCount++;
                }
			}

			FirstNode = newNode;
		}

		/// <summary>
		/// Appends an arc to the end of the path.
		/// </summary>
		/// <param name="arc">An arc connecting #LastNode either with #FirstNode or with a node not yet on the path. The arc may point in any direction.</param>
		/// <exception cref="ArgumentException">The arc is not valid or the path is a cycle.</exception>
		public void AddLast(Arc arc)
		{
			Node u = U(arc), v = V(arc);
			var newNode = (u == LastNode ? v : u);
            if ((u != LastNode && v != LastNode) || _nextArc.ContainsKey(LastNode) || _prevArc.ContainsKey(newNode))
            {
                throw new ArgumentException("Arc not valid or path is a cycle.");
            }

            if (newNode != FirstNode)
            {
                _nodeCount++;
            }

			_nextArc[LastNode] = arc;
			_prevArc[newNode] = arc;
			if (!_arcs.Contains(arc))
			{
				_arcs.Add(arc);
                if (IsEdge(arc))
                {
                    _edgeCount++;
                }
			}

			LastNode = newNode;
		}

		/// <summary>
		/// Reverses the path in O(1) time.
		/// </summary>
		/// <remarks>
		/// For example, the u — v → w path becomes the w ← v — u path.
		/// </remarks>
		public void Reverse()
		{
			{ (FirstNode, LastNode) = (LastNode, FirstNode); }
			{ (_nextArc, _prevArc) = (_prevArc, _nextArc); }
		}

		/// <inheritdoc />
		public Arc NextArc(Node node)
		{
            return _nextArc.TryGetValue(node, out var arc) 
                ? arc 
                : Arc.Invalid;
		}

        /// <inheritdoc />
		public Arc PrevArc(Node node)
		{
            return _prevArc.TryGetValue(node, out var arc) 
                ? arc 
                : Arc.Invalid;
		}

		/// <inheritdoc />
        public Dictionary<string, object>? GetNodeProperties(Node node)
        {
            return NodePropertiesDictionary.TryGetValue(node, out var p)
                ? p.Properties
                : Graph.GetNodeProperties(node);
        }

        /// <inheritdoc />
        public Dictionary<string, object>? GetArcProperties(Arc arc)
        {
            return ArcPropertiesDictionary.TryGetValue(arc, out var p)
                ? p.Properties
                : Graph.GetArcProperties(arc);
        }

		/// <inheritdoc />
		public Node U(Arc arc)
		{
			return Graph.U(arc);
		}

        /// <inheritdoc />
		public Node V(Arc arc)
		{
			return Graph.V(arc);
		}

        /// <inheritdoc />
		public bool IsEdge(Arc arc)
		{
			return Graph.IsEdge(arc);
		}

        /// <inheritdoc />
		public IEnumerable<Node> Nodes()
		{
			var n = FirstNode;
            if (n == Node.Invalid)
            {
                yield break;
            }

			while (true)
			{
				yield return n;
				var arc = NextArc(n);
                if (arc == Arc.Invalid)
                {
                    yield break;
                }

				n = Graph.Other(arc, n);
                if (n == FirstNode)
                {
                    yield break;
                }
			}
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
			if (filter == ArcFilter.All) return _arcs;
            if (_edgeCount == 0)
            {
                return Enumerable.Empty<Arc>();
            }

			return _arcs.Where(IsEdge);
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			return this.ArcsHelper(u, filter);
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			return Arcs(u, filter).Where(arc => this.Other(arc, u) == v);
		}

        /// <inheritdoc />
		public int NodeCount()
		{
			return _nodeCount;
		}

        /// <inheritdoc />
		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
			return filter == ArcFilter.All 
                ? _arcs.Count 
                : _edgeCount;
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
			return _prevArc.ContainsKey(node) || (node != Node.Invalid && node == FirstNode);
		}

        /// <inheritdoc />
		public bool HasArc(Arc arc)
		{
			return _arcs.Contains(arc);
		}
    }
}
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

using Unchase.Satsuma.Adapters.Abstractions;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;

namespace Unchase.Satsuma.Adapters
{
    /// <summary>
	/// Adapter for storing a matching of an underlying graph.
	/// </summary>
	/// <remarks>
	/// <para>The Node and Arc set of the adapter is a subset of that of the original graph.</para>
	/// <para>
	/// The underlying graph can be modified while using this adapter,
	/// as long as no matched nodes and matching arcs are deleted.
	/// </para>
	/// <para>A newly created Matching object has zero arcs.</para>
	/// </remarks>
	public sealed class Matching : 
        IMatching, 
        IClearable
	{
		/// <summary>
		/// The input graph.
		/// </summary>
		public IGraph Graph { get; }

        /// <inheritdoc />
        public Dictionary<Node, NodeProperties> NodePropertiesDictionary { get; } = new();

        /// <inheritdoc />
        public Dictionary<Arc, ArcProperties> ArcPropertiesDictionary { get; } = new();

		private readonly Dictionary<Node, Arc> _matchedArc;
		private readonly HashSet<Arc> _arcs;
		private int _edgeCount;

		/// <summary>
		/// Initialize <see cref="Matching"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		public Matching(IGraph graph)
		{
			Graph = graph;

			_matchedArc = new();
			_arcs = new();

			Clear();
		}

		/// <inheritdoc />
		public void Clear()
		{
			_matchedArc.Clear();
			_arcs.Clear();
			_edgeCount = 0;
		}

		/// <summary>
		/// Enables/disables an arc (adds/removes it from the matching).
		/// </summary>
		/// <remarks>
		/// If the arc is already enabled/disabled, does nothing.
		/// </remarks>
		/// <param name="arc">An arc of <see cref="Graph"/>.</param>
		/// <param name="enabled">Enabled.</param>
		/// <exception cref="ArgumentException">Trying to enable an illegal arc.</exception>
		public void Enable(Arc arc, bool enabled)
		{
            if (enabled == _arcs.Contains(arc))
            {
                return;
            }

			Node u = Graph.U(arc), v = Graph.V(arc);
			if (enabled)
			{
                if (u == v)
                {
                    throw new ArgumentException("Matchings cannot have loop arcs.");
                }

                if (_matchedArc.ContainsKey(u))
                {
                    throw new ArgumentException("Node is already matched: " + u);
                }

                if (_matchedArc.ContainsKey(v))
                {
                    throw new ArgumentException("Node is already matched: " + v);
                }

				_matchedArc[u] = arc;
				_matchedArc[v] = arc;
				_arcs.Add(arc);
                if (Graph.IsEdge(arc))
                {
                    _edgeCount++;
                }
			}
			else
			{
				_matchedArc.Remove(u);
				_matchedArc.Remove(v);
				_arcs.Remove(arc);
                if (Graph.IsEdge(arc))
                {
                    _edgeCount--;
                }
			}
		}

        /// <inheritdoc />
		public Arc MatchedArc(Node node)
		{
            return _matchedArc.TryGetValue(node, out var arc) 
                ? arc 
                : Arc.Invalid;
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
			return _matchedArc.Keys;
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
            if (filter == ArcFilter.All)
            {
                return _arcs;
            }

            if (_edgeCount == 0)
            {
                return Enumerable.Empty<Arc>();
            }

			return _arcs.Where(arc => IsEdge(arc));
		}

		// arc must contain u
		private bool YieldArc(Node u, ArcFilter filter, Arc arc)
		{
			return filter == ArcFilter.All || IsEdge(arc) ||
				(filter == ArcFilter.Forward && U(arc) == u) ||
				(filter == ArcFilter.Backward && V(arc) == u);
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			var arc = MatchedArc(u);
            if (arc != Arc.Invalid && YieldArc(u, filter, arc))
            {
                yield return arc;
            }
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (u != v)
			{
				var arc = MatchedArc(u);
                if (arc != Arc.Invalid && arc == MatchedArc(v) && YieldArc(u, filter, arc))
                {
                    yield return arc;
                }
			}
		}

        /// <inheritdoc />
		public int NodeCount()
		{
			return _matchedArc.Count;
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
			var arc = MatchedArc(u);
			return arc != Arc.Invalid && YieldArc(u, filter, arc) 
                ? 1 
                : 0;
		}

        /// <inheritdoc />
		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			if (u != v)
			{
				var arc = MatchedArc(u);
				return arc != Arc.Invalid && arc == MatchedArc(v) && YieldArc(u, filter, arc) 
                    ? 1 
                    : 0;
			}

			return 0;
		}

        /// <inheritdoc />
		public bool HasNode(Node node)
		{
			return Graph.HasNode(node) && _matchedArc.ContainsKey(node);
		}

        /// <inheritdoc />
		public bool HasArc(Arc arc)
		{
			return Graph.HasArc(arc) && _arcs.Contains(arc);
		}
	}
}
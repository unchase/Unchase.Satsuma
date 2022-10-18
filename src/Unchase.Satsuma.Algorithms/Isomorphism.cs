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

using Unchase.Satsuma.Algorithms.Connectivity;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;

namespace Unchase.Satsuma.Algorithms
{
	/// <inheritdoc cref="Isomorphism{TNodeProperty, TArcProperty}"/>
	public class Isomorphism :
        Isomorphism<object, object>
    {
		/// <summary>
		/// Initialize <seealso cref="Isomorphism"/>.
		/// </summary>
		/// <param name="firstGraph">The first of the two input graphs.</param>
		/// <param name="secondGraph">The second of the two input graphs.</param>
		/// <param name="maxIterations">Maximum iterations.</param>
		public Isomorphism(
            IGraph firstGraph,
            IGraph secondGraph,
            int maxIterations = 16)
		        : base(firstGraph, secondGraph, maxIterations)
        {
		}
    }

	/// <summary>
	/// Determines whether two graphs, digraphs or mixed graphs are isomorphic.
	/// </summary>
	/// <remarks>
	/// <para>Uses simple color refinement, but the multisets are hashed at every step, so only the hashes are stored.</para>
	/// <para>The current implementation is fast but will not be able to identify isomorphisms in many cases.</para>
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
	/// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public class Isomorphism<TNodeProperty, TArcProperty>
	{
        /// <summary>
		/// The first of the two input graphs.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> FirstGraph;

		/// <summary>
		/// The second of the two input graphs.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> SecondGraph;

		/// <summary>
		/// Whether the graphs are isomorphic. Will be null if the algorithm could not decide.
		/// </summary>
		/// <remarks>
		/// <para>If true, the graphs are isomorphic and FirstToSecond is a valid isomorphism.</para>
		/// <para>If false, the graphs are not isomorphic.</para>
		/// <para>If null, the graphs may be isomorphic or not. The algorithm could not decide.</para>
		/// </remarks>
		public bool? Isomorphic;

		/// <summary>
		/// A mapping from the nodes of the first graph to the nodes of the second graph.
		/// </summary>
		/// <remarks>
		/// <para>Only valid if Isomorphic is true.</para>
		/// <para>If u is a node of the first graph, then FirstToSecond[u] is the corresponding node in the second graph.</para>
		/// </remarks>
		public Dictionary<Node, Node>? FirstToSecond;

		private class NodeHash
		{
            private readonly IGraph<TNodeProperty, TArcProperty> _graph;
            private readonly int _minDegree;
            private readonly int _maxDegree;
            private Dictionary<Node, ulong> _buffer;

			/// <summary>
			/// Initialize <seealso cref="NodeHash"/>.
			/// </summary>
			/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
			public NodeHash(IGraph<TNodeProperty, TArcProperty> graph)
			{
				_graph = graph;
				_minDegree = int.MaxValue;
				_maxDegree = int.MinValue;
				Coloring = new(_graph.NodeCount());
				foreach (var n in _graph.Nodes())
				{
					var degree = _graph.ArcCount(n);
                    if (degree < _minDegree)
                    {
                        _minDegree = degree;
                    }

                    if (degree > _maxDegree)
                    {
                        _maxDegree = degree;
                    }

					Coloring[n] = (ulong)degree;
				}
				ComputeHash();
				_buffer = new(_graph.NodeCount());
			}

			public bool RegularGraph => _maxDegree == _minDegree;

            private Dictionary<Node, ulong> Coloring { get; set; }

            /// <summary>
			/// Sorts the nodes by color and returns the result.
			/// </summary>
            public List<KeyValuePair<Node, ulong>> GetSortedColoring()
			{
				var result = Coloring.ToList();
				result.Sort((a, b) => a.Value.CompareTo(b.Value));
				return result;
			}

			public ulong ColoringHash { get; private set; }

            private void ComputeHash()
			{
				// FNV-1a hash
				var hash = 2366353228990522973UL;
				foreach (var kv in Coloring)
				{
					hash = (hash ^ kv.Value) * 18395225509790253667UL;
				}

				ColoringHash = hash;
			}

			/// <summary>
			/// Perform a step of color refinement and hashing.
			/// </summary>
			public void Iterate()
			{
                foreach (var n in _graph.Nodes())
                {
                    _buffer[n] = 0;
                }

				foreach (var a in _graph.Arcs())
				{
					var u = _graph.U(a);
					var v = _graph.V(a);
					if (_graph.IsEdge(a))
					{
						_buffer[u] += Utils.ReversibleHash1(Coloring[v]);
						_buffer[v] += Utils.ReversibleHash1(Coloring[u]);
					}
					else
					{
						_buffer[u] += Utils.ReversibleHash2(Coloring[v]);
						_buffer[v] += Utils.ReversibleHash3(Coloring[u]);
					}
				}

				(Coloring, _buffer) = (_buffer, Coloring);

                ComputeHash();
			}
		}

		/// <summary>
		/// Initialize <seealso cref="Isomorphism{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="firstGraph"><see cref="FirstGraph"/>.</param>
		/// <param name="secondGraph"><see cref="SecondGraph"/>.</param>
		/// <param name="maxIterations">Maximum iterations.</param>
		public Isomorphism(
            IGraph<TNodeProperty, TArcProperty> firstGraph, 
            IGraph<TNodeProperty, TArcProperty> secondGraph, 
            int maxIterations = 16)
		{
			FirstGraph = firstGraph;
			SecondGraph = secondGraph;
			FirstToSecond = null;

			if (firstGraph.NodeCount() != secondGraph.NodeCount()
				|| firstGraph.ArcCount() != secondGraph.ArcCount()
				|| firstGraph.ArcCount(ArcFilter.Edge) != secondGraph.ArcCount(ArcFilter.Edge))
			{
				Isomorphic = false;
			}
			else
			{
				var firstCc = new ConnectedComponents<TNodeProperty, TArcProperty>(firstGraph, ConnectedComponentsFlags.CreateComponents);
				var secondCc = new ConnectedComponents<TNodeProperty, TArcProperty>(secondGraph, ConnectedComponentsFlags.CreateComponents);

                if (firstCc.Count != secondCc.Count || firstCc.Components != null && secondCc.Components != null &&
                    !firstCc.Components.Select(s => s.Count).OrderBy(x => x)
                        .SequenceEqual(secondCc.Components.Select(s => s.Count).OrderBy(x => x)))
                {
                    Isomorphic = false;
                }
                else
				{
					var firstHash = new NodeHash(firstGraph);
					var secondHash = new NodeHash(secondGraph);
					if (firstHash.ColoringHash != secondHash.ColoringHash)
					{
						// degree distribution not equal
						Isomorphic = false;
					}
					else if (firstHash.RegularGraph && secondHash.RegularGraph
						&& firstHash.ColoringHash == secondHash.ColoringHash)
					{
						// TODO do something with regular graphs
						// maybe spectral test
						Isomorphic = null;
					}
					else
					{
						Isomorphic = null;
						for (var i = 0; i < maxIterations; ++i)
						{
							firstHash.Iterate();
							secondHash.Iterate();
							if (firstHash.ColoringHash != secondHash.ColoringHash)
							{
								Isomorphic = false;
								break;
							}
						}

						if (Isomorphic == null)
						{
							// graphs are very probably isomorphic (or tricky), try to find the mapping
							var firstColor = firstHash.GetSortedColoring();
							var secondColor = secondHash.GetSortedColoring();

							// is the canonical coloring the same, and does it uniquely identify nodes?
							Isomorphic = true;
							for (var i = 0; i < firstColor.Count; ++i)
							{
								if (firstColor[i].Value != secondColor[i].Value)
								{
									// unlikely because the hashes matched
									Isomorphic = false;
									break;
								}
								else if (i > 0 && firstColor[i].Value == firstColor[i - 1].Value)
								{
									// two nodes colored the same way (this may happen)
									// TODO handle this case. Else we won't work for graphs with symmetries.
									Isomorphic = null;
									break;
								}
							}

							if (Isomorphic == true)
							{
								FirstToSecond = new(firstColor.Count);
                                for (var i = 0; i < firstColor.Count; ++i)
                                {
                                    FirstToSecond[firstColor[i].Key] = secondColor[i].Key;
                                }
							}
						}
					}
				}
			}
		}
	}
}
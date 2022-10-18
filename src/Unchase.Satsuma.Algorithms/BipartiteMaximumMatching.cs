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

using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Adapters.Abstractions;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Algorithms
{
	/// <inheritdoc cref="BipartiteMaximumMatching{TNodeProperty, TArcProperty}"/>
	public sealed class BipartiteMaximumMatching :
        BipartiteMaximumMatching<object, object>
    {
		/// <summary>
        /// Initialize <see cref="BipartiteMaximumMatching"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="isRed">Describes a bipartition of the input graph by dividing its nodes into red and blue ones.</param>
        public BipartiteMaximumMatching(
            IGraph graph,
            Func<Node, bool> isRed)
		        : base(graph, isRed)
        {
		}
    }

	/// <summary>
	/// Finds a maximum matching in a bipartite graph using the alternating path algorithm.
	/// </summary>
	/// <remarks>
	/// See also <seealso cref="BipartiteMinimumCostMatching{TNodeProperty, TArcProperty}"/>.
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
	/// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public class BipartiteMaximumMatching<TNodeProperty, TArcProperty> : 
        IClearable
	{
		/// <summary>
		/// The input graph.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

		/// <summary>
		/// Describes a bipartition of the input graph by dividing its nodes into red and blue ones.
		/// </summary>
		public Func<Node, bool> IsRed { get; }

		private readonly Matching<TNodeProperty, TArcProperty> _matching;

		/// <summary>
		/// The current matching.
		/// </summary>
		public IMatching<TNodeProperty, TArcProperty> Matching => _matching;

        private readonly HashSet<Node>? _unmatchedRedNodes;

		/// <summary>
		/// Initialize <see cref="BipartiteMaximumMatching{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
		/// <param name="isRed">Describes a bipartition of the input graph by dividing its nodes into red and blue ones.</param>
		public BipartiteMaximumMatching(
            IGraph<TNodeProperty, TArcProperty> graph, 
            Func<Node, bool> isRed)
		{
			Graph = graph;
			IsRed = isRed;
			_matching = new(Graph);
			_unmatchedRedNodes = new();

			Clear();
		}

		/// <summary>
		/// Removes all arcs from the matching.
		/// </summary>
		public void Clear()
		{
			_matching.Clear();
			_unmatchedRedNodes?.Clear();
			foreach (var n in Graph.Nodes())
            {
                if (IsRed(n))
                {
                    _unmatchedRedNodes?.Add(n);
                }
            }
        }

		/// <summary>
		/// Grows the current matching greedily.
		/// </summary>
		/// <remarks>
		/// Can be used to speed up optimization by finding a reasonable initial matching.
		/// </remarks>
		/// <param name="maxImprovements">The maximum number of arcs to grow the current matching with.</param>
		/// <returns>Returns the number of arcs added to the matching.</returns>
		public int GreedyGrow(int maxImprovements = int.MaxValue)
		{
			var result = 0;
			var matchedRedNodes = new List<Node>();
			foreach (var x in _unmatchedRedNodes ?? new())
            {
                foreach (var arc in Graph.Arcs(x))
				{
					var y = Graph.Other(arc, x);
					if (!_matching.HasNode(y))
					{
						_matching.Enable(arc, true);
						matchedRedNodes.Add(x);
						result++;
                        if (result >= maxImprovements)
                        {
                            goto BreakAll;
                        }

						break;
					}
				}
            }

			BreakAll:
            foreach (var n in matchedRedNodes)
            {
                _unmatchedRedNodes?.Remove(n);
            }
			return result;
		}

		/// <summary>
		/// Tries to add a specific arc to the current matching.
		/// </summary>
		/// <remarks>
		/// If the arc is already present, does nothing.
		/// </remarks>
		/// <param name="arc">An arc of <see cref="Graph"/>.</param>
		/// <exception cref="ArgumentException">Trying to add an illegal arc.</exception>
		public void Add(Arc arc)
		{
            if (_matching.HasArc(arc))
            {
                return;
            }

			_matching.Enable(arc, true);
			var u = Graph.U(arc);
			_unmatchedRedNodes?.Remove(IsRed(u) ? u : Graph.V(arc));
		}

		private Dictionary<Node, Arc>? _parentArc;

		private Node Traverse(Node node)
		{
			Arc matchedArc = _matching.MatchedArc(node);
            if (IsRed(node))
			{
				foreach (var arc in Graph.Arcs(node))
                {
                    if (arc != matchedArc)
					{
						var y = Graph.Other(arc, node);
						if (_parentArc != null && !_parentArc.ContainsKey(y))
						{
							_parentArc[y] = arc;
                            if (!_matching.HasNode(y))
                            {
                                return y;
                            }

							var result = Traverse(y);
                            if (result != Node.Invalid)
                            {
                                return result;
                            }
						}
					}
                }
			}
			else
			{
				var y = Graph.Other(matchedArc, node);
				if (_parentArc != null && !_parentArc.ContainsKey(y))
				{
					_parentArc[y] = matchedArc;
					var result = Traverse(y);
                    if (result != Node.Invalid)
                    {
                        return result;
                    }
				}
			}

			return Node.Invalid;
		}

		/// <summary>
		/// Grows the current matching to a maximum matching by running the whole alternating path algorithm.
		/// </summary>
		/// <remarks>
		/// Calling <see cref="GreedyGrow"/> before <see cref="Run"/> may speed up operation.
		/// </remarks>
		public void Run()
		{
			var matchedRedNodes = new List<Node>();
			_parentArc = new();
			foreach (var x in _unmatchedRedNodes ?? new())
			{
				_parentArc.Clear();
				_parentArc[x] = Arc.Invalid;

				// find an alternating path
				var y = Traverse(x);
                if (y == Node.Invalid)
                {
                    continue;
                }

				// modify matching along the alternating path
				while (true)
				{
					// y ----arc---- z (====arc2===)
					var arc = _parentArc[y];
					var z = Graph.Other(arc, y);
					var arc2 = (z == x ? Arc.Invalid : _parentArc[z]);
                    if (arc2 != Arc.Invalid)
                    {
                        _matching.Enable(arc2, false);
                    }

					_matching.Enable(arc, true);
                    if (arc2 == Arc.Invalid)
                    {
                        break;
                    }

					y = Graph.Other(arc2, z);
				}

				matchedRedNodes.Add(x);
			}

			_parentArc = null;

            foreach (var n in matchedRedNodes)
            {
                _unmatchedRedNodes?.Remove(n);
            }
		}
	}
}
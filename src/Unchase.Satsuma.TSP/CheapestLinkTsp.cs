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

using Unchase.Satsuma.Algorithms.SpanningForest;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;
using Unchase.Satsuma.TSP.Contracts;

namespace Unchase.Satsuma.TSP
{
    /// <summary>
	/// Solves the symmetric "traveling salesman problem" by using the cheapest link heuristic.
	/// </summary>
	/// <remarks>
	/// <remarks>
	/// <para>Works in a way very similar to Kruskal's algorithm.</para>
	/// <para>It maintains a forest as well, but this time the forest consists of paths only.</para>
	/// <para>In each step, it tries to glue two paths together, by using the cheapest possible link.</para>
	/// <para>Running time: O(n<sup>2</sup> \e log n), memory usage: O(n<sup>2</sup>); where \e n is the number of nodes.</para>
	/// </remarks>
	/// </remarks>
	/// <typeparam name="TNode"></typeparam>
	public sealed class CheapestLinkTsp<TNode> : 
        ITsp<TNode>
	{
        /// <summary>
		/// The nodes the salesman has to visit.
		/// </summary>
		/// <remarks>
		/// If your original node collection is not an <see cref="IList{T}"/>, you can convert it to a list using Enumerable.ToList.
		/// </remarks>
		public IList<TNode> Nodes { get; }

		/// <summary>
		/// A finite cost function on the node pairs. Must be symmetric, or at least close to symmetric.
		/// </summary>
		public Func<TNode, TNode, double> Cost { get; }

		private readonly List<TNode> _tour;

		/// <inheritdoc />
		public IEnumerable<TNode> Tour => _tour;

        /// <inheritdoc />
		public double TourCost { get; private set; }

		/// <summary>
		/// Initialize <see cref="CheapestLinkTsp{TNode}"/>.
		/// </summary>
		/// <param name="nodes"><see cref="Nodes"/>.</param>
		/// <param name="cost"><see cref="Cost"/>.</param>
		public CheapestLinkTsp(
            IList<TNode> nodes,
            Func<TNode, TNode, double> cost)
		{
			Nodes = nodes;
			Cost = cost;
			_tour = new();

			Run();
		}

		private void Run()
		{
			// create a complete graph and run Kruskal with maximum degree constraint 2
			var graph = new CompleteGraph(Nodes.Count, Directedness.Undirected);
            double ArcCost(Arc arc) => Cost(Nodes[CompleteGraph.GetNodeIndex(graph.U(arc))], Nodes[CompleteGraph.GetNodeIndex(graph.V(arc))]);
            var kruskal = new Kruskal<double>(graph, ArcCost, _ => 2);
			kruskal.Run();

			Dictionary<Node, Arc> firstArc = new();
			Dictionary<Node, Arc> secondArc = new();
			foreach (var arc in kruskal.Forest)
			{
				var u = graph.U(arc);
				(firstArc.ContainsKey(u) ? secondArc : firstArc)[u] = arc;
				var v = graph.V(arc);
				(firstArc.ContainsKey(v) ? secondArc : firstArc)[v] = arc;
			}

			foreach (var startNode in graph.Nodes())
			{
				if (kruskal.Degree[startNode] == 1)
				{
					var prevArc = Arc.Invalid;
					var n = startNode;
					while (true)
					{
						_tour.Add(Nodes[CompleteGraph.GetNodeIndex(n)]);
                        if (prevArc != Arc.Invalid && kruskal.Degree[n] == 1)
                        {
                            break;
                        }

						var arc1 = firstArc[n];
						prevArc = (arc1 != prevArc ? arc1 : secondArc[n]);
						n = graph.Other(prevArc, n);
					}

					_tour.Add(Nodes[CompleteGraph.GetNodeIndex(startNode)]);
					break;
				}
			}

			TourCost = TspUtils.GetTourCost(_tour, Cost);
		}
	}
}
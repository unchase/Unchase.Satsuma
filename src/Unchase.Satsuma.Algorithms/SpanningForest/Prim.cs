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
using Unchase.Satsuma.Algorithms.Connectivity;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Algorithms.SpanningForest
{
    /// <summary>
	/// Finds a minimum cost spanning forest in a graph using Prim's algorithm.
	/// </summary>
	/// <remarks>
	/// <para>Most suitable for dense graphs. For sparse (i.e. everyday) graphs, use <see cref="Kruskal{TCost}"/>.</para>
	/// <para>
	/// Running time: O((m+n) log n), memory usage: O(n); 
	/// where n is the number of nodes and m is the number of arcs.
	/// </para>
	/// <para>Example:</para>
	/// <para>
	/// <code>
	/// CompleteGraph g = new CompleteGraph(10);
	/// Node u = g.GetNode(0);
	/// Node v = g.GetNode(1);
	/// Node w = g.GetNode(2);
	/// var expensiveArcs = new HashSet&lt;Arc&gt;() { g.GetArc(u, v), g.GetArc(v, w) };
	/// Func&lt;Arc, double&gt; cost = (arc => expensiveArcs.Contains(arc) ? 1.5 : 1.0);
	/// var p = new Prim&lt;double&gt;(g, cost);
	/// // the graph is connected, so the spanning forest is a tree
	/// Console.WriteLine("Total cost of a minimum cost spanning tree: "+p.Forest.Sum(cost));
	/// Console.WriteLine("A minimum cost spanning tree:");
	/// foreach (var arc in p.Forest) Console.WriteLine(g.ArcToString(arc));
	/// </code>
	/// </para>
	/// <para>The graph in the example is a complete graph, which is dense.</para>
	/// <para>That's why we have used <see cref="Prim{TCost}"/> instead of <see cref="Kruskal{TCost}"/>.</para>
	/// </remarks>
	/// <typeparam name="TCost">The arc cost type.</typeparam>
	public sealed class Prim<TCost>
		where TCost : IComparable<TCost>
	{
		/// <summary>
		/// The input graph.
		/// </summary>
		public IGraph Graph { get; }

		/// <summary>
		/// An arbitrary function assigning costs to the arcs.
		/// </summary>
		public Func<Arc, TCost> Cost { get; }

		/// <summary>
		/// Contains the arcs of a cheapest spanning forest.
		/// </summary>
		public HashSet<Arc> Forest { get; }

		/// <summary>
		/// The cheapest spanning forest as a subgraph of the original graph.
		/// </summary>
		public Subgraph ForestGraph { get; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="graph"></param>
		/// <param name="cost"></param>
		public Prim(
            IGraph graph, 
            Func<Arc, TCost> cost)
		{
			Graph = graph;
			Cost = cost;
			Forest = new();
			ForestGraph = new(graph);
			ForestGraph.EnableAllArcs(false);

			Run();
		}

		/// <summary>
		/// Initialize <see cref="Prim{TCost}"/>.
		/// </summary>
		/// <param name="graph"><see cref="Graph"/>.</param>
		/// <param name="cost"><see cref="Cost"/>.</param>
		public Prim(
            IGraph graph, 
            Dictionary<Arc, TCost> cost)
			    : this(graph, arc => cost[arc])
		{
		}

		private void Run()
		{
			Forest.Clear();
			var priorityQueue = new Core.Collections.PriorityQueue<Node, TCost>();
			var processed = new HashSet<Node>();
			var parentArc = new Dictionary<Node, Arc>();

			// start with one point from each component
			var components = new ConnectedComponents(Graph, ConnectedComponentsFlags.CreateComponents);
			foreach (var c in components.Components ?? new())
			{
				var root = c.First();
				processed.Add(root);
				foreach (var arc in Graph.Arcs(root))
				{
					var v = Graph.Other(arc, root);
					parentArc[v] = arc;
					priorityQueue[v] = Cost(arc);
				}
			}

            while (priorityQueue.Count != 0)
			{
				var n = priorityQueue.Peek();
				priorityQueue.Pop();
				processed.Add(n);
				var arcToAdd = parentArc[n];
				Forest.Add(arcToAdd);
				ForestGraph.Enable(arcToAdd, true);

				foreach (var arc in Graph.Arcs(n))
				{
					var v = Graph.Other(arc, n);
					if (processed.Contains(v)) continue;

					var arcCost = Cost(arc);
                    var vInPriorityQueue = priorityQueue.TryGetPriority(v, out var vCost);
					if (!vInPriorityQueue || arcCost.CompareTo(vCost) < 0)
					{
						priorityQueue[v] = arcCost;
						parentArc[v] = arc;
					}
				}
			}
		}
	}
}
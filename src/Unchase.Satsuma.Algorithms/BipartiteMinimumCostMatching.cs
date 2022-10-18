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
using Unchase.Satsuma.Adapters.Enums;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;

namespace Unchase.Satsuma.Algorithms
{
	/// <inheritdoc cref="BipartiteMinimumCostMatching{TNodeProperty, TArcProperty}"/>
	public sealed class BipartiteMinimumCostMatching :
        BipartiteMinimumCostMatching<object, object>
    {
		/// <summary>
		/// Initialize <see cref="BipartiteMinimumCostMatching"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		/// <param name="isRed">Describes a bipartition of Graph by dividing its nodes into red and blue ones.</param>
		/// <param name="cost">A finite cost function on the arcs of Graph.</param>
		/// <param name="minimumMatchingSize">Minimum constraint on the size (number of arcs) of the returned matching.</param>
		/// <param name="maximumMatchingSize">Maximum constraint on the size (number of arcs) of the returned matching.</param>
		public BipartiteMinimumCostMatching(
            IGraph graph,
            Func<Node, bool> isRed,
            Func<Arc, double> cost,
            int minimumMatchingSize = 0,
            int maximumMatchingSize = int.MaxValue)
		        : base(graph, isRed, cost, minimumMatchingSize, maximumMatchingSize)
        {
		}
    }

	/// <summary>
	/// Finds a minimum cost matching in a bipartite graph using the network simplex method.
	/// </summary>
	/// <remarks>
	/// See also <seealso cref="BipartiteMaximumMatching{TNodeProperty, TArcProperty}"/>.
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
	/// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public class BipartiteMinimumCostMatching<TNodeProperty, TArcProperty>
	{
        /// <summary>
		/// The input graph.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

		/// <summary>
		/// Describes a bipartition of <see cref="Graph"/> by dividing its nodes into red and blue ones.
		/// </summary>
		public Func<Node, bool> IsRed { get; }

		/// <summary>
		/// A finite cost function on the arcs of <see cref="Graph"/>.
		/// </summary>
		public Func<Arc, double> Cost { get; }

		/// <summary>
		/// Minimum constraint on the size (number of arcs) of the returned matching.
		/// </summary>
		public int MinimumMatchingSize { get; }

		/// <summary>
		/// Maximum constraint on the size (number of arcs) of the returned matching.
		/// </summary>
		public int MaximumMatchingSize { get; }

		/// <summary>
		/// The minimum cost matching, computed using the network simplex method.
		/// </summary>
		/// <remarks>
		/// Null if a matching of the specified size could not be found.
		/// </remarks>
		public IMatching<TNodeProperty, TArcProperty>? Matching { get; private set; }

		/// <summary>
		/// Initialize <see cref="BipartiteMinimumCostMatching{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
		/// <param name="isRed"><see cref="IsRed"/>.</param>
		/// <param name="cost"><see cref="Cost"/>.</param>
		/// <param name="minimumMatchingSize"><see cref="MinimumMatchingSize"/>.</param>
		/// <param name="maximumMatchingSize"><see cref="MaximumMatchingSize"/>.</param>
		public BipartiteMinimumCostMatching(
            IGraph<TNodeProperty, TArcProperty> graph,
            Func<Node, bool> isRed,
            Func<Arc, double> cost,
			int minimumMatchingSize = 0,
            int maximumMatchingSize = int.MaxValue)
		{
			Graph = graph;
			IsRed = isRed;
			Cost = cost;
			MinimumMatchingSize = minimumMatchingSize;
			MaximumMatchingSize = maximumMatchingSize;

			Run();
		}

		private void Run()
		{
			// direct all edges from the red nodes to the blue nodes
			var redToBlue = new RedirectedGraph<TNodeProperty, TArcProperty>(Graph,
				x => (IsRed(Graph.U(x)) ? ArcDirection.Forward : ArcDirection.Backward));

			// add a source and a target to the graph and some edges
			var flowGraph = new Supergraph<TNodeProperty, TArcProperty>(redToBlue);
			var source = flowGraph.AddNode();
			var target = flowGraph.AddNode();
			foreach (var node in Graph.Nodes())
            {
                if (IsRed(node))
                {
                    flowGraph.AddArc(source, node, Directedness.Directed);
                }
                else
                {
                    flowGraph.AddArc(node, target, Directedness.Directed);
                }
            }
			var reflow = flowGraph.AddArc(target, source, Directedness.Directed);

			// run the network simplex
			var ns = new NetworkSimplex<TNodeProperty, TArcProperty>(flowGraph,
				lowerBound: x => (x == reflow ? MinimumMatchingSize : 0),
				upperBound: x => (x == reflow ? MaximumMatchingSize : 1),
				cost: x => (Graph.HasArc(x) ? Cost(x) : 0));
			ns.Run();

			if (ns.State == SimplexState.Optimal)
			{
				var matching = new Matching<TNodeProperty, TArcProperty>(Graph);
				foreach (var arc in ns.UpperBoundArcs.Concat(ns.Forest.Where(kv => kv.Value == 1).Select(kv => kv.Key)))
                {
                    if (Graph.HasArc(arc))
                    {
                        matching.Enable(arc, true);
                    }
                }

				Matching = matching;
			}
		}
	}
}
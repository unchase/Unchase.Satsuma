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

namespace Unchase.Satsuma.TSP
{
    /// <summary>
	/// Attempts to find a (directed) Hamiltonian cycle in a graph using TSP solvers.
	/// </summary>
	/// <remarks>
	/// <para>Edges can be traversed in both directions.</para>
	/// <para>If no Hamiltonian cycle is found by this class, that does not prove the nonexistence thereof.</para>
	/// <para>However, there are some easy graph properties which prohibit the existence of a Hamiltonian cycle.</para>
	/// <para>
	/// Namely, if a graph is not 2-connected (see Connectivity.BiNodeConnectedComponents), 
    /// then it cannot contain a Hamiltonian cycle.
	/// </para>
	/// </remarks>
	public sealed class HamiltonianCycle
	{
        /// <summary>
		/// The input graph
		/// </summary>
		public IGraph Graph { get; }

		/// <summary>
		/// A Hamiltonian cycle in the input graph, or null if none has been found.
		/// </summary>
		/// <remarks>
		/// <remarks>
		/// <para>The returned path is a cycle, that is, its start and end nodes are always equal.</para>
		/// <para>The existence of a Hamiltonian cycle does not guarantee that this class finds it.</para>
		/// </remarks>
		/// </remarks>
		public IPath? Cycle { get; private set; }

		/// <summary>
		/// Initialize <see cref="HamiltonianCycle"/>.
		/// </summary>
		/// <param name="graph"><see cref="Graph"/>.</param>
		public HamiltonianCycle(IGraph graph)
		{
			Graph = graph;
			Cycle = null;

			Run();
		}

		private void Run()
		{
            double Cost(Node u, Node v) => (Graph.Arcs(u, v, ArcFilter.Forward).Any() ? 1 : 10);
            IEnumerable<Node>? tour = null;
			double minimumTourCost = Graph.NodeCount();

			// Use the insertion tsp combined with 2-OPT heuristic.
			var insertionTsp = new InsertionTsp<Node>(Graph.Nodes(), Cost);
			insertionTsp.Run();
            if (insertionTsp.TourCost.Equals(minimumTourCost))
            {
                tour = insertionTsp.Tour;
            }
			else
			{
				var opt2Tsp = new Opt2Tsp<Node>(Cost, insertionTsp.Tour, insertionTsp.TourCost);
				opt2Tsp.Run();
                if (opt2Tsp.TourCost.Equals(minimumTourCost))
                {
                    tour = opt2Tsp.Tour;
                }
			}

			// convert the tour (node sequence) into a path (arc sequence connecting two nodes)
            if (tour == null)
            {
                Cycle = null;
            }
			else
			{
				var cycle = new Adapters.Path(Graph);
                var tourList = tour.ToList();
				if (tourList.Any())
				{
					var prev = Node.Invalid;
					foreach (var n in tourList)
					{
                        if (prev == Node.Invalid)
                        {
                            cycle.Begin(n);
                        }
                        else
                        {
                            cycle.AddLast(Graph.Arcs(prev, n, ArcFilter.Forward).First());
                        }

						prev = n;
					}

					cycle.AddLast(Graph.Arcs(prev, tourList.First(), ArcFilter.Forward).First());
				} // if tour is not empty

				Cycle = cycle;
			}
		}
	}
}
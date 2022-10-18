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
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Algorithms.SpanningForest
{
	/// <summary>
	/// Finds a minimum cost spanning forest in a graph using Kruskal's algorithm.
	/// </summary>
	/// <remarks>
	/// <para>Most suitable for sparse (i.e. everyday) graphs. For dense graphs, use Prim&lt;TCost&gt;.</para>
	/// <para>
	/// The algorithm starts with an empty forest, and gradually expands it with one arc at a time,
	/// taking the cheapest possible arc in each step.
	/// </para>
	/// <para>At the end of the algorithm, this yields a cheapest spanning forest.</para>
	/// <para>
	/// Running time: O(m log n), memory usage: O(m); 
	/// where n is the number of nodes and m is the number of arcs.
	/// </para>
	/// <para>This class also allows finding a cheapest forest containing some fixed arc set.</para>
	/// <para>
	/// Call <see cref="AddArc"/> several times at the beginning to set an initial forest which needs to be contained,
	/// then call <see cref="Run"/> to complete the forest.
	/// </para>
	/// <para>It can be proven that the found spanning forest is optimal among those which contain the given arc set.</para>
	/// <para>
	/// A maximum degree constraint can also be imposed on the spanning forest,
	/// and arbitrary arcs can be added to the forest at any time using <see cref="AddArc"/>.
	/// </para>
	/// <para>However, if using these features, the resulting forest may not be optimal.</para>
	/// <para>See <see cref="Prim{TCost, TNodeProperty, TArcProperty}"/> for a usage example.</para>
	/// </remarks>
	/// <typeparam name="TCost">The arc cost type.</typeparam>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
	/// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public sealed class Kruskal<TCost, TNodeProperty, TArcProperty>
		where TCost : IComparable<TCost>
	{
		/// <summary>
		/// The input graph.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

		/// <summary>
		/// An arbitrary function assigning costs to the arcs.
		/// </summary>
		public Func<Arc, TCost> Cost { get; }

		/// <summary>
		/// An optional per-node maximum degree constraint on the resulting spanning forest. Can be null.
		/// </summary>
		/// <remarks>
		/// The algorithm will most probably find a suboptimal solution if a maximum degree constraint is imposed,
        /// as the minimum cost Hamiltonian path problem can be formulated as a minimum cost spanning tree problem
        /// with maximum degree 2.
		/// </remarks>
		public Func<Node, int>? MaxDegree { get; }

		/// <summary>
		/// Contains the arcs of the current forest.
		/// </summary>
		/// <remarks>
		/// <para>The forest is empty at the beginning.</para>
		/// <para><see cref="Run"/> can be used to run the whole algorithm and make a cheapest spanning forest.</para>
		/// </remarks>
		public HashSet<Arc> Forest { get; }

		/// <summary>
		/// The current forest as a subgraph of the original graph.
		/// </summary>
		public Subgraph<TNodeProperty, TArcProperty> ForestGraph { get; }

		/// <summary>
		/// Contains the degree of a node in the found spanning forest.
		/// </summary>
		public Dictionary<Node, int> Degree { get; }

		/// <summary>
		/// Enumerates the arcs by cost increasing.
		/// </summary>
		private IEnumerator<Arc>? _arcEnumerator;
		private int _arcsToGo;

		/// <summary>
		/// The components of the current spanning forest.
		/// </summary>
		private readonly DisjointSet<Node> _components;

		/// <summary>
		/// Initialize <see cref="Kruskal{TCost, TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="Graph"/>.</param>
		/// <param name="cost"><see cref="Cost"/>.</param>
		/// <param name="maxDegree"><see cref="MaxDegree"/>.</param>
		public Kruskal(
            IGraph<TNodeProperty, TArcProperty> graph,
            Func<Arc, TCost> cost,
            Func<Node, int>? maxDegree = null)
		{
			Graph = graph;
			Cost = cost;
			MaxDegree = maxDegree;

			Forest = new();
			ForestGraph = new(graph);
			ForestGraph.EnableAllArcs(false);
			Degree = new();
			foreach (var node in Graph.Nodes()) Degree[node] = 0;

			var arcs = Graph.Arcs().ToList();
			arcs.Sort((a, b) => Cost(a).CompareTo(Cost(b)));
			_arcEnumerator = arcs.GetEnumerator();
			_arcsToGo = Graph.NodeCount() - new ConnectedComponents<TNodeProperty, TArcProperty>(Graph).Count;
			_components = new();
		}

		/// <summary>
		/// Initialize <seealso cref="Kruskal{TCost, TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="Graph"/>.</param>
		/// <param name="cost"><see cref="Cost"/>.</param>
		public Kruskal(
            IGraph<TNodeProperty, TArcProperty> graph, 
            Dictionary<Arc, TCost> cost)
			    : this(graph, arc => cost[arc])
		{
		}

		/// <summary>
		/// Performs a step in Kruskal's algorithm.
		/// </summary>
		/// <remarks>
		/// A step means trying to insert the next arc into the forest.
		/// </remarks>
		/// <returns>Returns true if the forest has not been completed with this step.</returns>
		public bool Step()
		{
			if (_arcsToGo <= 0 || _arcEnumerator == null || !_arcEnumerator.MoveNext())
			{
				_arcEnumerator = null;
				return false;
			}

			AddArc(_arcEnumerator.Current);
			return true;
		}

		/// <summary>
		/// Runs the algorithm and completes the current forest to a spanning forest.
		/// </summary>
		public void Run()
		{
            while (Step())
            {
            }
		}

		/// <summary>
		/// Tries to add the specified arc to the current forest.
		/// </summary>
		/// <remarks>
		/// An arc cannot be added if it would either create a cycle in the forest,
		/// or the maximum degree constraint would be violated with the addition.
		/// </remarks>
		/// <param name="arc">The arc.</param>
		/// <returns>Returns true if the arc could be added.</returns>
		public bool AddArc(Arc arc)
		{
			var u = Graph.U(arc);
            if (MaxDegree != null && Degree[u] >= MaxDegree(u))
            {
                return false;
            }

			var x = _components.WhereIs(u);

			var v = Graph.V(arc);
            if (MaxDegree != null && Degree[v] >= MaxDegree(v))
            {
                return false;
            }

			var y = _components.WhereIs(v);

			if (x == y) 
            {
                return false; // cycle
            }

			Forest.Add(arc);
			ForestGraph.Enable(arc, true);
			_components.Union(x, y);
			Degree[u]++;
			Degree[v]++;
			_arcsToGo--;

			return true;
		}
	}
}
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
   distribution.*/
#endregion

using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Extensions;

using Path = Unchase.Satsuma.Adapters.Path;

namespace Unchase.Satsuma.Algorithms
{
	/// <summary>
	/// Finds cheapest paths in a graph from a set of source nodes to all nodes, or a negative cycle reachable from the sources.
	/// </summary>
	/// <remarks>
	/// <para>Edges count as 2-cycles.</para>
	/// <para>
	/// There is no restriction on the cost function (as opposed to <see cref="AStar"/> and <see cref="Dijkstra"/>),
	/// but if a negative cycle is reachable from the sources, the algorithm terminates and
	/// does not calculate the distances.
	/// </para>
	/// <para>If the cost function is non-negative, use <see cref="Dijkstra"/>, as it runs faster.</para>
	/// <para>Querying the results:</para>
	/// <para>- If a negative cycle has been reached, then <see cref="NegativeCycle"/> is not null and contains such a cycle.</para>
	/// <para>  - In this case, <see cref="GetDistance"/>, <see cref="GetParentArc"/> and <see cref="GetPath"/> throw an exception.</para>
	/// <para>- If no negative cycle could be reached, then <see cref="NegativeCycle"/> is null.</para>
	/// <para>  - In this case, use <see cref="GetDistance"/>, <see cref="GetParentArc"/> and <see cref="GetPath"/> for querying the results.</para>
	/// <para>  - For unreachable nodes, <see cref="GetDistance"/>, <see cref="GetParentArc"/> and <see cref="GetPath"/> <see cref="double.PositiveInfinity"/>, <see cref="Arc.Invalid"/> and null respectively.</para>
	/// </remarks>
	public sealed class BellmanFord
	{
        /// <summary>
		/// The input graph.
		/// </summary>
		public IGraph Graph { get; }

		/// <summary>
		/// The arc cost function. Each value must be finite or positive infinity.
		/// </summary>
		/// <remarks>
		/// <see cref="double.PositiveInfinity"/> means that an arc is impassable.
		/// </remarks>
		public Func<Arc, double> Cost { get; }

		/// <summary>
		/// A negative cycle reachable from the sources, or null if none exists.
		/// </summary>
		public IPath? NegativeCycle { get; private set; }

		private const string NegativeCycleMessage = "A negative cycle was found.";
		private readonly Dictionary<Node, double> _distance;
		private readonly Dictionary<Node, Arc> _parentArc;

		/// <summary>
		/// Runs the Bellman-Ford algorithm.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		/// <param name="cost"><see cref="Cost"/>.</param>
		/// <param name="sources">The source nodes.</param>
		public BellmanFord(
            IGraph graph,
            Func<Arc, double> cost,
            IEnumerable<Node> sources)
		{
			Graph = graph;
			Cost = cost;

			_distance = new();
			_parentArc = new();

			foreach (var n in sources)
			{
				_distance[n] = 0;
				_parentArc[n] = Arc.Invalid;
			}

			Run();
		}

		private void Run()
		{
			for (var i = Graph.NodeCount(); i > 0; i--)
			{
				foreach (var arc in Graph.Arcs())
				{
					var u = Graph.U(arc);
					var v = Graph.V(arc);
					var du = GetDistance(u);
					var dv = GetDistance(v);
					var c = Cost(arc);

					if (Graph.IsEdge(arc))
					{
						if (du > dv)
						{
							(u, v) = (v, u);
                            (du, dv) = (dv, du);
                        }

						if (!double.IsPositiveInfinity(du) && c < 0)
						{
							var cycle = new Path(Graph);
							cycle.Begin(u);
							cycle.AddLast(arc);
							cycle.AddLast(arc);
							NegativeCycle = cycle;
							return;
						}
					}

					if (du + c < dv)
					{
						_distance[v] = du + c;
						_parentArc[v] = arc;

						if (i == 0)
						{
							var p = u;
                            for (var j = Graph.NodeCount() - 1; j > 0; j--)
                            {
                                p = Graph.Other(_parentArc[p], p);
                            }

							var cycle = new Path(Graph);
							cycle.Begin(p);
							var x = p;
							while (true)
							{
								var a = _parentArc[x];
								cycle.AddFirst(a);
								x = Graph.Other(a, x);
								if (x == p) break;
							}

							NegativeCycle = cycle;
							return;
						}
					}
				} // for all arcs
			} // for i
		}

		/// <summary>
		/// Returns whether a node has been reached.
		/// </summary>
		/// <param name="node">Node.</param>
        public bool Reached(Node node)
		{
			return _parentArc.ContainsKey(node);
		}

		/// <summary>
		/// Returns the reached nodes.
		/// </summary>
		public IEnumerable<Node> ReachedNodes => _parentArc.Keys;

        /// <summary>
		/// Gets the cost of the cheapest path from the source nodes to a given node.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Returns the distance, or <see cref="double.PositiveInfinity"/> if the node is unreachable from the source nodes.</returns>
		/// <exception cref="InvalidOperationException">A reachable negative cycle has been found (i.e. <see cref="NegativeCycle"/> is not null).</exception>
		public double GetDistance(Node node)
		{
            if (NegativeCycle != null)
            {
                throw new InvalidOperationException(NegativeCycleMessage);
            }

            return _distance.TryGetValue(node, out var result) 
                ? result 
                : double.PositiveInfinity;
		}

		/// <summary>
		/// Gets the arc connecting a node with its parent in the forest of cheapest paths.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Returns the arc, or <see cref="Arc.Invalid"/> if the node is a source or is unreachable.</returns>
		/// <exception cref="InvalidOperationException">Returns a reachable negative cycle has been found (i.e. <see cref="NegativeCycle"/> is not null).</exception>
		public Arc GetParentArc(Node node)
		{
            if (NegativeCycle != null)
            {
                throw new InvalidOperationException(NegativeCycleMessage);
            }

            return _parentArc.TryGetValue(node, out var result) 
                ? result 
                : Arc.Invalid;
		}

		/// <summary>
		/// Gets a cheapest path from the source nodes to a given node.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Returns a cheapest path, or null if the node is unreachable.</returns>
		/// <exception cref="InvalidOperationException">A reachable negative cycle has been found (i.e. <see cref="NegativeCycle"/> is not null).</exception>
		public IPath? GetPath(Node node)
		{
            if (NegativeCycle != null)
            {
                throw new InvalidOperationException(NegativeCycleMessage);
            }

            if (!Reached(node))
            {
                return null;
            }

			var result = new Path(Graph);
			result.Begin(node);
			while (true)
			{
				var arc = GetParentArc(node);
                if (arc == Arc.Invalid)
                {
                    break;
                }

				result.AddFirst(arc);
				node = Graph.Other(arc, node);
			}

			return result;
		}
	}
}
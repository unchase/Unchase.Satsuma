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

using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Algorithms
{
	/// <summary>
	/// Uses the A* search algorithm to find cheapest paths in a graph.
	/// </summary>
	/// <remarks>
	/// <para><see cref="AStar{TNodeProperty, TArcProperty}"/> is essentially <see cref="Dijkstra{TNodeProperty, TArcProperty}"/>'s algorithm with an optional heuristic which can speed up path search.</para>
	/// <para>Usage:</para>
	/// <para>- <see cref="AddSource"/> can be used to initialize the class by providing the source nodes.</para>
	/// <para>- Then <see cref="RunUntilReached(Node)"/> can be called to obtain cheapest paths to a target set.</para>
	/// <para>
	/// A target node is reached if a cheapest path leading to it is known.
	/// Unlike <see cref="Dijkstra{TNodeProperty, TArcProperty}"/>, A* does not use the notion of fixed nodes.
	/// </para>
	/// <para>Example (finding a shortest path between two nodes):</para>
	/// <para>
	/// <code>
	/// var g = new CompleteGraph(50);
	/// var pos = new Dictionary&lt;Node, double&gt;();
	/// var r = new Random();
	/// foreach (var node in g.Nodes())
	/// 	pos[node] = r.NextDouble();
	/// Node source = g.GetNode(0);
	/// Node target = g.GetNode(1);
	/// var astar = new AStar(g, arc => Math.Abs(pos[g.U(arc)] - pos[g.V(arc)]), node => Math.Abs(pos[node] - pos[target]));
	/// astar.AddSource(source);
	/// astar.RunUntilReached(target);
	/// Console.WriteLine("Distance of target from source: "+astar.GetDistance(target));
	/// </code>
	/// </para>
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public sealed class AStar<TNodeProperty, TArcProperty>
	{
        /// <summary>
		/// The input graph.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

		/// <summary>
		/// A non-negative arc cost function.
		/// </summary>
		public Func<Arc, double> Cost { get; }

		/// <summary>
		/// The A* heuristic function.
		/// </summary>
		/// <remarks>
		/// <para><see cref="Heuristic"/> must be a function that is</para>
		/// <para>- non-negative,</para>
		/// <para>- admissible: it must assign for each node a lower bound on the cost of the cheapest path from the given node to the target node set,</para>
		/// <para>- and consistent: for each uv arc, <tt>Heuristic(u) &lt;= Cost(uv) + Heuristic(v)</tt>.</para>
		/// <para>From the above it follows that <see cref="Heuristic"/> must return 0 for all target nodes.</para>
		/// <para>If <see cref="Heuristic"/> is the constant zero function, then the algorithm is equivalent to <see cref="Dijkstra{TNodeProperty, TArcProperty}"/>'s algorithm.</para>
		/// </remarks>
		public Func<Node, double> Heuristic { get; }

		private readonly Dijkstra<TNodeProperty, TArcProperty> _dijkstra;

		/// <summary>
		/// Initialize <see cref="AStar{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
		/// <param name="cost"><see cref="Cost"/>.</param>
		/// <param name="heuristic"><see cref="Heuristic"/>.</param>
		public AStar(
            IGraph<TNodeProperty, TArcProperty> graph, 
            Func<Arc, double> cost, 
            Func<Node, double> heuristic)
		{
			Graph = graph;
			Cost = cost;
			Heuristic = heuristic;

			_dijkstra = new(Graph, arc => Cost(arc) - Heuristic(Graph.U(arc)) + Heuristic(Graph.V(arc)), DijkstraMode.Sum);
		}

		private Node CheckTarget(Node node)
		{
            if (node != Node.Invalid && Heuristic(node) != 0)
            {
                throw new ArgumentException("Heuristic is nonzero for a target");
            }

			return node;
		}

		/// <summary>
		/// Adds a new source node.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <exception cref="InvalidOperationException">The node has already been reached.</exception>
		public void AddSource(Node node)
		{
			_dijkstra.AddSource(node, Heuristic(node));
		}

		/// <summary>
		/// Runs the algorithm until the given node is reached.
		/// </summary>
		/// <param name="target">The node to reach.</param>
		/// <returns>Returns target if it was successfully reached, or <see cref="Node.Invalid"/> if it is unreachable.</returns>
		/// <exception cref="ArgumentException"><see cref="Heuristic"/>(target) is not 0.</exception>
		public Node RunUntilReached(Node target)
		{
			return CheckTarget(_dijkstra.RunUntilFixed(target));
		}

		/// <summary>
		/// Runs the algorithm until a node satisfying the given condition is reached.
		/// </summary>
		/// <param name="isTarget">Condition.</param>
		/// <returns>Returns a target node if one was successfully reached, or <see cref="Node.Invalid"/> if all the targets are unreachable.</returns>
		/// <exception cref="ArgumentException"><see cref="Heuristic"/> is not 0 for the returned node.</exception>
		public Node RunUntilReached(Func<Node, bool> isTarget)
		{
			return CheckTarget(_dijkstra.RunUntilFixed(isTarget));
		}

		/// <summary>
		/// Gets the cost of the cheapest path from the source nodes to a given node (that is, its distance from the sources).
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Returns the distance, or <see cref="double.PositiveInfinity"/> if the node has not been reached yet.</returns>
		/// <exception cref="ArgumentException"><see cref="Heuristic"/> is not 0.</exception>
		public double GetDistance(Node node)
		{
			CheckTarget(node);

			return _dijkstra.Fixed(node) 
                ? _dijkstra.GetDistance(node) 
                : double.PositiveInfinity;
		}

		/// <summary>
		/// Gets a cheapest path from the source nodes to a given node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns>Returns a cheapest path, or null if the node has not been reached yet.</returns>
		/// <exception cref="ArgumentException"><see cref="Heuristic"/> is not 0.</exception>
		public IPath<TNodeProperty, TArcProperty>? GetPath(Node node)
		{
			CheckTarget(node);
            if (!_dijkstra.Fixed(node))
            {
                return null;
            }

			return _dijkstra.GetPath(node);
		}
	}
}
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

using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;

using Path = Unchase.Satsuma.Adapters.Path;

namespace Unchase.Satsuma.Algorithms
{
    /// <summary>
	/// Uses <see cref="Dijkstra"/>'s algorithm to find cheapest paths in a graph.
	/// </summary>
	/// <remarks>
	/// <para>See <see cref="DijkstraMode"/> for constraints on the cost function.</para>
	/// <para>Usage:</para>
	/// <para>- <see cref="AddSource(Node)"/> can be used to initialize the class by providing the source nodes.</para>
	/// <para>- Then <see cref="Run"/> or <see cref="RunUntilFixed(Node)"/> may be called to obtain a forest of cheapest paths to a given set of nodes.</para>
	/// <para>- Alternatively, <see cref="Step"/> can be called several times.</para>
	/// <para>The algorithm reaches and fixes nodes one after the other (see <see cref="Reached"/> and <see cref="Fixed"/>).</para>
	/// <para>Querying the results:</para>
	/// <para>- For fixed nodes, use <see cref="GetDistance"/>, <see cref="GetParentArc"/> and <see cref="GetPath"/>.</para>
	/// <para>- For reached but unfixed nodes, these methods return valid but not yet optimal values.</para>
	/// <para>- For currently unreached nodes, <see cref="GetDistance"/>, <see cref="GetParentArc"/> and <see cref="GetPath"/> return <see cref="double.PositiveInfinity"/>, <see cref="Arc.Invalid"/> and null respectively.</para>
	/// <para>Example (finding a shortest path between two nodes):</para>
	/// <para>
	/// <code>
	/// var g = new CompleteGraph(50);
    /// var pos = new Dictionary&lt;Node, double&gt;();
    /// var r = new Random();
    /// foreach (var node in g.Nodes())
    /// 	pos[node] = r.NextDouble();
    /// var dijkstra = new Dijkstra(g, arc =&gt; Math.Abs(pos[g.U(arc)] - pos[g.V(arc)]), DijkstraMode.Sum);
    /// Node a = g.GetNode(0), b = g.GetNode(1);
    /// dijkstra.AddSource(a);
    /// dijkstra.RunUntilFixed(b);
    /// Console.WriteLine("Distance of b from a: "+dijkstra.GetDistance(b));
	/// </code>
	/// </para>
	/// </remarks>
	public sealed class Dijkstra
	{
        /// <summary>
		/// The input graph.
		/// </summary>
		public IGraph Graph { get; }

		/// <summary>
		/// The arc cost function.
		/// </summary>
		/// <remarks>
		/// <para><see cref="double.PositiveInfinity"/> means that an arc is impassable.</para>
		/// <para>See <see cref="DijkstraMode"/> for restrictions on cost functions.</para>
		/// </remarks>
		public Func<Arc, double> Cost { get; }

		/// <summary>
		/// The path cost calculation mode.
		/// </summary>
		public DijkstraMode Mode { get; }

		/// <summary>
		/// The lowest possible cost value.
		/// </summary>
		/// <remarks>
		/// <para>- 0 if <see cref="Mode"/> == <see cref="DijkstraMode.Sum"/></para>
		/// <para>- <see cref="double.NegativeInfinity"/> if <see cref="Mode"/> == <see cref="DijkstraMode.Maximum"/></para>
		/// </remarks>
		public double NullCost { get; }

		private readonly Dictionary<Node, double> _distance;
		private readonly Dictionary<Node, Arc> _parentArc;
		private readonly Core.Collections.PriorityQueue<Node, double> _priorityQueue;

		/// <summary>
		/// Initialize <see cref="Dijkstra"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		/// <param name="cost"><see cref="Cost"/>.</param>
		/// <param name="mode"><see cref="Mode"/>.</param>
		public Dijkstra(
            IGraph graph,
            Func<Arc, double> cost,
            DijkstraMode mode)
		{
			Graph = graph;
			Cost = cost;
			Mode = mode;
			NullCost = (mode == DijkstraMode.Sum 
                ? 0 
                : double.NegativeInfinity);

			_distance = new();
			_parentArc = new();
			_priorityQueue = new();
		}

		private void ValidateCost(double c)
		{
            if (Mode == DijkstraMode.Sum && c < 0)
            {
                throw new InvalidOperationException("Invalid cost: " + c);
            }
		}

		/// <summary>
		/// Adds a new source node.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <exception cref="InvalidOperationException">The node has already been reached.</exception>
		public void AddSource(Node node)
		{
			AddSource(node, NullCost);
		}

		/// <summary>
		/// Adds a new source node and sets its initial distance to nodeCost.
		/// </summary>
		/// <remarks>
		/// <para>Use this method only if you know what you are doing.</para>
		/// <para>Equivalent to deleting all arcs entering node, and adding a new source node s with a new arc from s to node whose cost equals to nodeCost.</para>
        /// </remarks>
		/// <param name="node">Node.</param>
		/// <param name="nodeCost">Node cost.</param>
		/// <exception cref="InvalidOperationException">The node has already been reached, or nodeCost is invalid as an arc cost.</exception>
		public void AddSource(Node node, double nodeCost)
		{
            if (Reached(node))
            {
                throw new InvalidOperationException("Cannot add a reached node as a source.");
            }

			ValidateCost(nodeCost);

			_parentArc[node] = Arc.Invalid;
			_priorityQueue[node] = nodeCost;
		}

		/// <summary>
		/// Performs a step in the algorithm and fixes a node.
		/// </summary>
		/// <returns>Returns the newly fixed node, or <see cref="Node.Invalid"/> if there was no reached but unfixed node.</returns>
		public Node Step()
		{
			if (_priorityQueue.Count == 0) return Node.Invalid;

			// find the closest reached but unfixed node
            var min = _priorityQueue.Peek(out var minDist);
			_priorityQueue.Pop();

			if (double.IsPositiveInfinity(minDist)) return Node.Invalid;
			_distance[min] = minDist; // fix the node

			// modify keys for neighboring nodes in the priority queue
			foreach (var arc in Graph.Arcs(min, ArcFilter.Forward))
			{
				var other = Graph.Other(arc, min);
				if (Fixed(other)) 
                {
                    continue; // already processed
                }

				var arcCost = Cost(arc);
				ValidateCost(arcCost);
				var newDist = (Mode == DijkstraMode.Sum ? minDist + arcCost : Math.Max(minDist, arcCost));

                if (!_priorityQueue.TryGetPriority(other, out var oldDist))
                {
                    oldDist = double.PositiveInfinity;
                }

				if (newDist < oldDist)
				{
					_priorityQueue[other] = newDist;
					_parentArc[other] = arc;
				}
			}

			return min;
		}

		/// <summary>
		/// Runs the algorithm until all possible nodes are fixed.
		/// </summary>
		public void Run()
		{
            while (Step() != Node.Invalid)
            {
            }
		}

		/// <summary>
		/// Runs the algorithm until a specific target node is fixed.
		/// </summary>
		/// <remarks>
		/// <see cref="Fixed"/>.
		/// </remarks>
		/// <param name="target">The node to fix.</param>
		/// <returns>Returns target if it was successfully fixed, or Node.Invalid if it is unreachable.</returns>
		public Node RunUntilFixed(Node target)
		{
			if (Fixed(target)) 
            {
                return target; // already fixed
            }

			while (true)
			{
				var fixedNode = Step();
                if (fixedNode == Node.Invalid || fixedNode == target)
                {
                    return fixedNode;
                }
			}
		}

		/// <summary>
		/// Runs the algorithm until a node satisfying the given condition is fixed.
		/// </summary>
		/// <param name="isTarget">Condition.</param>
		/// <returns>Returns a target node if one was successfully fixed, or <see cref="Node.Invalid"/> if all the targets are unreachable.</returns>
		public Node RunUntilFixed(Func<Node, bool> isTarget)
		{
			var fixedNode = FixedNodes.FirstOrDefault(isTarget);
			if (fixedNode != Node.Invalid) 
            {
                return fixedNode; // already fixed
            }

			while (true)
			{
				fixedNode = Step();
                if (fixedNode == Node.Invalid || isTarget(fixedNode))
                {
                    return fixedNode;
                }
			}
		}

		/// <summary>
		/// Returns whether a node has been reached. See #Fixed for more information.
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
		/// Returns whether a node has been fixed.
		/// </summary>
		/// <remarks>
		/// <para>- A node is called reached if it belongs to the current forest of cheapest paths. (see <see cref="Reached"/>)</para>
		/// <para>- Each reached node is either a source, or has a parent arc. (see <see cref="GetParentArc"/>)</para>
		/// <para>- A node is called fixed if it is reached and its distance will not change in the future.</para>
		/// <para>- At the beginning, only the source nodes are reached and none are fixed. (see <see cref="AddSource(Node)"/>)</para>
		/// <para>- In each step, the algorithm fixes a node and reaches some (maybe zero) other nodes.</para>
		/// <para>- The algorithm terminates if there is no node which is reached but not fixed.</para>
		/// </remarks>
		/// <param name="node">Node.</param>
		public bool Fixed(Node node)
		{
			return _distance.ContainsKey(node);
		}

		/// <summary>
		/// Returns the fixed nodes.
		/// </summary>
		public IEnumerable<Node> FixedNodes => _distance.Keys;

		/// <summary>
		/// Gets the cost of the current cheapest path from the source nodes to a given node (that is, its distance from the sources).
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Returns the distance, or <see cref="double.PositiveInfinity"/> if the node has not been reached yet.</returns>
		public double GetDistance(Node node)
		{
            return _distance.TryGetValue(node, out var result) 
                ? result 
                : double.PositiveInfinity;
		}

		/// <summary>
		/// Gets the arc connecting a node with its parent in the current forest of cheapest paths.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Returns the arc, or Arc.Invalid if the node is a source or has not been reached yet.</returns>
		public Arc GetParentArc(Node node)
		{
            return _parentArc.TryGetValue(node, out var result) 
                ? result 
                : Arc.Invalid;
		}

		/// <summary>
		/// Gets the current cheapest path from the source nodes to a given node.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Returns a current cheapest path, or null if the node has not been reached yet.</returns>
		public IPath? GetPath(Node node)
		{
            if (!Reached(node))
            {
                return null;
            }

			var result = new Path(Graph);
			result.Begin(node);
			while (true)
			{
				var arc = GetParentArc(node);
				if (arc == Arc.Invalid) break;
				result.AddFirst(arc);
				node = Graph.Other(arc, node);
			}
			return result;
		}
	}
}
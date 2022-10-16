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
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;

using Path = Unchase.Satsuma.Adapters.Path;

namespace Unchase.Satsuma.Algorithms
{
	/// <summary>
	/// Performs a breadth-first search (BFS) to find shortest paths from a set of source nodes to all nodes.
	/// </summary>
	/// <remarks>
	/// <para>In other words, <see cref="Bfs"/> finds cheapest paths for the constant 1 cost function.</para>
	/// <para>The advantage of <see cref="Bfs"/> over <see cref="Dijkstra"/> is its faster execution.</para>
	/// <para>Usage:</para>
	/// <para>- <see cref="AddSource"/> can be used to initialize the class by providing the source nodes.</para>
	/// <para>- Then <see cref="Run"/> or <see cref="RunUntilReached(Node)"/> may be called to obtain a forest of shortest paths to a given set of nodes.</para>
	/// <para>- Alternatively, <see cref="Step"/> can be called several times.</para>
	/// <para>The algorithm reaches nodes one after the other (see <see cref="Reached"/> for definition).</para>
	/// <para>Querying the results:</para>
	/// <para>- For reached nodes, use <see cref="GetLevel"/>, <see cref="GetParentArc"/> and <see cref="GetPath"/>.</para>
	/// <para>- For currently unreached nodes, <see cref="GetLevel"/>, <see cref="GetParentArc"/> and <see cref="GetPath"/> return -1, <see cref="Arc.Invalid"/> and null respectively.</para>
	/// </remarks>
	public sealed class Bfs
	{
        /// <summary>
		/// The input graph.
		/// </summary>
		public IGraph Graph { get; }

		private readonly Dictionary<Node, Arc> _parentArc;
		private readonly Dictionary<Node, int> _level;
		private readonly Queue<Node> _queue;

		/// <summary>
		/// Initialize <see cref="Bfs"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		public Bfs(IGraph graph)
		{
			Graph = graph;

			_parentArc = new();
			_level = new();
			_queue = new();
		}

		/// <summary>
		/// Adds a new source node.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <exception cref="InvalidOperationException">The node has already been reached.</exception>
		public void AddSource(Node node)
		{
            if (Reached(node))
            {
                return;
            }

			_parentArc[node] = Arc.Invalid;
			_level[node] = 0;
			_queue.Enqueue(node);
		}

		/// <summary>
		/// Performs an iteration which involves dequeueing a node.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The unreached neighbors of the dequeued node are enqueued, 
		/// and isTarget (which can be null) is called for each of them
		/// to find out if they belong to the target node set.
		/// </para>
		/// <para>If a target node is found among them, then the function returns immediately.</para>
		/// </remarks>
		/// <param name="isTarget">Returns true for target nodes. Can be null.</param>
		/// <param name="reachedTargetNode">The target node that has been newly reached, or <see cref="Node.Invalid"/>.</param>
		/// <returns>Returns true if no target node has been reached in this step, and there is at least one yet unreached node.</returns>
		public bool Step(Func<Node, bool>? isTarget, out Node reachedTargetNode)
		{
			reachedTargetNode = Node.Invalid;
            if (_queue.Count == 0)
            {
                return false;
            }

			var node = _queue.Dequeue();
			var d = _level[node] + 1;
			foreach (var arc in Graph.Arcs(node, ArcFilter.Forward))
			{
				var child = Graph.Other(arc, node);
                if (_parentArc.ContainsKey(child))
                {
                    continue;
                }

				_queue.Enqueue(child);
				_level[child] = d;
				_parentArc[child] = arc;

				if (isTarget != null && isTarget(child))
				{
					reachedTargetNode = child;
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Runs the algorithm until finished.
		/// </summary>
		public void Run()
		{
			Node dummy;
            while (Step(null, out dummy))
            {
            }
		}

		/// <summary>
		/// Runs the algorithm until a specific target node is reached.
		/// </summary>
		/// <param name="target">The node to reach.</param>
		/// <returns>Returns target if it was successfully reached, or <see cref="Node.Invalid"/>.</returns>
		public Node RunUntilReached(Node target)
		{
			if (Reached(target)) 
            {
                return target; // already reached
            }

			Node reachedTargetNode;
            while (Step(node => node == target, out reachedTargetNode))
            {
            }

			return reachedTargetNode;
		}

		/// <summary>
		/// Runs the algorithm until a node satisfying the given condition is reached.
		/// </summary>
		/// <param name="isTarget">Condition.</param>
		/// <returns>Returns a target node if one was successfully reached, or Node.Invalid if it is unreachable.</returns>
		public Node RunUntilReached(Func<Node, bool> isTarget)
		{
			var reachedTargetNode = ReachedNodes.FirstOrDefault(isTarget);
			if (reachedTargetNode != Node.Invalid) 
            {
                return reachedTargetNode; // already reached
            }

            while (Step(isTarget, out reachedTargetNode))
            {
            }

			return reachedTargetNode;
		}

		/// <summary>
		/// Returns whether a node has been reached.
		/// </summary>
		/// <remarks>
		/// <para>- A node is called reached if it belongs to the current Bfs forest.</para>
		/// <para>- Each reached node is either a source, or has a parent arc. (see <see cref="GetParentArc"/>).</para>
		/// <para>- At the beginning, only the source nodes are reached. (see <see cref="AddSource"/>).</para>
		/// </remarks>
		/// <param name="x">Node.</param>
		/// <returns>Returns whether a node has been reached.</returns>
		public bool Reached(Node x)
		{
			return _parentArc.ContainsKey(x);
		}

		/// <summary>
		/// Returns the reached nodes.
		/// </summary>
		/// <remarks>
		/// <seealso cref="Reached"/>
		/// </remarks>
		public IEnumerable<Node> ReachedNodes => _parentArc.Keys;

        /// <summary>
		/// Gets the current distance from the set of source nodes (that is, its level in the Bfs forest).
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Returns the distance, or -1 if the node has not been reached yet.</returns>
		public int GetLevel(Node node)
		{
            return _level.TryGetValue(node, out var result) 
                ? result 
                : -1;
		}

		/// <summary>
		/// Gets the arc connecting a node with its parent in the Bfs forest.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Returns the arc, or <see cref="Arc.Invalid"/> if the node is a source or has not been reached yet.</returns>
		public Arc GetParentArc(Node node)
		{
            return _parentArc.TryGetValue(node, out var result) 
                ? result 
                : Arc.Invalid;
		}

		/// <summary>
		/// Gets a shortest path from the sources to a node.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Returns a shortest path, or null if the node has not been reached yet.</returns>
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
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

using Unchase.Satsuma.TSP.Contracts;
using Unchase.Satsuma.TSP.Enums;

namespace Unchase.Satsuma.TSP
{
    /// <summary>
	/// Solves the "traveling salesman problem" by using the insertion heuristic.
	/// </summary>
	/// <remarks>
	/// <para>It starts from a small tour and gradually extends it by repeatedly choosing a yet unvisited node.</para>
	/// <para>The selected node is then inserted into the tour at the optimal place.</para>
	/// <para>Running time: O(n<sup>2</sup>).</para>
	/// </remarks>
	/// <typeparam name="TNode">The node type.</typeparam>
	public sealed class InsertionTsp<TNode> : 
        ITsp<TNode>
		    where TNode : IEquatable<TNode>
	{
        /// <summary>
		/// The nodes the salesman has to visit.
		/// </summary>
		public IEnumerable<TNode> Nodes { get; }

		/// <summary>
		/// A finite cost function on the node pairs.
		/// </summary>
		public Func<TNode, TNode, double> Cost { get; }

		/// <summary>
		/// The method of selecting new nodes for insertion.
		/// </summary>
		public TspSelectionRule SelectionRule { get; }

		private readonly LinkedList<TNode> _tour;

		/// <summary>
		/// A dictionary mapping each tour node to its containing linked list node.
		/// </summary>
		public Dictionary<TNode, LinkedListNode<TNode>> TourNodes { get; }

		/// <summary>
		/// The non-tour nodes.
		/// </summary>
		private readonly HashSet<TNode> _insertableNodes;

		/// <summary>
		/// The non-tour nodes in insertion order.
		/// </summary>
		private readonly Core.Collections.PriorityQueue<TNode, double> _insertableNodeQueue;

		/// <summary>
		/// Returns the nodes present in the current tour in visiting order.
		/// </summary>
		/// <remarks>
		/// The current tour contains only a subset of the nodes in the middle of the execution of the algorithm, 
		/// since the insertion TSP algorithm works by gradually extending a small tour.
		/// </remarks>
		public IEnumerable<TNode> Tour => _tour;

        /// <inheritdoc />
		public double TourCost { get; private set; }

		/// <summary>
		/// Initialize <see cref="InsertionTsp{TNode}"/>.
		/// </summary>
		/// <param name="nodes"><see cref="Nodes"/>.</param>
		/// <param name="cost"><see cref="Cost"/>.</param>
		/// <param name="selectionRule"><see cref="SelectionRule"/>.</param>
		public InsertionTsp(
            IEnumerable<TNode> nodes,
            Func<TNode, TNode, double> cost,
			TspSelectionRule selectionRule = TspSelectionRule.Farthest)
		{
			Nodes = nodes;
			Cost = cost;
			SelectionRule = selectionRule;

			_tour = new();
			TourNodes = new();
			_insertableNodes = new();
			_insertableNodeQueue = new();

			Clear();
		}

		private double PriorityFromCost(double c)
        {
            return SelectionRule switch
            {
                TspSelectionRule.Farthest => -c,
                _ => c
            };
        }

		/// <summary>
		/// Reverts the tour to a one-node tour, or a null tour if no node is available.
		/// </summary>
		public void Clear()
		{
			_tour.Clear();
			TourCost = 0;
			TourNodes.Clear();
			_insertableNodes.Clear();
			_insertableNodeQueue.Clear();

			if (Nodes.Any())
			{
				var startNode = Nodes.First();
				_tour.AddFirst(startNode);
				TourNodes[startNode] = _tour.AddFirst(startNode);
				foreach (var node in Nodes)
                {
                    if (!node.Equals(startNode))
					{
						_insertableNodes.Add(node);
						_insertableNodeQueue[node] = PriorityFromCost(Cost(startNode, node));
					}
                }
			}
		}

		/// <summary>
		/// Inserts a given node into the current tour at the optimal place.
		/// </summary>
		/// <param name="node"></param>
		/// <returns>Returns true if the node was inserted, false if it was already in the tour.</returns>
		public bool Insert(TNode node)
		{
			if (!_insertableNodes.Contains(node)) return false;
			_insertableNodes.Remove(node);
			_insertableNodeQueue.Remove(node);

			// find the optimal insertion place
			LinkedListNode<TNode>? insertAfter = null;
			var bestIncrease = double.PositiveInfinity;
			for (var llnode = _tour.First; llnode != _tour.Last; llnode = llnode.Next)
			{
				var llnode2 = llnode!.Next;
				var increase = Cost(llnode.Value, node) + Cost(node, llnode2!.Value);
                if (!llnode.Value.Equals(llnode2.Value))
                {
                    increase -= Cost(llnode.Value, llnode2.Value);
                }

				if (increase < bestIncrease)
				{
					bestIncrease = increase;
					insertAfter = llnode;
				}
			}

			TourNodes[node] = _tour.AddAfter(insertAfter!, node);
			TourCost += bestIncrease;

			// update distances
			foreach (var n in _insertableNodes)
			{
				var newPriority = PriorityFromCost(Cost(node, n));
				if (newPriority < _insertableNodeQueue[n]) _insertableNodeQueue[n] = newPriority;
			}

			return true;
		}

		/// <summary>
		/// Inserts a new node into the tour according to <see cref="SelectionRule"/>.
		/// </summary>
		/// <returns>Returns true if a new node was inserted, or false if the tour was already full.</returns>
		public bool Insert()
		{
            if (_insertableNodes.Count == 0)
            {
                return false;
            }

			Insert(_insertableNodeQueue.Peek());
			return true;
		}

		/// <summary>
		/// Completes the tour.
		/// </summary>
		public void Run()
		{
            while (Insert())
            {
            }
		}
	}
}
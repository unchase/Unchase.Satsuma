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
using Unchase.Satsuma.Algorithms.Abstractions;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Algorithms.Extensions;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Algorithms
{
	/// <inheritdoc cref="NetworkSimplex{TNodeProperty, TArcProperty}"/>
	public sealed class NetworkSimplex :
        NetworkSimplex<object, object>
    {
		/// <summary>
		/// Initialize <see cref="NetworkSimplex"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		/// <param name="lowerBound">The lower bound for the circulation.</param>
		/// <param name="upperBound">The upper bound for the circulation.</param>
		/// <param name="supply">The desired difference of outgoing and incoming flow for a node. Must be finite.</param>
		/// <param name="cost">The cost of sending a unit of circulation through an arc. Must be finite.</param>
		public NetworkSimplex(
            IGraph graph,
            Func<Arc, long>? lowerBound = null,
            Func<Arc, long>? upperBound = null,
            Func<Node, long>? supply = null,
            Func<Arc, double>? cost = null)
                : base(graph, lowerBound, upperBound, supply, cost)
        {
		}
    }

	/// <summary>
	/// Finds a minimum cost feasible circulation using the network simplex method.
	/// </summary>
	/// <remarks>
	/// <para>Lower/upper bounds and supply must be integral, but cost can be double.</para>
	/// <para>
	/// Edges are treated as directed arcs, but this is not a real restriction 
	/// if, for all edges, lower bound + upper bound = 0.
	/// </para>
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
	/// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public class NetworkSimplex<TNodeProperty, TArcProperty> : 
        IClearable
	{
		/// <summary>
		/// The input graph.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

		/// <summary>
		/// The lower bound for the circulation.
		/// </summary>
		/// <remarks>
		/// <para><see cref="long.MinValue"/> means negative infinity (unbounded).</para>
		/// <para>If null is supplied in the constructor, then the constant 0 function is taken.</para>
		/// </remarks>
		public Func<Arc, long>? LowerBound { get; }

		/// <summary>
		/// The upper bound for the circulation.
		/// </summary>
		/// <remarks>
		/// <para>Must be greater or equal to the lower bound.</para>
		/// <para><see cref="long.MaxValue"/> means positive infinity (unbounded).</para>
		/// <para>If null is supplied in the constructor, then the constant <see cref="long.MaxValue"/> function is taken.</para>
		/// </remarks>
		public Func<Arc, long>? UpperBound { get; }

		/// <summary>
		/// The desired difference of outgoing and incoming flow for a node. Must be finite.
		/// </summary>
		/// <remarks>
		/// <para>The sum must be zero for each graph component.</para>
		/// <para>If null is supplied in the constructor, then the constant 0 function is taken.</para>
		/// </remarks>
		public Func<Node, long>? Supply { get; }

		/// <summary>
		/// The cost of sending a unit of circulation through an arc. Must be finite.
		/// </summary>
		/// <remarks>
		/// If null is supplied in the constructor, then the constant 1.0 function is taken.
		/// </remarks>
		public Func<Arc, double>? Cost { get; }

		private readonly double _epsilon;

		// *** Current state
		// This is the graph augmented with a node and artificial arcs 
		private Supergraph<TNodeProperty, TArcProperty> _myGraph;
		private Node _artificialNode;
		private HashSet<Arc> _artificialArcs = new();

		// During execution, the network simplex method maintains a basis.
		// This consists of:
		// - a spanning tree
		// - a partition of the non-tree arcs into empty and saturated arcs
		// ** Primal vector
		private Dictionary<Arc, long> _tree = new();
		private Subgraph<TNodeProperty, TArcProperty> _treeSubgraph;
		private HashSet<Arc> _saturated = new();

		// ** Dual vector
		private Dictionary<Node, double> _potential = new();

		// An enumerator for finding an entering arc
		private IEnumerator<Arc> _enteringArcEnumerator;

		/// The current execution state of the simplex algorithm.
		public SimplexState State { get; private set; }

		/// <summary>
		/// Initialize <see cref="NetworkSimplex{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
		/// <param name="lowerBound"><see cref="LowerBound"/>.</param>
		/// <param name="upperBound"><see cref="UpperBound"/>.</param>
		/// <param name="supply"><see cref="Supply"/>.</param>
		/// <param name="cost"><see cref="Cost"/>.</param>
		public NetworkSimplex(
            IGraph<TNodeProperty, TArcProperty> graph,
			Func<Arc, long>? lowerBound = null, 
            Func<Arc, long>? upperBound = null,
			Func<Node, long>? supply = null, 
            Func<Arc, double>? cost = null)
		{
			Graph = graph;
            _myGraph = new(Graph);
			LowerBound = lowerBound ?? (_ => 0);
			UpperBound = upperBound ?? (_ => long.MaxValue);
			Supply = supply ?? (_ => 0);
			Cost = cost ?? (_ => 1);

			_epsilon = 1;
			foreach (var arc in graph.Arcs())
			{
				var x = Math.Abs(Cost(arc));
				if (x > 0 && x < _epsilon) _epsilon = x;
			}
			_epsilon *= 1e-12;

            _enteringArcEnumerator = _myGraph.Arcs().GetEnumerator();
            _treeSubgraph = new(_myGraph);

			Clear();
		}

		/// <summary>
		/// Returns the amount currently circulating on an arc.
		/// </summary>
		/// <param name="arc">The arc.</param>
        public long Flow(Arc arc)
		{
            if (_saturated.Contains(arc) && UpperBound != null)
            {
                return UpperBound(arc);
            }

            if (_tree.TryGetValue(arc, out var result))
            {
                return result;
            }

            if (LowerBound == null)
            {
                return 0;
            }

			result = LowerBound(arc);
			return result == long.MinValue 
                ? 0 
                : result;
		}

		/// <summary>
		/// Returns those arcs which belong to the basic forest.
		/// </summary>
		public IEnumerable<KeyValuePair<Arc, long>> Forest
		{
			get
			{
				return _tree.Where(kv => Graph.HasArc(kv.Key));
			}
		}

		/// <summary>
		/// Returns those arcs which are saturated (the flow equals to the upper bound),
        /// but are not in the basic forest.
		/// </summary>
		public IEnumerable<Arc> UpperBoundArcs => _saturated;

		/// <summary>
		/// Reverts the solver to its initial state.
		/// </summary>
		public void Clear()
		{
			var excess = new Dictionary<Node, long>();
            foreach (var n in Graph.Nodes())
            {
                if (Supply != null)
                {
                    excess[n] = Supply(n);
				}
            }

			_saturated = new();
			foreach (var arc in Graph.Arcs())
			{
                LowerBound?.Invoke(arc);

                var g = UpperBound?.Invoke(arc);
                if (g < long.MaxValue)
                {
                    _saturated.Add(arc);
                }

                var flow = Flow(arc);
				excess[Graph.U(arc)] -= flow;
				excess[Graph.V(arc)] += flow;
			}

			_potential = new();
			_myGraph = new(Graph);
			_artificialNode = _myGraph.AddNode();
			_potential[_artificialNode] = 0;
			_artificialArcs = new();
			var artificialArcOf = new Dictionary<Node, Arc>();
			foreach (var n in Graph.Nodes())
			{
				var e = excess[n];
				var arc = e > 0 
                    ? _myGraph.AddArc(n, _artificialNode, Directedness.Directed) 
                    : _myGraph.AddArc(_artificialNode, n, Directedness.Directed);
				_potential[n] = (e > 0 ? -1 : 1);
				_artificialArcs.Add(arc);
				artificialArcOf[n] = arc;
			}

			_tree = new();
			_treeSubgraph = new(_myGraph);
			_treeSubgraph.EnableAllArcs(false);
			foreach (var kv in artificialArcOf)
			{
				_tree[kv.Value] = Math.Abs(excess[kv.Key]);
				_treeSubgraph.Enable(kv.Value, true);
			}

			State = SimplexState.FirstPhase;
			_enteringArcEnumerator = _myGraph.Arcs().GetEnumerator();
			_enteringArcEnumerator.MoveNext();
		}

		private long ActualLowerBound(Arc arc)
		{
			return _artificialArcs.Contains(arc) ? 0 : LowerBound?.Invoke(arc) ?? 0;
		}

		private long ActualUpperBound(Arc arc)
		{
			return _artificialArcs.Contains(arc) ?
				(State == SimplexState.FirstPhase ? long.MaxValue : 0) : UpperBound?.Invoke(arc) ?? 0;
		}

		private double ActualCost(Arc arc)
		{
			return _artificialArcs.Contains(arc) 
                ? 1 
                : (State == SimplexState.FirstPhase ? 0 : Cost?.Invoke(arc) ?? 0);
		}

		/// <summary>
		/// Recalculates the potential at the beginning of the second phase.
		/// </summary>
		private class RecalculatePotentialDfs : 
            Dfs<TNodeProperty, TArcProperty>
		{
			/// <summary>
			/// Parent.
			/// </summary>
            private readonly NetworkSimplex<TNodeProperty, TArcProperty> _parent;

			/// <summary>
			/// Initialize <see cref="RecalculatePotentialDfs"/>.
			/// </summary>
			/// <param name="parent">Parent <see cref="NetworkSimplex{TNodeProperty, TArcProperty}"/>.</param>
			/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
			public RecalculatePotentialDfs(
                NetworkSimplex<TNodeProperty, TArcProperty> parent,
                IGraph<TNodeProperty, TArcProperty> graph)
                    : base(graph)
            {
                _parent = parent;
            }

			/// <inheritdoc />
			protected override void Start(out Direction direction)
			{
				direction = Direction.Undirected;
			}

            /// <inheritdoc />
			protected override bool NodeEnter(Node node, Arc arc)
			{
                if (arc == Arc.Invalid)
                {
                    _parent._potential[node] = 0;
                }
				else
				{
					var other = _parent._myGraph.Other(arc, node);

					_parent._potential[node] = _parent._potential[other] +
						(node == _parent._myGraph.V(arc) 
                            ? _parent.ActualCost(arc) 
                            : -_parent.ActualCost(arc));
				}

				return true;
			}
		}

		private class UpdatePotentialDfs : 
            Dfs<TNodeProperty, TArcProperty>
        {
            /// <summary>
            /// Parent.
            /// </summary>
            private readonly NetworkSimplex<TNodeProperty, TArcProperty> _parent;

			/// <summary>
			/// Diff.
			/// </summary>
			public double Diff;

			/// <summary>
			/// Initialize <see cref="UpdatePotentialDfs"/>.
			/// </summary>
			/// <param name="parent">Parent <see cref="NetworkSimplex{TNodeProperty, TArcProperty}"/>.</param>
			/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
			public UpdatePotentialDfs(
                NetworkSimplex<TNodeProperty, TArcProperty> parent,
                IGraph<TNodeProperty, TArcProperty> graph)
			        : base(graph)
            {
                _parent = parent;
            }

			/// <inheritdoc />
			protected override void Start(out Direction direction)
			{
				direction = Direction.Undirected;
			}

            /// <inheritdoc />
			protected override bool NodeEnter(Node node, Arc arc)
			{
				_parent._potential[node] += Diff;
				return true;
			}
		}

		/// <summary>
		/// Returns a-b for two longs (a > b). long.MaxValue/long.MinValue is taken for positive/negative infinity.
		/// </summary>
		/// <param name="a">The first value.</param>
		/// <param name="b">The second value.</param>
        private static ulong MySubtract(long a, long b)
		{
            if (a == long.MaxValue || b == long.MinValue)
            {
                return ulong.MaxValue;
            }

			return (ulong)(a - b);
		}

		/// <summary>
		/// Performs an iteration in the simplex algorithm.
		/// </summary>
		/// <remarks>
		/// Modifies the State field according to what happened.
		/// </remarks>
		public void Step()
		{
            if (State != SimplexState.FirstPhase && State != SimplexState.SecondPhase)
            {
                return;
            }

			// calculate reduced costs and find an entering arc
			var firstArc = _enteringArcEnumerator.Current;
			var enteringArc = Arc.Invalid;
			var enteringReducedCost = double.NaN;
			var enteringSaturated = false;
			while (true)
			{
				var arc = _enteringArcEnumerator.Current;
				if (!_tree.ContainsKey(arc))
				{
					var saturated = _saturated.Contains(arc);
					var reducedCost = ActualCost(arc) - (_potential[_myGraph.V(arc)] - _potential[_myGraph.U(arc)]);
					if ((reducedCost < -_epsilon && !saturated) ||
						(reducedCost > _epsilon && (saturated || ActualLowerBound(arc) == long.MinValue)))
					{
						enteringArc = arc;
						enteringReducedCost = reducedCost;
						enteringSaturated = saturated;
						break;
					}
				}

				// iterate
				if (!_enteringArcEnumerator.MoveNext())
				{
					_enteringArcEnumerator = _myGraph.Arcs().GetEnumerator();
					_enteringArcEnumerator.MoveNext();
				}

                if (_enteringArcEnumerator.Current == firstArc)
                {
                    break;
                }
			}

			if (enteringArc == Arc.Invalid)
			{
				if (State == SimplexState.FirstPhase)
				{
					State = SimplexState.SecondPhase;
					foreach (var arc in _artificialArcs) 
                    {
                        if (Flow(arc) > 0)
					    {
						    State = SimplexState.Infeasible;
						    break;
					    }
                    }

                    if (State == SimplexState.SecondPhase)
                    {
                        new RecalculatePotentialDfs(this, _treeSubgraph).Run();
                    }
				}
                else
                {
                    State = SimplexState.Optimal;
                }

				return;
			}

			// find the basic circle of the arc: this consists of forward and reverse arcs
			Node u = _myGraph.U(enteringArc), v = _myGraph.V(enteringArc);
			var forwardArcs = new List<Arc>();
			var backwardArcs = new List<Arc>();
			var pathU = _treeSubgraph.FindPath(v, u, Direction.Undirected);
			foreach (var n in pathU?.Nodes() ?? Array.Empty<Node>())
			{
				var arc = pathU!.NextArc(n);
				(_myGraph.U(arc) == n ? forwardArcs : backwardArcs).Add(arc);
			}

			// calculate flow modification delta and exiting arc/root
			var delta = enteringReducedCost < 0 
                ? MySubtract(ActualUpperBound(enteringArc), Flow(enteringArc)) 
                : MySubtract(Flow(enteringArc), ActualLowerBound(enteringArc));
			var exitingArc = enteringArc;
			var exitingSaturated = !enteringSaturated;
			foreach (var arc in forwardArcs)
			{
				var q = enteringReducedCost < 0 
                    ? MySubtract(ActualUpperBound(arc), _tree[arc]) 
                    : MySubtract(_tree[arc], ActualLowerBound(arc));

                if (q < delta)
                {
                    delta = q; exitingArc = arc;
                    exitingSaturated = enteringReducedCost < 0;
                }
			}
			foreach (var arc in backwardArcs)
			{
				var q = enteringReducedCost > 0 
                    ? MySubtract(ActualUpperBound(arc), _tree[arc]) 
                    : MySubtract(_tree[arc], ActualLowerBound(arc));

                if (q < delta)
                {
                    delta = q; exitingArc = arc;
                    exitingSaturated = enteringReducedCost > 0;
                }
			}

			// modify the primal solution along the circle
			long signedDelta = 0;
			if (delta != 0)
			{
                if (delta == ulong.MaxValue)
                {
                    State = SimplexState.Unbounded; 
                    return;
                }

				signedDelta = enteringReducedCost < 0 
                    ? (long)delta 
                    : -(long)delta;

                foreach (var arc in forwardArcs)
                {
                    _tree[arc] += signedDelta;
                }

                foreach (var arc in backwardArcs)
                {
                    _tree[arc] -= signedDelta;
                }
			}

			// modify the basis
			if (exitingArc == enteringArc)
			{
                if (enteringSaturated)
                {
                    _saturated.Remove(enteringArc);
                }
                else
                {
                    _saturated.Add(enteringArc);
                }
			}
			else
			{
				// remove exiting arc/root
				_tree.Remove(exitingArc);
				_treeSubgraph.Enable(exitingArc, false);
                if (exitingSaturated)
                {
                    _saturated.Add(exitingArc);
                }

				// modify the dual solution along a cut
				var diff = ActualCost(enteringArc) - (_potential[v] - _potential[u]);
				if (diff != 0) 
                {
                    new UpdatePotentialDfs(this, _treeSubgraph) 
                    {
                        Diff = diff
                    }.Run(new[] { v });
                }

				// add entering arc
				_tree[enteringArc] = Flow(enteringArc) + signedDelta;
                if (enteringSaturated)
                {
                    _saturated.Remove(enteringArc);
                }

				_treeSubgraph.Enable(enteringArc, true);
			}
		}

		/// <summary>
		/// Runs the algorithm until the problem is found to be infeasible, 
        /// an optimal solution is found, or the objective is found to be unbounded.
		/// </summary>
		public void Run()
		{
			while (State is SimplexState.FirstPhase or SimplexState.SecondPhase) Step();
		}
	}
}
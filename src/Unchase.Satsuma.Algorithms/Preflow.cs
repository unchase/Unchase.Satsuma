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
using Unchase.Satsuma.Algorithms.Contracts;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Algorithms
{
	/// <summary>
	/// Finds a maximum flow using the Goldberg-Tarjan preflow algorithm.
	/// </summary>
	/// <remarks>
	/// <para>Let D denote the sum of capacities for all arcs exiting Source.</para>
	/// <para>- If all capacities are integers, and D &lt; 2<sup>53</sup>, then the returned flow is exact and optimal.</para>
	/// <para>- Otherwise, small round-off errors may occur and the returned flow is \"almost-optimal\" (see <see cref="Error"/>).</para>
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public sealed class Preflow<TNodeProperty, TArcProperty> : 
        IFlow<double, TNodeProperty, TArcProperty>
	{
		/// <summary>
		/// The input graph.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

		/// <summary>
		/// The arc capacity function.
		/// </summary>
		public Func<Arc, double> Capacity { get; }

		/// <summary>
		/// The source node.
		/// </summary>
		public Node Source { get; }

		/// <summary>
		/// The target node.
		/// </summary>
		public Node Target { get; }

		/// <summary>
		/// The flow size.
		/// </summary>
		public double FlowSize { get; }

		private readonly Dictionary<Arc, double> _flow;

		/// <summary>
		/// A (usually very small) approximate upper bound for the difference
		/// between #FlowSize and the actual maximum flow value.
		/// </summary>
		/// <remarks>
		/// Due to floating-point roundoff errors, the maximum flow cannot be calculated exactly.
		/// </remarks>
		public double Error { get; }

		/// <summary>
		/// Initialize <see cref="Preflow{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
		/// <param name="capacity"><see cref="Capacity"/>.</param>
		/// <param name="source"><see cref="Source"/>.</param>
		/// <param name="target"><see cref="Target"/>.</param>
		public Preflow(
            IGraph<TNodeProperty, TArcProperty> graph,
            Func<Arc, double> capacity,
            Node source,
            Node target)
		{
			Graph = graph;
			Capacity = capacity;
			Source = source;
			Target = target;

			_flow = new();

			// calculate bottleneck capacity to get an upper bound for the flow value
			var dijkstra = new Dijkstra<TNodeProperty, TArcProperty>(Graph, a => -Capacity(a), DijkstraMode.Maximum);
			dijkstra.AddSource(Source);
			dijkstra.RunUntilFixed(Target);
			var bottleneckCapacity = -dijkstra.GetDistance(Target);

			if (double.IsPositiveInfinity(bottleneckCapacity))
			{
				// flow value is infinity
				FlowSize = double.PositiveInfinity;
				Error = 0;
				for (Node n = Target, n2; n != Source; n = n2)
				{
					var arc = dijkstra.GetParentArc(n);
					_flow[arc] = double.PositiveInfinity;
					n2 = Graph.Other(arc, n);
				}
			}
			else
			{
				// flow value is finite
				if (double.IsNegativeInfinity(bottleneckCapacity))
                {
                    bottleneckCapacity = 0; // Target is not accessible
                }

				_u = Graph.ArcCount() * bottleneckCapacity;

				// calculate other upper bounds for the flow
				double uSource = 0;
				foreach (var arc in Graph.Arcs(Source, ArcFilter.Forward))
                {
                    if (Graph.Other(arc, Source) != Source)
					{
						uSource += Capacity(arc);
						if (uSource > _u) break;
					}
                }

				_u = Math.Min(_u, uSource);
				double uTarget = 0;
				foreach (var arc in Graph.Arcs(Target, ArcFilter.Backward))
                {
                    if (Graph.Other(arc, Target) != Target)
					{
						uTarget += Capacity(arc);
						if (uTarget > _u) break;
					}
                }

				_u = Math.Min(_u, uTarget);

				var sg = new Supergraph<TNodeProperty, TArcProperty>(Graph);
				var newSource = sg.AddNode();
				_artificialArc = sg.AddArc(newSource, Source, Directedness.Directed);

				_capacityMultiplier = Utils.LargestPowerOfTwo(long.MaxValue / _u);
                if (_capacityMultiplier == 0)
                {
                    _capacityMultiplier = 1;
                }

				var p = new IntegerPreflow<TNodeProperty, TArcProperty>(sg, IntegralCapacity, newSource, Target);
				FlowSize = p.FlowSize / _capacityMultiplier;
				Error = Graph.ArcCount() / _capacityMultiplier;
                foreach (var kv in p.NonzeroArcs)
                {
                    _flow[kv.Key] = kv.Value / _capacityMultiplier;
                }
			}
		}

		private readonly Arc _artificialArc;
		private readonly double _u;
        private readonly double _capacityMultiplier;

        private long IntegralCapacity(Arc arc)
		{
			return (long)(_capacityMultiplier * (arc == _artificialArc ? _u : Math.Min(_u, Capacity(arc))));
		}

		/// <summary>
		/// Get nonzero arcs.
		/// </summary>
		public IEnumerable<KeyValuePair<Arc, double>> NonzeroArcs
		{
			get
			{
				return _flow.Where(kv => kv.Value != 0.0);
			}
		}

		/// <summary>
		/// Get the arc flow.
		/// </summary>
		/// <param name="arc">The arc.</param>
        public double Flow(Arc arc)
		{
            _flow.TryGetValue(arc, out var result);
			return result;
		}
	}
}
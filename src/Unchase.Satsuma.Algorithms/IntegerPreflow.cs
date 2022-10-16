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

using Unchase.Satsuma.Algorithms.Contracts;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Algorithms
{
    /// <summary>
	/// Finds a maximum flow for integer capacities using the Goldberg-Tarjan preflow algorithm.
	/// </summary>
	/// <remarks>
	/// The sum of capacities on the outgoing edges of Source must be at most <see cref="long.MaxValue"/>.
	/// </remarks>
	public sealed class IntegerPreflow : 
        IFlow<long>
	{
		/// <summary>
		/// The input graph.
		/// </summary>
		public IGraph Graph { get; }

		/// <summary>
		/// The arc capacity function.
		/// </summary>
		public Func<Arc, long> Capacity { get; }

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
		public long FlowSize { get; private set; }

		private readonly Dictionary<Arc, long> _flow;
		private readonly Dictionary<Node, long> _excess;
		private readonly Dictionary<Node, long> _label;
		private readonly Core.Collections.PriorityQueue<Node, long> _active;

		/// <summary>
		/// Initialize <see cref="IntegerPreflow"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		/// <param name="capacity"><see cref="Capacity"/>.</param>
		/// <param name="source"><see cref="Source"/>.</param>
		/// <param name="target"><see cref="Target"/>.</param>
		public IntegerPreflow(
            IGraph graph,
            Func<Arc, long> capacity,
            Node source,
            Node target)
		{
			Graph = graph;
			Capacity = capacity;
			Source = source;
			Target = target;

			_flow = new();
			_excess = new();
			_label = new();
			_active = new();

			Run();

			_excess = new();
			_label = new();
			_active = new();
		}

		private void Run()
		{
			foreach (var node in Graph.Nodes())
			{
				_label[node] = (node == Source ? -Graph.NodeCount() : 0);
				_excess[node] = 0;
			}

			long outgoing = 0;
			foreach (var arc in Graph.Arcs(Source, ArcFilter.Forward))
			{
				var other = Graph.Other(arc, Source);
                if (other == Source)
                {
                    continue;
                }

				var initialFlow = (Graph.U(arc) == Source ? Capacity(arc) : -Capacity(arc));
                if (initialFlow == 0)
                {
                    continue;
                }

				_flow[arc] = initialFlow;
				initialFlow = Math.Abs(initialFlow);
                checked
                {
                    outgoing += initialFlow; // throws if outgoing source capacity is too large
				}

				_excess[other] += initialFlow;
                if (other != Target)
                {
                    _active[other] = 0;
                }
			}

			_excess[Source] = -outgoing;

			while (_active.Count > 0)
			{
                var z = _active.Peek(out var labelZ);
				_active.Pop();
				var e = _excess[z];
				var newlabelZ = long.MinValue;

				foreach (var arc in Graph.Arcs(z))
				{
					Node u = Graph.U(arc), v = Graph.V(arc);
                    if (u == v)
                    {
                        continue;
                    }

					var other = (z == u ? v : u);
					var isEdge = Graph.IsEdge(arc);

                    _flow.TryGetValue(arc, out var f);
					var c = Capacity(arc);
					var lowerBound = (isEdge ? -Capacity(arc) : 0);

					if (u == z)
					{
						if (f == c) 
                        {
                            continue; // saturated, cannot push
                        }

						var labelOther = _label[other];
                        if (labelOther <= labelZ)
                        {
                            newlabelZ = Math.Max(newlabelZ, labelOther - 1);
                        }
						else
						{
							var amount = (long)Math.Min((ulong)e, (ulong)(c - f));
							_flow[arc] = f + amount;
							_excess[v] += amount;
                            if (v != Source && v != Target)
                            {
                                _active[v] = _label[v];
                            }

							e -= amount;
                            if (e == 0)
                            {
                                break;
                            }
						}
					}
					else
					{
						if (f == lowerBound) 
                        {
                            continue; // cannot pull
                        }

						var labelOther = _label[other];
                        if (labelOther <= labelZ)
                        {
                            newlabelZ = Math.Max(newlabelZ, labelOther - 1);
                        }
						else
						{
							var amount = (long)Math.Min((ulong)e, (ulong)(f - lowerBound));
							_flow[arc] = f - amount;
							_excess[u] += amount;
                            if (u != Source && u != Target)
                            {
                                _active[u] = _label[u];
                            }

							e -= amount;
                            if (e == 0)
                            {
                                break;
                            }
						}
					}
				}

				_excess[z] = e;
				if (e > 0)
				{
                    if (newlabelZ == long.MinValue)
                    {
                        throw new InvalidOperationException("Internal error.");
                    }

					_active[z] = _label[z] = newlabelZ;
				}
			}

			FlowSize = 0;
			foreach (var arc in Graph.Arcs(Source))
			{
				Node u = Graph.U(arc), v = Graph.V(arc);
                if (u == v)
                {
                    continue;
                }

                if (!_flow.TryGetValue(arc, out var f))
                {
                    continue;
                }

                if (u == Source)
                {
                    FlowSize += f;
                }
                else
                {
                    FlowSize -= f;
                }
			}
		}

		/// <summary>
		/// Nonzero arcs.
		/// </summary>
		public IEnumerable<KeyValuePair<Arc, long>> NonzeroArcs
		{
			get
			{
				return _flow.Where(kv => kv.Value != 0);
			}
		}

        /// <summary>
        /// Get the arc flow.
        /// </summary>
        /// <param name="arc">The arc.</param>
		public long Flow(Arc arc)
		{
            _flow.TryGetValue(arc, out var result);
			return result;
		}
	}
}
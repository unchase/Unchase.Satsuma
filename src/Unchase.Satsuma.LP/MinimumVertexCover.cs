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

using System.Diagnostics;

using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.LP.Contracts;
using Unchase.Satsuma.LP.Enums;

namespace Unchase.Satsuma.LP
{
	/// <inheritdoc cref="MinimumVertexCover{TNodeProperty, TArcProperty}"/>
	public sealed class MinimumVertexCover :
        MinimumVertexCover<object, object>
    {
		/// <summary>
		/// Initialize <see cref="MinimumVertexCover"/>.
		/// </summary>
		/// <param name="solver"><see cref="ISolver"/>.</param>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		/// <param name="nodeCost">A finite cost function on the nodes of Graph.</param>
		/// <param name="arcWeight">A finite weight function on the arcs of Graph.</param>
		/// <param name="relaxed">If true, each node can be chosen with a fractional weight.</param>
		public MinimumVertexCover(
            ISolver solver,
            IGraph graph,
            Func<Node, double>? nodeCost = null,
            Func<Arc, double>? arcWeight = null,
            bool relaxed = false)
		        : base(solver, graph, nodeCost, arcWeight, relaxed)
        {
		}
    }

	/// <summary>
	/// Finds a minimum cost vertex cover.
	/// </summary>
	/// <remarks>
	/// <para>Edges may have different weights, which means that they have to be covered the given times.</para>
	/// <para>Also the vertex cover may be relaxed (fractional weights per node).</para>
	/// <para>
	/// A vertex cover is a multiset of nodes so that each arc
	/// is incident to at least a given number of nodes in the set.
	/// </para>
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
	/// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public class MinimumVertexCover<TNodeProperty, TArcProperty>
	{
        /// <summary>
		/// The input graph.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

		/// <summary>
		/// A finite cost function on the nodes of <see cref="Graph"/>.
		/// </summary>
		/// <remarks>
		/// <para>Determines the cost of including a specific node in the covering set.</para>
		/// <para>Can be null, in this case each node has cost=1.</para>
		/// </remarks>
		public Func<Node, double>? NodeCost { get; }

		/// <summary>
		/// A finite weight function on the arcs of <see cref="Graph"/>.
		/// </summary>
		/// <remarks>
		/// <para>Determines the minimum number of times the arc has to be covered.</para>
		/// <para>Can be null, in this case each arc has to be covered once.</para>
		/// </remarks>
		public Func<Arc, double>? ArcWeight { get; }

		/// <summary>
		/// If true, each node can be chosen with a fractional weight.
		/// </summary>
		/// <remarks>
		/// Otherwise, each node has to be chosen an integer number of times (default).
		/// </remarks>
		public bool Relaxed { get; }

		/// <summary>
		/// LP solution type.
		/// </summary>
		public SolutionType SolutionType;

		/// <summary>
		/// Contains null, or a valid and possibly optimal weighted covering set, depending on <see cref="SolutionType"/>.
		/// </summary>
		/// <remarks>
		/// <para>If <see cref="SolutionType"/> is <see cref="Enums.SolutionType.Optimal"/>, this is a minimum cost vertex cover with multiplicities.</para>
		/// <para>If <see cref="SolutionType"/> is <see cref="Enums.SolutionType.Feasible"/>, <see cref="Nodes"/> is a valid weighted vertex cover, but not optimal.</para>
		/// <para>Otherwise, <see cref="Nodes"/> is null.</para>
		/// </remarks>
		public Dictionary<Node, double>? Nodes { get; }

		/// <summary>
		/// Initialize <see cref="MinimumVertexCover{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="solver"><see cref="ISolver"/>.</param>
		/// <param name="graph"><see cref="Graph"/>.</param>
		/// <param name="nodeCost"><see cref="NodeCost"/>.</param>
		/// <param name="arcWeight"><see cref="ArcWeight"/>.</param>
		/// <param name="relaxed"><see cref="Relaxed"/>.</param>
		public MinimumVertexCover(
            ISolver solver, 
            IGraph<TNodeProperty, TArcProperty> graph,
			Func<Node, double>? nodeCost = null, 
            Func<Arc, double>? arcWeight = null,
			bool relaxed = false)
		{
			Graph = graph;
            nodeCost ??= _ => 1;

			NodeCost = nodeCost;
            arcWeight ??= _ => 1;

			ArcWeight = arcWeight;
			Relaxed = relaxed;

			var problem = new Problem
            {
                Mode = OptimizationMode.Minimize
            };

            foreach (var n in graph.Nodes())
			{
				var v = problem.GetVariable(n);
				v.LowerBound = 0;
                if (!relaxed)
                {
                    v.Type = VariableType.Integer;
                }

				problem.Objective.Coefficients[v] = nodeCost(n);
			}
			foreach (var a in graph.Arcs())
			{
				var u = graph.U(a);
				var v = graph.V(a);
                if (u != v)
                {
                    problem.Constraints.Add(problem.GetVariable(u) + problem.GetVariable(v) >= arcWeight(a));
                }
                else
                {
                    problem.Constraints.Add((Expression)problem.GetVariable(u) >= arcWeight(a));
                }
			}

			var solution = solver.Solve(problem);
			SolutionType = solution.Type;
			Debug.Assert(SolutionType != SolutionType.Unbounded);
			if (solution.Valid)
			{
				Nodes = new();
				foreach (var kv in solution.Primal)
				{
					if (kv.Value > 0)
					{
						var n = (Node)kv.Key.Id;
						Nodes[n] = kv.Value;
					}
				}
			}
            else
            {
                Nodes = null;
            }
		}
	}
}
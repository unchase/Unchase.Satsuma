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
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.LP.Contracts;
using Unchase.Satsuma.LP.Enums;

namespace Unchase.Satsuma.LP
{
	/// <inheritdoc cref="OptimalSubgraph{TNodeProperty, TArcProperty}"/>
	public sealed class OptimalSubgraph :
        OptimalSubgraph<object, object>
    {
		/// <summary>
        /// Initialize <see cref="OptimalSubgraph"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        public OptimalSubgraph(
            IGraph graph)
		        : base(graph)
        {
		}
    }

	/// <summary>
	/// Finds a degree-bounded subgraph with one or more cost functions on the edges.
	/// </summary>
	/// <remarks>
	/// <para>Uses integer programming to achieve this goal.</para>
	/// <para>Minimizes a linear objective function which is a combination of cost functions on the edges.</para>
	/// <para>Number of variables: O(ArcCount)</para>
	/// <para>Number of equations: O(NodeCount + CostFunctionCount)</para>
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
	/// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public class OptimalSubgraph<TNodeProperty, TArcProperty>
	{
        /// <summary>
		/// The definition of a cost function.
		/// </summary>
		/// <remarks>
		/// Cost functions can be used to impose lower/upper bounds on the properties of the resulting subgraph,
        /// or to include additional terms in the linear objective function.
		/// </remarks>
		public class CostFunction
		{
            /// <summary>
			/// The cost function itself. Cannot be null.
			/// </summary>
			/// <remarks>
			/// <para>Should assign an arbitrary real weight to each edge.</para>
			/// <para>
			/// The "sum" of the cost function is defined as the sum of its values on the chosen edges
            /// for a given subgraph.
			/// </para>
			/// </remarks>
			public Func<Arc, double> Cost { get; }

			/// <summary>
			/// The (inclusive) lower bound on the sum of the cost function. Default: double.NegativeInfinity.
			/// </summary>
			public double LowerBound { get; set; }

			/// <summary>
			/// The (inclusive) upper bound on the sum of the cost function. Default: double.PositiveInfinity.
			/// </summary>
			public double UpperBound { get; set; }

			/// <summary>
			/// The weight of the sum of this cost function in the LP objective function. Default: 0.
			/// </summary>
			/// <remarks>
			/// <para>May be positive, zero or negative.</para>
			/// <para>Keep in mind that the LP objective function is always minimized.</para>
			/// </remarks>
			public double ObjectiveWeight { get; set; }

			/// <summary>
			/// Initialize <see cref="CostFunction"/>.
			/// </summary>
			/// <param name="cost"><see cref="Cost"/>.</param>
			/// <param name="lowerBound"><see cref="LowerBound"/>.</param>
			/// <param name="upperBound"><see cref="UpperBound"/>.</param>
			/// <param name="objectiveWeight"><see cref="ObjectiveWeight"/>.</param>
			public CostFunction(
                Func<Arc, double> cost,
				double lowerBound = double.NegativeInfinity, 
                double upperBound = double.PositiveInfinity,
				double objectiveWeight = 0.0)
			{
				Cost = cost;
				LowerBound = lowerBound;
				UpperBound = upperBound;
				ObjectiveWeight = objectiveWeight;
			}

			/// <summary>
			/// Get sum cost.
			/// </summary>
			/// <param name="arcs">Arc collection.</param>
            public double GetSum(IEnumerable<Arc> arcs)
			{
				return arcs.Select(Cost).Sum();
			}
		}

		/// <summary>
		/// The original graph.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

		/// <summary>
		/// The weight of a specific arc when calculating the weighted node degrees.
		/// </summary>
		/// <remarks>
		/// If null, all arcs have weight==1.
		/// </remarks>
		public Func<Arc, double>? DegreeWeight { get; set; }

		/// <summary>
		/// The (inclusive) lower bound on weighted node in-degrees. If null, no lower bound is imposed at all.
		/// </summary>
		/// <remarks>
		/// Loop edges count twice.
		/// </remarks>
		public Func<Node, double>? MinInDegree { get; set; }

		/// <summary>
		/// The (inclusive) upper bound on weighted node in-degrees. If null, no upper bound is imposed at all.
		/// </summary>
		/// <remarks>
		/// Loop edges count twice.
		/// </remarks>
		public Func<Node, double>? MaxInDegree { get; set; }

		/// <summary>
		/// The (inclusive) lower bound on weighted node out-degrees. If null, no lower bound is imposed at all.
		/// </summary>
		/// <remarks>
		/// Loop edges count twice.
		/// </remarks>
		public Func<Node, double>? MinOutDegree { get; set; }

		/// <summary>
		/// The (inclusive) upper bound on weighted node out-degrees. If null, no upper bound is imposed at all.
		/// </summary>
		/// <remarks>
		/// Loop edges count twice.
		/// </remarks>
		public Func<Node, double>? MaxOutDegree { get; set; }

		/// <summary>
		/// The (inclusive) lower bound on weighted node degrees. If null, no lower bound is imposed at all.
		/// </summary>
		/// <remarks>
		/// Keep in mind that the degree is the sum of the indegree and the outdegree, so loop arcs count twice.
		/// </remarks>
		public Func<Node, double>? MinDegree { get; set; }

		/// <summary>
		/// The (inclusive) upper bound on weighted node degrees. If null, no upper bound is imposed at all.
		/// </summary>
		/// <remarks>
		/// Keep in mind that the degree is the sum of the indegree and the outdegree, so loop arcs count twice.
		/// </remarks>
		public Func<Node, double>? MaxDegree { get; set; }

		/// <summary>
		/// The (inclusive) lower bound on the number of arcs in the subgraph. Default: 0.
		/// </summary>
		public int MinArcCount { get; set; }

		/// <summary>
		/// The (inclusive) upper bound on the number of arcs in the subgraph. Default: <see cref="int.MaxValue"/>.
		/// </summary>
		public int MaxArcCount { get; set; }

		/// <summary>
		/// The weight of the number of arcs in the subgraph in the LP objective function. Default: 0.
		/// </summary>
		/// <remarks>
		/// <para>May be positive, zero or negative.</para>
		/// <para>Keep in mind that the LP objective function is always minimized.</para>
		/// </remarks>
		public double ArcCountWeight { get; set; }

        /// <summary>
        /// The cost functions used to make additional restrictions and additive terms in the objective function.
        /// </summary>
        public List<CostFunction> CostFunctions { get; }

		/// <summary>
		/// The LP solution type. Invalid if optimization has not been run.
		/// </summary>
		public SolutionType SolutionType { get; private set; }

		/// <summary>
		/// The resulting optimal subgraph. Null if the optimization has not been run, or if no solution was found.
		/// </summary>
		/// <remarks>
		/// A subgraph of <see cref="Graph"/>.
		/// </remarks>
		public Subgraph<TNodeProperty, TArcProperty>? ResultGraph { get; private set; }

		/// <summary>
		/// Initialize <see cref="OptimalSubgraph{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="Graph"/>.</param>
		public OptimalSubgraph(
            IGraph<TNodeProperty, TArcProperty> graph)
		{
			Graph = graph;
			DegreeWeight = null;
			MinInDegree = null;
			MaxInDegree = null;
			MinOutDegree = null;
			MaxOutDegree = null;
			MinDegree = null;
			MaxDegree = null;
			MinArcCount = 0;
			MaxArcCount = int.MaxValue;
			ArcCountWeight = 0;
			CostFunctions = new();
			SolutionType = SolutionType.Invalid;
			ResultGraph = null;
		}

		/// <summary>
		/// Solves the optimization problem.
		/// </summary>
		/// <remarks>
		/// Result will be put in #SolutionType and (if solution is valid) <see cref="ResultGraph"/>.
		/// </remarks>
		/// <param name="solver"><see cref="ISolver"/>.</param>
		public void Run(ISolver solver)
		{
			if (MinArcCount > 0 || MaxArcCount < int.MaxValue || ArcCountWeight != 0)
			{
				CostFunctions.Add(new(cost: (_ => 1.0),
					lowerBound: MinArcCount > 0 ? MinArcCount - 0.5 : double.NegativeInfinity,
					upperBound: MaxArcCount < int.MaxValue ? MaxArcCount + 0.5 : double.PositiveInfinity,
					objectiveWeight: ArcCountWeight));
			}

			var problem = new Problem
            {
                Mode = OptimizationMode.Minimize
            };

            // a binary variable for the inclusion of each arc
			foreach (var a in Graph.Arcs())
			{
				var v = problem.GetVariable(a);
				v.Type = VariableType.Binary;
			}

			// constraints and objective for each cost function
			foreach (var c in CostFunctions)
			{
				if (c.ObjectiveWeight != 0 || c.LowerBound > double.MinValue || c.UpperBound < double.MaxValue)
				{
					Expression cSum = 0.0;
					foreach (var a in Graph.Arcs())
					{
						cSum.Add(problem.GetVariable(a), c.Cost(a));
					}

                    if (c.ObjectiveWeight != 0)
                    {
                        problem.Objective += c.ObjectiveWeight * cSum;
                    }

                    if (c.LowerBound > double.MinValue)
                    {
                        problem.Constraints.Add(cSum >= c.LowerBound);
                    }

                    if (c.UpperBound < double.MaxValue)
                    {
                        problem.Constraints.Add(cSum <= c.UpperBound);
                    }
				}
			}

			// constraints for degrees
			if (MinInDegree != null || MaxInDegree != null
				|| MinOutDegree != null || MaxOutDegree != null
				|| MinDegree != null || MaxDegree != null)
			{
				foreach (var n in Graph.Nodes())
				{
					var myMinInDegree = MinInDegree?.Invoke(n) ?? double.NegativeInfinity;
					var myMaxInDegree = MaxInDegree?.Invoke(n) ?? double.PositiveInfinity;
					var myMinOutDegree = MinOutDegree?.Invoke(n) ?? double.NegativeInfinity;
					var myMaxOutDegree = MaxOutDegree?.Invoke(n) ?? double.PositiveInfinity;
					var myMinDegree = MinDegree?.Invoke(n) ?? double.NegativeInfinity;
					var myMaxDegree = MaxDegree?.Invoke(n) ?? double.PositiveInfinity;

					if (myMinInDegree > double.MinValue || myMaxInDegree < double.MaxValue
						|| myMinOutDegree > double.MinValue || myMaxOutDegree < double.MaxValue
						|| myMinDegree > double.MinValue || myMaxDegree < double.MaxValue)
					{
						Expression inDegree = 0;
						Expression outDegree = 0;
						Expression degree = 0;
						foreach (var a in Graph.Arcs(n))
						{
							var weight = DegreeWeight?.Invoke(a) ?? 1;
							if (weight != 0)
							{
								var u = Graph.U(a);
								var v = Graph.V(a);
								var isEdge = Graph.IsEdge(a);
								var isLoop = u == v;
								var aVar = problem.GetVariable(a);
								degree.Add(aVar, isLoop ? 2 * weight : weight);
								if (u == n || isEdge)
									outDegree.Add(aVar, (isLoop && isEdge) ? 2 * weight : weight);
								if (v == n || isEdge)
									inDegree.Add(aVar, (isLoop && isEdge) ? 2 * weight : weight);
							}
						}

                        if (myMinInDegree > double.MinValue)
                        {
                            problem.Constraints.Add(inDegree >= myMinInDegree);
                        }

                        if (myMaxInDegree < double.MaxValue)
                        {
                            problem.Constraints.Add(inDegree <= myMaxInDegree);
                        }

                        if (myMinOutDegree > double.MinValue)
                        {
                            problem.Constraints.Add(outDegree >= myMinOutDegree);
                        }

                        if (myMaxOutDegree < double.MaxValue)
                        {
                            problem.Constraints.Add(outDegree <= myMaxOutDegree);
                        }

                        if (myMinDegree > double.MinValue)
                        {
                            problem.Constraints.Add(degree >= myMinDegree);
                        }

                        if (myMaxDegree < double.MaxValue)
                        {
                            problem.Constraints.Add(degree <= myMaxDegree);
                        }
					}
				}
			}

			var solution = solver.Solve(problem);
			SolutionType = solution.Type;
			if (solution.Valid)
			{
				ResultGraph = new(Graph);
                foreach (var arc in Graph.Arcs())
                {
                    ResultGraph.Enable(arc, solution[problem.GetVariable(arc)] >= 0.5);
                }
			}
            else
            {
                ResultGraph = null;
            }
		}
	}
}
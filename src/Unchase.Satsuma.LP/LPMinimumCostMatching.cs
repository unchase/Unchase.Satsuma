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
using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Adapters.Abstractions;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.LP.Contracts;
using Unchase.Satsuma.LP.Enums;

namespace Unchase.Satsuma.LP
{
    /// <summary>
    /// Finds a minimum cost matching in an arbitrary graph using integer programming.
    /// </summary>
    /// <remarks>
    /// See also <seealso cref="LpMaximumMatching"/>.
    /// </remarks>
    public sealed class LpMinimumCostMatching
    {
        /// <summary>
        /// The input graph.
        /// </summary>
        public IGraph Graph { get; }

        /// <summary>
        /// A finite cost function on the arcs of <see cref="Graph"/>.
        /// </summary>
        public Func<Arc, double> Cost { get; }

        /// <summary>
        /// Minimum constraint on the size (number of arcs) of the returned matching.
        /// </summary>
        public int MinimumMatchingSize { get; }

        /// <summary>
        /// Maximum constraint on the size (number of arcs) of the returned matching.
        /// </summary>
        public int MaximumMatchingSize { get; }

        /// <summary>
        /// LP solution type.
        /// </summary>
        public SolutionType SolutionType;

        private readonly Matching? _matching;

        /// <summary>
        /// Contains null, or a valid and possibly optimal matching, depending on <see cref="SolutionType"/>.
        /// </summary>
        /// <remarks>
        /// <para>If <see cref="SolutionType"/> is <see cref="Enums.SolutionType.Optimal"/>, this <see cref="Matching"/> is an optimal matching.</para>
        /// <para>If <see cref="SolutionType"/> is <see cref="Enums.SolutionType.Feasible"/>, <see cref="Matching"/> is valid but not optimal.</para>
        /// <para>Otherwise, <see cref="Matching"/> is null.</para>
        /// </remarks>
        public IMatching? Matching => _matching;

        /// <summary>
        /// Initialize <see cref="LpMinimumCostMatching"/>.
        /// </summary>
        /// <param name="solver"><see cref="ISolver"/>.</param>
        /// <param name="graph"><see cref="Graph"/>.</param>
        /// <param name="cost"><see cref="Cost"/>.</param>
        /// <param name="minimumMatchingSize"><see cref="MinimumMatchingSize"/>.</param>
        /// <param name="maximumMatchingSize"><see cref="MaximumMatchingSize"/>.</param>
        public LpMinimumCostMatching(
            ISolver solver,
            IGraph graph,
            Func<Arc, double> cost,
            int minimumMatchingSize = 0,
            int maximumMatchingSize = int.MaxValue)
        {
            Graph = graph;
            Cost = cost;
            MinimumMatchingSize = minimumMatchingSize;
            MaximumMatchingSize = maximumMatchingSize;

            var g = new OptimalSubgraph(Graph)
            {
                MaxDegree = _ => 1.0,
                MinArcCount = MinimumMatchingSize,
                MaxArcCount = MaximumMatchingSize
            };
            var c = new OptimalSubgraph.CostFunction(cost: cost, objectiveWeight: 1);
            g.CostFunctions.Add(c);
            g.Run(solver);

            SolutionType = g.SolutionType;
            Debug.Assert(SolutionType != SolutionType.Unbounded);
            if (g.ResultGraph != null)
            {
                _matching = new(Graph);
                foreach (var arc in g.ResultGraph.Arcs())
                {
                    _matching.Enable(arc, true);
                }
            }
            else
            {
                _matching = null;
            }
        }
    }
}
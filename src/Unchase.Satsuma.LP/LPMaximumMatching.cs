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
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.LP.Contracts;
using Unchase.Satsuma.LP.Enums;

namespace Unchase.Satsuma.LP
{
    /// <summary>
    /// Computes a maximum matching in an arbitrary graph, using integer programming.
    /// </summary>
    /// <remarks>
    /// See also <seealso cref="LpMinimumCostMatching{TNodeProperty, TArcProperty}"/>.
    /// </remarks>
    /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
    public sealed class LpMaximumMatching<TNodeProperty, TArcProperty>
    {
        /// <summary>
        /// The input graph.
        /// </summary>
        public IGraph<TNodeProperty, TArcProperty> Graph { get; }

        /// <summary>
        /// LP solution type.
        /// </summary>
        public SolutionType SolutionType;

        private readonly Matching<TNodeProperty, TArcProperty>? _matching;

        /// <summary>
        /// Contains null, or a valid and possibly optimal matching, depending on <see cref="SolutionType"/>.
        /// </summary>
        /// <remarks>
        /// <para>If <see cref="SolutionType"/> is <see cref="Enums.SolutionType.Optimal"/>, this <see cref="Matching"/> is an optimal matching.</para>
        /// <para>If <see cref="SolutionType"/> is <see cref="Enums.SolutionType.Feasible"/>, <see cref="Matching"/> is valid but not optimal.</para>
        /// <para>Otherwise, <see cref="Matching"/> is null.</para>
        /// </remarks>
        public IMatching<TNodeProperty, TArcProperty>? Matching => _matching;

        /// <summary>
        /// Initialize <see cref="LpMaximumMatching{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <param name="solver"><see cref="ISolver"/>.</param>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        public LpMaximumMatching(
            ISolver solver, 
            IGraph<TNodeProperty, TArcProperty> graph)
        {
            Graph = graph;

            var g = new OptimalSubgraph<TNodeProperty, TArcProperty>(Graph)
            {
                MaxDegree = _ => 1.0,
                ArcCountWeight = -1.0
            };
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
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
    /// <summary>
    /// Finds a maximum weight stable set in an arbitrary graph, using integer programming.
    /// </summary>
    /// <remarks>
    /// A stable set is a set of nodes with no arcs between any of the nodes.
    /// </remarks>
    /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
    public sealed class MaximumStableSet<TNodeProperty, TArcProperty>
    {
        /// <summary>
        /// The input graph.
        /// </summary>
        public IGraph<TNodeProperty, TArcProperty> Graph { get; }

        /// <summary>
        /// A finite weight function on the nodes of <see cref="Graph"/>.
        /// </summary>
        /// <remarks>
        /// Can be null, in this case each node has weight 1.
        /// </remarks>
        public Func<Node, double>? Weight { get; }

        /// <summary>
        /// LP solution type.
        /// </summary>
        public SolutionType SolutionType;

        /// <summary>
        /// Contains null, or a valid and possibly optimal stable set, depending on <see cref="SolutionType"/>.
        /// </summary>
        /// <remarks>
        /// <para>If <see cref="SolutionType"/> is <see cref="Enums.SolutionType.Optimal"/>, this is an optimal set.</para>
        /// <para>If <see cref="SolutionType"/> is <see cref="Enums.SolutionType.Feasible"/>, <see cref="Nodes"/> is valid but not optimal.</para>
        /// <para>Otherwise, <see cref="Nodes"/> is null.</para>
        /// </remarks>
        public HashSet<Node>? Nodes { get; }

        /// <summary>
        /// Initialize <see cref="MaximumStableSet{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <param name="solver"><see cref="ISolver"/>.</param>
        /// <param name="graph"><see cref="Graph"/>.</param>
        /// <param name="weight"><see cref="Weight"/>.</param>
        public MaximumStableSet(
            ISolver solver, 
            IGraph<TNodeProperty, TArcProperty> graph, 
            Func<Node, double>? weight = null)
        {
            Graph = graph;
            weight ??= _ => 1.0;

            Weight = weight;

            var problem = new Problem
            {
                Mode = OptimizationMode.Maximize
            };

            foreach (var n in graph.Nodes())
            {
                var v = problem.GetVariable(n);
                v.Type = VariableType.Binary;
                problem.Objective.Coefficients[v] = weight(n);
            }

            foreach (var a in graph.Arcs())
            {
                var u = graph.U(a);
                var v = graph.V(a);
                if (u != v)
                {
                    problem.Constraints.Add(problem.GetVariable(u) + problem.GetVariable(v) <= 1);
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
                    if (kv.Value > 0.5)
                    {
                        var n = (Node)kv.Key.Id;
                        Nodes.Add(n);
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
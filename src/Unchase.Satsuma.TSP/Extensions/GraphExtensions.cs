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

using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.TSP.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IGraph{TNodeProperty,TArcProperty}"/> and <see cref="IGraph"/>.
    /// </summary>
    public static class GraphExtensions
    {
        #region HamiltonianCycle

        /// <summary>
        /// Convert <see cref="IGraph{TNodeProperty, TArcProperty}"/> to <see cref="HamiltonianCycle{TNodeProperty, TArcProperty}"/> algorithm.
        /// </summary>
        /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
        /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        /// <returns>Returns <see cref="HamiltonianCycle{TNodeProperty, TArcProperty}"/> algorithm.</returns>
        public static HamiltonianCycle<TNodeProperty, TArcProperty> ToHamiltonianCycle<TNodeProperty, TArcProperty>(
            this IGraph<TNodeProperty, TArcProperty> graph)
        {
            return new(graph);
        }

        /// <summary>
        /// Convert <see cref="IGraph"/> to <see cref="HamiltonianCycle"/> algorithm.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <returns>Returns <see cref="HamiltonianCycle"/> algorithm.</returns>
        public static HamiltonianCycle ToHamiltonianCycle(
            this IGraph graph)
        {
            return new(graph);
        }

        #endregion HamiltonianCycle
    }
}
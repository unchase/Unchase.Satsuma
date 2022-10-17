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

namespace Unchase.Satsuma.TSP
{
    /// <summary>
    /// Utilities regarding the "traveling salesman problem".
    /// </summary>
    public static class TspUtils
    {
        /// <summary>
        /// Returns the total cost of a TSP tour.
        /// </summary>
        /// <typeparam name="TNode"></typeparam>
        /// <param name="tour">
        /// <para>A node sequence representing a tour.</para>
        /// <para>If the tour is not empty, then the starting node must be repeated at the end.</para>
        /// </param>
        /// <param name="cost">A finite cost function on the node pairs.</param>
        /// <returns></returns>
        public static double GetTourCost<TNode>(IEnumerable<TNode> tour, Func<TNode, TNode, double> cost)
        {
            double result = 0;
            var tourList = tour.ToList();
            if (tourList.Any())
            {
                var prev = tourList.First();
                foreach (var node in tourList.Skip(1))
                {
                    result += cost(prev, node);
                    prev = node;
                }
            }

            return result;
        }
    }
}
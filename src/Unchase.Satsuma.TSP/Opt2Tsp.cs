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

using Unchase.Satsuma.TSP.Contracts;

namespace Unchase.Satsuma.TSP
{
    /// <summary>
	/// Improves a solution for the "traveling salesman problem" by using the 2-OPT method.
	/// </summary>
	/// <remarks>
	/// <para>
	/// It starts from a precomputed tour (e.g. one returned by InsertionTsp&lt;TNode&gt;) and gradually improves it by 
	/// repeatedly swapping two edges.
	/// </para>
	/// <para>It is advised to use this class for symmetric cost functions only.</para>
	/// </remarks>
	/// <typeparam name="TNode">The node type.</typeparam>
	public sealed class Opt2Tsp<TNode> : 
        ITsp<TNode>
	{
        /// <summary>
		/// A finite cost function on the node pairs.
		/// </summary>
		public Func<TNode, TNode, double> Cost { get; }

		private readonly List<TNode> _tour;

        /// <inheritdoc />
		public IEnumerable<TNode> Tour => _tour;

        /// <inheritdoc />
		public double TourCost { get; private set; }

		/// <summary>
		/// Initialize <see cref="Opt2Tsp{TNode}"/>.
		/// </summary>
		/// <param name="cost">The cost function (should be symmetrical).</param>
		/// <param name="tour">The tour to improve with 2-OPT. The starting node must be repeated at the end.</param>
		/// <param name="tourCost">
		/// The known cost of tour. Use this parameter to speed up initialization. 
        /// If null is supplied, then the tour cost is recalculated.
		/// </param>
		public Opt2Tsp(
            Func<TNode, TNode, double> cost, 
            IEnumerable<TNode> tour,
            double? tourCost)
		{
			Cost = cost;
			_tour = tour.ToList();
			TourCost = tourCost ?? TspUtils.GetTourCost(_tour, cost);
		}

		/// <summary>
		/// Performs an improvement step.
		/// </summary>
		/// <returns>Returns true if the objective could be improved.</returns>
		public bool Step()
		{
			var improved = false;
			for (var i = 0; i < _tour.Count - 3; i++) // first arc
			{
				for (int j = i + 2, jMax = _tour.Count - (i == 0 ? 2 : 1); j < jMax; j++) // second arc
				{
					var increase = Cost(_tour[i], _tour[j]) + Cost(_tour[i + 1], _tour[j + 1]) -
                                   (Cost(_tour[i], _tour[i + 1]) + Cost(_tour[j], _tour[j + 1]));

					if (increase < 0)
					{
						TourCost += increase;
						_tour.Reverse(i + 1, j - i);
						improved = true;
					}
				}
			}

			return improved;
		}

		/// <summary>
		/// Performs 2-OPT improvement steps until the tour cannot be improved this way.
		/// </summary>
		public void Run()
		{
            while (Step())
            {
            }
		}
	}
}
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

using Unchase.Satsuma.LP.Enums;

namespace Unchase.Satsuma.LP
{
	/// <summary>
	/// The <see cref="Problem"/> solution.
	/// </summary>
	public class Solution
	{
        /// <summary>
		/// The Problem whose solution this is.
		/// </summary>
		public Problem Problem;

		/// <summary>
		/// True if this is a valid solution to the problem.
		/// </summary>
		public bool Valid => Type is SolutionType.Unbounded or SolutionType.Feasible or SolutionType.Optimal;

		/// <summary>
		/// The type of this solution.
		/// </summary>
		public SolutionType Type;

		/// <summary>
		/// A valid finite bound on the objective function,
		/// or +-Infinity if the problem is infeasible or the solver could find no such bound.
		/// </summary>
		/// <remarks>
		/// <para>Equals to the value of this Solution if Type is Optimal.</para>
		/// <para>If the objective is to be minimized, then Bound equals +Infinity if and only if Type is Infeasible.</para>
		/// </remarks>
		public double Bound;

		/// <summary>
		/// The objective value for the current solution, if valid.
		/// </summary>
		public double Value;

        /// <summary>
        /// The values assigned to the primal variables.
        /// </summary>
        /// <remarks>
        /// May not contain all variables: those which are equal to 0 may be absent.
        /// </remarks>
        public Dictionary<Variable, double> Primal { get; private set; } = new();

		private double GetInfeasibleBound()
		{
			return Problem.Mode == OptimizationMode.Minimize 
                ? double.PositiveInfinity 
                : double.NegativeInfinity;
		}

		private double GetUnboundedBound()
		{
			return Problem.Mode == OptimizationMode.Minimize 
                ? double.NegativeInfinity 
                : double.PositiveInfinity;
		}

		/// <summary>
		/// Sets Bound and Value according to the solution type, if it is unequivocal.
		/// </summary>
		public void SetBoundAndValueForType()
        {
            Bound = Type == SolutionType.Infeasible 
                ? GetInfeasibleBound() 
                : GetUnboundedBound();

            Value = Bound;
        }

		/// <summary>
		/// Sets an invalid solution.
		/// </summary>
		public void SetInvalid()
		{
			Type = SolutionType.Invalid;
			SetBoundAndValueForType();
			Primal = new();
		}

		/// <summary>
		/// Initialize <see cref="Solution"/>.
		/// </summary>
		/// <param name="problem"><see cref="Problem"/>.</param>
		public Solution(Problem problem)
		{
			Problem = problem;
			SetInvalid();
		}

		/// <summary>
		/// Gets or sets the value assigned to a specific variable in this solution.
		/// </summary>
		/// <param name="variable"><see cref="Variable"/>.</param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Throws an exception if solution is not Valid.</exception>
		public double this[Variable variable]
		{
			get
			{
                if (!Valid)
                {
                    throw new InvalidOperationException("Cannot get value from invalid solution");
                }

                Primal.TryGetValue(variable, out var result); // will return 0 if not found
				return result;
			}

			set
			{
                if (!Valid)
                {
                    throw new InvalidOperationException("Cannot set value in invalid solution");
                }

                if (value == 0)
                {
                    Primal.Remove(variable);
                }
                else
                {
                    Primal[variable] = value;
                }
			}
		}
	}
}
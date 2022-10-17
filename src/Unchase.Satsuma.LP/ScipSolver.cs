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
using System.Globalization;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.LP.Contracts;
using Unchase.Satsuma.LP.Enums;

namespace Unchase.Satsuma.LP
{
    /// <summary>
	/// LP solver using the SCIP MIP solver.
	/// </summary>
	/// <remarks>
	/// SCIP (<see href="http://scip.zib.de"/>) must be installed in order to use this class.
	/// </remarks>
	public class ScipSolver : 
        ISolver
	{
        /// <summary>
		/// The path to the SCIP executable. Must be set before attempting to solve a problem.
		/// </summary>
		public string? ScipPath;

		/// <summary>
		/// The path to a designated temporary folder.
		/// </summary>
		public string TempFolder;

		/// <summary>
		/// Maximum allowed time for SCIP to run, in seconds.
		/// </summary>
		public int TimeoutSeconds;

		/// <summary>
		/// Initialize <see cref="ScipSolver"/>.
		/// </summary>
		/// <param name="scipPath">SCIP path.</param>
		public ScipSolver(string scipPath)
		{
			ScipPath = scipPath;
			TempFolder = Path.GetTempPath();
		}

		private void LoadSolution(Solution solution, string filename)
		{
			solution.SetInvalid();
			var lines = File.ReadAllLines(filename);
			var status = lines.LastOrDefault(line => line.StartsWith("SCIP Status"));
			if (status != null)
			{
                if (status.IndexOf("[optimal", StringComparison.Ordinal) >= 0)
                {
                    solution.Type = SolutionType.Optimal;
                }
				else if (status.IndexOf("[infeasible]", StringComparison.Ordinal) >= 0)
				{
					solution.Type = SolutionType.Infeasible;
					solution.SetBoundAndValueForType();
				}
				else if (status.IndexOf("[unbounded]", StringComparison.Ordinal) >= 0)
				{
					solution.Type = SolutionType.Unbounded;
					solution.SetBoundAndValueForType();
				}

				var primalValues = lines
					.SkipWhile(s => !s.StartsWith("objective value:"))
					.TakeWhile(s => s.Length > 0)
					.ToList();

				if (primalValues.Count >= 1) // the program output a valid solution
				{
					const char[]? ws = null;
					string[]? tokens;

					if (!solution.Valid) // not optimal, but feasible
					{
						solution.Type = SolutionType.Feasible;
						solution.SetBoundAndValueForType();

						// find a bound, if available
						var dualBound = lines.LastOrDefault(line => line.StartsWith("Dual Bound"));
						if (dualBound != null)
						{
							tokens = dualBound.Split(ws, StringSplitOptions.RemoveEmptyEntries);
							solution.Bound = double.Parse(tokens[3], CultureInfo.InvariantCulture);
						}
					}

					// find the value of the solution
					tokens = primalValues[0].Split(ws, StringSplitOptions.RemoveEmptyEntries);
					solution.Value = double.Parse(tokens[2], CultureInfo.InvariantCulture);

					if (solution.Type == SolutionType.Optimal)
						solution.Bound = solution.Value;

					// load variable values
					foreach (var s in primalValues.Skip(1))
					{
						tokens = s.Split(ws, StringSplitOptions.RemoveEmptyEntries);
						var v = solution.Problem.VariablesBySerialNumber[int.Parse(tokens[0][1..])];
						solution[v] = double.Parse(tokens[1], CultureInfo.InvariantCulture);
					}
				}
			}
		}

		/// <summary>
		/// Solves a problem using the SCIP solver.
		/// </summary>
		/// <param name="problem"><see cref="Problem"/>.</param>
		/// <returns>Returns the <see cref="Solution"/>.</returns>
		/// <exception cref="FileNotFoundException">SCIP executable cannot be found.</exception>
		/// <exception cref="DirectoryNotFoundException">Temporary folder does not exist.</exception>
		public Solution Solve(Problem problem)
		{
            if (ScipPath == null || !File.Exists(ScipPath))
            {
                throw new FileNotFoundException("SCIP executable cannot be found at '" + ScipPath + "'");
            }

            if (!Directory.Exists(TempFolder))
            {
                throw new DirectoryNotFoundException("Temporary folder '" + TempFolder + "' does not exist");
            }

			string lpFilename, outFilename;
			var rng = new Random();
			var filePrefix = "p" + Process.GetCurrentProcess().Id + "t" + Thread.CurrentThread.ManagedThreadId + "r";
			do
			{
				var filename = filePrefix + rng.Next();
				lpFilename = Path.Combine(TempFolder, filename + ".lp");
				outFilename = Path.Combine(TempFolder, filename + ".out");
			}
			while (File.Exists(lpFilename) || File.Exists(outFilename));

			CplexLPFormat.Save(problem, lpFilename);
			//CplexLPFormat.Save(problem, @"d:\temp\temp.lp"); // debug
			var solution = new Solution(problem);
            if (Utils.ExecuteCommand(ScipPath, "-f \"" + lpFilename + "\" -l \"" + outFilename + '"', TimeoutSeconds))
            {
                LoadSolution(solution, outFilename);
            }

			File.Delete(lpFilename);
			File.Delete(outFilename);
			return solution;
		}
	}
}
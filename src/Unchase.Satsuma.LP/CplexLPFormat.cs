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

using System.Globalization;
using System.Text;
using Unchase.Satsuma.LP.Enums;

// ReSharper disable InconsistentNaming

namespace Unchase.Satsuma.LP
{
	internal static class CplexLPFormat
	{
        /// <summary>
		/// Saves the problem in the CPLEX LP file format.
		/// </summary>
		/// <param name="problem"><see cref="Problem"/>.</param>
		/// <param name="filename">The file name.</param>
		internal static void Save(Problem problem, string filename)
        {
            using var sw = new StreamWriter(filename);
            Save(problem, sw);
        }

		private static string Encode(Variable v)
		{
			return "x" + v.SerialNumber;
		}

		private static void Encode(Expression e, StringBuilder sb, double multiplier = 1)
		{
			foreach (var kv in e.Coefficients)
			{
				var v = kv.Key;
				var coeff = kv.Value;
                if (coeff == 0)
                {
                    continue;
                }

				sb.Append((coeff * multiplier).ToString("+0;-#", CultureInfo.InvariantCulture));
				sb.Append(' ');
				sb.Append(Encode(v));
				sb.Append(' ');
			}

			if (e.Bias != 0)
			{
				sb.Append((e.Bias * multiplier).ToString("+0;-#", CultureInfo.InvariantCulture));
				sb.Append(' ');
			}
		}

		private static string Encode(Expression e)
		{
			var sb = new StringBuilder();
			Encode(e, sb);
			return sb.ToString();
		}

		private static void Encode(ComparisonOperator op, StringBuilder sb)
		{
			switch (op)
			{
				case ComparisonOperator.Less:
					sb.Append('<');
					break;
				case ComparisonOperator.LessEqual:
					sb.Append("<=");
					break;
				case ComparisonOperator.Greater:
					sb.Append('>');
					break;
				case ComparisonOperator.GreaterEqual:
					sb.Append(">=");
					break;
				case ComparisonOperator.Equal:
					sb.Append('=');
					break;
			}
		}

		private static void Encode(Constraint c, StringBuilder sb)
		{
			// Apparently SCIP does not like x1 + x2 - 1 <= 0 for binary variables
			// but x1 + x2 <= 1 works fine.
			// So we use the second variant, only bring the variables from Rhs to Lhs, leave the bias there.
			var lhs = c.Lhs;
			var rhsValue = c.Rhs.Bias;
			if (!c.Rhs.IsConstant)
			{
				// bring all vars to lhs
				lhs -= c.Rhs;
				lhs.Bias = c.Lhs.Bias;
			}

			Encode(lhs, sb);
			Encode(c.Operator, sb);
			sb.Append(" " + rhsValue.ToString("+0;-#", CultureInfo.InvariantCulture));
		}

		private static string Encode(Constraint c)
		{
			var sb = new StringBuilder();
			Encode(c, sb);
			return sb.ToString();
		}

		internal static void Save(Problem problem, StreamWriter sw)
		{
			sw.WriteLine(problem.Mode);
			sw.WriteLine(Encode(problem.Objective));
			sw.WriteLine("Subject to");
            foreach (var constraint in problem.Constraints)
            {
                sw.WriteLine(Encode(constraint));
            }

			sw.WriteLine("Bounds");
			foreach (var v in problem.Variables.Values)
			{
				var binaryTrivial = (v.Type == VariableType.Binary && v.LowerBound <= 0 && v.UpperBound >= 1);
                if (!binaryTrivial)
                {
                    sw.WriteLine(v.LowerBound.ToString(CultureInfo.InvariantCulture) + " <= " + Encode(v) + " <= " + v.UpperBound.ToString(CultureInfo.InvariantCulture));
                }
			}

			sw.WriteLine("General");
			foreach (var v in problem.Variables.Values)
            {
                if (v.Type == VariableType.Integer)
                {
                    sw.WriteLine(Encode(v));
                }
            }

			sw.WriteLine("Binary");
			foreach (var v in problem.Variables.Values)
            {
                if (v.Type == VariableType.Binary)
                {
                    sw.WriteLine(Encode(v));
                }
            }

			sw.WriteLine("End");
		}
	}
}
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
	/// The weighted sum of some variables, plus an optional constant.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Expressions can be combined with Variables, Expressions and constants using the + - * / operators
	/// to yield another Expression.
	/// </para>
	/// <para>
	/// Expressions can be compared with Variables, Expressions and constants using the &lt;, &lt;=, &gt;, &gt;= or = operators
    /// to yield a Constraint. Note that these operators do not compare the Expression objects themselves,
    /// yielding bool values, but they create Constraint objects describing the relationship of two Expressions.
	/// </para>
	/// </remarks>
	public class Expression
	{
        /// <summary>
		/// Multiplication factors for each variable that comprises this expression.
		/// </summary>
		public Dictionary<Variable, double> Coefficients;

		/// <summary>
		/// The constant summand in the expression.
		/// </summary>
		public double Bias;

		// Initializes a constant expression.
		/// <summary>
		/// Initialize <see cref="Expression"/>.
		/// </summary>
		/// <param name="bias"></param>
		public Expression(double bias = 0)
		{
			Coefficients = new();
			Bias = bias;
		}

		/// <summary>
		/// Makes a copy of the supplied <see cref="Expression"/>.
		/// </summary>
		/// <param name="x">The <see cref="Expression"/>.</param>
		public Expression(Expression x)
		{
			Coefficients = new();
            foreach (var v in x.Coefficients.Keys)
            {
                Coefficients[v] = x.Coefficients[v];
            }

			Bias = x.Bias;
		}

		/// <summary>
		/// Is constant.
		/// </summary>
		public bool IsConstant => Coefficients.Count == 0;

		/// <summary>
		/// Is zero.
		/// </summary>
        public bool IsZero => Coefficients.Count == 0 && Bias == 0;

		/// <summary>
		/// Add.
		/// </summary>
		/// <param name="v"><see cref="Variable"/>.</param>
		/// <param name="coeff">Coefficient.</param>
        public void Add(Variable v, double coeff)
		{
            if (Coefficients.ContainsKey(v))
            {
                Coefficients[v] += coeff;
            }
            else
            {
                Coefficients[v] = coeff;
            }
		}

		/// <summary>
		/// Implicit operator for <see cref="Variable"/>.
		/// </summary>
		/// <param name="x">The operand.</param>
		public static implicit operator Expression(Variable x)
		{
			var expr = new Expression
            {
                Coefficients =
                {
                    [x] = 1
                }
            };
            return expr;
		}

		/// <summary>
		/// Implicit operator for double.
		/// </summary>
		/// <param name="x">The operand.</param>
		public static implicit operator Expression(double x)
		{
			return new(x);
		}

		/// <summary>
		/// - operator.
		/// </summary>
		/// <param name="x">The operand.</param>
        public static Expression operator -(Expression x)
		{
			var expr = new Expression();
            foreach (var v in x.Coefficients.Keys)
            {
                expr.Coefficients[v] = -x.Coefficients[v];
            }

			expr.Bias = -x.Bias;
			return expr;
		}

		/// <summary>
		/// + operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="q">The second operand.</param>
        public static Expression operator +(Expression x, double q)
		{
			var expr = new Expression(x);
			expr.Bias += q;
			return expr;
		}

		/// <summary>
		/// + operator.
		/// </summary>
		/// <param name="q">The first operand.</param>
		/// <param name="x">The second operand.</param>
        public static Expression operator +(double q, Expression x)
		{
			return x + q;
		}

		/// <summary>
		/// + operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="v">The second operand.</param>
        public static Expression operator +(Expression x, Variable v)
		{
			var expr = new Expression(x);
			expr.Add(v, 1);
			return expr;
		}

		/// <summary>
		/// + operator.
		/// </summary>
		/// <param name="v">The first operand.</param>
		/// <param name="x">The second operand.</param>
        public static Expression operator +(Variable v, Expression x)
		{
			return x + v;
		}

		/// <summary>
		/// + operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="y">The second operand.</param>
        public static Expression operator +(Expression x, Expression y)
		{
            if (y.IsZero)
            {
                return x;
            }

            if (x.IsZero)
            {
                return y;
            }

			var expr = new Expression(x);
            foreach (var kv in y.Coefficients)
            {
                expr.Add(kv.Key, kv.Value);
            }

			expr.Bias += y.Bias;
			return expr;
		}

		/// <summary>
		/// - operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="q">The second operand.</param>
        public static Expression operator -(Expression x, double q)
		{
			var expr = new Expression(x);
			expr.Bias -= q;
			return expr;
		}

		/// <summary>
		/// - operator.
		/// </summary>
		/// <param name="q">The first operand.</param>
		/// <param name="x">The second operand.</param>
        public static Expression operator -(double q, Expression x)
		{
			var expr = -x;
			expr.Bias += q;
			return expr;
		}

		/// <summary>
		/// - operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="v">The second operand.</param>
        public static Expression operator -(Expression x, Variable v)
		{
			var expr = new Expression(x);
			expr.Add(v, -1);
			return expr;
		}

		/// <summary>
		/// - operator.
		/// </summary>
		/// <param name="v">The first operand.</param>
		/// <param name="x">The second operand.</param>
		public static Expression operator -(Variable v, Expression x)
		{
			var expr = -x;
			expr.Add(v, 1);
			return expr;
		}

		/// <summary>
		/// - operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="y">The second operand.</param>
		public static Expression operator -(Expression x, Expression y)
		{
            if (y.IsZero)
            {
                return x;
            }

			var expr = new Expression(x);
            foreach (var kv in y.Coefficients)
            {
                expr.Add(kv.Key, -kv.Value);
            }

			expr.Bias -= y.Bias;
			return expr;
		}

		/// <summary>
		/// * operator.
		/// </summary>
		/// <param name="q">The first operand.</param>
		/// <param name="x">The second operand.</param>
		public static Expression operator *(double q, Expression x)
		{
			if (q == 0)
			{
				return new();
			}
			else
			{
				var expr = new Expression();
                foreach (var v in x.Coefficients.Keys)
                {
                    expr.Coefficients[v] = q * x.Coefficients[v];
                }

				expr.Bias = q * x.Bias;
				return expr;
			}
		}

		/// <summary>
		/// * operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="q">The second operand.</param>
		public static Expression operator *(Expression x, double q)
		{
			return q * x;
		}

		/// <summary>
		/// / operator.
		/// </summary>
		/// <remarks>
		/// The divisor q must not be zero.
		/// </remarks>
		/// <param name="x">The first operand.</param>
		/// <param name="q">Divisor.</param>
        public static Expression operator /(Expression x, double q)
		{
			return (1.0 / q) * x;
		}

		/// <summary>
		/// &lt; operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="y">The second operand.</param>
        public static Constraint operator <(Expression x, Expression y)
		{
			return new(x, ComparisonOperator.Less, y);
		}

		/// <summary>
		/// &lt;= operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="y">The second operand.</param>
        public static Constraint operator <=(Expression x, Expression y)
		{
			return new(x, ComparisonOperator.LessEqual, y);
		}

		/// <summary>
		/// &gt; operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="y">The second operand.</param>
        public static Constraint operator >(Expression x, Expression y)
		{
			return new(x, ComparisonOperator.Greater, y);
		}

		/// <summary>
		/// &gt;= operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="y">The second operand.</param>
        public static Constraint operator >=(Expression x, Expression y)
		{
			return new(x, ComparisonOperator.GreaterEqual, y);
		}

		/// <summary>
		/// == operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="y">The second operand.</param>
        public static Constraint operator ==(Expression x, Expression y)
		{
			return new(x, ComparisonOperator.Equal, y);
		}

        /// <summary>
        /// != operator.
        /// </summary>
        /// <param name="x">The first operand.</param>
        /// <param name="y">The second operand.</param>
        /// <exception cref="InvalidOperationException">Not-equal LP constraints are not supported.</exception>
        public static Constraint operator !=(Expression x, Expression y)
        {
            throw new InvalidOperationException("Not-equal LP constraints are not supported.");
		}
	}
}
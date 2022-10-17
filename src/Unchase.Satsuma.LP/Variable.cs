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

using Unchase.Satsuma.Core;
using Unchase.Satsuma.LP.Enums;

namespace Unchase.Satsuma.LP
{
    /// <summary>
	/// A linear programming variable.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Variables can be combined with other Variables, Expressions and constants using the + - * / operators
	/// to yield an Expression.
	/// </para>
	/// <para>
	///Variables cannot be shared among Problem instances! A Variable created by a Problem can only be used
    /// in the same Problem.
	/// </para>
	/// </remarks>
	public class Variable
		: IEquatable<Variable>
	{
        /// <summary>
		/// The identifier of the Variable can be any object (but not null).
		/// </summary>
		/// <remarks>
		/// <para>This way, variables can be associated with any kinds of objects (e.g. <see cref="Node"/>, <see cref="Arc"/>, etc.)</para>
		/// <para>Variables with Id's comparing equal are considered the same variable.</para>
        /// </remarks>
		public readonly object Id;

		/// <summary>
		/// An ordinal, assigned to the Variable by the Problem owning it.
		/// </summary>
		internal int SerialNumber;

		/// <summary>
		/// The type of this <see cref="Variable"/> (real, integer or binary). Default: real.
		/// </summary>
		public VariableType Type;

		/// <summary>
		/// The minimum allowed value for this <see cref="Variable"/>. Default: negative infinity.
		/// </summary>
		/// <remarks>
		/// Can be negative infinity or any finite number.
		/// </remarks>
		public double LowerBound;

		/// <summary>
		/// The maximum allowed value for this <see cref="Variable"/>. Default: positive infinity.
		/// </summary>
		/// <remarks>
		/// Can be positive infinity or any finite number.
		/// </remarks>
		public double UpperBound;

		/// <summary>
		/// Initialize <see cref="Variable"/>.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="serialNumber"></param>
		/// <exception cref="ArgumentException">Id cannot be null.</exception>
		internal Variable(object id, int serialNumber)
		{
            Id = id ?? throw new ArgumentException("LP.Variable.Id cannot be null");
			SerialNumber = serialNumber;
			Type = VariableType.Real;
			LowerBound = double.NegativeInfinity;
			UpperBound = double.PositiveInfinity;
		}

		/// <inheritdoc />
		public override bool Equals(object? obj)
		{
            if (obj is Variable variable)
            {
                return Equals(variable);
            }

			return base.Equals(obj);
		}

        /// <inheritdoc />
		public bool Equals(Variable? obj)
		{
			return Id.Equals(obj?.Id);
		}

        /// <inheritdoc />
		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

        /// <inheritdoc />
		public override string? ToString()
		{
			return Id.ToString();
		}

		/// <summary>
		/// - operator.
		/// </summary>
		/// <param name="x">The operand.</param>
        public static Expression operator -(Variable x)
		{
			var expr = new Expression
            {
                Coefficients =
                {
                    [x] = -1
                }
            };
            return expr;
		}

		/// <summary>
		/// + operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="y">The second operand.</param>
		public static Expression operator +(Variable x, Variable y)
		{
			var expr = new Expression
            {
                Coefficients =
                {
                    [x] = 1,
                    [y] = 1
                }
            };
            return expr;
		}

		/// <summary>
		/// + operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="q">The second operand.</param>
		public static Expression operator +(Variable x, double q)
		{
			var expr = new Expression
            {
                Coefficients =
                {
                    [x] = 1
                },
                Bias = q
            };
            return expr;
		}

		/// <summary>
		/// + operator.
		/// </summary>
		/// <param name="q">The first operand.</param>
		/// <param name="x">The second operand.</param>
		public static Expression operator +(double q, Variable x)
		{
			return x + q;
		}

		/// <summary>
		/// - operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="y">The second operand.</param>
		public static Expression operator -(Variable x, Variable y)
		{
			var expr = new Expression
            {
                Coefficients =
                {
                    [x] = 1,
                    [y] = -1
                }
            };
            return expr;
		}

		/// <summary>
		/// + operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="q">The second operand.</param>
		public static Expression operator -(Variable x, double q)
		{
			var expr = new Expression
            {
                Coefficients =
                {
                    [x] = 1
                },
                Bias = -q
            };
            return expr;
		}

		/// <summary>
		/// + operator.
		/// </summary>
		/// <param name="q">The first operand.</param>
		/// <param name="x">The second operand.</param>
		public static Expression operator -(double q, Variable x)
		{
			var expr = new Expression
            {
                Coefficients =
                {
                    [x] = -1
                },
                Bias = q
            };
            return expr;
		}

		/// <summary>
		/// * operator.
		/// </summary>
		/// <param name="q">The first operand.</param>
		/// <param name="x">The second operand.</param>
        public static Expression operator *(double q, Variable x)
		{
			var expr = new Expression
            {
                Coefficients =
                {
                    [x] = q
                }
            };
            return expr;
		}

		/// <summary>
		/// * operator.
		/// </summary>
		/// <param name="x">The first operand.</param>
		/// <param name="q">The second operand.</param>
        public static Expression operator *(Variable x, double q)
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
		public static Expression operator /(Variable x, double q)
		{
			return (1.0 / q) * x;
		}
	}
}
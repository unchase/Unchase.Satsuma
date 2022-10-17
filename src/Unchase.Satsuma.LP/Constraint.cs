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
    /// An equality or inequality: two expressions (left-hand side and right-hand side) joined by a comparison operator.
    /// </summary>
    public class Constraint
    {
        /// <summary>
        /// The left-hand side of the equality/inequality.
        /// </summary>
        public Expression Lhs;

        /// <summary>
        /// The operator for comparing the <see cref="Lhs"/> and the <see cref="Rhs"/>.
        /// </summary>
        public ComparisonOperator Operator;

        /// <summary>
        /// The right-hand side of the equality/inequality.
        /// </summary>
        public Expression Rhs;

        /// <summary>
        /// Initialize <see cref="Constraint"/>
        /// </summary>
        public Constraint()
        {
            Lhs = new();
            Operator = ComparisonOperator.Equal;
            Rhs = new();
        }

        /// <summary>
        /// Initialize <see cref="Constraint"/>
        /// </summary>
        /// <param name="lhs"><see cref="Lhs"/>.</param>
        /// <param name="operator"><see cref="Operator"/></param>
        /// <param name="rhs"><see cref="Rhs"/>.</param>
        public Constraint(
            Expression lhs, 
            ComparisonOperator @operator, 
            Expression rhs)
        {
            Lhs = lhs;
            Operator = @operator;
            Rhs = rhs;
        }
    }
}
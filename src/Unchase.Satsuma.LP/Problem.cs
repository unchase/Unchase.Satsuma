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
    /// A linear, integer or mixed integer programming problem.
    /// </summary>
    /// <remarks>
    /// Be aware that variables cannot be shared among Problems.
    /// </remarks>
    public class Problem
    {
        /// <summary>
        /// Describes whether the objective function should be minimized or maximized. Default: <see cref="OptimizationMode.Minimize"/>.
        /// </summary>
        public OptimizationMode Mode;

        /// <summary>
        /// The objective function. Constant zero by default, can be modified.
        /// </summary>
        public Expression Objective;

        /// <summary>
        /// The constraints, subject to which the objective should be minimized/maximized.
        /// </summary>
        public List<Constraint> Constraints;

        /// <summary>
        /// The variables used in the Objective and Constraints, stored in a dictionary by id.
        /// </summary>
        /// <remarks>
        /// Variables cannot be shared among Problems.
        /// </remarks>
        internal Dictionary<object, Variable> Variables;

        internal List<Variable> VariablesBySerialNumber;

        /// <summary>
        /// Initialize <see cref="Problem"/>.
        /// </summary>
        public Problem()
        {
            Mode = OptimizationMode.Minimize;
            Objective = new();
            Constraints = new();
            Variables = new();
            VariablesBySerialNumber = new();
        }

        /// <summary>
        /// Looks up an existing variable by its Id or creates a new one.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <exception cref="InvalidOperationException">Number of LP variables exceeds limit (2^31-1).</exception>
        public Variable GetVariable(object id)
        {
            if (!Variables.TryGetValue(id, out var result))
            {
                if (Variables.Count == int.MaxValue)
                {
                    throw new InvalidOperationException("Number of LP variables exceeds limit (2^31-1).");
                }

                result = new(id, Variables.Count);
                Variables[id] = result;
                VariablesBySerialNumber.Add(result);
            }

            return result;
        }
    }
}
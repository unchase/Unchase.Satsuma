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

namespace Unchase.Satsuma.LP.Enums
{
    /// <summary>
    /// Indicates the validity and optimality of an LP Solution.
    /// </summary>
    public enum SolutionType
    {
        /// <summary>
        /// The solution is invalid, but there may be a valid solution to the problem.
        /// </summary>
        /// <remarks>
        /// Indicates that the solver was unable to find either a valid solution or a proof that no valid solution exists.
        /// </remarks>
        Invalid,

        /// <summary>
        /// The solution is invalid, and there is no valid solution to the problem.
        /// </summary>
        Infeasible,

        /// <summary>
        /// The solution is valid, but the objective function is unbounded.
        /// </summary>
        /// <remarks>
        /// This means that there exist infinitely many solutions but there is no optimal one.
        /// </remarks>
        Unbounded,

        /// <summary>
        /// The solution is valid but may or may not be optimal.
        /// </summary>
        Feasible,

        /// <summary>
        /// The solution is valid and optimal.
        /// </summary>
        Optimal,
    }
}
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
# endregion

namespace Unchase.Satsuma.Algorithms.Enums
{
    /// <summary>
    /// Corresponds to the states of the two-phase primal simplex algorithm.
    /// </summary>
    public enum SimplexState
    {
        /// <summary>
        /// The first phase (finding a feasible solution) is still running.
        /// </summary>
        FirstPhase,

        /// <summary>
        /// No feasible solution exists as deduced by the first phase.
        /// </summary>
        Infeasible,

        /// <summary>
        /// The second phase (finding an optimal solution) is still running.
        /// </summary>
        SecondPhase,

        /// <summary>
        /// The value of the objective function was found to be unbounded.
        /// </summary>
        Unbounded,

        /// <summary>
        /// The current solution is optimal.
        /// </summary>
        Optimal
    }
}
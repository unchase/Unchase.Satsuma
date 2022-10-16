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

namespace Unchase.Satsuma.Core.Contracts
{
    /// <summary>
    /// Interface to a read-only disjoint-set data structure.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
    public interface IReadOnlyDisjointSet<T>
        where T : IEquatable<T>
    {
        /// <summary>
        /// Get the set where the given element belongs.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Returns the set where the given element belongs.</returns>
        public DisjointSetSet<T> WhereIs(T element);

        /// <summary>
        /// Get the elements of a set.
        /// </summary>
        /// <param name="aSet"><see cref="DisjointSetSet{T}"/>.</param>
        /// <returns>Returns the elements of a set.</returns>
        public IEnumerable<T> Elements(DisjointSetSet<T> aSet);
    }
}
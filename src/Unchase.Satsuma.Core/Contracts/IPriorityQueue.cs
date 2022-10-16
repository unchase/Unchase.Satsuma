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
   distribution.*/
# endregion

namespace Unchase.Satsuma.Core.Contracts
{
    /// <summary>
    /// Interface to a priority queue which does not allow duplicate elements.
    /// </summary>
    /// <remarks>Elements with lower priorities are prioritized more.</remarks>
    /// <typeparam name="TElement">Element type.</typeparam>
    /// <typeparam name="TPriority">Priority type.</typeparam>
    public interface IPriorityQueue<TElement, TPriority>
        : IReadOnlyPriorityQueue<TElement, TPriority>, IClearable
    {
        /// <summary>
        /// Gets or sets the priority of an element.
        /// </summary>
        /// <param name="element">Element.</param>
        public TPriority? this[TElement element] { get; set; }

        /// <summary>
        /// Removes a certain element from the queue, if present.
        /// </summary>
        /// <param name="element">Element.</param>
        /// <returns>Returns true if the given element was present in the queue.</returns>
        public bool Remove(TElement element);

        /// <summary>
        /// Removes the most prioritized element from the queue, if it is not empty.
        /// </summary>
        /// <returns>Returns true if an element could be removed, i.e. the queue was not empty.</returns>
        public bool Pop();
    }
}
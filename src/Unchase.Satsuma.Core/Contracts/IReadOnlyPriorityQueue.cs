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
    /// Interface to a read-only priority queue.
    /// </summary>
    /// <remarks>
    /// Elements with lower priorities are prioritized more.
    /// </remarks>
    /// <typeparam name="TElement">Element type.</typeparam>
    /// <typeparam name="TPriority">Priority type.</typeparam>
    public interface IReadOnlyPriorityQueue<TElement, TPriority>
    {
        /// <summary>
        /// The count of elements currently in the queue.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Returns all the element-priority pairs.
        /// </summary>
        public IEnumerable<KeyValuePair<TElement, TPriority?>> Items { get; }

        /// <summary>
        /// Returns whether the specified element is in the priority queue.
        /// </summary>
        /// <param name="element">Element.</param>
        public bool Contains(TElement element);

        /// <summary>
        /// Gets the priority of an element without throwing an exception.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="priority">Becomes default(P) if the element is not in the queue, and the priority of the element otherwise.</param>
        /// <returns>Returns true if the specified element is in the priority queue.</returns>
        public bool TryGetPriority(TElement element, out TPriority? priority);

        /// <summary>
        /// Returns the most prioritized element (that is, which has the lowest priority).
        /// </summary>
        public TElement Peek();

        /// <summary>
        /// Returns the most prioritized element (that is, which has the lowest priority) and its priority.
        /// </summary>
        /// <param name="priority">Priority.</param>
        public TElement Peek(out TPriority? priority);
    }
}
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
#endregion

namespace Unchase.Satsuma.Adapters.Abstractions
{
    /// <summary>
    /// Allocates integer identifiers.
    /// </summary>
    internal abstract class IdAllocator
    {
        private long _randomSeed;
        private long _lastAllocated;

        /// <summary>
        /// Initialize <see cref="IdAllocator"/>.
        /// </summary>
        protected IdAllocator()
        {
            _randomSeed = 205891132094649; // 3^30
            Rewind();
        }

        private long Random()
        {
            return (_randomSeed *= 3);
        }

        /// <summary>
        /// Is the given identifier already allocated.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <returns>Returns true if the given identifier is already allocated.</returns>
        protected abstract bool IsAllocated(long id);

        /// <summary>
        /// The allocator will try to allocate the next identifier from 1.
        /// </summary>
        public void Rewind()
        {
            _lastAllocated = 0;
        }

        /// <summary>
        /// Allocates and returns a new identifier.
        /// </summary>
        /// <remarks>
        /// Must not be called if the number of currently allocated identifiers is at least int.MaxValue.
        /// </remarks>
        /// <returns></returns>
        public long Allocate()
        {
            var id = _lastAllocated + 1;
            var streak = 0;
            while (true)
            {
                if (id == 0) id = 1;
                if (!IsAllocated(id))
                {
                    _lastAllocated = id;
                    return id;
                }

                id++;
                streak++;
                if (streak >= 100)
                {
                    id = Random();
                    streak = 0;
                }
            }
        }
    }
}
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

using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Core
{
    /// <summary>
    /// Implementation of the disjoint-set data structure.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class DisjointSet<T> : 
        IDisjointSet<T>
            where T : IEquatable<T>
    {
        private readonly Dictionary<T, T> _parent;

        /// <summary>
        /// The first child of a representative, or the next sibling of a child.
        /// </summary>
        private readonly Dictionary<T, T> _next;

        /// <summary>
        /// The last child of a representative.
        /// </summary>
        private readonly Dictionary<T, T> _last;
        private readonly List<T> _tmpList;

        /// <summary>
        /// Initialize <see cref="DisjointSet{T}"/>.
        /// </summary>
        public DisjointSet()
        {
            _parent = new();
            _next = new();
            _last = new();
            _tmpList = new();
        }

        /// <inheritdoc />
        public void Clear()
        {
            _parent.Clear();
            _next.Clear();
            _last.Clear();
        }

        /// <inheritdoc />
        public DisjointSetSet<T> WhereIs(T element)
        {
            while (true)
            {
                if (!_parent.TryGetValue(element, out var p))
                {
                    foreach (var a in _tmpList) _parent[a] = element;
                    _tmpList.Clear();
                    return new(element);
                }
                else
                {
                    _tmpList.Add(element);
                    element = p;
                }
            }
        }

        private T GetLast(T x)
        {
            if (_last.TryGetValue(x, out var y))
            {
                return y;
            }

            return x;
        }

        /// <inheritdoc />
        public DisjointSetSet<T> Union(DisjointSetSet<T> a, DisjointSetSet<T> b)
        {
            var x = a.Representative;
            var y = b.Representative;

            if (!x.Equals(y))
            {
                _parent[x] = y;
                _next[GetLast(y)] = x;
                _last[y] = GetLast(x);
            }

            return b;
        }

        /// <inheritdoc />
        public IEnumerable<T> Elements(DisjointSetSet<T> aSet)
        {
            var element = aSet.Representative;
            while (true)
            {
                yield return element;
                if (!_next.TryGetValue(element, out element))
                {
                    break;
                }
            }
        }
    }
}
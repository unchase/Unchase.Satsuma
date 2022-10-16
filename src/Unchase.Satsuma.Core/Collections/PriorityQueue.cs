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

using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Core.Collections
{
    /// <summary>
	/// A heap-based no-duplicates priority queue implementation.
	/// </summary>
	/// <typeparam name="TElement">Element type.</typeparam>
	/// <typeparam name="TPriority">Priority type.</typeparam>
	public sealed class PriorityQueue<TElement, TPriority> : 
        IPriorityQueue<TElement, TPriority>
		    where TPriority : IComparable<TPriority> 
            where TElement : notnull
    {
		private readonly List<TElement> _payloads = new();
		private readonly List<TPriority?> _priorities = new();
		private readonly Dictionary<TElement, int> _positions = new();

        /// <inheritdoc />
		public void Clear()
		{
			_payloads.Clear();
			_priorities.Clear();
			_positions.Clear();
		}

        /// <inheritdoc />
		public int Count => _payloads.Count;

        /// <inheritdoc />
		public IEnumerable<KeyValuePair<TElement, TPriority?>> Items
		{
			get
			{
                for (int i = 0, n = Count; i < n; i++)
                {
                    yield return new(_payloads[i], _priorities[i]);
                }
			}
		}

        /// <inheritdoc />
		public TPriority? this[TElement element]
		{
			get => _priorities[_positions[element]];

            set
			{
                if (_positions.TryGetValue(element, out var pos))
				{
					var oldPriority = _priorities[pos];
					_priorities[pos] = value;
					var priorityDelta = value?.CompareTo(oldPriority);
                    if (priorityDelta < 0)
                    {
                        MoveUp(pos);
                    }
					else if (priorityDelta > 0)
                    {
                        MoveDown(pos);
                    }
				}
				else
				{
					_payloads.Add(element);
					_priorities.Add(value);
					pos = Count - 1;
					_positions[element] = pos;
					MoveUp(pos);
				}
			}
		}

        /// <inheritdoc />
		public bool Contains(TElement element)
		{
			return _positions.ContainsKey(element);
		}

        /// <inheritdoc />
		public bool TryGetPriority(TElement element, out TPriority? priority)
		{
            if (!_positions.TryGetValue(element, out var pos))
			{
				priority = default;
				return false;
			}
			priority = _priorities[pos];
			return true;
		}

		private void RemoveAt(int pos)
		{
			var count = Count;
			var oldPayload = _payloads[pos];
			var oldPriority = _priorities[pos];
			_positions.Remove(oldPayload);

			var empty = (count <= 1);
			if (!empty && pos != count - 1)
			{
				// move the last element up to this place
				_payloads[pos] = _payloads[count - 1];
				_priorities[pos] = _priorities[count - 1];
				_positions[_payloads[pos]] = pos;
			}

			// delete the last element
			_payloads.RemoveAt(count - 1);
			_priorities.RemoveAt(count - 1);

			if (!empty && pos != count - 1)
			{
				var priorityDelta = _priorities[pos]?.CompareTo(oldPriority);
                if (priorityDelta > 0)
                {
                    MoveDown(pos);
                }
				else if (priorityDelta < 0)
                {
                    MoveUp(pos);
                }
			}
		}

        /// <inheritdoc />
		public bool Remove(TElement element)
		{
            var success = _positions.TryGetValue(element, out var pos);
            if (success)
            {
                RemoveAt(pos);
            }

			return success;
		}

        /// <inheritdoc />
		public TElement Peek()
		{
			return _payloads[0];
		}

        /// <inheritdoc />
		public TElement Peek(out TPriority? priority)
		{
			priority = _priorities[0];
			return _payloads[0];
		}

		/// <inheritdoc />
		public bool Pop()
		{
            if (Count == 0)
            {
                return false;
            }

			RemoveAt(0);
			return true;
		}

		private void MoveUp(int index)
		{
			var payload = _payloads[index];
			var priority = _priorities[index];

			var i = index;
			while (i > 0)
			{
				var parent = i / 2;
                if (priority?.CompareTo(_priorities[parent]) >= 0)
                {
                    break;
                }

				_payloads[i] = _payloads[parent];
				_priorities[i] = _priorities[parent];
				_positions[_payloads[i]] = i;

				i = parent;
			}

			if (i != index)
			{
				_payloads[i] = payload;
				_priorities[i] = priority;
				_positions[payload] = i;
			}
		}

		private void MoveDown(int index)
		{
			var payload = _payloads[index];
			var priority = _priorities[index];

			var i = index;
			while (2 * i < Count)
			{
				var min = i;
				var minPriority = priority;

				var child = 2 * i;
				if (minPriority?.CompareTo(_priorities[child]) >= 0)
				{
					min = child;
					minPriority = _priorities[child];
				}

				child++;
				if (child < Count && minPriority?.CompareTo(_priorities[child]) >= 0)
				{
					min = child;
					minPriority = _priorities[child];
				}

				if (min == i) break;

				_payloads[i] = _payloads[min];
				_priorities[i] = minPriority;
				_positions[_payloads[i]] = i;

				i = min;
			}

			if (i != index)
			{
				_payloads[i] = payload;
				_priorities[i] = priority;
				_positions[payload] = i;
			}
		}
	}
}
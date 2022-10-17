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

using System.Diagnostics;
using System.Xml.Linq;

namespace Unchase.Satsuma.Core
{
    /// <summary>
	/// Various utilities used by other classes.
	/// </summary>
	/// <remarks>
	/// This class was made public because the functions can prove useful for Satsuma users,
    /// but there is no guarantee that the interfaces will remain the same in the future.
	/// </remarks>
	public static class Utils
	{
        /// <summary>
		/// Returns the first child element that matches the given local name, or null if none found.
		/// </summary>
		/// <param name="xParent"><see cref="XElement"/>.</param>
		/// <param name="localName">Local name.</param>
        internal static XElement? ElementLocal(
            XElement xParent, 
            string localName)
		{
			return ElementsLocal(xParent, localName)
                .FirstOrDefault();
		}

		/// <summary>
		/// Get all child elements filtered by local name.
		/// </summary>
		/// <param name="xParent"><see cref="XElement"/>.</param>
		/// <param name="localName">Local name.</param>
		/// <returns>Returns all child elements filtered by local name.</returns>
		internal static IEnumerable<XElement> ElementsLocal(
            XElement xParent, 
            string localName)
		{
			return xParent.Elements()
                .Where(x => x.Name.LocalName == localName);
		}

		/// <summary>
		/// Executes a program with a given list of arguments.
		/// </summary>
        /// <param name="filename">File name.</param>
		/// <param name="processArgs">Process arguments.</param>
		/// <param name="timeoutSeconds">Timeout in seconds.</param>
		/// <param name="silent">Silent mode.</param>
		/// <returns>Returns true in process completed execution, false if an error occurred or execution timed out.</returns>
		internal static bool ExecuteCommand(
            string filename,
            string processArgs,
            int timeoutSeconds = 0,
			bool silent = false)
		{
			try
			{
				var processStartInfo = new ProcessStartInfo(filename, processArgs)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };

                var process = new Process
                {
                    StartInfo = processStartInfo
                };

                if (!silent)
				{
					process.OutputDataReceived += (_, a) => Console.WriteLine(a.Data);
					process.ErrorDataReceived += (_, a) => Console.Error.WriteLine(a.Data);
				}

				process.Start();

				if (!silent)
				{
					process.BeginOutputReadLine();
					process.BeginErrorReadLine();
				}

				if (timeoutSeconds != 0)
				{
					var tStart = Environment.TickCount;
					while (!process.HasExited && Environment.TickCount - tStart < timeoutSeconds * 1000)
					{
						Thread.Sleep(100);
					}

					if (!process.HasExited)
					{
						process.Kill();
                        while (!process.HasExited)
                        {
                            Thread.Sleep(100);
						}

						return false;
					}
				}

				// TODO we could return these
				//string stdout = p.StandardOutput.ReadToEnd();
				//string stderr = p.StandardError.ReadToEnd();
				process.WaitForExit();
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Get the largest power of two which is at most Math.Abs(value), or 0 if none exists.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Returns the largest power of two which is at most Math.Abs(d), or 0 if none exists.</returns>
		public static double LargestPowerOfTwo(double value)
		{
			var bits = BitConverter.DoubleToInt64Bits(value);
			bits &= 0x7FF0000000000000; // clear mantissa
			if (bits == 0x7FF0000000000000) 
            {
                bits = 0x7FE0000000000000; // deal with infinity
            }

			return BitConverter.Int64BitsToDouble(bits);
		}

		/// <summary>
		/// Make entry.
		/// </summary>
		/// <typeparam name="TKey">Key type.</typeparam>
		/// <typeparam name="TValue">Value type.</typeparam>
		/// <param name="dict">Dictionary.</param>
		/// <param name="key">Key.</param>
        public static TValue MakeEntry<TKey, TValue>(
            Dictionary<TKey, TValue> dict, 
            TKey key)
			    where TValue : new() 
                where TKey : notnull
        {
            if (dict.TryGetValue(key, out var result))
            {
                return result;
            }

			return dict[key] = new();
		}

		/// <summary>
		/// Remove all elements from the list by condition.
		/// </summary>
		/// <remarks>
		/// May reorder the elements.
		/// </remarks>
		/// <typeparam name="T">Type of elements.</typeparam>
		/// <param name="list">List.</param>
		/// <param name="condition">Condition.</param>
		public static void RemoveAll<T>(
            List<T> list, 
            Func<T, bool> condition)
		{
			for (var i = 0; i < list.Count; ++i)
			{
				if (condition(list[i]))
				{
					if (i < list.Count - 1)
					{
						list[i] = list[^1];
						i--;
					}

					list.RemoveAt(list.Count - 1);
				}
			}
		}

		/// <summary>
		/// Remove all elements from the hash set by condition.
		/// </summary>
		/// <typeparam name="T">Type of elements.</typeparam>
		/// <param name="set">Set.</param>
		/// <param name="condition">Condition.</param>
		public static void RemoveAll<T>(
            HashSet<T> set, 
            Func<T, bool> condition)
		{
            foreach (var x in set.Where(condition).ToList())
            {
                set.Remove(x);
			}
        }

		/// <summary>
		/// Remove all elements from the dictionary by condition.
		/// </summary>
		/// <typeparam name="TKey">Type of keys.</typeparam>
		/// <typeparam name="TValue">Type of values.</typeparam>
		/// <param name="dictionary">Dictionary.</param>
		/// <param name="condition">Condition.</param>
		public static void RemoveAll<TKey, TValue>(
            Dictionary<TKey, TValue> dictionary, 
            Func<TKey, bool> condition)
                where TKey : notnull
        {
            foreach (var key in dictionary.Keys.Where(condition).ToList())
            {
                dictionary.Remove(key);
            }
		}

		/// <summary>
		/// Remove last element from the list.
		/// </summary>
		/// <typeparam name="T">Type of elements.</typeparam>
		/// <param name="list">List.</param>
		/// <param name="element">Element.</param>
		public static void RemoveLast<T>(
            List<T> list, 
            T element)
			    where T : IEquatable<T>
		{
			for (var i = list.Count - 1; i >= 0; i--)
			{
				if (element.Equals(list[i]))
				{
					list.RemoveAt(i);
					break;
				}
			}
		}

		// 
		/// <summary>
		/// Implements a random-seeming but deterministic 1-to-1 mapping on ulong.
		/// </summary>
		/// <param name="x">Value.</param>
        public static ulong ReversibleHash1(ulong x)
		{
			x += 11633897956271718833UL;
			x *= 8363978825398262719UL;
			x ^= x >> 13;
			return x;
		}

		/// <summary>
		/// Implements a random-seeming but deterministic 1-to-1 mapping on ulong.
		/// </summary>
		/// <param name="x">Value.</param>
        public static ulong ReversibleHash2(ulong x)
		{
			x += 2254079448387046741UL;
			x *= 16345256107010564221UL;
			x ^= x << 31;
			return x;
		}

		/// <summary>
		/// Implements a random-seeming but deterministic 1-to-1 mapping on ulong.
		/// </summary>
		/// <param name="x">Value.</param>
        public static ulong ReversibleHash3(ulong x)
		{
			x += 5687820266445524563UL;
			x *= 15961264385709064403UL;
			x ^= x >> 19;
			return x;
		}

		/// <summary>
		/// Swap elements.
		/// </summary>
		/// <typeparam name="T">Type of elements.</typeparam>
		/// <param name="x">The first element.</param>
		/// <param name="y">The second element.</param>
		public static void Swap<T>(
            ref T x,
            ref T y)
		{
			(x, y) = (y, x);
        }

		/// <summary>
		/// Swap elements in list by their indexes.
		/// </summary>
		/// <typeparam name="T">Type of elements.</typeparam>
		/// <param name="list">List.</param>
		/// <param name="index1">The first element index.</param>
		/// <param name="index2">The second element index.</param>
		public static void Swap<T>(IList<T> list, int index1, int index2)
		{
			if (index1 != index2)
			{
				(list[index1], list[index2]) = (list[index2], list[index1]);
            }
		}

		/// <summary>
		/// Shuffle elements.
		/// </summary>
		/// <typeparam name="T">Type of elements.</typeparam>
		/// <param name="rng"><see cref="Random"/>.</param>
		/// <param name="array">Array.</param>
		public static void Shuffle<T>(Random rng, IList<T> array)
		{
			for (var n = array.Count - 1; n > 0; --n)
			{
				Swap(array, rng.Next(n + 1), n);
			}
		}
	}
}
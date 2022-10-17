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

using System.Xml.Linq;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.IO.GraphML.Abstractions
{
    /// <summary>
	/// A property which can store values in a dictionary.
	/// </summary>
	/// <typeparam name="T">The type.</typeparam>
	public abstract class DictionaryProperty<T> : 
        GraphMLProperty, 
        IClearable
	{
        /// <summary>
		/// True if <see cref="DefaultValue"/> should be taken into account as the default value for this property.
		/// </summary>
		public bool HasDefaultValue { get; set; }

		/// <summary>
		/// The default value of the property. Undefined if <see cref="HasDefaultValue"/> is \c false.
		/// </summary>
		public T? DefaultValue { get; set; }

		/// <summary>
		/// The values of the property for the individual objects.
		/// </summary>
		/// <remarks>
		/// <para>Keys must be of type <see cref="Node"/>, <see cref="Arc"/> or <see cref="IGraph"/>, as specified by #Domain.</para>
		/// <para>This dictionary need not contain entries for all objects (e.g. nodes, arcs).</para>
		/// </remarks>
		public Dictionary<object, T> Values { get; }

		/// <summary>
		/// Initialize <see cref="DictionaryProperty{T}"/>.
		/// </summary>
		protected DictionaryProperty()
		{
			HasDefaultValue = false;
			Values = new();
		}

		/// <summary>
		/// Clears all values (including the default value) stored by the property.
		/// </summary>
		public void Clear()
		{
			HasDefaultValue = false;
			Values.Clear();
		}

		/// <summary>
		/// Tries to get the property value for a given object.
		/// </summary>
		/// <remarks>
		/// First, key is looked up in <see cref="Values"/>. If not found, <see cref="DefaultValue"/> is used, unless <see cref="HasDefaultValue"/> is false.
		/// </remarks>
		/// <param name="key">A <see cref="Node"/>, <see cref="Arc"/> or <see cref="IGraph"/>.</param>
		/// <param name="result">The property value assigned to the key is returned here, or <tt>default(T)</tt> if none found.</param>
		/// <returns>Returns true if key was found as a key in <see cref="Values"/>, or <see cref="HasDefaultValue"/> is true.</returns>
		public bool TryGetValue(object key, out T? result)
		{
            if (Values.TryGetValue(key, out result))
            {
                return true;
            }

			if (HasDefaultValue)
			{
				result = DefaultValue;
				return true;
			}

			result = default;
			return false;
		}

		/// <summary>
		/// Get element by key.
		/// </summary>
		/// <param name="key">Key.</param>
        public T? this[object key]
		{
			get
			{
                TryGetValue(key, out var result);
				return result;
			}
		}

		/// <inheritdoc />
		public override void ReadData(XElement? x, object? key)
		{
			if (x == null)
			{
				// erase
                if (key == null)
                {
                    HasDefaultValue = false;
                }
                else
                {
                    Values.Remove(key);
                }
			}
			else
			{
				// load
				var value = ReadValue(x);
				if (key == null)
				{
					HasDefaultValue = true;
					DefaultValue = value;
				}
                else
                {
                    Values[key] = value;
                }
			}
		}

        /// <inheritdoc />
		public override XElement? WriteData(object? key)
		{
			if (key == null)
			{
				return HasDefaultValue 
                    ? WriteValue(DefaultValue) 
                    : null;
			}
			else
			{
                if (!Values.TryGetValue(key, out var value))
                {
                    return null;
                }

				return WriteValue(value);
			}
		}

		/// <summary>
		/// Parses an XML value definition.
		/// </summary>
		/// <param name="x">A non-null <tt>&lt;data&gt;</tt> or <tt>&lt;default&gt;</tt> element compatible with the property.</param>
		/// <returns>Returns the parsed value.</returns>
		protected abstract T ReadValue(XElement x);
 
		/// <summary>
		/// Writes an XML value definition.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>Returns a data element containing the definition of value.</returns>
		protected abstract XElement? WriteValue(T? value);
	}
}
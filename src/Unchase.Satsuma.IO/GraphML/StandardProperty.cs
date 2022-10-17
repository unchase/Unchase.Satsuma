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

using System.Globalization;
using System.Xml.Linq;
using Unchase.Satsuma.IO.GraphML.Abstractions;
using Unchase.Satsuma.IO.GraphML.Enums;

// ReSharper disable InconsistentNaming

namespace Unchase.Satsuma.IO.GraphML
{
    /// <summary>
	/// Represents a standard GraphML property (attribute), which may assign primitive values to objects.
	/// </summary>
	/// <remarks>
	/// <para>Example: <b>Assigning string values to nodes</b></para>
	/// <para>
	/// <code>
	/// using GraphML = Satsuma.IO.GraphML;
    /// // [...]
    /// GraphML.GraphMLFormat f = new GraphML.GraphMLFormat();
    /// var g = new CompleteGraph(4);
    /// f.Graph = g;
    /// var color = new GraphML.StandardProperty&lt;string&gt;
    ///		{ Name = "color", Domain = GraphML.PropertyDomain.Node,
    ///		  HasDefaultValue = true, DefaultValue = "black" };
    /// color.Values[g.GetNode(0)] = "red";
    /// color.Values[g.GetNode(1)] = "green";
    /// color.Values[g.GetNode(2)] = "blue";
    /// // the color of node #3 defaults to black
    /// f.Properties.Add(color);
    /// f.Save(@"c:\my_little_graph.graphml");
	/// </code>
	/// </para>
	/// </remarks>
	/// <typeparam name="T">Must be one of the types corresponding to the values of <see cref="StandardType"/>.</typeparam>
	public sealed class StandardProperty<T> : 
        DictionaryProperty<T>
	{
        /// <summary>
		/// The type parameter of this property.
		/// </summary>
		private static readonly StandardType Type = ParseType(typeof(T));

		/// <summary>
		/// The GraphML string representation of the type of this property.
		/// </summary>
		private static readonly string TypeString = TypeToGraphML(Type);

		/// <summary>
		/// Initialize <see cref="StandardProperty{T}"/>.
		/// </summary>
		public StandardProperty()
        {
        }

		/// <summary>
		/// Tries to construct a property from its declaration.
		/// </summary>
		/// <param name="xKey"><see cref="XElement"/>.</param>
		/// <exception cref="ArgumentException">The key element was not recognized as a declaration of this property.</exception>
		internal StandardProperty(XElement xKey)
			: this()
		{
			var attrType = xKey.Attribute("attr.type");
            if (attrType == null || attrType.Value != TypeString)
            {
                throw new ArgumentException("Key not compatible with property.");
            }

			LoadFromKeyElement(xKey);
		}

		/// <summary>
		/// Converts a Type to its <see cref="StandardType"/> equivalent.
		/// </summary>
        private static StandardType ParseType(Type t)
		{
            if (t == typeof(bool))
            {
                return StandardType.Bool;
            }

            if (t == typeof(double))
            {
                return StandardType.Double;
            }

            if (t == typeof(float))
            {
                return StandardType.Float;
            }

            if (t == typeof(int))
            {
                return StandardType.Int;
            }

            if (t == typeof(long))
            {
                return StandardType.Long;
            }

            if (t == typeof(string))
            {
                return StandardType.String;
            }

			throw new ArgumentException("Invalid type for a standard GraphML property.");
		}

		/// <summary>
		/// Gets the GraphML string representation of the type of this property.
		/// </summary>
		/// <param name="type"><see cref="StandardType"/>.</param>
        private static string TypeToGraphML(StandardType type)
		{
			switch (type)
			{
				case StandardType.Bool: 
                    return "boolean"; // !
				case StandardType.Double: 
                    return "double";
				case StandardType.Float: 
                    return "float";
				case StandardType.Int: 
                    return "int";
				case StandardType.Long: 
                    return "long";
				default: 
                    return "string";
			}
		}

		private static object ParseValue(string value)
		{
			switch (Type)
			{
				case StandardType.Bool: 
                    return value == "true";
				case StandardType.Double: 
                    return double.Parse(value, CultureInfo.InvariantCulture);
				case StandardType.Float: 
                    return float.Parse(value, CultureInfo.InvariantCulture);
				case StandardType.Int: 
                    return int.Parse(value, CultureInfo.InvariantCulture);
				case StandardType.Long: 
                    return long.Parse(value, CultureInfo.InvariantCulture);
				default: 
                    return value;
			}
		}

		private static string? ValueToGraphML(object value)
		{
			switch (Type)
			{
				case StandardType.Bool: 
                    return (bool)value ? "true" : "false";
				case StandardType.Double: 
                    return ((double)value).ToString(CultureInfo.InvariantCulture);
				case StandardType.Float: 
                    return ((float)value).ToString(CultureInfo.InvariantCulture);
				case StandardType.Int: 
                    return ((int)value).ToString(CultureInfo.InvariantCulture);
				case StandardType.Long: 
                    return ((long)value).ToString(CultureInfo.InvariantCulture);
				default: 
                    return value.ToString();
			}
		}

		/// <inheritdoc />
		public override XElement GetKeyElement()
		{
			var x = base.GetKeyElement();
			x.SetAttributeValue("attr.type", TypeString);
			return x;
		}

        /// <inheritdoc />
		protected override T ReadValue(XElement x)
		{
			return (T)ParseValue(x.Value);
		}

        /// <inheritdoc />
		protected override XElement WriteValue(T? value)
		{
			return new("dummy", ValueToGraphML(value));
		}
	}
}
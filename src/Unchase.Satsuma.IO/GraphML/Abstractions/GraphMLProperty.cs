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
using Unchase.Satsuma.IO.GraphML.Enums;

// ReSharper disable InconsistentNaming

namespace Unchase.Satsuma.IO.GraphML.Abstractions
{
    /// <summary>
	/// Represents a GraphML property (or attribute).
	/// </summary>
	/// <remarks>
	/// <para>Properties can assign extra values to nodes, arcs, or the whole graph.</para>
	/// <para>
	/// Descendants of this abstract class must define ways to declare and recognize properties of this type,
	/// and store or retrieve property values from a GraphML file.
	/// </para>
	/// <para>
	/// Properties are called attributes in the original GraphML terminology,
	/// but Attribute has a special meaning in .NET identifiers, so, in Satsuma, they are called properties.
	/// </para>
	/// <para>
	/// In addition, this class had to be named GraphMLProperty instead of Property because of a name collision
    /// with the reserved Visual Basic .NET keyword Property.
	/// </para>
	/// </remarks>
	public abstract class GraphMLProperty
	{
        /// <summary>
		/// The name of the property.
		/// </summary>
		/// <remarks>
		/// Can be either null or a nonempty string. It is advisable but not necessary to keep names unique.
		/// </remarks>
		public string? Name { get; set; }

		/// <summary>
		/// The domain of the property, i.e. the kind of objects the property applies to.
		/// </summary>
		public PropertyDomain Domain { get; set; }

		/// <summary>
		/// The <b>unique identifier</b> of the property in the GraphML file.
		/// </summary>
		/// <remarks>
		/// <para>This field is for internal use.</para>
		/// <para>When saving, it is ignored and replaced by an auto-generated identifier.</para>
		/// </remarks>
		public string? Id { get; set; }

		/// <summary>
		/// Initialize <see cref="GraphMLProperty"/>.
		/// </summary>
		protected GraphMLProperty()
		{
			Domain = PropertyDomain.All;
		}

		// 
		/// <summary>
		/// Converts the domain to a GraphML string representation.
		/// </summary>
		/// <param name="domain"><see cref="PropertyDomain"/>.</param>
		protected static string DomainToGraphML(PropertyDomain domain)
        {
            return domain switch
            {
                PropertyDomain.Node => "node",
                PropertyDomain.Arc => "edge",
                PropertyDomain.Graph => "graph",
                _ => "all"
            };
        }

		/// <summary>
		/// Parses the string representation of a GraphML domain.
		/// </summary>
		/// <param name="s">
		/// <para>Input value.</para>
		/// <para>Possible input values: "node", "edge", "graph", "all".</para>
		/// </param>
		/// <returns></returns>
		protected static PropertyDomain ParseDomain(string? s)
        {
            return s switch
            {
                "node" => PropertyDomain.Node,
                "edge" => PropertyDomain.Arc,
                "graph" => PropertyDomain.Graph,
                _ => PropertyDomain.All
            };
        }

		/// <summary>
		/// Loads the declaration of the property from the given <tt>&lt;key&gt;</tt> element (including the default value).
		/// </summary>
		/// <param name="xKey"><see cref="XElement"/>.</param>
		protected virtual void LoadFromKeyElement(XElement xKey)
		{
			var attrName = xKey.Attribute("attr.name");
			Name = attrName?.Value;

			Domain = ParseDomain(xKey.Attribute("for")?.Value);
			Id = xKey.Attribute("id")?.Value;

			var @default = Utils.ElementLocal(xKey, "default");
			ReadData(@default, null);
		}

		/// <summary>
		/// Returns a <tt>&lt;key&gt;</tt> element for the property.
		/// </summary>
		/// <remarks>
		/// This element declares the property in a GraphML file.
		/// </remarks>
		public virtual XElement GetKeyElement()
		{
			var xKey = new XElement(GraphMlFormat.Xmlns + "key");
			xKey.SetAttributeValue("attr.name", Name);
			xKey.SetAttributeValue("for", DomainToGraphML(Domain));
			xKey.SetAttributeValue("id", Id);

			var xDefault = WriteData(null);
			if (xDefault != null)
			{
				xDefault.Name = GraphMlFormat.Xmlns + "default";
				xKey.Add(xDefault);
			}

			return xKey;
		}

		/// <summary>
		/// Parses an XML value definition.
		/// </summary>
		/// <param name="x">
		/// A <tt>&lt;data&gt;</tt> or <tt>&lt;default&gt;</tt> element,
		/// which stores either the default value or the value taken on a node, arc or graph. If null, the data for key is erased.
		/// </param>
		/// <param name="key">
		/// A Node, Arc or IGraph, for which the loaded value will be stored.
		/// If null, the default value is loaded/erased.
		/// </param>
		public abstract void ReadData(XElement? x, object? key);

		/// <summary>
		/// Writes an XML value definition.
		/// </summary>
		/// <param name="key">A <see cref="Node"/>, <see cref="Arc"/> or <see cref="IGraph"/>, whose value will be returned as an XML representation.</param>
		/// <returns>Returns a data element, or null if there was no special value stored for the object.</returns>
		public abstract XElement? WriteData(object? key);
	}
}
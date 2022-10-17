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
using Unchase.Satsuma.Core;
using Unchase.Satsuma.IO.GraphML.Enums;

namespace Unchase.Satsuma.IO.GraphML
{
    /// <summary>
	/// The visual appearance of a GraphML node.
	/// </summary>
	/// <remarks>
	/// See also <seealso cref="NodeGraphicsProperty"/>.
	/// </remarks>
	public sealed class NodeGraphics
	{
        /// <summary>
		/// The X coordinate of the center of shape representing the node.
		/// </summary>
		public double X { get; set; }

		/// <summary>
		/// The Y coordinate of the center of shape representing the node.
		/// </summary>
		public double Y { get; set; }

		/// <summary>
		/// The width of the shape representing the node.
		/// </summary>
		public double Width { get; set; }

		/// <summary>
		/// The height of the shape representing the node.
		/// </summary>
		public double Height { get; set; }

		/// <summary>
		/// The shape of the node.
		/// </summary>
		public NodeShape Shape { get; set; }

		/// <summary>
		/// Initialize <see cref="NodeGraphics"/>.
		/// </summary>
		public NodeGraphics()
		{
			X = Y = 0;
			Width = Height = 10;
			Shape = NodeShape.Rectangle;
		}

		private readonly string[] _nodeShapeToString = { "rectangle", "roundrectangle", "ellipse", "parallelogram",
														  "hexagon", "triangle", "rectangle3d", "octagon",
														  "diamond", "trapezoid", "trapezoid2"};

		/// <summary>
		/// Parses the string representation of a node shape.
		/// </summary>
		/// <param name="s">Input value.</param>
        private NodeShape ParseShape(string s)
		{
			return (NodeShape)Math.Max(0, Array.IndexOf(_nodeShapeToString, s));
		}

		/// <summary>
		/// Converts a node shape to its string representation.
		/// </summary>
		/// <param name="shape"><see cref="NodeShape"/>.</param>
		/// <returns></returns>
		private string ShapeToGraphMl(NodeShape shape)
		{
			return _nodeShapeToString[(int)shape];
		}

		/// <summary>
		/// Constructs a node graphics object from a data element.
		/// </summary>
		/// <param name="xData"><see cref="XElement"/>.</param>
		public NodeGraphics(XElement xData)
		{
			var xGeometry = Utils.ElementLocal(xData, "Geometry");
			if (xGeometry != null)
            {
                var xAttribute = xGeometry.Attribute("x");
                if (xAttribute != null)
                {
                    X = double.Parse(xAttribute.Value, CultureInfo.InvariantCulture);
                }

				var yAttribute = xGeometry.Attribute("y");
                if (yAttribute != null)
                {
                    Y = double.Parse(yAttribute.Value, CultureInfo.InvariantCulture);
                }

                var widthAttribute = xGeometry.Attribute("width");
                if (widthAttribute != null)
                {
                    Width = double.Parse(widthAttribute.Value, CultureInfo.InvariantCulture);
				}
				
				var heightAttribute = xGeometry.Attribute("height");
                if (heightAttribute != null)
                {
                    Height = double.Parse(heightAttribute.Value, CultureInfo.InvariantCulture);
                }
			}

			var xShape = Utils.ElementLocal(xData, "Shape");
            var shapeAttribute = xShape?.Attribute("type");
            if (shapeAttribute != null)
            {
                Shape = ParseShape(shapeAttribute.Value);
            }
        }

		/// <summary>
		/// Converts the node graphics object to a data element.
		/// </summary>
        public XElement ToXml()
		{
			return new("dummy",
				new XElement(GraphMlFormat.XmlnsY + "ShapeNode",
					new XElement(GraphMlFormat.XmlnsY + "Geometry",
						new XAttribute("x", X.ToString(CultureInfo.InvariantCulture)),
						new XAttribute("y", Y.ToString(CultureInfo.InvariantCulture)),
						new XAttribute("width", Width.ToString(CultureInfo.InvariantCulture)),
						new XAttribute("height", Height.ToString(CultureInfo.InvariantCulture))),
					new XElement(GraphMlFormat.XmlnsY + "Shape",
						new XAttribute("type", ShapeToGraphMl(Shape)))
				)
			);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return ToXml().ToString();
		}
	}
}
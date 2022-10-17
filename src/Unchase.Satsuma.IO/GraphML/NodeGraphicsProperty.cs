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
using Unchase.Satsuma.IO.GraphML.Abstractions;
using Unchase.Satsuma.IO.GraphML.Enums;

namespace Unchase.Satsuma.IO.GraphML
{
    /// <summary>
    /// A GraphML property describing the visual appearance of the nodes.
    /// </summary>
    /// <remarks>
    /// <para>Example: <b>Defining node appearances</b></para>
    /// <para>
    /// <code>
    /// using GraphML = Satsuma.IO.GraphML;
    /// // [...]
    /// GraphML.GraphMLFormat f = new GraphML.GraphMLFormat();
    /// var g = new CompleteGraph(4);
    /// f.Graph = g;
    /// var ng = new GraphML.NodeGraphicsProperty();
    /// ng.Values[g.GetNode(0)] = new GraphML.NodeGraphics { X =   0, Y =   0, Width = 20, Height = 20 };
    /// ng.Values[g.GetNode(1)] = new GraphML.NodeGraphics { X =   0, Y = 100, Width = 20, Height = 20 };
    /// ng.Values[g.GetNode(2)] = new GraphML.NodeGraphics { X = 100, Y = 100, Width = 20, Height = 20 };
    /// ng.Values[g.GetNode(3)] = new GraphML.NodeGraphics { X = 100, Y =   0, Width = 20, Height = 20 };
    /// f.Properties.Add(ng);
    /// f.Save(@"c:\my_little_graph.graphml");
    /// </code>
    /// </para>
    /// </remarks>
    public sealed class NodeGraphicsProperty : 
        DictionaryProperty<NodeGraphics>
    {
        /// <summary>
        /// Initialize <seealso cref="NodeGraphicsProperty"/>.
        /// </summary>
        public NodeGraphicsProperty()
        {
            Domain = PropertyDomain.Node;
        }

        // Tries to construct a property from its declaration.
        // \exception ArgumentException The key element was not recognized as a declaration of this property.
        internal NodeGraphicsProperty(XElement xKey)
            : this()
        {
            var attrYFilesType = xKey.Attribute("yfiles.type");
            if (attrYFilesType is not { Value: "nodegraphics" })
            {
                throw new ArgumentException("Key not compatible with property.");
            }

            LoadFromKeyElement(xKey);
        }

        /// <inheritdoc />
        public override XElement GetKeyElement()
        {
            var x = base.GetKeyElement();
            x.SetAttributeValue("yfiles.type", "nodegraphics");
            return x;
        }

        /// <inheritdoc />
        protected override NodeGraphics ReadValue(XElement x)
        {
            return new(x);
        }

        /// <inheritdoc />
        protected override XElement? WriteValue(NodeGraphics? value)
        {
            return value?.ToXml();
        }
    }
}
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

using System.Drawing;
using Unchase.Satsuma.Drawing.Contracts;
using Unchase.Satsuma.Drawing.Enums;

namespace Unchase.Satsuma.Drawing
{
    /// <summary>
    /// The visual style for a drawn node.
    /// </summary>
    public sealed class NodeStyle
    {
        /// <summary>
        /// The pen used to draw the node.
        /// </summary>
        /// <remarks>
        /// Default: <see cref="Pens.Black"/>.
        /// </remarks>
        public Pen Pen { get; set; }

        /// <summary>
        /// The brush used to draw the node.
        /// </summary>
        /// <remarks>
        /// Default: <see cref="Brushes.White"/>.
        /// </remarks>
        public Brush Brush { get; set; }

        /// <summary>
        /// The shape of the node.
        /// </summary>
        /// <remarks>
        /// Default: <see cref="DefaultShape"/>.
        /// </remarks>
        public INodeShape Shape { get; set; }

        /// <summary>
        /// The font used to draw the caption.
        /// </summary>
        /// <remarks>
        /// Default: <see cref="SystemFonts.DefaultFont"/>.
        /// </remarks>
        public Font TextFont { get; set; }

        /// <summary>
        /// The brush used to draw the caption.
        /// </summary>
        /// <remarks>
        /// Default: <see cref="Brushes.Black"/>.
        /// </remarks>
        public Brush TextBrush { get; set; }

        /// <summary>
        /// The default node shape.
        /// </summary>
        public static readonly INodeShape DefaultShape = new NodeShape(NodeShapeKind.Ellipse, new(10, 10));

        /// <summary>
        /// Initialize <see cref="NodeStyle"/>.
        /// </summary>
        public NodeStyle()
        {
            Pen = Pens.Black;
            Brush = Brushes.White;
            Shape = DefaultShape;
            TextFont = SystemFonts.DefaultFont;
            TextBrush = Brushes.Black;
        }

        internal void DrawNode(Graphics graphics, float x, float y, string text)
        {
            var state = graphics.Save();
            graphics.TranslateTransform(x, y);
            Shape.Draw(graphics, Pen, Brush);
            if (text != string.Empty)
            {
                graphics.DrawString(text, TextFont, TextBrush, 0, 0,
                    new() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            }
            graphics.Restore(state);
        }
    }
}
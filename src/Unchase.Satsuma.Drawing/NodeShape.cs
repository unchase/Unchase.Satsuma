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
	/// A standard implementation of <see cref="INodeShape"/> (immutable).
	/// </summary>
	public sealed class NodeShape : 
        INodeShape
	{
        /// <summary>
		/// The kind of the shape.
		/// </summary>
		public NodeShapeKind Kind { get; }

		/// <summary>
		/// The size of the shape, in graphic units.
		/// </summary>
		public PointF Size { get; }

		private readonly RectangleF _rect;
		private readonly PointF[] _points = Array.Empty<PointF>();

		/// <summary>
		/// Initialize <see cref="NodeShape"/>.
		/// </summary>
		/// <param name="kind"><see cref="NodeShapeKind"/>.</param>
		/// <param name="size"><see cref="Size"/>.</param>
		public NodeShape(NodeShapeKind kind, PointF size)
		{
			Kind = kind;
			Size = size;

			_rect = new(-size.X * 0.5f, -size.Y * 0.5f, size.X, size.Y);
			switch (Kind)
			{
				case NodeShapeKind.Diamond:
					_points = new[] { P(_rect, 0, 0.5f), P(_rect, 0.5f, 1),
						P(_rect, 1, 0.5f), P(_rect, 0.5f, 0) }; 
                    break;
				case NodeShapeKind.Rectangle:
					_points = new[] { _rect.Location, new(_rect.Left, _rect.Bottom),
							new(_rect.Right, _rect.Bottom), new(_rect.Right, _rect.Top) }; 
                    break;
				case NodeShapeKind.Triangle:
					_points = new[] { P(_rect, 0.5f, 0), P(_rect, 0, 1),
						P(_rect, 1, 1) }; 
                    break;
				case NodeShapeKind.UpsideDownTriangle:
					_points = new[] { P(_rect, 0.5f, 1), P(_rect, 0, 0),
						P(_rect, 1, 0) }; 
                    break;
			}
		}

		private static PointF P(RectangleF rect, float x, float y)
		{
			return new(rect.Left + rect.Width * x, rect.Top + rect.Height * y);
		}

		/// <inheritdoc />
		public void Draw(Graphics graphics, Pen pen, Brush brush)
		{
			switch (Kind)
			{
				case NodeShapeKind.Ellipse:
					graphics.FillEllipse(brush, _rect);
					graphics.DrawEllipse(pen, _rect);
					break;

				default:
					graphics.FillPolygon(brush, _points);
					graphics.DrawPolygon(pen, _points);
					break;
			}
		}

        /// <inheritdoc />
		public PointF GetBoundary(double angle)
		{
			double cos = Math.Cos(angle), sin = Math.Sin(angle);
			switch (Kind)
			{
				case NodeShapeKind.Ellipse:
					return new((float)(Size.X * 0.5f * cos), (float)(Size.Y * 0.5f * sin));

				default:
					// we have a polygon, try to intersect all sides with the ray
					for (var i = 0; i < _points.Length; i++)
					{
						var i2 = (i + 1) % _points.Length;
						var t = (float)((_points[i].Y * cos - _points[i].X * sin) /
                                        ((_points[i2].X - _points[i].X) * sin - (_points[i2].Y - _points[i].Y) * cos));
						if (t is >= 0 and <= 1)
						{
							var result = new PointF(_points[i].X + t * (_points[i2].X - _points[i].X),
								_points[i].Y + t * (_points[i2].Y - _points[i].Y));
                            if (result.X * cos + result.Y * sin > 0)
                            {
                                return result;
                            }
						}
					}

					return new(0, 0); // should not happen
			}
		}
	}
}
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
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Drawing
{
	/// <summary>
	/// Draws a graph on a Graphics.
	/// </summary>
	/// <remarks>
	/// <para>Example:</para>
	/// <para>
	/// <code>
	/// var graph = new CompleteGraph(7);
	/// // compute a nice layout of the graph
	/// var layout = new ForceDirectedLayout(graph);
	/// layout.Run();
	/// // draw the graph using the computed layout
	/// var nodeShape = new NodeShape(NodeShapeKind.Diamond, new PointF(40, 40));
	/// var nodeStyle = new NodeStyle { Brush = Brushes.Yellow, Shape = nodeShape };
	/// var drawer = new GraphDrawer()
	/// {
	/// 	Graph = graph,
	/// 	NodePositions = (node => (PointF)layout.NodePositions[node]),
	/// 	NodeCaptions = (node => graph.GetNodeIndex(node).ToString()),
	/// 	NodeStyles = (node => nodeStyle)
	/// };
	/// drawer.Draw(300, 300, Color.White).Save(@"c:\graph.png", ImageFormat.Png);
	/// </code>
	/// </para>
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public sealed class GraphDrawer<TNodeProperty, TArcProperty>
	{
        /// <summary>
		/// The graph to draw.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

        /// <summary>
        /// Assigns its position to a node.
        /// </summary>
        public Func<Node, PointF> NodePosition { get; }

		/// <summary>
		/// Assigns its caption to a node.
		/// </summary>
		/// <remarks>
		/// Default value: assign the empty string (i.e. no caption) to each node.
		/// </remarks>
		public Func<Node, string> NodeCaption { get; set; }

		/// <summary>
		/// Assigns its style to a node.
		/// </summary>
		/// <remarks>
		/// <para>Default value: assign a default <see cref="NodeStyle"/> to each node.</para>
		/// <para>This function is called lots of times (at least twice for each arc).</para>
		/// <para>Avoid creating a <see cref="NodeStyle"/> object on each call.</para>
		/// <para>Return pre-made objects from some collection instead.</para>
		/// </remarks>
		public Func<Node, NodeStyle> NodeStyle { get; set; }

		/// <summary>
		/// Assigns a pen to each arc.
		/// </summary>
		/// <remarks>
		/// Default value: assign <see cref="DirectedPen"/> to directed arcs, and <see cref="UndirectedPen"/> to edges.
		/// </remarks>
		public Func<Arc, Pen> ArcPen { get; set; }

		/// <summary>
		/// The pen used for directed arcs.
		/// </summary>
		/// <remarks>
		/// <para>Default value: a black pen with an arrow end.</para>
		/// <para>Unused if <see cref="ArcPen"/> is set to a custom value.</para>
		/// </remarks>
		public Pen DirectedPen { get; set; }

		/// <summary>
		/// The pen used for undirected arcs.
		/// </summary>
		/// <remarks>
		/// <para>Default value: <see cref="Pens.Black"/>.</para>
		/// <para>Unused if <see cref="ArcPen"/> is set to a custom value.</para>
		/// </remarks>
		public Pen UndirectedPen { get; set; }

		/// <summary>
		/// Initialize <see cref="GraphDrawer{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
		/// <param name="nodePosition"><see cref="NodePosition"/>.</param>
		public GraphDrawer(
            IGraph<TNodeProperty, TArcProperty> graph,
            Func<Node, PointF> nodePosition)
		{
			Graph = graph;
			NodePosition = nodePosition;
			NodeCaption = _ => string.Empty;
			var defaultNodeStyle = new NodeStyle();
			NodeStyle = _ => defaultNodeStyle;
			ArcPen = arc => (Graph.IsEdge(arc)
                ? UndirectedPen 
                : DirectedPen) ?? throw new InvalidOperationException();

			DirectedPen = new(Color.Black)
            {
                CustomEndCap = new AdjustableArrowCap(3, 5)
            };
			UndirectedPen = Pens.Black;
		}

		/// <summary>
		/// Draws the graph.
		/// </summary>
		/// <param name="graphics"><see cref="Graphics"/>.</param>
		/// <param name="matrix">
		/// <para>The transformation matrix to be applied to the node positions (but not to the node and arc shapes).</para>
		/// <para>If null, the identity matrix is used.</para>
		/// </param>
		public void Draw(Graphics graphics, Matrix? matrix = null)
		{
			// draw arcs
			var arcPos = new PointF[2];
			var boundary = new PointF[2];
			foreach (var arc in Graph.Arcs())
			{
				var u = Graph.U(arc);
				arcPos[0] = NodePosition(u);
				var v = Graph.V(arc);
				arcPos[1] = NodePosition(v);
                matrix?.TransformPoints(arcPos);

                // an arc should run between shape boundaries
				var angle = Math.Atan2(arcPos[1].Y - arcPos[0].Y, arcPos[1].X - arcPos[0].X);
				boundary[0] = NodeStyle(u).Shape.GetBoundary(angle);
				boundary[1] = NodeStyle(v).Shape.GetBoundary(angle + Math.PI);

				graphics.DrawLine(ArcPen(arc), arcPos[0].X + boundary[0].X, arcPos[0].Y + boundary[0].Y,
					arcPos[1].X + boundary[1].X, arcPos[1].Y + boundary[1].Y);
			}

			// draw nodes
			var nodePos = new PointF[1];
			foreach (var node in Graph.Nodes())
			{
				nodePos[0] = NodePosition(node);
                matrix?.TransformPoints(nodePos);
                NodeStyle(node).DrawNode(graphics, nodePos[0].X, nodePos[0].Y, NodeCaption(node));
			}
		}

		/// <summary>
		/// Draws the graph to fit the given bounding box.
		/// </summary>
		/// <param name="graphics"><see cref="Graphics"/>.</param>
		/// <param name="box">The desired bounding box for the drawn graph.</param>
		public void Draw(Graphics graphics, RectangleF box)
		{
			if (!Graph.Nodes().Any()) return;

			float maxShapeWidth = 0, maxShapeHeight = 0;
			float xMin = float.PositiveInfinity, yMin = float.PositiveInfinity;
			float xMax = float.NegativeInfinity, yMax = float.NegativeInfinity;
			foreach (var node in Graph.Nodes())
			{
				var size = NodeStyle(node).Shape.Size;
				maxShapeWidth = Math.Max(maxShapeWidth, size.X);
				maxShapeHeight = Math.Max(maxShapeHeight, size.Y);

				var pos = NodePosition(node);
				xMin = Math.Min(xMin, pos.X);
				xMax = Math.Max(xMax, pos.X);
                yMin = Math.Min(yMin, pos.Y);
				yMax = Math.Max(yMax, pos.Y);
			}

			var xSpan = xMax - xMin;
            if (xSpan == 0)
            {
                xSpan = 1;
            }

			var ySpan = yMax - yMin;
            if (ySpan == 0)
            {
                ySpan = 1;
            }

			var matrix = new Matrix();
			matrix.Translate(maxShapeWidth * 0.6f, maxShapeHeight * 0.6f);
			matrix.Scale((box.Width - maxShapeWidth * 1.2f) / xSpan, (box.Height - maxShapeHeight * 1.2f) / ySpan);
			matrix.Translate(-xMin, -yMin);
			Draw(graphics, matrix);
		}

		/// <summary>
		/// Draws the graph to a new bitmap and returns the bitmap.
		/// </summary>
		/// <param name="width">The width of the bitmap.</param>
		/// <param name="height">The height of the bitmap.</param>
		/// <param name="backColor">The background color for the bitmap.</param>
		/// <param name="antialias">Specifies whether anti-aliasing should take place when drawing.</param>
		/// <param name="pixelFormat">The pixel format of the bitmap. Default value: 32-bit ARGB.</param>
        public Bitmap Draw(
            int width, 
            int height, 
            Color backColor,
			bool antialias = true,
            PixelFormat pixelFormat = PixelFormat.Format32bppArgb)
		{
			var bm = new Bitmap(width, height, pixelFormat);
			using (var g = Graphics.FromImage(bm))
			{
				g.SmoothingMode = antialias ? SmoothingMode.AntiAlias : SmoothingMode.None;
				g.Clear(backColor);
				Draw(g, new RectangleF(0, 0, width, height));
			}

			return bm;
		}
	}
}
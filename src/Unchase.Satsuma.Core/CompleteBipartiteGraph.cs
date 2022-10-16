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
using Unchase.Satsuma.Core.Enums;

namespace Unchase.Satsuma.Core
{
    /// <summary>
	/// A complete bipartite graph on a given number of nodes.
	/// </summary>
	/// <remarks>
	/// <para>The two color classes of the bipartite graph are referred to as red and blue nodes.</para>
	/// <para>The graph may be either directed (from the red to the blue nodes) or undirected.</para>
	/// <para>Memory usage: O(1).</para>
	/// <para>This type is thread safe.</para>
	/// </remarks>
	public sealed class CompleteBipartiteGraph : 
        IGraph
	{
        /// <summary>
		/// The count of nodes in the first color class.
		/// </summary>
		public int RedNodeCount { get; }

		/// <summary>
		/// The count of nodes in the second color class.
		/// </summary>
		public int BlueNodeCount { get; }

		/// <summary>
		/// True if the graph is directed from red to blue nodes, 
        /// false if it is undirected.
		/// </summary>
		public bool Directed { get; }

		/// <summary>
		/// Initialize <see cref="CompleteBipartiteGraph"/>.
		/// </summary>
		/// <param name="redNodeCount">Red node count.</param>
		/// <param name="blueNodeCount">Blue node count.</param>
		/// <param name="directedness">If <see cref="Directedness.Directed"/>, then the graph is directed from the red to the blue nodes. Otherwise, the graph is undirected.</param>
        public CompleteBipartiteGraph(
            int redNodeCount, 
            int blueNodeCount, 
            Directedness directedness)
		{
            if (redNodeCount < 0 || blueNodeCount < 0)
            {
                throw new ArgumentException("Invalid node count: " + redNodeCount + ";" + blueNodeCount);
            }

            if ((long)redNodeCount + blueNodeCount > int.MaxValue || (long)redNodeCount * blueNodeCount > int.MaxValue)
            {
                throw new ArgumentException("Too many nodes: " + redNodeCount + ";" + blueNodeCount);
            }

			RedNodeCount = redNodeCount;
			BlueNodeCount = blueNodeCount;
			Directed = directedness == Directedness.Directed;
		}

		/// <summary>
		/// Gets a red node by its index.
		/// </summary>
		/// <param name="index">An integer between 0 (inclusive) and RedNodeCount (exclusive).</param>
		public Node GetRedNode(int index)
		{
			return new(1L + index);
		}

		/// <summary>
		/// Gets a blue node by its index.
		/// </summary>
		/// <param name="index">An integer between 0 (inclusive) and BlueNodeCount (exclusive).</param>
        public Node GetBlueNode(int index)
		{
			return new(1L + RedNodeCount + index);
		}

		/// <summary>
		/// Node is red.
		/// </summary>
		/// <param name="node">Node.</param>
        public bool IsRed(Node node)
		{
			return node.Id <= RedNodeCount;
		}

		/// <summary>
		/// Gets the unique arc between two nodes.
		/// </summary>
		/// <param name="u">The first node.</param>
		/// <param name="v">The second node.</param>
		/// <returns>The arc whose two ends are u and v, or <see cref="Arc.Invalid"/> if the two nodes are of the same color.</returns>
		public Arc GetArc(Node u, Node v)
		{
			var ured = IsRed(u);
			var vred = IsRed(v);

			if (ured == vred) return Arc.Invalid;
			if (vred)
			{
				(u, v) = (v, u);
            }

			var uindex = (int)(u.Id - 1);
			var vindex = (int)(v.Id - RedNodeCount - 1);

			return new(1 + (long)vindex * RedNodeCount + uindex);
		}

		/// <inheritdoc />
		/// 
		public Node U(Arc arc)
		{
			return new(1L + (arc.Id - 1) % RedNodeCount);
		}

		/// <inheritdoc />
		/// <returns>Returns the blue node of an arc.</returns>
		public Node V(Arc arc)
		{
			return new(1L + RedNodeCount + (arc.Id - 1) / RedNodeCount);
		}

        /// <inheritdoc />
		public bool IsEdge(Arc arc)
		{
			return !Directed;
		}

		/// <summary>
		/// Gets all nodes of a given color.
		/// </summary>
		/// <param name="color"><see cref="Color"/>.</param>
		/// <returns></returns>
		public IEnumerable<Node> Nodes(Color color)
		{
			switch (color)
			{
				case Color.Red:
                    for (var i = 0; i < RedNodeCount; i++)
                    {
                        yield return GetRedNode(i);
                    }
					break;

				case Color.Blue:
                    for (var i = 0; i < BlueNodeCount; i++)
                    {
                        yield return GetBlueNode(i);
                    }
					break;
			}
		}

        /// <inheritdoc />
		public IEnumerable<Node> Nodes()
		{
            for (var i = 0; i < RedNodeCount; i++)
            {
                yield return GetRedNode(i);
            }

            for (var i = 0; i < BlueNodeCount; i++)
            {
                yield return GetBlueNode(i);
            }
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All)
		{
            if (Directed && filter == ArcFilter.Edge)
            {
                yield break;
            }

			for (var i = 0; i < RedNodeCount; i++)
            {
                for (var j = 0; j < BlueNodeCount; j++)
                {
                    yield return GetArc(GetRedNode(i), GetBlueNode(j));
                }
            }
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All)
		{
			var isRed = IsRed(u);
            if (Directed && (filter == ArcFilter.Edge ||
                             (filter == ArcFilter.Forward && !isRed) ||
                             (filter == ArcFilter.Backward && isRed)))
            {
                yield break;
            }

			if (isRed)
			{
                for (var i = 0; i < BlueNodeCount; i++)
                {
                    yield return GetArc(u, GetBlueNode(i));
                }
			}
			else
			{
                for (var i = 0; i < RedNodeCount; i++)
                {
                    yield return GetArc(GetRedNode(i), u);
                }
			}
		}

        /// <inheritdoc />
		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
			var arc = GetArc(u, v);
            if (arc != Arc.Invalid && ArcCount(u, filter) > 0)
            {
                yield return arc;
            }
		}

        /// <inheritdoc />
		public int NodeCount()
		{
			return RedNodeCount + BlueNodeCount;
		}

        /// <inheritdoc />
		public int ArcCount(ArcFilter filter = ArcFilter.All)
		{
            if (Directed && filter == ArcFilter.Edge)
            {
                return 0;
            }

			return RedNodeCount * BlueNodeCount;
		}

        /// <inheritdoc />
		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All)
		{
			var isRed = IsRed(u);
            if (Directed && (filter == ArcFilter.Edge || (filter == ArcFilter.Forward && !isRed) ||
                             (filter == ArcFilter.Backward && isRed)))
            {
                return 0;
            }

			return isRed 
                ? BlueNodeCount 
                : RedNodeCount;
		}

        /// <inheritdoc />
		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All)
		{
            if (IsRed(u) == IsRed(v))
            {
                return 0;
            }

			return ArcCount(u, filter) > 0 
                ? 1 
                : 0;
		}

        /// <inheritdoc />
		public bool HasNode(Node node)
		{
			return node.Id >= 1 && node.Id <= RedNodeCount + BlueNodeCount;
		}

        /// <inheritdoc />
		public bool HasArc(Arc arc)
		{
			return arc.Id >= 1 && arc.Id <= RedNodeCount * BlueNodeCount;
		}
	}
}
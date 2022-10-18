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

using Unchase.Satsuma.Algorithms.Abstractions;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Algorithms.Connectivity
{
	/// <summary>
	/// Decides whether the graph is bipartite and finds a bipartition into red and blue nodes.
	/// </summary>
	/// <remarks>
	/// <para>Example:</para>
	/// <para>
	/// <code>
	/// var g = new PathGraph(12, PathGraph.Topology.Cycle, Directedness.Undirected);
	/// var bp = new Bipartition(g, Bipartition.Flags.CreateRedNodes | Bipartition.Flags.CreateBlueNodes);
	/// Console.WriteLine("Bipartite: " + (bp.Bipartite ? "yes" : "no")); // should print 'yes'
	/// if (bp.Bipartite)
	/// {
	/// 	Console.WriteLine("Red nodes: " + string.Join(" ", bp.RedNodes));
	/// 	Console.WriteLine("Blue nodes: " + string.Join(" ", bp.BlueNodes));
	/// }
	/// </code>
	/// </para>
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public sealed class Bipartition<TNodeProperty, TArcProperty>
	{
        /// <summary>
		/// The input graph.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

		/// <summary>
		/// True if the graph is bipartite.
		/// </summary>
		public bool Bipartite { get; private set; }

		/// <summary>
		/// The elements of the red color class.
		/// </summary>
		/// <remarks>
		/// Null if <see cref="BipartitionFlags.CreateRedNodes"/> was not set during construction.
        /// Otherwise, empty if the graph is not bipartite.
		/// </remarks>
		public HashSet<Node>? RedNodes { get; }

		/// <summary>
		/// The elements of the blue color class.
		/// </summary>
		/// <remarks>
		/// Null if <see cref="BipartitionFlags.CreateBlueNodes"/> was not set during construction.
		/// Otherwise, empty if the graph is not bipartite.
		/// </remarks>
		public HashSet<Node>? BlueNodes { get; }

		private class MyDfs : 
            Dfs<TNodeProperty, TArcProperty>
		{
			/// <summary>
			/// Parent <see cref="Bipartition{TNodeProperty, TArcProperty}"/>.
			/// </summary>
			private readonly Bipartition<TNodeProperty, TArcProperty> _parent;

			private HashSet<Node> _redNodes = new();

			/// <summary>
			/// Initialize <see cref="MyDfs"/>.
			/// </summary>
			/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
			/// <param name="parent"><see cref="Bipartition{TNodeProperty, TArcProperty}"/>.</param>
			public MyDfs(
                IGraph<TNodeProperty, TArcProperty> graph, 
                Bipartition<TNodeProperty, TArcProperty> parent) 
                    : base(graph)
            {
                _parent = parent;
            }

            /// <inheritdoc />
			protected override void Start(out Direction direction)
			{
				direction = Direction.Undirected;
				_parent.Bipartite = true;
				_redNodes = _parent.RedNodes ?? new HashSet<Node>();
			}

            /// <inheritdoc />
			protected override bool NodeEnter(Node node, Arc arc)
			{
                if ((Level & 1) == 0)
                {
                    _redNodes.Add(node);
                }
				else
                {
                    _parent.BlueNodes?.Add(node);
                }

                return true;
			}

            /// <inheritdoc />
			protected override bool BackArc(Node node, Arc arc)
			{
				var other = Graph.Other(arc, node);
				if (_redNodes.Contains(node) == _redNodes.Contains(other))
				{
					_parent.Bipartite = false;
                    _parent.RedNodes?.Clear();
                    _parent.BlueNodes?.Clear();
                    return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Initialize <see cref="Bipartition{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
		/// <param name="flags"><see cref="BipartitionFlags"/>.</param>
		public Bipartition(
            IGraph<TNodeProperty, TArcProperty> graph, 
            BipartitionFlags flags = 0)
		{
			Graph = graph;
            if (0 != (flags & BipartitionFlags.CreateRedNodes))
            {
                RedNodes = new();
            }

            if (0 != (flags & BipartitionFlags.CreateBlueNodes))
            {
                BlueNodes = new();
            }

			new MyDfs(graph, this).Run();
		}
	}
}
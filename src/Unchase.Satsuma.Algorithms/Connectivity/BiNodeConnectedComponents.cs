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

using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Algorithms.Connectivity
{
	/// <inheritdoc cref="BiNodeConnectedComponents{TNodeProperty, TArcProperty}"/>
	public class BiNodeConnectedComponents :
        BiNodeConnectedComponents<object, object>
    {
        /// <summary>
        /// Initialize <see cref="BiNodeConnectedComponents"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="flags"><see cref="BiNodeConnectedComponentsFlags"/>.</param>
        public BiNodeConnectedComponents(
            IGraph graph,
            BiNodeConnectedComponentsFlags flags = 0)
		        : base(graph, flags)
        {
		}
    }

	/// <summary>
	/// Finds the cutvertices and blocks (2-node-connected components) of a graph.
	/// </summary>
	/// <remarks>
	/// Blocks (2-node-connected components) are maximal 2-node-connected subgraphs and bridge arcs.
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
	/// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public class BiNodeConnectedComponents<TNodeProperty, TArcProperty>
	{
        /// <summary>
		/// The input graph.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

		/// <summary>
		/// The number of blocks (2-node-connected components) in the graph.
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		/// The blocks (2-node-connected components) of the graph.
		/// </summary>
		/// <remarks>
		/// Null if <see cref="BiNodeConnectedComponentsFlags.CreateComponents"/> was not set during construction.
		/// </remarks>
		public List<HashSet<Node>>? Components { get; }

		/// <summary>
		/// Stores the increase in the number of connected components upon deleting a node.
		/// </summary>
		/// <remarks>
		/// <para>Null if <see cref="BiNodeConnectedComponentsFlags.CreateCutvertices"/> was not set during construction.</para>
		/// <para>The only keys are cutvertices (value &gt; 0) and one-node components (value = -1).</para>
		/// <para>Other nodes are not contained as keys, as they would all have 0 value assigned.</para>
		/// </remarks>
		public Dictionary<Node, int>? Cutvertices { get; }

		private class BlockDfs : 
            LowpointDfs<TNodeProperty, TArcProperty>
		{
            private readonly BiNodeConnectedComponents<TNodeProperty, TArcProperty> _parent;
			private Stack<Node> _blockStack = new();
			private bool _oneNodeComponent;

			/// <summary>
			/// Initialize <see cref="BlockDfs"/>.
			/// </summary>
			/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
			/// <param name="parent"><see cref="BiNodeConnectedComponents{TNodeProperty, TArcProperty}"/>.</param>
			public BlockDfs(
                IGraph<TNodeProperty, TArcProperty> graph, 
                BiNodeConnectedComponents<TNodeProperty, TArcProperty> parent) 
                : base(graph)
            {
                _parent = parent;
            }

            protected override void Start(out Direction direction)
			{
				base.Start(out direction);
				if (_parent.Components != null) _blockStack = new();
			}

            /// <inheritdoc />
			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (!base.NodeEnter(node, arc)) return false;

                if (_parent.Cutvertices != null && arc == Arc.Invalid)
                {
                    _parent.Cutvertices[node] = -1;
                }

                if (_parent.Components != null)
                {
                    _blockStack.Push(node);
                }

				_oneNodeComponent = (arc == Arc.Invalid);
				return true;
			}

			/// <inheritdoc />
			protected override bool NodeExit(Node node, Arc arc)
			{
				if (arc == Arc.Invalid)
				{
					if (_oneNodeComponent)
					{
						_parent.Count++;
                        _parent.Components?.Add(new() { node });
                    }

                    if (_parent.Cutvertices != null && _parent.Cutvertices[node] == 0)
                    {
                        _parent.Cutvertices.Remove(node);
                    }

                    if (_parent.Components != null)
                    {
                        _blockStack.Clear();
                    }
				}
				else
				{
					// parent is a cutvertex or root?
					var parent = Graph.Other(arc, node);
					if (Lowpoints[node] >= Level - 1)
					{
						if (_parent.Cutvertices != null)
						{
                            _parent.Cutvertices[parent] = (_parent.Cutvertices.TryGetValue(parent, out var degree) ? degree : 0) + 1;
						}

						_parent.Count++;
						if (_parent.Components != null)
						{
							var block = new HashSet<Node>();
							while (true)
							{
								var n = _blockStack.Pop();
								block.Add(n);
								if (n == node) break;
							}
							block.Add(parent);
							_parent.Components.Add(block);
						}
					}
				}

				return base.NodeExit(node, arc);
			}
		}

		/// <summary>
		/// Initialize <see cref="BiNodeConnectedComponents{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
		/// <param name="flags"><see cref="BiNodeConnectedComponentsFlags"/>.</param>
		public BiNodeConnectedComponents(
            IGraph<TNodeProperty, TArcProperty> graph,
            BiNodeConnectedComponentsFlags flags = 0)
		{
			Graph = graph;
            if (0 != (flags & BiNodeConnectedComponentsFlags.CreateComponents))
            {
                Components = new();
            }

            if (0 != (flags & BiNodeConnectedComponentsFlags.CreateCutvertices))
            {
                Cutvertices = new();
            }

			new BlockDfs(graph, this).Run();
		}
	}
}
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

namespace Unchase.Satsuma.Algorithms.Connectivity
{
	/// <inheritdoc cref="StrongComponents{TNodeProperty, TArcProperty}"/>
	public sealed class StrongComponents :
        StrongComponents<object, object>
    {
		/// <summary>
        /// Initialize <see cref="StrongComponents"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        /// <param name="flags"><see cref="StrongComponentsFlags"/>.</param>
        public StrongComponents(
            IGraph graph,
            StrongComponentsFlags flags = 0)
		        : base(graph, flags)
        {
		}
    }

	/// <summary>
	/// Finds the strongly connected components of a digraph.
	/// </summary>
	/// <remarks>
	/// Edges count as 2-cycles.
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
	/// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public class StrongComponents<TNodeProperty, TArcProperty>
	{
        /// <summary>
		/// The input digraph.
		/// </summary>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; }

		/// <summary>
		/// The number of strongly connected components in the digraph.
		/// </summary>
		public int Count { get; private set; }

		/// <summary>
		/// The strongly connected components of the digraph,
		/// in a topological order of the component DAG (initial components first).
		/// </summary>
		/// <remarks>
		/// Null if <see cref="StrongComponentsFlags.CreateComponents"/> was not set during construction.
		/// </remarks>
		public List<HashSet<Node>>? Components { get; }

		private class ForwardDfs : 
            Dfs<TNodeProperty, TArcProperty>
		{
			public List<Node> ReverseExitOrder = new();

			/// <summary>
			/// Initialize <see cref="ForwardDfs"/>.
			/// </summary>
			/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
			public ForwardDfs(
                IGraph<TNodeProperty, TArcProperty> graph) 
                    : base(graph)
            {
            }

			/// <inheritdoc />
            protected override void Start(out Direction direction)
			{
				direction = Direction.Forward;
				ReverseExitOrder = new();
			}

            /// <inheritdoc />
			protected override bool NodeExit(Node node, Arc arc)
			{
				ReverseExitOrder.Add(node);
				return true;
			}

            /// <inheritdoc />
			protected override void StopSearch()
			{
				ReverseExitOrder.Reverse();
			}
		}

		private class BackwardDfs : 
            Dfs<TNodeProperty, TArcProperty>
		{
			/// <summary>
			/// Parent <see cref="StrongComponents{TNodeProperty, TArcProperty}"/>.
			/// </summary>
			private readonly StrongComponents<TNodeProperty, TArcProperty> _parent;

			/// <summary>
			/// Initialize <see cref="BackwardDfs"/>.
			/// </summary>
			/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
			/// <param name="parent"><see cref="StrongComponents{TNodeProperty, TArcProperty}"/>.</param>
			public BackwardDfs(
                IGraph<TNodeProperty, TArcProperty> graph, 
                StrongComponents<TNodeProperty, TArcProperty> parent) 
                    : base(graph)
            {
                _parent = parent;
            }

			/// <inheritdoc />
            protected override void Start(out Direction direction)
			{
				direction = Direction.Backward;
			}

            /// <inheritdoc />
			protected override bool NodeEnter(Node node, Arc arc)
			{
				if (arc == Arc.Invalid)
				{
					_parent.Count++;
                    _parent.Components?.Add(new() { node });
                }
				else
                {
                    _parent.Components?[^1].Add(node);
                }

                return true;
			}
		}

		/// <summary>
		/// Initialize <see cref="StrongComponents{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
		/// <param name="flags"><see cref="StrongComponentsFlags"/>.</param>
		public StrongComponents(
            IGraph<TNodeProperty, TArcProperty> graph, 
            StrongComponentsFlags flags = 0)
		{
			Graph = graph;
            if (0 != (flags & StrongComponentsFlags.CreateComponents))
            {
                Components = new();
            }

			var forwardDfs = new ForwardDfs(graph);
			forwardDfs.Run();
			var backwardDfs = new BackwardDfs(graph, this);
			backwardDfs.Run(forwardDfs.ReverseExitOrder);
		}
	}
}
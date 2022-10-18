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
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Algorithms.Abstractions
{
	/// <summary>
	/// Performs a customizable depth-first search (DFS).
	/// </summary>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public abstract class Dfs<TNodeProperty, TArcProperty>
	{
        /// <summary>
		/// The input graph.
		/// </summary>
		protected IGraph<TNodeProperty, TArcProperty> Graph { get; }

		private HashSet<Node> _traversed = new();
		private ArcFilter _arcFilter;

		/// <summary>
		/// The level of the current node (starting from zero).
		/// </summary>
		protected int Level { get; private set; }

		/// <summary>
		/// Initialize <see cref="Dfs{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		/// <param name="graph">The input graph.</param>
		protected Dfs(
            IGraph<TNodeProperty, TArcProperty> graph)
        {
            Graph = graph;
        }

		/// <summary>
		/// Runs the depth-first search. Can be called an arbitrary number of times.
		/// </summary>
		/// 
		/// <param name="roots">The roots where the search should start, or null if all the graph nodes should be considered.</param>
		public void Run(
            IEnumerable<Node>? roots = null)
		{
            Start(out var direction);
            _arcFilter = direction switch
            {
                Direction.Undirected => ArcFilter.All,
                Direction.Forward => ArcFilter.Forward,
                _ => ArcFilter.Backward
            };

            _traversed = new();
			foreach (var node in roots ?? Graph.Nodes())
			{
                if (_traversed.Contains(node))
                {
                    continue;
                }

				Level = 0;
                if (!Traverse(node, Arc.Invalid))
                {
                    break;
                }
			}

			_traversed = new();

			StopSearch();
		}

		private bool Traverse(Node node, Arc arc)
		{
			_traversed.Add(node);
            if (!NodeEnter(node, arc))
            {
                return false;
            }

			foreach (var b in Graph.Arcs(node, _arcFilter))
			{
                if (b == arc)
                {
                    continue;
                }

				var other = Graph.Other(b, node);
				if (_traversed.Contains(other))
				{
                    if (!BackArc(node, b))
                    {
                        return false;
                    }

					continue;
				}

				Level++;
                if (!Traverse(other, b))
                {
                    return false;
                }

				Level--;
			}

			return NodeExit(node, arc);
		}

		/// <summary>
		/// Called before starting the search.
		/// </summary>
		/// <param name="direction"><see cref="Direction"/>.</param>
		protected abstract void Start(out Direction direction);

		/// <summary>
		/// Called when entering a node through an arc.
		/// </summary>
		/// <param name="node">The node being entered.</param>
		/// <param name="arc">The arc connecting the node to its parent in the <see cref="Dfs{TNodeProperty, TArcProperty}"/> forest, or <see cref="Arc.Invalid"/> if the node is a root.</param>
		/// <returns>Returns true if the traversal should continue.</returns>
		protected virtual bool NodeEnter(Node node, Arc arc)
        {
            return true;
        }

		/// <summary>
		/// Called when exiting a node and going back through an arc.
		/// </summary>
		/// <param name="node">The node being exited.</param>
		/// <param name="arc">The arc connecting the node to its parent in the <see cref="Dfs{TNodeProperty, TArcProperty}"/> forest, or <see cref="Arc.Invalid"/> if the node is a root.</param>
		/// <returns>Returns true if the traversal should continue.</returns>
		protected virtual bool NodeExit(Node node, Arc arc)
        {
            return true;
        }

		/// <summary>
		/// Called when encountering a non-forest arc pointing to an already visited node (this includes loop arcs).
		/// </summary>
		/// <param name="node">The node being processed by the <see cref="Dfs{TNodeProperty, TArcProperty}"/>.</param>
		/// <param name="arc">The non-forest arc encountered.</param>
		/// <returns>Returns true if the traversal should continue.</returns>
		protected virtual bool BackArc(Node node, Arc arc)
        {
            return true;
        }

        /// <summary>
        /// Called after finishing the search.
        /// </summary>
        protected virtual void StopSearch()
        {
        }
	}
}
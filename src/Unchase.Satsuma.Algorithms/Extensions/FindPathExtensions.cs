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

namespace Unchase.Satsuma.Algorithms.Extensions
{
    /// <summary>
	/// Extension methods for <see cref="IGraph"/>, for finding paths.
	/// </summary>
	public static class FindPathExtensions
	{
		private class PathDfs : 
            Dfs
		{
			// input
            private readonly Direction _pathDirection;
            private readonly Func<Node, bool> _isTarget;

			// output
			public Node StartNode;
			public List<Arc>? Path;
			public Node EndNode;

			/// <summary>
			/// Initialize <see cref="PathDfs"/>.
			/// </summary>
			/// <param name="graph"><see cref="IGraph"/>.</param>
			/// <param name="direction">The direction of the Dfs used to search for the path.</param>
			/// <param name="target">A function determining whether a node belongs to the set of target nodes.</param>
			public PathDfs(
                IGraph graph,
                Direction direction,
                Func<Node, bool> target)
			        : base(graph)
            {
				_pathDirection = direction;
                _isTarget = target;
            }

			/// <inheritdoc />
			protected override void Start(out Direction direction)
			{
				direction = _pathDirection;

				StartNode = Node.Invalid;
				Path = new();
				EndNode = Node.Invalid;
			}

            /// <inheritdoc />
			protected override bool NodeEnter(Node node, Arc arc)
			{
                if (arc == Arc.Invalid)
                {
                    StartNode = node;
                }
                else
                {
                    Path?.Add(arc);
                }

				if (_isTarget(node))
				{
					EndNode = node;
					return false;
				}

				return true;
			}

            /// <inheritdoc />
			protected override bool NodeExit(Node node, Arc arc)
			{
                if (arc != Arc.Invalid && EndNode == Node.Invalid)
                {
                    Path?.RemoveAt(Path.Count - 1);
                }

				return true;
			}
		}

		/// <summary>
		/// Finds a path in a graph from a source node to a target node.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		/// <param name="source">The set of source nodes.</param>
		/// <param name="target">A function determining whether a node belongs to the set of target nodes.</param>
		/// <param name="direction">The direction of the Dfs used to search for the path.</param>
		/// <returns>Returns a path from a source node to a target node, or null if none exists.</returns>
		public static IPath? FindPath(this IGraph graph, IEnumerable<Node> source, Func<Node, bool> target, Direction direction)
		{
			var dfs = new PathDfs(graph, direction, target);
            dfs.Run(source);
            if (dfs.EndNode == Node.Invalid)
            {
                return null;
            }

			var result = new Adapters.Path(graph);
			result.Begin(dfs.StartNode);

            foreach (var arc in dfs.Path ?? new())
            {
                result.AddLast(arc);
            }

			return result;
		}

		/// <summary>
		/// Convenience function for finding a path between two nodes.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		/// <param name="source">The source node.</param>
		/// <param name="target">The target node.</param>
		/// <param name="direction"><see cref="Direction"/>.</param>
        public static IPath? FindPath(this IGraph graph, Node source, Node target, Direction direction)
		{
			return FindPath(graph, new[] { source }, x => x == target, direction);
		}
	}
}
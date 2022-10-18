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
    /// <inheritdoc cref="LowpointDfs{TNodeProperty, TArcProperty}"/>
    internal class LowpointDfs :
        LowpointDfs<object, object>
    {
        /// <summary>
        /// Initialize <see cref="LowpointDfs"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        public LowpointDfs(
            IGraph graph)
            : base(graph)
        {
        }
    }

    /// <summary>
    /// Calculates the lowpoint for each node.
    /// </summary>
    /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
    internal class LowpointDfs<TNodeProperty, TArcProperty> : 
        Dfs<TNodeProperty, TArcProperty>
    {
        protected Dictionary<Node, int> Levels = new();
        protected Dictionary<Node, int> Lowpoints = new();

        /// <summary>
        /// Initialize <see cref="LowpointDfs{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        public LowpointDfs(
            IGraph<TNodeProperty, TArcProperty> graph) 
                : base(graph)
        {
        }

        private void UpdateLowpoint(Node node, int newLowpoint)
        {
            if (Lowpoints[node] > newLowpoint)
            {
                Lowpoints[node] = newLowpoint;
            }
        }

        /// <inheritdoc />
        protected override void Start(out Direction direction)
        {
            direction = Direction.Undirected;
            Levels = new();
            Lowpoints = new();
        }

        /// <inheritdoc />
        protected override bool NodeEnter(Node node, Arc arc)
        {
            Levels[node] = Level;
            Lowpoints[node] = Level;
            return true;
        }

        /// <inheritdoc />
        protected override bool NodeExit(Node node, Arc arc)
        {
            if (arc != Arc.Invalid)
            {
                var parent = Graph.Other(arc, node);
                UpdateLowpoint(parent, Lowpoints[node]);
            }
            return true;
        }

        /// <inheritdoc />
        protected override bool BackArc(Node node, Arc arc)
        {
            var other = Graph.Other(arc, node);
            UpdateLowpoint(node, Levels[other]);
            return true;
        }

        /// <inheritdoc />
        protected override void StopSearch()
        {
            Levels = new();
            Lowpoints = new();
        }
    }
}
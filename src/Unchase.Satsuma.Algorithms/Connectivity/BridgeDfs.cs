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

namespace Unchase.Satsuma.Algorithms.Connectivity
{
    /// <inheritdoc cref="BridgeDfs{TNodeProperty, TArcProperty}"/>
    internal class BridgeDfs :
        BridgeDfs<object, object>
    {
        /// <summary>
        /// Initialize <see cref="BridgeDfs"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph"/>.</param>
        public BridgeDfs(
            IGraph graph)
                : base(graph)
        {
        }
    }

    /// <summary>
    /// Calculates bridges.
    /// </summary>
    /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	internal class BridgeDfs<TNodeProperty, TArcProperty> : 
        LowpointDfs<TNodeProperty, TArcProperty>
    {
        /// <summary>
        /// Component count.
        /// </summary>
        public int ComponentCount;

        /// <summary>
        /// The set of bridges.
        /// </summary>
        public HashSet<Arc>? Bridges;

        /// <summary>
        /// Initialize <see cref="BridgeDfs{TNodeProperty, TArcProperty}"/>.
        /// </summary>
        /// <param name="graph"><see cref="IGraph{TNodeProperty, TArcProperty}"/>.</param>
        public BridgeDfs(
            IGraph<TNodeProperty, TArcProperty> graph) 
                : base(graph)
        {
        }

        /// <inheritdoc />
        protected override void Start(out Direction direction)
        {
            base.Start(out direction);
            ComponentCount = 0;
            Bridges = new();
        }

        /// <inheritdoc />
        protected override bool NodeExit(Node node, Arc arc)
        {
            if (arc == Arc.Invalid) ComponentCount++;
            else
            {
                if (Lowpoints[node] == Level)
                {
                    Bridges?.Add(arc);
                    ComponentCount++;
                }
            }

            return base.NodeExit(node, arc);
        }
    }
}
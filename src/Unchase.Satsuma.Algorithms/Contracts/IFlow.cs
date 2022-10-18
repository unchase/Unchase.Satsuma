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

using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Algorithms.Contracts
{
    /// <inheritdoc cref="IFlow{TCapacity,TNodeProperty,TArcProperty}"/>
    /// <typeparam name="TCapacity">The arc capacity type.</typeparam>
    public interface IFlow<TCapacity> :
        IFlow<TCapacity, object, object>
    {
    }

    /// <summary>
    /// Interface to a flow in a network.
    /// </summary>
    /// <remarks>
    /// Edges work as bidirectional channels, as if they were two separate arcs.
    /// </remarks>
    /// <typeparam name="TCapacity">The arc capacity type.</typeparam>
    /// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
    public interface IFlow<TCapacity, TNodeProperty, TArcProperty>
    {
        /// <summary>
        /// The graph of the network.
        /// </summary>
        public IGraph<TNodeProperty, TArcProperty> Graph { get; }

        /// <summary>
        /// The capacity of the arcs.
        /// </summary>
        /// <remarks>
        /// Must be nonnegative (including positive infinity, if applicable).
        /// </remarks>
        public Func<Arc, TCapacity> Capacity { get; }

        /// <summary>
        /// The source of the flow.
        /// </summary>
        public Node Source { get; }

        /// <summary>
        /// The target (sink) of the flow.
        /// </summary>
        public Node Target { get; }

        /// <summary>
        /// The total amount of flow exiting the source node.
        /// </summary>
        public TCapacity FlowSize { get; }

        /// <summary>
        /// Those of the arcs where there is nonzero flow.
        /// </summary>
        /// <remarks>
        /// For each nonzero arc, yields a pair consisting of the arc itself and the flow value on the arc.
        /// </remarks>
        public IEnumerable<KeyValuePair<Arc, TCapacity>> NonzeroArcs { get; }

        /// <summary>
        /// The amount flowing through an arc.
        /// </summary>
        /// <param name="arc"></param>
        /// <returns>
        /// A number between 0 and <tt>Capacity(arc)</tt> if the arc is NOT an edge,
        /// or between <tt>-Capacity(arc)</tt> and <tt>Capacity(arc)</tt> if the arc is an edge.
        /// </returns>
        public TCapacity Flow(Arc arc);
    }
}
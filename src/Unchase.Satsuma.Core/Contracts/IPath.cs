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
# endregion

namespace Unchase.Satsuma.Core.Contracts
{
    /// <summary>
    /// Interface to a read-only path.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Here path is used in a sense that no nodes may be repeated.
    /// The only exception is that the start and end nodes may be equal. 
    /// In this case, the path is called a cycle if it has at least one arc.
    /// </para>
    /// <para>
    /// If the path is a cycle with two nodes, then its two arcs may be equal,
    /// but this is the only case when arc equality is allowed (in fact, possible).
    /// </para>
    /// <para>The path arcs may be undirected or point in any direction (forward/backward).</para>
    /// <para>The #Nodes method always returns the nodes in path order.</para>
    /// <para>
    /// The length of a path is defined as the number of its arcs.
    /// A path is called empty if it has no nodes.
    /// </para>
    /// </remarks>
    public interface IPath : 
        IGraph
    {
        /// <summary>
        /// The first node of the path, or Node.Invalid if the path is empty.
        /// </summary>
        public Node FirstNode { get; }

        /// <summary>
        /// The last node of the path, or Node.Invalid if the path is empty.
        /// </summary>
        /// <remarks>
        /// Equals #FirstNode if the path is a cycle.
        /// </remarks>
        public Node LastNode { get; }

        /// <summary>
        /// Get the arc connecting a node with its successor in the path.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="Arc.Invalid"/> if the node is not on the path or has no successor.
        /// If the path is a cycle, then each node has a successor.
        /// </remarks>
        /// <param name="node">The node.</param>
        /// <returns>Returns the arc connecting a node with its successor in the path.</returns>
        public Arc NextArc(Node node);

        /// <summary>
        /// Get the arc connecting a node with its predecessor in the path.
        /// </summary>
        /// <remarks>
        /// Returns <see cref="Arc.Invalid"/> if the node is not on the path or has no predecessor.
        /// If the path is a cycle, then each node has a predecessor.
        /// </remarks>
        /// <param name="node">The node.</param>
        /// <returns>Returns the arc connecting a node with its predecessor in the path.</returns>
        public Arc PrevArc(Node node);
    }
}
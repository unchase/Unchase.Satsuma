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

using Unchase.Satsuma.Core.Enums;

namespace Unchase.Satsuma.Core.Contracts
{
    /// <summary>
    /// A graph which can build new nodes and arcs.
    /// </summary>
    public interface IBuildableGraph : 
        IClearable
    {
        /// <summary>
        /// Adds a new node to the graph.
        /// </summary>
        /// <returns>Return new added node.</returns>
        public Node AddNode();

        /// <summary>
        /// Adds a directed arc or an edge (undirected arc) between u and v to the graph.
        /// </summary>
        /// <remarks>
        /// Only works if the two nodes are valid and belong to the graph,
        /// otherwise no exception is guaranteed to be thrown and the result is undefined behaviour.
        /// </remarks>
        /// <param name="u">The source node.</param>
        /// <param name="v">The target node.</param>
        /// <param name="directedness">Determines whether the new arc will be directed or an edge (i.e. undirected).</param>
        /// <returns>Returns a directed arc or an edge (undirected arc) between u and v to the graph.</returns>
        public Arc AddArc(Node u, Node v, Directedness directedness);
    }
}
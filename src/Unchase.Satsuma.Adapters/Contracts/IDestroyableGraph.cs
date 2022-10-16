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
# endregion

using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;

namespace Unchase.Satsuma.Adapters.Contracts
{
    /// <summary>
    /// A graph which can destroy its nodes and arcs.
    /// </summary>
    public interface IDestroyableGraph : 
        IClearable
    {
        /// <summary>
        /// Deletes a node from the graph.
        /// </summary>
        /// <param name="node">Deleted node.</param>
        /// <returns>Returns true if the deletion was successful.</returns>
        public bool DeleteNode(Node node);

        /// <summary>
        /// Deletes a directed or undirected arc from the graph.
        /// </summary>
        /// <param name="arc">Deleted arc.</param>
        /// <returns>Returns true if the deletion was successful.</returns>
        public bool DeleteArc(Arc arc);
    }
}
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

namespace Unchase.Satsuma.Core.Enums
{
    /// <summary>
    /// Allows filtering arcs. Can be passed to functions which return a collection of arcs.
    /// </summary>
    public enum ArcFilter
    {
        /// <summary>
        /// All arcs.
        /// </summary>
        All,

        /// <summary>
        /// Only undirected arcs.
        /// </summary>
        Edge,

        /// <summary>
        /// Only edges, or directed arcs from the first point (to the second point, if any).
        /// </summary>
        Forward,

        /// <summary>
        /// Only edges, or directed arcs to the first point (from the second point, if any).
        /// </summary>
        Backward
    }
}
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
#endregion

using Unchase.Satsuma.Core.Enums;

namespace Unchase.Satsuma.Core.Contracts
{
    /// <summary>
	/// Interface to a read-only graph.
	/// </summary>
	public interface IGraph : 
        IArcLookup,
        INodeLookup
	{
        /// <summary>
		/// Get all nodes of the graph.
		/// </summary>
		/// <returns>Returns all nodes of the graph.</returns>
		public IEnumerable<Node> Nodes();

		/// <summary>
		/// Get all arcs of the graph satisfying a given filter.
		/// </summary>
		/// <param name="filter">
		/// <para>Cannot be <see cref="ArcFilter.Forward"/> or <see cref="ArcFilter.Backward"/>.</para>
		/// <para>If <see cref="ArcFilter.All"/>, then all arcs are returned.</para>
		/// <para>If <see cref="ArcFilter.Edge"/>, only the edges (undirected arcs) are returned.</para>
		/// </param>
		/// <returns>Returns all arcs of the graph satisfying a given filter.</returns>
		public IEnumerable<Arc> Arcs(ArcFilter filter = ArcFilter.All);

		/// <summary>
		/// Get all arcs adjacent to a specific node satisfying a given filter.
		/// </summary>
		/// <param name="u"></param>
		/// <param name="filter">
		/// <para>If <see cref="ArcFilter.All"/>, then all arcs are returned.</para>
		/// <para>If <see cref="ArcFilter.Edge"/>, only the edges (undirected arcs) are returned.</para>
		/// <para>If <see cref="ArcFilter.Forward"/>, only the arcs exiting u (this includes edges) are returned.</para>
		/// <para>If <see cref="ArcFilter.Backward"/>, only the arcs entering u (this includes edges) are returned.</para>
		/// </param>
		/// <returns>Returns all arcs adjacent to a specific node satisfying a given filter.</returns>
        public IEnumerable<Arc> Arcs(Node u, ArcFilter filter = ArcFilter.All);

		/// <summary>
		/// Get all arcs adjacent to two nodes satisfying a given filter.
		/// </summary>
		/// <param name="u">U node.</param>
		/// <param name="v">V node.</param>
		/// <param name="filter">
		/// <para>If <see cref="ArcFilter.All"/>, then all arcs are returned.</para>
		/// <para>If <see cref="ArcFilter.Edge"/>, only the edges (undirected arcs) are returned.</para>
		/// <para>If <see cref="ArcFilter.Forward"/>, only the arcs from \e u to \e v (this includes edges) are returned.</para>
		/// <para>If <see cref="ArcFilter.Backward"/>, only the arcs from \e v to \e u (this includes edges) are returned.</para>
		/// </param>
		/// <returns>Returns all arcs adjacent to two nodes satisfying a given filter.</returns>
		public IEnumerable<Arc> Arcs(Node u, Node v, ArcFilter filter = ArcFilter.All);

		/// <summary>
		/// Get the total number of nodes in O(1) time.
		/// </summary>
		/// <returns>Returns the total number of nodes in O(1) time.</returns>
		public int NodeCount();

		/// <summary>
		/// Get the total number of arcs satisfying a given filter.
		/// </summary>
		/// <param name="filter">
		/// <para>Cannot be <see cref="ArcFilter.Forward"/> or <see cref="ArcFilter.Backward"/>.</para>
		/// <para>If <see cref="ArcFilter.All"/>, then all arcs are returned.</para>
		/// <para>If <see cref="ArcFilter.Edge"/>, only the edges (undirected arcs) are returned.</para>
		/// </param>
		/// <returns>Returns the total number of arcs satisfying a given filter.</returns>
		public int ArcCount(ArcFilter filter = ArcFilter.All);

		/// <summary>
		/// Get the number of arcs adjacent to a specific node satisfying a given filter.
		/// </summary>
		/// <param name="u">U node.</param>
		/// <param name="filter">
		/// <para>If <see cref="ArcFilter.All"/>, then all arcs are returned.</para>
        /// <para>If <see cref="ArcFilter.Edge"/>, only the edges (undirected arcs) are returned.</para>
        /// <para>If <see cref="ArcFilter.Forward"/>, only the arcs exiting u (this includes edges) are returned.</para>
        /// <para>If <see cref="ArcFilter.Backward"/>, only the arcs entering u (this includes edges) are returned.</para>
		/// </param>
		/// <returns>Returns the number of arcs adjacent to a specific node satisfying a given filter.</returns>
		public int ArcCount(Node u, ArcFilter filter = ArcFilter.All);

		/// <summary>
		/// Get the number of arcs adjacent to two nodes satisfying a given filter.
		/// </summary>
		/// <param name="u">U node.</param>
		/// <param name="v">V node.</param>
		/// <param name="filter">
		/// <para>If <see cref="ArcFilter.All"/>, then all arcs are returned.</para>
        /// <para>If <see cref="ArcFilter.Edge"/>, only the edges (undirected arcs) are returned.</para>
        /// <para>If <see cref="ArcFilter.Forward"/>, only the arcs from \e u to \e v (this includes edges) are returned.</para>
        /// <para>If <see cref="ArcFilter.Backward"/>, only the arcs from \e v to \e u (this includes edges) are returned.</para>
		/// </param>
		/// <returns>Returns the number of arcs adjacent to two nodes satisfying a given filter.</returns>
		public int ArcCount(Node u, Node v, ArcFilter filter = ArcFilter.All);

		// Returns whether the given node is contained in the graph.
		// Must return the same value as <tt>%Nodes().Contains</tt> in all implementations, but faster if possible.
		// \note \c true may be returned for nodes coming from another graph as well,
		// if those nodes encapsulate an identifier which is valid for this graph, too.

		/// <summary>
		/// Get whether the given node is contained in the graph.
		/// </summary>
		/// <remarks>
		/// Must return the same value as Nodes().Contains in all implementations, but faster if possible.
        /// true may be returned for nodes coming from another graph as well,
        /// if those nodes encapsulate an identifier which is valid for this graph, too.
		/// </remarks>
		/// <param name="node">Node.</param>
		/// <returns>Returns whether the given node is contained in the graph.</returns>
		public bool HasNode(Node node);

		/// <summary>
		/// Get whether the given arc is contained in the graph.
		/// </summary>
		/// <remarks>
		/// Must return the same value as Arcs().Contains in all implementations, but faster if possible.
        /// true may be returned for arcs coming from another graph as well,
        /// if those arcs encapsulate an identifier which is valid for this graph, too.
		/// </remarks>
		/// <param name="arc"></param>
		/// <returns>Returns whether the given arc is contained in the graph.</returns>
		public bool HasArc(Arc arc);
	}
}
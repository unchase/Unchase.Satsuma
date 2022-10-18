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

using System.Globalization;
using System.Text.RegularExpressions;
using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Adapters.Contracts;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;

namespace Unchase.Satsuma.IO
{
	/// <summary>
	/// Loads and saves graphs which are stored in a very simple format.
	/// </summary>
	/// <remarks>
	/// <para>The first line must contain two numbers (the <b>count of nodes and arcs</b>).</para>
	/// <para>
	/// Each additional line must contain a pair of numbers for each arc
	/// (that is, the identifiers of the <b>start</b> and <b>end nodes</b> of the arc).
	/// </para>
	/// <para>Optionally, arc functions (extensions) can be defined as excess tokens after the arc definition.</para>
	/// <para>Extensions are separated by whitespaces and thus must be nonempty strings containing no whitespaces.</para>
	/// <para>
	/// The following example describes a path on 4 nodes,
	/// whose arcs each have a name and a cost associated to them.
	/// %Node numbering starts from 1 here.
	/// </para>
	/// <para>
	/// <code>
	/// 4 3
	/// 1 2 Arc1 0.2
	/// 2 3 Arc2 1.25
	/// 3 4 Arc3 0.33
	/// </code>
	/// </para>
	/// <para>The above example can be processed like this (provided that it is stored in c:\\graph.txt):</para>
	/// <para>
	/// <code>
	/// SimpleGraphFormat loader = new SimpleGraphFormat { StartIndex = 1 };
	/// Node[] nodes = loader.Load(@"c:\graph.txt", Directedness.Directed);
	/// // retrieve the loaded data
	/// IGraph graph = loader.Graph;
	/// Dictionary&lt;Arc, string&gt; arcNames = loader.Extensions[0];
	/// Dictionary&lt;Arc, double&gt; arcCosts = 
	///		loader.Extensions[1].ToDictionary(kv => kv.Key, kv => double.Parse(kv.Value, CultureInfo.InvariantCulture));
	/// </code>
	/// </para>
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
    /// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public sealed class SimpleGraphFormat<TNodeProperty, TArcProperty>
    {
		/// <summary>
		/// The graph itself.
		/// </summary>
		/// <remarks>
		/// <para>- <b>When loading</b>: Must be an <see cref="IBuildableGraph"/> to accomodate the loaded graph.</para>
		/// <para><see cref="CustomGraph{TNodeProperty, TArcProperty}"/> instance by default.</para>
		/// <para>- <b>When saving</b>: Can be an arbitrary graph (not null).</para>
		/// </remarks>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; set; } = new CustomGraph<TNodeProperty, TArcProperty>();

		/// <summary>
		/// The extensions (arc functions).
		/// </summary>
		/// <remarks>
		/// <para>All the contained dictionaries must assign values to all the arcs of the graph.</para>
		/// <para>Values must be nonempty strings containing no whitespaces.</para>
		/// <para>This is not checked when saving.</para>
		/// </remarks>
		public IList<Dictionary<Arc, string>> Extensions { get; private set; }

		/// <summary>
		/// The index where node numbering starts (0 by default).
		/// </summary>
		/// <remarks>
		/// Set this parameter to the correct value both before loading and saving.
		/// </remarks>
		public int StartIndex { get; set; }

		/// <summary>
		/// Initialize <see cref="SimpleGraphFormat{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		public SimpleGraphFormat()
		{
			Extensions = new List<Dictionary<Arc, string>>();
		}

		/// <summary>
		/// Loads from a reader.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="directedness">
		/// <para>Specifies the directedness of the graph to be loaded. Possible values:</para>
		/// <para>- <see cref="Directedness.Directed"/>: each created arc will be directed.</para>
		/// <para>- <see cref="Directedness.Undirected"/>: each created arc will be an edge (i.e. undirected).</para>
		/// </param>
		/// <returns>Returns the loaded nodes, by index ascending.</returns>
		public Node[] Load(TextReader reader, Directedness directedness)
		{
            //if (Graph == null)
            //{
            //    Graph = new CustomGraph();
            //}

			var buildableGraph = (IBuildableGraph)Graph;
			buildableGraph.Clear();

            var whitespaces = new Regex(@"\s+");

			// first line: number of nodes and arcs
			var tokens = whitespaces.Split(reader.ReadLine() ?? throw new InvalidOperationException());
			var nodeCount = int.Parse(tokens[0], CultureInfo.InvariantCulture);
			var arcCount = int.Parse(tokens[1], CultureInfo.InvariantCulture);

			var nodes = new Node[nodeCount];
            for (var i = 0; i < nodeCount; i++)
            {
                nodes[i] = buildableGraph.AddNode();
            }

			Extensions.Clear();

			for (var i = 0; i < arcCount; i++)
			{
				tokens = whitespaces.Split(reader.ReadLine() ?? throw new InvalidOperationException());
				var a = (int)(long.Parse(tokens[0], CultureInfo.InvariantCulture) - StartIndex);
				var b = (int)(long.Parse(tokens[1], CultureInfo.InvariantCulture) - StartIndex);

				var arc = buildableGraph.AddArc(nodes[a], nodes[b], directedness);

				var extensionCount = tokens.Length - 2;
                for (var j = 0; j < extensionCount - Extensions.Count; j++)
                {
                    Extensions.Add(new());
                }

                for (var j = 0; j < extensionCount; j++)
                {
                    Extensions[j][arc] = tokens[2 + j];
                }
			}

			return nodes;
		}

		/// <summary>
		/// Loads from a file.
		/// </summary>
		/// <param name="filename">The file name.</param>
		/// <param name="directedness"><see cref="Directedness"/>.</param>
        public Node[] Load(string filename, Directedness directedness)
        {
            using var reader = new StreamReader(filename);
            return Load(reader, directedness);
        }

		/// <summary>
		/// Saves to a writer.
		/// </summary>
		/// <param name="writer">A writer on the output file, e.g. a <see cref="StreamWriter"/>.</param>
		/// <exception cref="ArgumentException"></exception>
		public void Save(TextWriter writer)
		{
			var whitespace = new Regex(@"\s");

			writer.WriteLine(Graph.NodeCount() + " " + Graph.ArcCount());
			var index = new Dictionary<Node, long>();
			long indexFactory = StartIndex;
			foreach (var arc in Graph.Arcs())
			{
				var u = Graph.U(arc);
                if (!index.TryGetValue(u, out var uIndex))
                {
                    index[u] = uIndex = indexFactory++;
                }

				var v = Graph.V(arc);
                if (!index.TryGetValue(v, out var vIndex))
                {
                    index[v] = vIndex = indexFactory++;
                }

				writer.Write(uIndex + " " + vIndex);
				foreach (var ext in Extensions)
				{
                    ext.TryGetValue(arc, out var value);
                    if (string.IsNullOrEmpty(value) || whitespace.IsMatch(value))
                    {
                        throw new ArgumentException("Extension value is empty or contains whitespaces.");
                    }

					writer.Write(' ' + ext[arc]);
				}

				writer.WriteLine();
			}
		}

		/// <summary>
		/// Saves to a file.
		/// </summary>
		/// <param name="filename">The file name.</param>
		public void Save(string filename)
        {
            using var writer = new StreamWriter(filename);
            Save(writer);
        }
	}
}
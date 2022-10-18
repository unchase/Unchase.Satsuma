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

using System.Text;
using System.Text.RegularExpressions;
using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Adapters.Contracts;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;

namespace Unchase.Satsuma.IO
{
	/// <inheritdoc cref="LemonGraphFormat{TNodeProperty, TArcProperty}"/>
	public sealed class LemonGraphFormat :
        LemonGraphFormat<object, object>
    {
		/// <summary>
        /// Initialize <seealso cref="LemonGraphFormat"/>.
        /// </summary>
        public LemonGraphFormat()
		{
        }
    }

	/// <summary>
	/// Loads and saves graphs stored in the <em>Lemon Graph Format</em>.
	/// </summary>
	/// <remarks>
	/// See <a href='https://projects.coin-or.org/svn/LEMON/trunk/doc/lgf.dox'>this documentation page</a>
	/// for a specification of the LGF.
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
	/// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public class LemonGraphFormat<TNodeProperty, TArcProperty>
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
		/// The node maps, as contained in the nodes section of the input.
		/// </summary>
		/// <remarks>
		/// <tt>NodeMaps["label"]</tt> is never taken into account when saving, 
        /// as label is a special word in LGF,
        /// and node labels are always generated automatically to ensure uniqueness.
		/// </remarks>
		public Dictionary<string, Dictionary<Node, string>> NodeMaps { get; }

		/// <summary>
		/// The arc maps, as contained in the \@arcs and \@edges sections of the input.
		/// </summary>
		public Dictionary<string, Dictionary<Arc, string>> ArcMaps { get; }

		/// <summary>
		/// The attributes, as contained in the \@attributes section of the input.
		/// </summary>
		public Dictionary<string, string> Attributes { get; }

		/// <summary>
		/// Initialize <seealso cref="LemonGraphFormat{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		public LemonGraphFormat()
		{
			NodeMaps = new();
			ArcMaps = new();
			Attributes = new();
		}

		private static string Escape(string s)
		{
			var result = new StringBuilder();
			foreach (var c in s)
			{
				switch (c)
				{
					case '\n': result.Append("\\n"); break;
					case '\r': result.Append("\\r"); break;
					case '\t': result.Append("\\t"); break;
					case '"': result.Append("\\\""); break;
					case '\\': result.Append("\\\\"); break;
					default: result.Append(c); break;
				}
			}
			return result.ToString();
		}

		private static string Unescape(string s)
		{
			var result = new StringBuilder();
			var escaped = false;
			foreach (var c in s)
			{
				if (escaped)
				{
					switch (c)
					{
						case 'n': result.Append('\n'); break;
						case 'r': result.Append('\r'); break;
						case 't': result.Append('\t'); break;
						default: result.Append(c); break;
					}
					escaped = false;
				}
				else
				{
					escaped = (c == '\\');
					if (!escaped) result.Append(c);
				}
			}
			return result.ToString();
		}

		/// <summary>
		/// Loads from a reader.
		/// </summary>
		/// <param name="reader">A reader on the input file, e.g. a <see cref="StreamReader"/>.</param>
		/// <param name="directedness">
		/// <para>Specifies the directedness of the graph to be loaded. Possible values:</para>
		/// <para>- <see cref="Directedness.Directed"/>: each created arc will be directed.</para>
		/// <para>- <see cref="Directedness.Undirected"/>: each created arc will be undirected.</para>
		/// <para>
		/// - null (default): arcs defined in \@arcs sections will be directed, 
		///   while those defined in \@edges sections will be undirected.
		/// </para>
		/// </param>
		public void Load(TextReader reader, Directedness? directedness)
		{
            var buildableGraph = (IBuildableGraph)Graph;
			buildableGraph.Clear();

			NodeMaps.Clear();
			var nodeFromLabel = new Dictionary<string, Node>();
			ArcMaps.Clear();
			Attributes.Clear();

			var splitRegex = new Regex(@"\s*((""(\""|.)*"")|(\S+))\s*", RegexOptions.Compiled);
			var section = string.Empty;
			var currentDir = Directedness.Directed; // are currently read arcs directed?
			var prevHeader = false;
			List<string> columnNames = new();
			var labelColumnIndex = -1;

			while (true)
			{
				var line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

				line = line.Trim();
                if (line == string.Empty || line[0] == '#')
                {
                    continue;
                }

				var tokens = splitRegex.Matches(line)
					.Select(m =>
					{
						var s = m.Groups[1].Value;
                        if (s == string.Empty)
                        {
                            return s;
                        }

                        if (s[0] == '"' && s[^1] == '"')
                        {
                            s = Unescape(s[1..^1]);
                        }

						return s;
					}).ToList();
				var first = tokens.First();

				// header?
				if (line[0] == '@')
				{
					section = first[1..];
					currentDir = directedness 
                                 ?? (section == "arcs" ? Directedness.Directed : Directedness.Undirected);

					prevHeader = true;
					continue;
				}

				switch (section)
				{
					case "nodes":
					case "red_nodes":
					case "blue_nodes":
						{
							if (prevHeader)
							{
								columnNames = tokens;
								for (var i = 0; i < columnNames.Count; i++)
								{
									var column = columnNames[i];
                                    if (column == "label")
                                    {
                                        labelColumnIndex = i;
                                    }

                                    if (!NodeMaps.ContainsKey(column))
                                    {
                                        NodeMaps[column] = new();
                                    }
								}
							}
							else
							{
								var node = buildableGraph.AddNode();
								for (var i = 0; i < tokens.Count; i++)
								{
									NodeMaps[columnNames[i]][node] = tokens[i];
                                    if (i == labelColumnIndex)
                                    {
                                        nodeFromLabel[tokens[i]] = node;
                                    }
								}
							}
						}
						break;

					case "arcs":
					case "edges":
						{
							if (prevHeader)
							{
								columnNames = tokens;
								foreach (var column in columnNames)
                                {
                                    if (!ArcMaps.ContainsKey(column))
                                    {
                                        ArcMaps[column] = new();
                                    }
                                }
							}
							else
							{
								var u = nodeFromLabel[tokens[0]];
								var v = nodeFromLabel[tokens[1]];
								var arc = buildableGraph.AddArc(u, v, currentDir);
                                for (var i = 2; i < tokens.Count; i++)
                                {
                                    ArcMaps[columnNames[i - 2]][arc] = tokens[i];
                                }
							}
						}
						break;

					case "attributes":
						{
							Attributes[tokens[0]] = tokens[1];
						}
						break;
				}

				prevHeader = false;
			} // while can read from file
		}

		/// <summary>
		/// Loads from a file.
		/// </summary>
		/// <param name="filename">The file name.</param>
		/// <param name="directedness"><see cref="Directedness"/>.</param>
		public void Load(string filename, Directedness? directedness)
        {
            using var reader = new StreamReader(filename);
            Load(reader, directedness);
        }

		/// <summary>
		/// Saves to a writer.
		/// </summary>
		/// <remarks>
		/// All node and arc maps and attributes are saved as well, except <tt>NodeMaps["label"]</tt> (if present).
		/// </remarks>
		/// <param name="writer">A writer on the output file, e.g. a <see cref="StreamWriter"/>.</param>
		/// <param name="comment">Comment lines to write at the beginning of the file.</param>
		public void Save(TextWriter writer, IEnumerable<string>? comment = null)
		{
            if (comment != null)
            {
                foreach (var line in comment)
                {
                    writer.WriteLine("# " + line);
                }
            }

			// nodes
			writer.WriteLine("@nodes");
			writer.Write("label");
            foreach (var kv in NodeMaps)
            {
                if (kv.Key != "label")
                {
                    writer.Write(' ' + kv.Key);
                }
            }

			writer.WriteLine();
			foreach (var node in Graph.Nodes())
			{
				writer.Write(node.Id);
				foreach (var kv in NodeMaps) 
                {
                    if (kv.Key != "label")
				    {
                        if (!kv.Value.TryGetValue(node, out var value))
                        {
                            value = string.Empty;
                        }

					    writer.Write(" \"" + Escape(value) + '"');
				    }
                }

				writer.WriteLine();
			}

			writer.WriteLine();

			// arcs (including edges)
			for (var i = 0; i < 2; i++)
			{
				var arcs = (i == 0 ? Graph.Arcs().Where(arc => !Graph.IsEdge(arc)) : Graph.Arcs(ArcFilter.Edge));
				writer.WriteLine(i == 0 ? "@arcs" : "@edges");
                if (ArcMaps.Count == 0)
                {
                    writer.WriteLine('-');
                }
				else
				{
                    foreach (var kv in ArcMaps)
                    {
                        writer.Write(kv.Key + ' ');
                    }

					writer.WriteLine();
				}
				foreach (var arc in arcs)
				{
					writer.Write(Graph.U(arc).Id + ' ' + Graph.V(arc).Id);
					foreach (var kv in ArcMaps)
					{
                        if (!kv.Value.TryGetValue(arc, out var value))
                        {
                            value = "";
                        }

						writer.Write(" \"" + Escape(value) + '"');
					}

					writer.WriteLine();
				}

				writer.WriteLine();
			}

			// attributes
			if (Attributes.Count > 0)
			{
				writer.WriteLine("@attributes");
                foreach (var kv in Attributes)
                {
                    writer.WriteLine('"' + Escape(kv.Key) + "\" \"" + Escape(kv.Value) + '"');
                }

				writer.WriteLine();
			}
		}

		/// <summary>
		/// Saves to a file.
		/// </summary>
		/// <param name="filename">The file name.</param>
		/// <param name="comment">Comment lines to write at the beginning of the file.</param>
		public void Save(string filename, IEnumerable<string>? comment = null)
        {
            using var writer = new StreamWriter(filename);
            Save(writer, comment);
        }
	}
}
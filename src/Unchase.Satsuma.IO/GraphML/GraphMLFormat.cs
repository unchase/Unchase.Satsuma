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
using System.Xml;
using System.Xml.Linq;
using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Adapters.Contracts;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.IO.GraphML.Abstractions;
using Unchase.Satsuma.IO.GraphML.Enums;

namespace Unchase.Satsuma.IO.GraphML
{
    /// <inheritdoc cref="GraphMlFormat{TNodeProperty, TArcProperty}"/>
	public sealed class GraphMlFormat :
        GraphMlFormat<object, object>
    {
        internal static readonly XNamespace Xmlns = "http://graphml.graphdrawing.org/xmlns";
        internal static readonly XNamespace XmlnsXsi = "http://www.w3.org/2001/XMLSchema-instance"; // xmlns:xsi
        internal static readonly XNamespace XmlnsY = "http://www.yworks.com/xml/graphml"; // xmlns:y
        internal static readonly XNamespace XmlnsYed = "http://www.yworks.com/xml/yed/3"; // xmlns:yed
        internal const string XsiSchemaLocation = "http://graphml.graphdrawing.org/xmlns\n" + // xsi:schemaLocation
                                                  "http://graphml.graphdrawing.org/xmlns/1.0/graphml.xsd";

		/// <summary>
		/// Initialize <seealso cref="GraphMlFormat"/>.
		/// </summary>
		public GraphMlFormat()
        {
		}
    }

	/// <summary>
	/// Loads and saves graphs stored in GraphML format.
	/// </summary>
	/// <remarks>
	/// <para>
	/// See <a href='http://graphml.graphdrawing.org/'>the GraphML website</a>
	/// for information on the GraphML format.
	/// </para>
	/// <para>Example: <b>Loading a graph and some special values for objects</b></para>
	/// <para>
	/// <code>
	/// using GraphML = Satsuma.IO.GraphML;
	/// // [...]
	/// GraphML.GraphMLFormat f = new GraphML.GraphMLFormat();
	/// f.Load(@"c:\my_little_graph.graphml");
	/// // retrieve the loaded graph
	/// var g = f.Graph;
	/// // retrieve the property defining the appearance of the nodes
	/// GraphML.NodeGraphicsProperty ngProp = (GraphML.NodeGraphicsProperty)
	/// 	f.Properties.FirstOrDefault(x =&gt; x is GraphML.NodeGraphicsProperty);
	/// foreach (var node in g.Nodes())
	/// {
	/// 	GraphML.NodeGraphics ng = null;
	/// 	if (ngProp != null) ngProp.TryGetValue(node, out ng);
	/// 	Console.Write("Node "+node+": ");
	/// 	if (ng == null) Console.WriteLine("no position defined");
	/// 	else Console.WriteLine(string.Format("X={0};Y={1}", ng.X, ng.Y));
	/// }
	/// // retrieve some user-defined property defining weights for arcs
	/// GraphML.StandardProperty&lt;double&gt; weights = (GraphML.StandardProperty&lt;double&gt;)
	///		f.Properties.FirstOrDefault(x =&gt; x.Name == "weight" &amp;&amp; 
	///			(x.Domain == GraphML.PropertyDomain.All || x.Domain == GraphML.PropertyDomain.Arc) &amp;&amp;
	///			x is GraphML.StandardProperty&lt;double&gt;);
	/// foreach (var arc in g.Arcs())
	/// {
	///		double weight = 0;
	///		bool hasWeight = (weights != null &amp;&amp; weights.TryGetValue(arc, out weight));
	///		Console.WriteLine("Arc "+arc+": weight is "+(hasWeight ? weight.ToString() : "undefined"));
	/// }
	/// </code>
	/// </para>
	/// <para>Example: <b>Saving a complete bipartite graph without any bells and whistles</b></para>
	/// <para>
	/// <code>
	/// GraphML.GraphMLFormat f = new GraphML.GraphMLFormat();
	/// f.Graph = new CompleteBipartiteGraph(3, 5, Directedness.Undirected);
	/// f.Save(@"c:\my_little_graph.graphml");
	/// </code>
	/// </para>
	/// <para>Example: <b>Saving a graph with node and arc annotations</b></para>
	/// <para>
	/// <code>
	/// string[] nodeNames =       { "London", "Paris", "New York" };
	/// double[,] distanceMatrix = { {    0,     343.93, 5576.46 },
	///                              {  343.93,    0,    5843.78 },
	///                              { 5576.46, 5843.78,    0    } };
	/// CompleteGraph g = new CompleteGraph(nodeNames.Length, Directedness.Undirected);
	/// Kruskal&lt;double&gt; kruskal = new Kruskal&lt;double&gt;(g,
	///		arc => distanceMatrix[g.GetNodeIndex(g.U(arc)), g.GetNodeIndex(g.V(arc))]);
	/// kruskal.Run();
	/// GraphMLFormat gml = new GraphMLFormat();
	/// gml.Graph = kruskal.ForestGraph;
	/// gml.AddStandardNodeProperty("name", n => nodeNames[g.GetNodeIndex(n)]);
	/// gml.AddStandardArcProperty("color", a => distanceMatrix[g.GetNodeIndex(g.U(a)), g.GetNodeIndex(g.V(a))] &lt; 1000 ? "#ff0000" : "#0000ff");
	/// gml.AddStandardArcProperty("distance", a => distanceMatrix[g.GetNodeIndex(g.U(a)), g.GetNodeIndex(g.V(a))]);
	/// gml.Save("tree_with_annotations.graphml");
	/// </code>
	/// </para>
	/// <para>
	/// For more detailed examples on saving extra values for nodes, arcs or the graph itself;
	/// see the descendants of <see cref="GraphMLProperty"/>, such as <see cref="StandardProperty{T}"/> and <see cref="NodeGraphicsProperty"/>.
	/// </para>
	/// </remarks>
	/// <typeparam name="TNodeProperty">The type of stored node properties.</typeparam>
	/// <typeparam name="TArcProperty">The type of stored arc properties.</typeparam>
	public class GraphMlFormat<TNodeProperty, TArcProperty>
	{
        /// <summary>
		/// The graph itself.
		/// </summary>
		/// <remarks>
		/// <para>- <b>When loading</b>: Must be an <see cref="IBuildableGraph"/> to accomodate the loaded graph, or null.</para>
		/// <para>- <b>When saving</b>: Can be an arbitrary graph (not null).</para>
		/// </remarks>
		public IGraph<TNodeProperty, TArcProperty> Graph { get; set; } = new CustomGraph<TNodeProperty, TArcProperty>();
  
		/// <summary>
		/// Returns a GraphML identifier for each node. May be null.
		/// </summary>
		/// <remarks>
		/// <para>- <b>When saving</b>: No two nodes may have the same id.</para>
		/// <para>Nodes with no id specified will have a generated id.</para>
		/// </remarks>
		public Dictionary<Node, string> NodeId { get; set; } = new();

		/// <summary>
		/// Returns an optional GraphML identifier for each arc. May be null.
		/// </summary>
		/// <remarks>
		/// - <b>When saving</b>: Arcs with no id specified will not have any id in the resulting file.
		/// </remarks>
		public Dictionary<Arc, string> ArcId { get; set; } = new();

		/// <summary>
		/// The properties (special data for nodes, arcs and the graph itself).
		/// </summary>
		public IList<GraphMLProperty> Properties { get; }

		private readonly List<Func<XElement, GraphMLProperty>> _propertyLoaders;

		/// <summary>
		/// Initialize <seealso cref="GraphMlFormat{TNodeProperty, TArcProperty}"/>.
		/// </summary>
		public GraphMlFormat()
		{
			Properties = new List<GraphMLProperty>();
			_propertyLoaders = new()
            {
				x => new StandardProperty<bool>(x),
				x => new StandardProperty<double>(x),
				x => new StandardProperty<float>(x),
				x => new StandardProperty<int>(x),
				x => new StandardProperty<long>(x),
				x => new StandardProperty<string>(x),
				x => new NodeGraphicsProperty(x)
			};
		}

		/// <summary>
		/// Registers a new GraphML property loader.
		/// </summary>
		/// <remarks>
		/// <para>By default, recognition of StandardProperty&lt;T&gt; and NodeGraphicsProperty is supported when loading.</para>
		/// <para>You can define your own property classes by calling this method to add a loader.</para>
		/// <para>The loader chain is used to make properties from <tt>&lt;key&gt;</tt> elements.</para>
		/// </remarks>
		/// <param name="loader">
		/// Must take an XElement (the key) as argument,
		/// and return a property with the parameters defined by the key element.
		/// Must throw ArgumentException if the element could not be recognized
		/// as a definition of the property class supported by the loader.
		/// </param>
		public void RegisterPropertyLoader(Func<XElement, GraphMLProperty> loader)
		{
			_propertyLoaders.Add(loader);
		}

		private static void ReadProperties(Dictionary<string, GraphMLProperty> propertyById, XElement? x, object obj)
		{
            if (x != null)
            {
                foreach (var xData in Utils.ElementsLocal(x, "data"))
                {
                    var keyAttribute = xData.Attribute("key");
                    if (keyAttribute != null && propertyById.TryGetValue(keyAttribute.Value, out var p))
                    {
                        p.ReadData(xData, obj);
                    }
                }
			}
        }

		/// <summary>
		/// Loads from an XML document.
		/// </summary>
		/// <param name="doc"><see cref="XDocument"/>.</param>
		public void Load(XDocument doc)
		{
			// Namespaces are ignored so we can load broken documents.
            var buildableGraph = (IBuildableGraph)Graph;
			buildableGraph.Clear();
			var xGraphMl = doc.Root;

			// load properties
			Properties.Clear();
			var propertyById = new Dictionary<string, GraphMLProperty>();
            if (xGraphMl != null)
            {
                foreach (var xKey in Utils.ElementsLocal(xGraphMl, "key"))
                {
                    foreach (var handler in _propertyLoaders)
                    {
                        try
                        {
                            var p = handler(xKey);
                            Properties.Add(p);
                            if (p.Id != null)
                            {
                                propertyById[p.Id] = p;
                            }

                            break;
                        }
                        catch (ArgumentException) { }
                    }
                }

                // load graph
                var xGraph = Utils.ElementLocal(xGraphMl, "graph");
                var defaultDirectedness = (xGraph?.Attribute("edgedefault")?.Value == "directed"
                    ? Directedness.Directed
                    : Directedness.Undirected);
                ReadProperties(propertyById, xGraph, Graph);

                // load nodes
                NodeId = new();
                var nodeById = new Dictionary<string, Node>();
                foreach (var xNode in Utils.ElementsLocal(xGraph!, "node"))
                {
                    var node = buildableGraph.AddNode();
                    var id = xNode.Attribute("id")?.Value;
                    NodeId[node] = id!;
                    nodeById[id!] = node;
                    ReadProperties(propertyById, xNode, node);
                }

                // load arcs
                ArcId = new();
                foreach (var xArc in Utils.ElementsLocal(xGraph!, "edge"))
                {
                    var u = nodeById[xArc.Attribute("source")!.Value];
                    var v = nodeById[xArc.Attribute("target")!.Value];

                    var dir = defaultDirectedness;
                    var dirAttr = xArc.Attribute("directed");
                    if (dirAttr != null)
                    {
                        dir = dirAttr.Value == "true"
                            ? Directedness.Directed
                            : Directedness.Undirected;
                    }

                    var arc = buildableGraph.AddArc(u, v, dir);
                    var xId = xArc.Attribute("id");

                    if (xId != null)
                    {
                        ArcId[arc] = xId.Value;
                    }

                    ReadProperties(propertyById, xArc, arc);
                }
			}
        }

		/// <summary>
		/// Loads from an XML reader.
		/// </summary>
		/// <param name="xml"><see cref="XmlReader"/>.</param>
		public void Load(XmlReader xml)
		{
			var doc = XDocument.Load(xml);
			Load(doc);
		}

		/// <summary>
		/// Loads from a reader.
		/// </summary>
		/// <param name="reader">A reader on the input file, e.g. a <see cref="StreamReader"/>.</param>
		public void Load(TextReader reader)
        {
            using var xml = XmlReader.Create(reader);
            Load(xml);
        }

		/// <summary>
		/// Loads from a file.
		/// </summary>
		/// <param name="filename">The file name.</param>
		public void Load(string filename)
        {
            using var reader = new StreamReader(filename);
            Load(reader);
        }

		private void DefinePropertyValues(XmlWriter xml, object obj)
		{
			foreach (var p in Properties)
			{
				var x = p.WriteData(obj);
                if (x == null)
                {
                    continue;
                }

				x.Name = GraphMlFormat.Xmlns + "data";
				x.SetAttributeValue("key", p.Id);
				x.WriteTo(xml);
			}
		}

		/// <summary>
		/// Adds a standard node property with the given name and values.
		/// </summary>
		/// <remarks>
		/// <para>The newly added property will assign a value to each node of the graph.</para>
		/// <para>The values returned by getValueForNode are cached in a dictionary.</para>
		/// <para>Does not check whether a property with this name already exists!</para>
		/// </remarks>
		/// <typeparam name="T">Type of value.</typeparam>
		/// <param name="name">The name.</param>
		/// <param name="getValueForNode">Function for getting value for <see cref="Node"/>.</param>
		public void AddStandardNodeProperty<T>(string name, Func<Node, T> getValueForNode)
		{
			var prop = new StandardProperty<T>
            {
                Domain = PropertyDomain.Node,
                Name = name
            };

            foreach (var node in Graph.Nodes())
            {
                prop.Values[node] = getValueForNode(node);
            }

			Properties.Add(prop);
		}

		/// <summary>
		/// Adds a standard arc property with the given name and values.
		/// </summary>
		/// <remarks>
		/// <para>The newly added property will assign a value to each arc of the graph.</para>
		/// <para>The values returned by getValueForArc are cached in a dictionary.</para>
		/// <para>Does not check whether a property with this name already exists!</para>
		/// </remarks>
		/// <typeparam name="T">Type of value.</typeparam>
		/// <param name="name">The name.</param>
		/// <param name="getValueForArc">Function for getting value for <see cref="Arc"/>.</param>
		public void AddStandardArcProperty<T>(string name, Func<Arc, T> getValueForArc)
		{
			var prop = new StandardProperty<T>
            {
                Domain = PropertyDomain.Arc,
                Name = name
            };

            foreach (var arc in Graph.Arcs())
            {
                prop.Values[arc] = getValueForArc(arc);
            }

			Properties.Add(prop);
		}

		/// <summary>
		/// Saves to an XML writer.
		/// </summary>
		/// <param name="xml"><see cref="XmlWriter"/>.</param>
		private void Save(XmlWriter xml)
		{
			xml.WriteStartDocument();
			xml.WriteStartElement("graphml", GraphMlFormat.Xmlns.NamespaceName);
			xml.WriteAttributeString("xmlns", "xsi", null, GraphMlFormat.XmlnsXsi.NamespaceName);
			xml.WriteAttributeString("xmlns", "y", null, GraphMlFormat.XmlnsY.NamespaceName);
			xml.WriteAttributeString("xmlns", "yed", null, GraphMlFormat.XmlnsYed.NamespaceName);
			xml.WriteAttributeString("xsi", "schemaLocation", null, GraphMlFormat.XsiSchemaLocation);

			for (var i = 0; i < Properties.Count; i++)
			{
				var p = Properties[i];
				p.Id = "d" + i;
				p.GetKeyElement().WriteTo(xml);
			}

			xml.WriteStartElement("graph", GraphMlFormat.Xmlns.NamespaceName);
			xml.WriteAttributeString("id", "G");
			xml.WriteAttributeString("edgedefault", "directed");
			xml.WriteAttributeString("parse.nodes", Graph.NodeCount().ToString(CultureInfo.InvariantCulture));
			xml.WriteAttributeString("parse.edges", Graph.ArcCount().ToString(CultureInfo.InvariantCulture));
			xml.WriteAttributeString("parse.order", "nodesfirst");
			DefinePropertyValues(xml, Graph);

			var nodeById = new Dictionary<string, Node>();

            foreach (var kv in NodeId)
			{
                if (nodeById.ContainsKey(kv.Value))
                {
                    throw new("Duplicate node id " + kv.Value);
                }

				nodeById[kv.Value] = kv.Key;
			}

			foreach (var node in Graph.Nodes())
			{
                NodeId.TryGetValue(node, out var id);
				if (id == null)
				{
					id = node.Id.ToString(CultureInfo.InvariantCulture);
                    while (nodeById.ContainsKey(id))
                    {
                        id += '_';
                    }

					NodeId[node] = id;
					nodeById[id] = node;
				}

				xml.WriteStartElement("node", GraphMlFormat.Xmlns.NamespaceName);
				xml.WriteAttributeString("id", id);
				DefinePropertyValues(xml, node);
				xml.WriteEndElement(); // node
			}

			foreach (var arc in Graph.Arcs())
			{
				string? id;
                if (ArcId != null)
                {
                    ArcId.TryGetValue(arc, out id);
                }
                else
                {
                    id = null;
                }

				xml.WriteStartElement("edge", GraphMlFormat.Xmlns.NamespaceName);
                if (id != null)
                {
                    xml.WriteAttributeString("id", id);
                }

                if (Graph.IsEdge(arc))
                {
                    xml.WriteAttributeString("directed", "false");
                }

				xml.WriteAttributeString("source", NodeId[Graph.U(arc)]);
				xml.WriteAttributeString("target", NodeId[Graph.V(arc)]);
				DefinePropertyValues(xml, arc);
				xml.WriteEndElement(); // edge
			}

			xml.WriteEndElement(); // graph
			xml.WriteEndElement(); // graphml
		}

		/// <summary>
		/// Saves to a writer.
		/// </summary>
		/// <param name="writer">A writer on the output file, e.g. a <see cref="StreamWriter"/>.</param>
		public void Save(TextWriter writer)
        {
            using var xml = XmlWriter.Create(writer);
            Save(xml);
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
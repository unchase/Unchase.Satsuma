[**Satsuma**](https://satsumagraph.sourceforge.net/doc/html/) is a graph library for .NET, written in C#. The goal was to create an easy-to-use and flexible library, which implements all the necessary graph structures, and the fastest possible versions of common graph algorithms.

Ported and updated by [Unchase](https://github.com/unchase) © 2022

# Introduction

- Graphs consist of **nodes** connected with **arcs**.
- All graphs are **mixed** in **Satsuma**, which means that a graph can contain **directed** and **undirected** arcs at once. In short, there is no distinction between directed and undirected graphs. Loops and multiple arcs are also allowed.
- An undirected arc is called an **edge**.

Graphs can be accessed in a read-only way through the `IGraph` interface. The `IBuildableGraph` and `IDestroyableGraph` interfaces provide write access to a graph.

The structs `Node` and `Arc` represent the nodes and arcs of a graph. `Node.Invalid` and `Arc.Invalid` are null-like constants, denoting "no node" and "no arc", respectively.

> **Warning**
>
> Though all graphs use the same type for representing Node and Arc objects, they are generally not interchangeable between graphs. Each graph has its own Node and Arc set. If you pass a Node obtained from a graph to a method of another graph, you will likely get an error. (Adaptors are an exception from this general rule.)

Most of the algorithms can be divided into two categories:

- while there are operations which view the graph as **undirected** and treat all arcs as edges,
  - e.g. `ConnectedComponents`, `Bipartition`
- others view the graph as a directed graph and treat edges as bidirectional channels.
  - e.g. `TopologicalOrder`, `Dijkstra`, `Preflow`

# Creating graphs

A graph can have at most `int.MaxValue` nodes and `int.MaxValue` arcs.

To create your first graph, enter the following code:

```csharp
CustomGraph g = new CustomGraph();
Node a = g.AddNode();
Node b = g.AddNode();
Node c = g.AddNode();
Arc ab = g.AddArc(a, b, Directedness.Directed);
Arc bc = g.AddArc(b, c, Directedness.Undirected);
```

or (with extension methods):

```csharp
CustomGraph g = new CustomGraph()
    .WithNodes(1, 2, 3)
    .WithArcs(
        (1, 2, Directedness.Directed),
        (2, 3, Directedness.Undirected)));
```

The above code creates a graph on three nodes, consisting of a directed arc and an edge. As you can see, the `CustomGraph` class can be used to build a graph.

You can also use some "graph constants". These are predefined graphs which usually consume very little memory, due to their special nature. They include `CompleteBipartiteGraph` and `CompleteGraph`. Using them is more efficient than e.g. building a complete graph using `CustomGraph`.

Graphs can also be loaded and saved. See the `IO.SimpleGraphFormat`, `IO.LemonGraphFormat` and `IO.GraphML.GraphMLFormat` classes.

# Processing graphs

- To enumerate the elements of a graph, you can call `Nodes` or `Arcs`.
- You can get the two nodes of an `Arc` by calling the `U` and `V` methods of the containing graph.
- `IsEdge` tells you whether an arc is undirected.
- Use the `ArcToString` method to convert an arc to a readable representation.

Example:

```csharp
var g = new CompleteGraph(100); // create a complete graph on 100 nodes
var cost = new Dictionary<Node, double>(); // create a cost function on the nodes
int i = 0;
foreach (Node node in g.Nodes()) cost[node] = i++; // assign some integral costs to the nodes
Func<Arc, double> arcCost = 
    (arc => cost[g.U(arc)] + cost[g.V(arc)]); // a cost of an arc will be the sum of the costs of the two nodes
foreach (Arc arc in g.Arcs())
    Console.WriteLine("Cost of "+g.ArcToString(arc)+" is "+arcCost(arc));
```

# Using adaptors

By using adaptors, you can perform slight modifications on graphs without actually altering the graph object. Adaptors are analogous to camera filters as they allow you to see an underlying object in a different way.

Two basic adaptor examples are `Subgraph` and `Supergraph`.

- `Subgraph` allows you to temporarily disable nodes and arcs in a graph, and work with a smaller portion of it.
- `Supergraph` allows you to temporarily add new nodes and arcs to an existing graph.
  - With `Supergrap`h, you can e.g. extend a read-only graph like `CompleteGraph` with new nodes and arcs, even though the graph itself cannot be modified directly.

Example for `Subgraph`:

```csharp
var g = new CompleteGraph(10); // create a complete graph on 10 nodes

// var sg = new Subgraph(g);
var sg = g.ToSubgraph(); // create a subgraph of the complete graph
sg.Enable(g.GetNode(0), false); // disable a single node
Console.WriteLine("The subgraph contains "+sg.NodeCount()+" nodes and "+sg.ArcCount()+" arcs.");
```

Example for `Supergraph`:

```csharp
var g = new CompleteGraph(20); // create a complete graph on 20 nodes

// var sg = new Supergraph(g);
var sg = g.ToSupergraph(); // create a supergraph of the complete graph
var s = sg.AddNode(); // add a node
sg.AddArc(s, g.GetNode(3), Directedness.Directed); // add a directed arc
Console.WriteLine("The extended graph contains "+sg.NodeCount()+" nodes and "+sg.ArcCount()+" arcs.");
```

Generally speaking, `Node` and `Arc` objects are **interchangeable** between the underlying graph and the adaptor, as long as it "makes sense". For clarification, you should always read the description of the adaptor class you want to use. For example:

- nodes of the `Subgraph` are valid as nodes of the underlying graph as well,
- and nodes of the underlying graph are valid as nodes of the `Supergraph` as well.

Thus, the code below is correct and will not produce an error:

```csharp
CustomGraph g;
// Subgraph sg = new Subgraph(g);
Subgraph sg = g.ToSubgraph();
[...]
foreach (var arc in sg.Arcs()) // we are enumerating the arcs of the *subgraph*!
{
    // we are now querying arc properties using the *original graph*!
    Console.WriteLine("This arc goes from "+g.U(arc)+" to "+g.V(arc)+
        ", and is "+(g.IsEdge(arc) ? "" : "not ")+"an edge.");
    Console.WriteLine("As text: "+g.ArcToString(arc));
}
```

Other adaptor types:

- `Path` stores a path of the underlying graph.
- `ContractedGraph` allows merging multiple nodes into one node.
- `RedirectedGraph` allows changing the direction of some arcs.
- `ReverseGraph` allows reversing all arcs of the graph.
- `UndirectedGraph` allows viewing all arcs as edges.

> **Warning**
> 
> Modifying the underlying graph while using an adaptor is often illegal, depending on the adaptor type. You should always read the description of the adaptor carefully.

# Algorithms

The following algorithms are available:

- `BiEdgeConnectedComponents`, `BiNodeConnectedComponents`, `Bipartition`, `ConnectedComponents`, `StrongComponents`, `TopologicalOrder`.
- `FindPathExtensions`: finding a path in a graph.
- The abstract class `Dfs` can be extended to create additional depth-first-search-based algorithms.
- `Bfs`, `Dijkstra`, `BellmanFord`: finding shortest (minimum length) or cheapest (minimum cost) paths.
- `Kruskal<TCost>`, `Prim<TCost>`: finding minimum cost spanning forests.
- `MaximumMatching`, `MinimumCostMatching`: finding matchings.
- `Preflow`, `IntegerPreflow`: finding a maximum flow.
- `NetworkSimplex`: finding a minimum cost feasible circulation.
- Multiple heuristics for the **traveling salesman problem**.
- `Drawing.ForceDirectedLayout`: computing an aesthetically pleasing visual representation of a graph.

# The traveling salesman problem

Suppose that we have a bidirectional complete directed graph (that is, for each pair of nodes *a*, *b* there is a directed arc from *a* to *b*) and a cost function on the arcs.

We would like to find a minimum cost [Hamiltonian cycle](http://en.wikipedia.org/wiki/Hamiltonian_cycle) in the graph. This is called the [traveling salesman problem](http://en.wikipedia.org/wiki/Travelling_salesman_problem).

The TSP-related classes in **Satsuma** are: `CheapestLinkTsp<TNode>`, `InsertionTsp<TNode>`, `Opt2Tsp<TNode>`, `HamiltonianCycle`. They do not generally use the basic **Satsuma** types `Node`, `Arc` and `IGraph`, as the input graph in a TSP is always a bidirectional complete directed graph. Instead, there is a generic type parameter which is the type of the nodes to visit. TSP-solving classes like `InsertionTsp<TNode>` accept a collection of nodes and a cost function defined on node pairs, and produce a cost-effective visiting order of the supplied nodes.

TSP cost functions must always be finite. The algorithms generally perform well if the cost function is symmetric (that is, the cost of traveling from *a* to *b* is the same as traveling from *b* to *a*). In particular, they perform the best for metric cost functions (that is, the triangle inequality must hold as well).

> **Warning**
>
> As the traveling salesman problem is NP-hard, there is no known optimal polynomial solution, and it is unlikely that one exists. Therefore, all classes employ some kind of heuristic and the returned tours will be probably suboptimal.

The following example demonstrates how a reasonably cheap tour on Euclidean points can be found using a **Satsuma** TSP solver:

```csharp
// create a node collection: in this example, they are geographical locations
var points = new PointD[]
{
    new PointD(47.50704, 19.04592),
    new PointD(47.50284, 19.03171),
    new PointD(47.472331, 19.062706),
    new PointD(47.515373, 19.082839)
};
// define a cost function: in this example, it is the Euclidean distance
Func<PointD, PointD, double> cost = (p, q) => p.Distance(q);
// create a TSP solver instance and calculate a tour
InsertionTsp<PointD> tsp = new InsertionTsp<PointD>(points, cost);
tsp.Run();
Console.WriteLine("Total tour cost: "+tsp.TourCost);
Console.WriteLine("Visiting order:");
foreach (PointD p in tsp.Tour()) Console.WriteLine(p);
```

# Thread safety

A type is called *thread safe* if its instances can be simultaneously operated on by multiple threads without compromising integrity.

For example, a class is thread safe if its methods and properties behave correctly even if multiple threads call/access them at once.

The following rules apply for types defined in **Satsuma**, concerning thread safety:

- If not stated otherwise, all **classes** and **interfaces** should be treated as thread **unsafe**.
  - Do not assume that they are safe by just looking at their source code. If their description does not say anything about thread safety, then their implementation may introduce thread unsafe solutions anytime in the future, which may break your code.
- Nevertheless, all **structs** in **Satsuma** are thread **safe**.
  - All structs defined in the **Satsuma** library are **immutable**.

The thread safety of an **interface** is in fact a restriction on implementor classes. In a sense, thread safety is part of the interface definition. For example, if ISomething is thread safe, then this means that all implementors should be written in a way that ISomething instances can be used by multiple threads at once.

> **Warning**
> 
> If an interface or base class is thread safe, this does not imply that all **implementor** or **descendant** classes are thread safe. However, if an instance of the derived type is treated as an instance of the base type, then it should be thread safe. This statement may sound confusing, so let us take a fictional example.
>
> - Supposed that `IReadOnlySomething` is a thread safe interface.
> - WritableSomething, which implements `IReadOnlySomething`, may not be thread safe, as the data structure may be compromised if it is modified concurrently.
> - However, if only the `IReadOnlySomething` methods are called, then a `WritableSomething` instance should be freely usable by any number of threads.
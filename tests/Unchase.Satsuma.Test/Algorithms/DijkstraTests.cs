using FluentAssertions;
using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Algorithms;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Enums;
using Xunit;

namespace Unchase.Satsuma.Test.Algorithms
{
    /// <summary>
    /// <see cref="Dijkstra"/> tests.
    /// </summary>
    public class DijkstraTests
    {
        /// <summary>
        /// Get shortest path to node successfully.
        /// </summary>
        /// <param name="start">Start test.</param>
        [Theory]
        [InlineData(true)]
        public void Dijkstra_Returns_Success(bool start)
        {
            if (start)
            {
                // Arrange
                var graph = new CompleteGraph(1, Directedness.Directed);
                var superGraph = new Supergraph(graph);
                superGraph.AddNode(1);
                superGraph.AddNode(2);
                superGraph.AddNode(3);
                superGraph.AddNode(4);
                superGraph.AddNode(5);
                superGraph.AddNode(6);
                superGraph.AddNode(7);

                ((IGraph)superGraph).AddNodeProperties(new()
                {
                    { new(1), new(new() { { "testProperty", 0 } }) },
                    { new(2), new(new() { { "testProperty", 0 } }) },
                    { new(3), new(new() { { "testProperty", 15 } }) },
                    { new(4), new(new() { { "testProperty", 10 } }) },
                    { new(5), new(new() { { "testProperty", 11 } }) },
                    { new(6), new(new() { { "testProperty", 3 } }) },
                    { new(7), new(new() { { "testProperty", 8 } }) }
                });

                superGraph.AddArc(new(1), new(2), Directedness.Directed); // cost = 0 for 1 -> 2
                superGraph.AddArc(new(2), new(3), Directedness.Directed); // cost = 0 for 2 -> 3
                superGraph.AddArc(new(3), new(4), Directedness.Directed); // cost = 15 for 3 -> 4
                superGraph.AddArc(new(4), new(5), Directedness.Directed); // cost = 10 for 4 -> 5
                superGraph.AddArc(new(5), new(6), Directedness.Directed); // cost = 11 for 5 -> 6
                superGraph.AddArc(new(6), new(7), Directedness.Directed); // cost = 3 for 6 -> 7

                var dijkstra = new Dijkstra(superGraph, arc =>
                {
                    var u = superGraph.U(arc);
                    var uProperties = superGraph.GetNodeProperties(u);
                    var uCost = uProperties?.ContainsKey("testProperty") == true 
                        ? double.TryParse(uProperties["testProperty"].ToString(), out var cost) ? cost : 0
                        : 0;

                    return uCost;
                }, DijkstraMode.Sum);
                dijkstra.AddSource(superGraph.Nodes().First());

                // Act
                var resultNode = dijkstra.RunUntilFixed(node => superGraph
                    .GetNodeProperties(node)?
                    .Any(x => x.Key == "testProperty" && x.Value.Equals(11)) == true);

                // Assert
                resultNode.Id.Should().Be(5);
                dijkstra.GetDistance(new(4)).Should().Be(15);
            }
        }
    }
}
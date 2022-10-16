using FluentAssertions;
using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Algorithms;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Core;
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
                superGraph.AddNode(2, new()
                {
                    { "testProperty", 15 }
                });
                superGraph.AddNode(3, new()
                {
                    { "testProperty", 10 }
                });
                superGraph.AddNode(4, new()
                {
                    { "wrongProperty", 11 }
                });
                superGraph.AddNode(5);
                superGraph.AddNode(6, new()
                {
                    { "testProperty", 8 }
                });
                superGraph.AddNode(7, new()
                {
                    { "testProperty", 11 }
                });
                superGraph.AddArc(new(1), new(2), Directedness.Directed); // cost = 0
                superGraph.AddArc(new(2), new(3), Directedness.Directed); // cost = 15
                superGraph.AddArc(new(3), new(4), Directedness.Directed); // cost = 25
                superGraph.AddArc(new(4), new(5), Directedness.Directed); // cost = 25
                superGraph.AddArc(new(5), new(6), Directedness.Directed); // cost = 25
                superGraph.AddArc(new(6), new(7), Directedness.Directed); // cost = 33

                var dijkstra = new Dijkstra(superGraph, arc =>
                {
                    var u = superGraph.U(arc);
                    var uProperties = superGraph.Properties(u);
                    var uCost = uProperties?.ContainsKey("testProperty") == true 
                        ? double.TryParse(uProperties["testProperty"].ToString(), out var cost) ? cost : 0
                        : 0;

                    return uCost;
                }, DijkstraMode.Sum);
                dijkstra.AddSource(superGraph.Nodes().First());

                // Act
                var resultNode = dijkstra.RunUntilFixed(node => superGraph
                    .Properties(node)?
                    .Any(x => x.Key == "testProperty" && x.Value.Equals(11)) == true);

                // Assert
                resultNode.Id.Should().Be(7);
                dijkstra.GetDistance(new(6)).Should().Be(33);
            }
        }
    }
}
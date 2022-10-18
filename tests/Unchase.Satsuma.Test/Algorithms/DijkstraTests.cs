using FluentAssertions;
using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Adapters.Extensions;
using Unchase.Satsuma.Algorithms;
using Unchase.Satsuma.Algorithms.Enums;
using Unchase.Satsuma.Algorithms.Extensions;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;
using Xunit;

namespace Unchase.Satsuma.Test.Algorithms
{
    /// <summary>
    /// <see cref="Dijkstra{TNodeProperty, TArcProperty}"/> tests.
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
                var graph = new CustomGraph<int, int>();
                var superGraph = graph.ToSupergraph()
                    .WithNodesWithProperties<Supergraph<int, int>, int, int>(
                        (1, "testProperty2", 12),
                        (1, "testProperty", 0),
                        (2, "testProperty", 0),
                        (3, "testProperty", 15),
                        (4, "testProperty", 10),
                        (5, "testProperty", 11),
                        (6, "testProperty", 3),
                        (7, "testProperty", 8))
                    .WithArcs<Supergraph<int, int>, int, int>(
                        (1, 2, Directedness.Directed),
                        (2, 3, Directedness.Directed),
                        (3, 4, Directedness.Directed),
                        (4, 5, Directedness.Directed),
                        (5, 6, Directedness.Directed),
                        (6, 7, Directedness.Directed));

                var dijkstra = superGraph.ToDijkstra(arc =>
                {
                    var u = superGraph.U(arc);
                    var uProperties = u.GetProperties(superGraph);
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
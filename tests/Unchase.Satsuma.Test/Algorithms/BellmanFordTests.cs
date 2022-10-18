using FluentAssertions;
using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Adapters.Extensions;
using Unchase.Satsuma.Algorithms;
using Unchase.Satsuma.Algorithms.Extensions;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;
using Xunit;

namespace Unchase.Satsuma.Test.Algorithms
{
    /// <summary>
    /// <see cref="BellmanFord{TNodeProperty, TArcProperty}"/> tests.
    /// </summary>
    public class BellmanFordTests
    {
        /// <summary>
        /// Get shortest path to node successfully.
        /// </summary>
        /// <param name="start">Start test.</param>
        [Theory]
        [InlineData(true)]
        public void Generic_BellmanFord_Returns_Success(bool start)
        {
            if (start)
            {
                // Arrange
                var graph = new CustomGraph();
                var superGraph = graph.ToSupergraph()
                    .WithNodesCount(10)
                    .WithNodes(1, 2, 3, 4, 5, 6, 7)
                    .WithNodeProperties(
                        (1, "testProperty", 0),
                        (2, "testProperty", 0),
                        (3, "testProperty", 15),
                        (4, "testProperty", 10),
                        (5, "testProperty", 11),
                        (6, "testProperty", 3),
                        (7, "testProperty", 8))
                    .WithArcs(
                        (1, 2, Directedness.Directed), // cost = 0 for 1 -> 2
                        (2, 3, Directedness.Directed), // cost = 0 for 2 -> 3
                        (3, 4, Directedness.Directed), // cost = 15 for 3 -> 4
                        (4, 5, Directedness.Directed), // cost = 10 for 4 -> 5
                        (5, 6, Directedness.Directed), // cost = 11 for 5 -> 6
                        (6, 7, Directedness.Directed)); // cost = 3 for 6 -> 7

                // Act
                var bellmanFord = superGraph.ToBellmanFord(arc =>
                {
                    var u = superGraph.U(arc);
                    var uProperties = u.GetProperties(superGraph);
                    var uCost = uProperties?.ContainsKey("testProperty") == true
                        ? double.TryParse(uProperties["testProperty"].ToString(), out var cost) ? cost : 0
                        : 0;

                    return uCost;
                }, new List<Node> { new(4) });

                // Assert
                bellmanFord.GetDistance(new(6)).Should().Be(21);
                bellmanFord.GetDistance(new(7)).Should().Be(24);
            }
        }
    }
}

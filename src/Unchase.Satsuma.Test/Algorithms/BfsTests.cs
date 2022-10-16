using FluentAssertions;
using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Algorithms;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Enums;
using Xunit;

namespace Unchase.Satsuma.Test.Algorithms
{
    /// <summary>
    /// <see cref="Bfs"/> tests.
    /// </summary>
    public class BfsTests
    {
        /// <summary>
        /// Get node by condition successfully.
        /// </summary>
        /// <param name="start">Start test.</param>
        [Theory]
        [InlineData(true)]
        public void Bfs_Returns_Success(bool start)
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
                    { "testProperty", 11 }
                });
                superGraph.AddArc(new(1), new(2), Directedness.Directed);
                superGraph.AddArc(new(2), new(3), Directedness.Directed);
                superGraph.AddArc(new(3), new(5), Directedness.Directed);
                superGraph.AddArc(new(5), new(4), Directedness.Directed);
                superGraph.AddArc(new(4), new(6), Directedness.Directed);

                var bfs = new Bfs(superGraph);
                bfs.AddSource(superGraph.Nodes().First());

                // Act
                var resultNode = bfs.RunUntilReached(node => superGraph
                    .Properties(node)?
                    .Any(x => x.Key == "testProperty" && x.Value.Equals(11)) == true);

                // Assert
                resultNode.Id.Should().Be(6);
            }
        }
    }
}
using FluentAssertions;
using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Adapters.Extensions;
using Unchase.Satsuma.Algorithms;
using Unchase.Satsuma.Algorithms.Extensions;
using Unchase.Satsuma.Core.Enums;
using Unchase.Satsuma.Core.Extensions;
using Xunit;

namespace Unchase.Satsuma.Test.Algorithms
{
    /// <summary>
    /// <see cref="Bfs{TNodeProperty, TArcProperty}"/> tests.
    /// </summary>
    public class BfsTests
    {
        /// <summary>
        /// Successfully test.
        /// </summary>
        /// <param name="start">Start test.</param>
        [Theory]
        [InlineData(true)]
        public void Bfs_Returns_Success(bool start)
        {
            if (start)
            {
                // Arrange
                var graph = new CustomGraph<int, int>();
                var superGraph = graph.ToSupergraph();
                superGraph.AddNode(1);
                superGraph.AddNode(2);
                superGraph.AddNode(3);
                superGraph.AddNode(4);
                superGraph.AddNode(5);
                superGraph.AddNode(6);

                superGraph.AddArc(new(1), new(2), Directedness.Directed);
                superGraph.AddArc(new(2), new(3), Directedness.Directed);
                superGraph.AddArc(new(3), new(5), Directedness.Directed);
                superGraph.AddArc(new(5), new(4), Directedness.Directed);
                superGraph.AddArc(new(4), new(6), Directedness.Directed);

                superGraph.AddNodeProperties(new()
                {
                    { new(4), new(new() { { "testProperty", 11 } }) }
                });

                var bfs = superGraph.ToBfs();
                bfs.AddSource(superGraph.Nodes().First());

                // Act
                var resultNode = bfs.RunUntilReached(node => superGraph
                    .GetNodeProperties(node)?
                    .Any(x => x.Key == "testProperty" && x.Value.Equals(11)) == true);

                // Assert
                resultNode.Id.Should().Be(4);
            }
        }
    }
}
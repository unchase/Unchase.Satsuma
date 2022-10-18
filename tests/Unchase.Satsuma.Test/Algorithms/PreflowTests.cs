using FluentAssertions;
using Unchase.Satsuma.Adapters;
using Unchase.Satsuma.Algorithms;
using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Enums;
using Xunit;

namespace Unchase.Satsuma.Test.Algorithms
{
    /// <summary>
    /// <see cref="Preflow{TNodeProperty, TArcProperty}"/> tests.
    /// </summary>
    public class PreflowTests
    {
        /// <summary>
        /// <see cref="Preflow{TNodeProperty, TArcProperty}"/> tests.
        /// </summary>
        /// <param name="start">Start test.</param>
        [Theory]
        [InlineData(true)]
        public void Preflow_Returns_Success(bool start)
        {
            if (start)
            {
                // Arrange
                var graph = new CompleteGraph<int, int>(1, Directedness.Directed);
                var superGraph = new Supergraph<int, int>(graph);
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
                var random = new Random();

                // Act
                var preflow = new Preflow<int, int>(superGraph, _ => random.Next(6, 1000), new(1), new(6));

                // Assert
                preflow.FlowSize.Should().BeGreaterOrEqualTo(6).And.BeLessOrEqualTo(1000);
            }
        }
    }
}
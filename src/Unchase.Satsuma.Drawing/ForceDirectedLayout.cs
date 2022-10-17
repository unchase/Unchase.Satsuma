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

using Unchase.Satsuma.Core;
using Unchase.Satsuma.Core.Contracts;
using Unchase.Satsuma.Core.Extensions;

namespace Unchase.Satsuma.Drawing
{
    /// <summary>
	/// Attempts to draw a graph to the plane such that a certain equilibrium is attained.
	/// </summary>
	/// <remarks>
	/// <para>Models the graph as electrically charged nodes connected with springs.</para>
	/// <para>Nodes are attracted by the springs and repelled by electric forces.</para>
	/// <para>
	/// By default, the springs behave logarithmically, and (as in reality) the electric repulsion force is inversely
	/// proportional to the square of the distance.
	/// </para>
	/// <para>The formula for the attraction/repulsion forces can be configured through #SpringForce and #ElectricForce.</para>
	/// <para>
	/// The algorithm starts from a given configuration (e.g. a random placement)
	/// and lets the forces move the graph to an equilibrium.
	/// </para>
	/// <para>Simulated annealing is used to ensure good convergence.</para>
	/// <para>Each convergence step requires O(n<sup>2</sup>) time, where \e n is the number of the nodes.</para>
	/// <para>Force-directed layout algorithms work best for graphs with a few nodes (under about 100).</para>
	/// <para>
	/// Not only because of the running time, but also the probability of running into a poor local minimum 
	/// is quite high for a large graph. This decreases the chance that a nice arrangement is attained.
	/// </para>
	/// <para>Example:</para>
	/// <para>
	/// <code>
	/// var g = new CompleteGraph(7);
	/// var layout = new ForceDirectedLayout(g);
	/// layout.Run();
	/// foreach (var node in g.Nodes())
	///		Console.WriteLine("Node "+node+" is at "+layout.NodePositions[node]);
	/// </code>
	/// </para>
	/// </remarks>
	public sealed class ForceDirectedLayout
	{
        /// <summary>
		/// The default initial temperature for the simulated annealing.
		/// </summary>
		public const double DefaultStartingTemperature = 0.2;

		/// <summary>
		/// The temperature where the simulated annealing should stop.
		/// </summary>
		public const double DefaultMinimumTemperature = 0.01;

		/// <summary>
		/// The ratio between two successive temperatures in the simulated annealing.
		/// </summary>
		public const double DefaultTemperatureAttenuation = 0.95;

		/// <summary>
		/// The input graph.
		/// </summary>
		public IGraph Graph { get; }

		/// <summary>
		/// The current layout, which assigns positions to the nodes.
		/// </summary>
		public Dictionary<Node, PointD> NodePositions { get; }

		/// <summary>
		/// The function defining the attraction force between two connected nodes.
		/// </summary>
		/// <remarks>
		/// <para>Arcs are viewed as springs that want to bring the two connected nodes together.</para>
		/// <para>The function takes a single parameter, which is the distance of the two nodes.</para>
		/// <para>The default force function is 2 <em>ln</em>(d).</para>
		/// </remarks>
		public Func<double, double> SpringForce { get; set; }

		/// <summary>
		/// The function defining the repulsion force between two nodes.
		/// </summary>
		/// <remarks>
		/// <para>Nodes are viewed as electrically charged particles which repel each other.</para>
		/// <para>The function takes a single parameter, which is the distance of the two nodes.</para>
		/// <para>The default force function is 1/d<sup>2</sup>.</para>
		/// </remarks>
		public Func<double, double> ElectricForce { get; set; }

		/// <summary>
		/// The current temperature in the simulated annealing.
		/// </summary>
		public double Temperature { get; set; }

		/// <summary>
		/// The temperature attenuation factor used during the simulated annealing.
		/// </summary>
		public double TemperatureAttenuation { get; set; }

		/// <summary>
		/// Initialize <see cref="ForceDirectedLayout"/>.
		/// </summary>
		/// <param name="graph"><see cref="IGraph"/>.</param>
		/// <param name="initialPositions">If null, a random layout is used.</param>
		public ForceDirectedLayout(
            IGraph graph, 
            Func<Node, PointD>? initialPositions = null)
		{
			Graph = graph;
			NodePositions = new();
			SpringForce = (d => 2 * Math.Log(d));
			ElectricForce = (d => 1 / (d * d));
			TemperatureAttenuation = DefaultTemperatureAttenuation;

			Initialize(initialPositions);
		}

		/// <summary>
		/// Initializes the layout with the given one and resets the temperature.
		/// </summary>
		/// <param name="initialPositions">If null, a random layout is used.</param>
		public void Initialize(Func<Node, PointD>? initialPositions = null)
		{
			if (initialPositions == null)
			{
				// make a random layout
				var r = new Random();
				initialPositions = _ => new(r.NextDouble(), r.NextDouble());
			}

            foreach (var node in Graph.Nodes())
            {
                NodePositions[node] = initialPositions(node);
            }

			// reset the temperature
			Temperature = DefaultStartingTemperature;
		}

		/// Performs an optimization step.
		public void Step()
		{
			var forces = new Dictionary<Node, PointD>();

			foreach (var u in Graph.Nodes())
			{
				var uPos = NodePositions[u];
				double xForce = 0, yForce = 0;

				// attraction forces
				foreach (var arc in Graph.Arcs(u))
				{
					var vPos = NodePositions[Graph.Other(arc, u)];
					var d = uPos.Distance(vPos);
					var force = Temperature * SpringForce(d);
					xForce += (vPos.X - uPos.X) / d * force;
					yForce += (vPos.Y - uPos.Y) / d * force;
				}

				// repulsion forces
				foreach (var v in Graph.Nodes())
				{
					if (v == u) continue;
					var vPos = NodePositions[v];
					var d = uPos.Distance(vPos);
					var force = Temperature * ElectricForce(d);
					xForce += (uPos.X - vPos.X) / d * force;
					yForce += (uPos.Y - vPos.Y) / d * force;
				}

				forces[u] = new(xForce, yForce);
			}

            foreach (var node in Graph.Nodes())
            {
                NodePositions[node] += forces[node];
            }

			Temperature *= TemperatureAttenuation;
		}

		/// <summary>
		/// Runs the algorithm until a low temperature is reached.
		/// </summary>
		/// <param name="minimumTemperature">Minimum temperature.</param>
		public void Run(double minimumTemperature = DefaultMinimumTemperature)
		{
			while (Temperature > minimumTemperature) Step();
		}
	}
}
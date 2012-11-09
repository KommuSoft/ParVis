//  
//  SimulationSpecification.cs
//  
//  Author:
//       Willem Van Onsem <vanonsem.willem@gmail.com>
// 
//  Copyright (c) 2012 Willem Van Onsem
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ParallelVisualizer.Specification {
	
	[XmlRoot("Simulation")]
	public class SimulationSpecification {
		
		private List<NodeSpecification> nodes;
		private List<EdgeSpecification> edges;
		
		[XmlArray("Nodes")]
		[XmlArrayItem("Node")]
		public List<NodeSpecification> Nodes {
			get {
				return this.nodes;
			}
			set {
				this.nodes = value;
			}
		}
		[XmlArray("Edges")]
		[XmlArrayItem("Edge")]
		public List<EdgeSpecification> Edges {
			get {
				return this.edges;
			}
			set {
				this.edges = value;
			}
		}
		
		public SimulationSpecification ()
		{
		}
		
		public void AddToSimulator (ParallelSimulation ps, DllLoader loader)
		{
			Dictionary<string, ParallelAlgorithm> pas = new Dictionary<string, ParallelAlgorithm> ();
			foreach (NodeSpecification ns in this.Nodes) {
				ParallelAlgorithm pa = loader.CreateAlgorithm (ns.AlgorithmName);
				pa.Name = ns.NodeName;
				pas.Add (pa.Name, pa);
				ps.AddParallelAlgorithm (pa);
				if (ns.RelativePosition != null) {
					ps.AddRelativePosition (pa, ns.RelativePosition);
				}
				if(ns.InitializationArguments != null) {
					ps.AddInitArguments(pa,ns.InitializationArguments);
				}
			}
			foreach(EdgeSpecification es in this.Edges) {
				ps.AddEdge(pas[es.Node1],pas[es.Node2]);
			}
		}
		
	}
}
//  
//  ParallelSimulation.cs
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

namespace ParallelVisualizer {

	public class ParallelSimulation {
		
		private readonly List<ParallelAlgorithm> algorithms = new List<ParallelAlgorithm> ();
		private readonly HashSet<Edge> edges = new HashSet<Edge> ();
		private int time = 0;
		
		public List<ParallelAlgorithm> Algorithms {
			get {
				return this.algorithms;
			}
		}
		public IEnumerable<Edge> Edges {
			get {
				return this.edges;
			}
		}
		public int Time {
			get {
				return this.time;
			}
		}
		
		public ParallelSimulation (params ParallelAlgorithm[] pas)
		{
			foreach(ParallelAlgorithm pa in pas) {
				this.AddParallelAlgorithm(pa);
			}
		}
		
		public void AddParallelAlgorithm (ParallelAlgorithm pa)
		{
			this.algorithms.Add (pa);
			foreach (Edge e in pa.Edges) {
				this.edges.Add (e);
			}
		}
		public void AddEdge (ParallelAlgorithm pa1, ParallelAlgorithm pa2)
		{
			this.AddEdge (new Edge (pa1, pa2));
		}
		public void AddEdgeSequence (params ParallelAlgorithm[] pas)
		{
			for(int i = 0; i < pas.Length-1; i++) {
				this.AddEdge(pas[i],pas[i+1]);
			}
		}
		internal void AddEdge (Edge e)
		{
			if (!this.edges.Contains (e)) {
				e.Simulator = this;
				this.edges.Add (e);
				e.Node1.RegisterEdge (e);
				e.Node2.RegisterEdge (e);
			}
		}
		
	}
}


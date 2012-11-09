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
		private readonly List<IEnumerable<int>> executions = new List<IEnumerable<int>>();
		private readonly HashSet<Edge> edges = new HashSet<Edge> ();
		private readonly Dictionary<ParallelAlgorithm,RelativePosition> relativePositions = new Dictionary<ParallelAlgorithm, RelativePosition>();
		private readonly Dictionary<ParallelAlgorithm,string[]> initArgs = new Dictionary<ParallelAlgorithm, string[]>();
		private int time = 0;
		private bool initialized = false;
		
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
			pa.Id = this.algorithms.Count;
			this.algorithms.Add (pa);
			foreach (Edge e in pa.Edges) {
				this.edges.Add (e);
			}
		}
		public void ForwardTo (int t)
		{
			if (!initialized) {
				string[] args;
				foreach (ParallelAlgorithm pa in this.algorithms) {
					if (this.initArgs.TryGetValue (pa, out args)) {
						pa.Setup (args);
					}
					else {
						pa.Setup ();
					}
					this.executions.Add(pa.Steps());
				}
			}
			for(; this.time < t; this.time++) {
				
			}
		}
		
		internal void AddInitArguments (ParallelAlgorithm pa, string[] initializationArguments) {
			this.initArgs.Add(pa,initializationArguments);
		}

		public void AddEdge (ParallelAlgorithm pa1, ParallelAlgorithm pa2)
		{
			this.AddEdge (new Edge (pa1, pa2));
		}
		public void AddEdgeSequence (params ParallelAlgorithm[] pas)
		{
			for (int i = 0; i < pas.Length - 1; i++) {
				this.AddEdge (pas[i], pas[i + 1]);
			}
		}
		internal RelativePosition GetRelativePosition (ParallelAlgorithm pa)
		{
			RelativePosition res;
			if(!this.relativePositions.TryGetValue(pa,out res)) {
				return null;
			}
			return res;
		}
		internal void AddRelativePosition (ParallelAlgorithm pa, RelativePosition rp)
		{
			this.relativePositions.Add(pa,rp);
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


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
using Cairo;

namespace ParallelVisualizer {

	public class ParallelSimulation {
		
		private readonly List<ParallelAlgorithm> algorithms = new List<ParallelAlgorithm>();
		private readonly List<IEnumerator<int>> executions = new List<IEnumerator<int>>();
		private readonly HashSet<Edge> edges = new HashSet<Edge>();
		private readonly Dictionary<ParallelAlgorithm,RelativePosition> relativePositions = new Dictionary<ParallelAlgorithm, RelativePosition>();
		private readonly Dictionary<ParallelAlgorithm,string[]> initArgs = new Dictionary<ParallelAlgorithm, string[]>();
		private int time = 0;
		private int maxTime = 1024;
		private bool newChapter = false;
		
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
		public int MaxTime {
			get {
				return this.maxTime;
			}
			set {
				this.maxTime = value;
			}
		}
		
		public ParallelSimulation (params ParallelAlgorithm[] pas) {
			foreach(ParallelAlgorithm pa in pas) {
				this.AddParallelAlgorithm(pa);
			}
		}
		
		public void AddParallelAlgorithm (ParallelAlgorithm pa) {
			pa.Simulator = this;
			pa.Id = this.algorithms.Count;
			this.algorithms.Add(pa);
			foreach(Edge e in pa.Edges) {
				this.edges.Add(e);
			}
		}
		public SimulatorResult CollectResults () {
			string[] args;
			SimulatorResult sr = new SimulatorResult();
			Cairo.PointD nullp = new Cairo.PointD(-1.0d, 0.0d);
			RelativePosition rp;
			foreach(ParallelAlgorithm pa in this.algorithms) {
				rp = this.GetRelativePosition(pa);
				if(rp == null) {
					sr.AddNode(pa.Name, nullp);
				}
				else {
					sr.AddNode(pa.Name, new Cairo.PointD(rp.X, rp.Y));
				}
				if(this.initArgs.TryGetValue(pa, out args)) {
					pa.Setup(args);
				}
				else {
					pa.Setup();
				}
				this.executions.Add(pa.Steps().GetEnumerator());
			}
			foreach(Edge e in this.edges) {
				sr.AddEdge(e.Node1.Name, e.Node2.Name, e.Delay);
			}
			bool next = true;
			ImageSurface isf = new ImageSurface(Format.ARGB32, 1, 1);
			Context ctxm = new Context(isf);
			for(; next && this.time < this.MaxTime;) {
				this.calculateStates(sr, ctxm);
				foreach(Edge e in this.Edges) {
					e.DeliverPost();
				}
				next = false;
				foreach(IEnumerator<int> i in this.executions) {
					next |= i.MoveNext();
				}
				this.time++;
				foreach(Edge e in this.edges) {
					sr.AddEdgeMessages(e.Node1.Name, e.Node2.Name, this.time, e.GetDownwardsMessages(), e.GetUpwardsMessages());
				}
				if(this.newChapter) {
					this.newChapter = false;
					sr.AddChapter(this.time);
				}
			}
			((IDisposable)ctxm).Dispose();
			sr.Cleanup(this.time);
			return sr;
		}

		private void calculateStates (SimulatorResult sr, Context ctxm) {
			PointD sz;
			ImageSurface state;
			Context ctx;
			foreach(ParallelAlgorithm pa in this.algorithms) {
				sz = pa.MeasureStateSize(ctxm);
				state = new ImageSurface(Format.ARGB32, (int)Math.Ceiling(sz.X), (int)Math.Ceiling(sz.Y));
				ctx = new Context(state);
				pa.PaintState(ctx);
				((IDisposable)ctx).Dispose();
				sr.AddNodeState(pa.Name, state);
			}
		}
		
		internal void AddInitArguments (ParallelAlgorithm pa, string[] initializationArguments) {
			this.initArgs.Add(pa, initializationArguments);
		}

		public void AddEdge (ParallelAlgorithm pa1, ParallelAlgorithm pa2) {
			this.AddEdge(new Edge(pa1, pa2));
		}
		public void AddEdgeSequence (params ParallelAlgorithm[] pas) {
			for(int i = 0; i < pas.Length - 1; i++) {
				this.AddEdge(pas[i], pas[i+1]);
			}
		}
		internal RelativePosition GetRelativePosition (ParallelAlgorithm pa) {
			RelativePosition res;
			if(!this.relativePositions.TryGetValue(pa, out res)) {
				return null;
			}
			return res;
		}
		internal void AddRelativePosition (ParallelAlgorithm pa, RelativePosition rp) {
			this.relativePositions.Add(pa, rp);
		}
		
		public void NewChapter () {
			this.newChapter = true;
		}

		internal void AddEdge (Edge e) {
			if(!this.edges.Contains(e)) {
				e.Simulator = this;
				this.edges.Add(e);
				e.Node1.RegisterEdge(e);
				e.Node2.RegisterEdge(e);
			}
		}
		
	}
}


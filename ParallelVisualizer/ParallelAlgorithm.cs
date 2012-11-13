//  
//  ParallelAlgorithm.cs
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
using System.Linq;
using  Cairo;

namespace ParallelVisualizer {

	public abstract class ParallelAlgorithm {
		
		private int id;
		private readonly Dictionary<ParallelAlgorithm,Edge> edges = new Dictionary<ParallelAlgorithm, Edge>();
		private string name;
		private static int nameDispatcher = 0x00;
		private List<ParallelAlgorithm> neighbours = null;
		private ParallelSimulation sim;
		
		public int Id {
			get {
				return this.id;
			}
			internal set {
				this.id = value;
			}
		}
		
		public abstract string SourceCode {
			get;
		}
		
		internal ParallelSimulation Simulator {
			get {
				return this.sim;
			}
			set {
				this.sim = value;
			}
		}
		
		internal IEnumerable<Edge> Edges {
			get {
				return this.edges.Values;
			}
		}
		protected List<ParallelAlgorithm> Neighbours {
			get {
				if(this.neighbours == null) {
					this.neighbours = new List<ParallelAlgorithm>(this.edges.Keys.OrderBy(x => x.Id));
				}
				return this.neighbours;
			}
		}
		protected IEnumerable<int> NeighbourIds {
			get {
				foreach(ParallelAlgorithm pa in Neighbours) {
					yield return pa.Id;
				}
			}
		}
		
		internal void RegisterEdges (IEnumerable<Edge> edges) {
			foreach(Edge e in edges) {
				RegisterEdge(e);
			}
		}
		internal void RegisterEdge (Edge e) {
			this.neighbours = null;
			if(e.IsNode(this) && !this.edges.ContainsKey(e.Other(this))) {
				this.edges.Add(e.Other(this), e);
			}
		}
		public string Name {
			get {
				return this.name;
			}
			protected internal set {
				this.name = value;
			}
		}
		
		protected ParallelAlgorithm () {
			this.name = "Algorithm"+(nameDispatcher++).ToString();
		}
		
		public abstract void Setup (params string[] args);
		
		public abstract IEnumerable<int> Steps ();
		
		public abstract PointD MeasureStateSize (Context ctx);

		public abstract void PaintState (Context ctx);

		protected internal abstract void ReciveMessage (Message message);
		
		protected void NewChapter () {
			this.sim.NewChapter();
		}
		protected void SendMessage (Message message) {
			if(message.Sender != this) {
				throw new ArgumentException("An algorithm can only send message with itself as sender.");
			}
			ParallelAlgorithm receiver = message.Receiver;
			Edge e;
			if(receiver == this) {
				this.ReciveMessage(message);
			}
			else if(this.edges.TryGetValue(receiver, out e)) {
				e.RouteMessage(message);
			}
			else {
				throw new ArgumentException("Can't route message: the node is  not connected to the receiving node.");
			}
		}
		
	}
}


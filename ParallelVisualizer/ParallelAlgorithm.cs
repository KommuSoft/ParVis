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
using  Cairo;

namespace ParallelVisualizer {

	public abstract class ParallelAlgorithm {
		
		private int id;
		private readonly Dictionary<ParallelAlgorithm,Edge> edges = new Dictionary<ParallelAlgorithm, Edge> ();
		
		public int Id {
			get {
				return this.id;
			}
			internal set {
				this.id = value;
			}
		}
		
		internal void RegisterEdges (IEnumerable<Edge> edges) {
			foreach (Edge e in edges) {
				if (e.IsNode (this)) {
					this.edges.Add (e.Other (this), e);
				}
			}
		}
		
		public abstract void Setup (string[] args);
		
		public abstract IEnumerable<string> Steps ();
		
		public abstract void PaintState (Context ctx);

		protected internal abstract void ReciveMessage (Message message);
		
		protected void SendMessage (Message message) {
			if (message.Sender != this) {
				throw new ArgumentException ("An algorithm can only send message with itself as sender.");
			}
			ParallelAlgorithm receiver = message.Receiver;
			Edge e;
			if (receiver == this) {
				this.ReciveMessage (message);
			} else if (this.edges.TryGetValue (receiver, out e)) {
				e.RouteMessage (message);
			} else {
				throw new ArgumentException ("Can't route message: the node is  not connected to the receiving node.");
			}
		}
		
	}
}


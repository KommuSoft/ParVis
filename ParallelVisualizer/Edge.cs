//  
//  Edges.cs
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

	public class Edge {
		
		private readonly ParallelAlgorithm node1;
		private readonly ParallelAlgorithm node2;
		private readonly Queue<Message> atob = new Queue<Message> ();
		private readonly Queue<Message> btoa = new Queue<Message> ();
		
		public ParallelAlgorithm Node1 {
			get {
				return this.node1;
			}
		}

		public ParallelAlgorithm Node2 {
			get {
				return this.node2;
			}
		}
		
		public Edge () {
		}

		public bool IsNode (ParallelAlgorithm node) {
			return (this.Node1 == node || this.Node2 == node);
		}

		public ParallelAlgorithm Other (ParallelAlgorithm node) {
			if (node == this.Node1) {
				return this.Node2;
			} else {
				return this.Node1;
			}
		}

		public void RouteMessage (Message message) {
			//TODO: implement
		}
	}
}


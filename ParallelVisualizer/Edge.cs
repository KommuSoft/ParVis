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
	
	using PostedMessage = Tuple<int,Message>;

	public class Edge {
		
		private readonly ParallelAlgorithm node1;
		private readonly ParallelAlgorithm node2;
		private readonly Queue<PostedMessage> atob = new Queue<PostedMessage> ();
		private readonly Queue<PostedMessage> btoa = new Queue<PostedMessage> ();
		private int delay = 3;
		private ParallelSimulation simulator;
		
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
		
		public ParallelSimulation Simulator {
			get {
				return this.simulator;
			}
			internal set {
				this.simulator = value;
			}
		}
		
		public Edge (ParallelAlgorithm node1, ParallelAlgorithm node2)
		{
			this.node1 = node1;
			this.node2 = node2;
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

		public void RouteMessage (Message message)
		{
			if (message.Sender == Node1 && message.Receiver == Node2) {
				this.atob.Enqueue (new PostedMessage (this.Simulator.Time, message));
			}
			else if (message.Sender == Node2 && message.Receiver == Node1) {
				this.btoa.Enqueue (new PostedMessage (this.Simulator.Time, message));
			}
		}
		
		public IEnumerable<Tuple<double, string>> GetUpwardsMessages ()
		{
			foreach (PostedMessage pm in this.atob) {
				double t = ((double) this.Simulator.Time-pm.Item1)/this.delay;
				yield return new Tuple<double, string>(t,pm.Item2.ToString());
			}
		}
		
		public IEnumerable<Tuple<double, string>> GetDownwardsMessages () {
			foreach (PostedMessage pm in this.btoa) {
				double t = ((double) this.Simulator.Time - pm.Item1) / this.delay;
				yield return new Tuple<double, string> (t, pm.Item2.ToString ());
			}
		}
		
		
		void deliverPost (Queue<PostedMessage> queue)
		{
			while (queue.Count > 0 && Simulator.Time - queue.Peek ().Item1 >= delay) {
				Message pm = queue.Dequeue ().Item2;
				pm.Receiver.ReciveMessage (pm);
			}
		}

		public void DeliverPost ()
		{
			deliverPost (this.atob);
			deliverPost (this.btoa);
		}
		
	}
}


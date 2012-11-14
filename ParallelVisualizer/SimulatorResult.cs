//
//  SimulatorResult.cs
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

	using Node = Tuple<string,PointD>;
	using NodeData = Tuple<string,PointD,List<ImageSurface>>;
	using EdgeData = Tuple<int,int,Dictionary<int,List<Tuple<double,string>>>,Dictionary<int,List<Tuple<double,string>>>>;
	using EdgeNotation = Tuple<double,string>;
	using NotatedEdge = Tuple<PointD,PointD,IEnumerable<Tuple<double,string>>,IEnumerable<Tuple<double,string>>>;

	public class SimulatorResult {

		private readonly List<NodeData> nodes = new List<NodeData>();
		private readonly List<EdgeData> edges = new List<EdgeData>();
		private readonly Dictionary<string,int> accessNodes = new Dictionary<string,int>();
		private readonly Dictionary<int,int> accessEdges = new Dictionary<int,int>();
		private readonly List<int> chapters = new List<int>(new int[] {0});
		private int endTime;

		public int ChapterCount {
			get {
				return this.chapters.Count-1;
			}
		}
		public int EndTime {
			get {
				return this.endTime;
			}
			internal set {
				this.endTime = value;
			}
		}

		public SimulatorResult () {
		}

		public void AddNode (string name, PointD relpo) {
			this.accessNodes.Add(name, this.nodes.Count);
			this.nodes.Add(new NodeData(name, relpo, new List<ImageSurface>()));
		}
		public void AddEdge (string frm, string to, int delay) {
			int ft = accessNodes[frm]<<16|accessNodes[to];
			this.accessEdges.Add(ft, this.edges.Count);
			this.edges.Add(new EdgeData(ft, delay, new Dictionary<int,List<EdgeNotation>>(), new Dictionary<int,List<EdgeNotation>>()));
		}
		public void AddNodeState (string node, ImageSurface sf) {
			this.nodes[this.accessNodes[node]].Item3.Add(sf);
		}
		public void AddEdgeMessages (string frm, string to, int time, IEnumerable<EdgeNotation> downwards, IEnumerable<EdgeNotation> upwards) {
			int ei = accessEdges[accessNodes[frm]<<16|accessNodes[to]];
			EdgeData ed = this.edges[ei];
			List<EdgeNotation> dn = new List<EdgeNotation>(downwards);
			List<EdgeNotation> up = new List<EdgeNotation>(upwards);
			if(dn.Count > 0) {
				ed.Item3.Add(time, dn);
			}
			if(up.Count > 0) {
				ed.Item4.Add(time, up);
			}
		}
		public void AddChapter (int time) {
			this.chapters.Add(time);
		}
		public void GetBounds (int chapter, out int start, out int end) {
			start = chapters[chapter];
			end = chapters[chapter+1];
		}

		public ImageSurface GetSurface (int node, int time) {
			return this.nodes[node].Item3[time];
		}

		public void Cleanup (int endTime) {
			this.chapters.Add(endTime);
			this.endTime = endTime;
			this.accessNodes.Clear();
			double gamma = 2.0d*Math.PI/nodes.Count;
			for(int i = 0; i < nodes.Count; i++) {
				if(this.nodes[i].Item2.X < 0.0d) {
					this.nodes[i].Item2.X = 0.5d+0.375d*Math.Sin(i*gamma);
					this.nodes[i].Item2.Y = 0.5d+0.375d*Math.Cos(i*gamma);
				}
			}
		}

		public IEnumerable<Node> GetNodes () {
			foreach(NodeData n in nodes) {
				yield return new Node(n.Item1, n.Item2);
			}
		}
		public IEnumerable<NotatedEdge> GetEdges (double t) {
			int low, high;
			int ti = (int)Math.Floor(t);
			double tf = t-ti;
			List<EdgeNotation> ia, ib;
			foreach(EdgeData ed in this.edges) {
				low = ed.Item1;
				high = low>>16;
				low &= 0xffff;
				double v = tf/ed.Item2;
				if(!ed.Item3.TryGetValue(ti, out ia)) {
					ia = null;
				}
				if(!ed.Item4.TryGetValue(ti, out ib)) {
					ib = null;
				}
				yield return new NotatedEdge(nodes[low].Item2, nodes[high].Item2, TweakMessages(ia, v), TweakMessages(ib, v));
			}
		}
		public IEnumerable<EdgeNotation> TweakMessages (IEnumerable<EdgeNotation> source, double tweak) {
			if(source != null) {
				foreach(EdgeNotation en in source) {
					yield return new EdgeNotation(en.Item1+tweak, en.Item2);
				}
			}
		}

	}
}


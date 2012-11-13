//
//  ProductSumFactorParallelAlgorithm.cs
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
using System.Linq;
using System.Collections.Generic;
using Cairo;

namespace ParallelVisualizer {

	[AlgorithmName("ProductSumFactor")]
	public class ProductSumFactorParallelAlgorithm : MailboxAlgorithm {

		private Array supermatrix;
		private int[] dim;
		private readonly Dictionary<int,int> indexMapping = new Dictionary<int, int>();

		#region implemented abstract members of ParallelVisualizer.ParallelAlgorithm
		public override string SourceCode {
			get {
				return @"Test";
			}
		}
		
		public override void Setup (params string[] args) {
			if(args != null && args.Length > 0) {
				List<ParallelAlgorithm> nbs = this.Neighbours;
				int ne = nbs.Count;
				int ne2 = ne*2;
				string[] selements = args[0].Split(',');
				if(selements.Length < ne2) {
					throw new ArgumentException("The parameterized matrix of the factor algorithm must have enough arguments!");
				}
				Dictionary<string,int> nna = new Dictionary<string,int>();
				this.dim = new int[ne];
				foreach(ParallelAlgorithm n in nbs) {
					nna.Add(n.Name, n.Id);
				}
				int ni;
				int s = 1, i = 0;
				for(int j = 0; i < ne2; i++) {
					if(!nna.TryGetValue(selements[i], out ni)) {
						throw new ArgumentException(string.Format("\"{0}\" is not a neighbour of \"{1}\"! Cannot parse parameterized matrix for the factor algorithm!", selements[i], this.Name));
					}
					this.indexMapping.Add(ni, j);
					try {
						dim[j] = int.Parse(selements[++i]);
						s *= dim[j++];
						if(s <= 0) {
							throw new ArgumentException("Cannot parse parameterized matrix: all length operators must be strict positive integers!");
						}
					}
					catch(Exception e) {
						throw new ArgumentException(string.Format("Cannot parse parameterized matrix: \"{0}\" is not an integer!", selements[i]), e);
					}
				}
				int se = s+ne2;
				if(selements.Length < se) {
					throw new ArgumentException("The parameterized matrix of the factor algorithm must have enough arguments!");
				}
				this.supermatrix = Array.CreateInstance(typeof(double), dim);
				int[] iis = new int[ne];
				double p;
				for(; i < se; i++) {
					try {
						p = double.Parse(selements[i]);
					}
					catch(Exception e) {
						throw new ArgumentException(string.Format("Cannot parse parameterized matrix: \"{0}\" is not an floating point!", selements[i]), e);
					}
					this.supermatrix.SetValue(p, iis);
					int iisi = ne-1;
					iis[iisi]++;
					while(iisi > 0 && iis[iisi] >= dim[iisi]) {
						iis[iisi--] = 0;
						iis[iisi]++;
					}
				}
			}
			else {
				throw new ArgumentException("A factor algorithm must be initialized with a parameterized matrix!");
			}
		}
		
		
		public override IEnumerable<int> Steps () {
			while(!this.HasReceivedMessagesGeneric<VectorMessage>(this.HigherIds)) {
				yield return 0x00;
			}
			if(this.LowerIds.Count() <= 0) {
				this.NewChapter();
			}
			yield return 0;
			foreach(ParallelAlgorithm pa in this.LowerNeighbours) {
				this.SendMessage(new VectorMessage(this, pa, this.calculateVector(pa.Id)));
			}
			yield return 0;
			while(!this.HasReceivedMessagesGeneric<VectorMessage>(this.LowerIds)) {
				yield return 0x00;
			}
			yield return 0;
			foreach(ParallelAlgorithm pa in this.HigherNeighbours) {
				this.SendMessage(new VectorMessage(this, pa, this.calculateVector(pa.Id)));
			}
			yield return 0;
		}

		private double[] calculateVector (int id) {
			double[][] source = new double[this.Neighbours.Count][];
			foreach(Tuple<int,VectorMessage> el in this.PeekMessagesGeneric<VectorMessage>(this.NeighbourIds.Where(x => x != id))) {
				source[indexMapping[el.Item1]] = el.Item2.Data;
			}
			int im = indexMapping[id];
			double[] res = new double[dim[im]];
			int[] iis = new int[this.dim.Length];
			double p;
			while(iis[0] < dim[0]) {
				p = (double)this.supermatrix.GetValue(iis);
				for(int i = 0; i < im; i++) {
					p *= source[i][iis[i]];
				}
				for(int i = im+1; i < iis.Length; i++) {
					p *= source[i][iis[i]];
				}
				res[iis[im]] += p;
				int iisi = iis.Length-1;
				iis[iisi]++;
				while(iisi > 0 && iis[iisi] >= dim[iisi]) {
					iis[iisi--] = 0;
					iis[iisi]++;
				}
			}
			return res;
		}

		public override PointD MeasureStateSize (Cairo.Context ctx) {
			return new PointD(200.0d, 100.0d);
		}
		
		public override void PaintState (Context ctx) {
			IEnumerable<Tuple<int,VectorMessage>> peek = this.PeekMessagesGeneric<VectorMessage>(this.NeighbourIds);
			object[][] matrix = peek.Select(x => (x.Item2 != null) ? x.Item2.Data.Select(y => y.ToString("0.000")).Cast<object>().ToArray() : null).ToArray();
			int n = matrix.Select(x => ((x == null) ? 0 : x.Length)).FirstOrDefault(x => x > 0);
			object[] nullrow = new object[n];
			for(int i = 0; i < n; i++) {
				nullrow[i] = string.Empty;
			}
			for(int i = 0; i < matrix.Length; i++) {
				if(matrix[i] == null) {
					matrix[i] = nullrow;
				}
			}
			GraphicsUtils.PaintVectorSpace(ctx, matrix, NeighbourIds.ToArray(), new object[0], 2, new Color(0.0d, 0.7d, 0.0d));
		}
		#endregion
		
		
	}
}

/*
 * TextExtents te;
			VectorMessage vm;
			double[] vals;
			List<Tuple<int,Message>> data = new List<Tuple<int, Message>>(this.PeekMessages(typeof(VectorMessage), this.NeighbourIds));
			double colw = 0.0d, colh = 0.0d;
			double[] ciws = new double[data.Count];
			int i = 0;
			foreach(Tuple<int,Message> im in data) {
				te = ctx.TextExtents(im.Item1.ToString());
				ciws[i++] = te.Width;
				colw = Math.Max(colw, te.Width);
				colh = Math.Max(colh, te.Height);
				if(im.Item2 != null) {
					vm = (VectorMessage)im.Item2;
					vals = vm.Data;
					foreach(double x in vals) {
						te = ctx.TextExtents(x.ToString("0.000"));
						colw = Math.Max(colw, te.Width);
						colh = Math.Max(colh, te.Height);
					}
				}
			}
			colw = Math.Max(10.0d, colw)+10.0d;
			colh += 3.0d;
			double colx = 5.0d;
			double coly;
			i = 0;
			foreach(Tuple<int,Message> im in data) {
				coly = colh+5.0d;
				ctx.MoveTo(colx+0.5d*(colw-ciws[i]), coly);
				ctx.ShowText(im.Item1.ToString());
				coly += 2.0d*colh;
				if(im.Item2 != null) {
					vm = (VectorMessage)im.Item2;
					vals = vm.Data;
					foreach(double x in vals) {
						te = ctx.TextExtents(x.ToString("0.000"));
						ctx.MoveTo(colx+colw-te.Width, coly);
						ctx.ShowText(x.ToString("0.000"));
						coly += colh;
					}
				}
				i++;
				colx += colw;
			}*/
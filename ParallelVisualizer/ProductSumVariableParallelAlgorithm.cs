//  
//  ProductSumVariableParallelAlgorithm.cs
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
	
	[AlgorithmName("ProductSumVariable")]
	public class ProductSumVariableParallelAlgorithm : MailboxAlgorithm {

		private string[] possibleValues = new string[0];

		#region implemented abstract members of ParallelVisualizer.ParallelAlgorithm
		public override string SourceCode {
			get {
				return @"Test";
			}
		}
		
		public override void Setup (params string[] args) {
			if(args != null && args.Length > 0) {
				this.possibleValues = args[0].Split(',');
			}
		}
		
		
		public override IEnumerable<int> Steps () {
			while(!this.HasReceivedMessagesGeneric<VectorMessage>(this.HigherIds)) {
				yield return 0x00;
			}
			yield return 0;
			foreach(ParallelAlgorithm pa in this.LowerNeighbours) {
				this.SendMessage(new VectorMessage(this, pa, calculateResult(pa.Id)));
			}
			yield return 0;
			while(!this.HasReceivedMessagesGeneric<VectorMessage>(this.LowerIds)) {
				yield return 0x00;
			}
			yield return 0;
			foreach(ParallelAlgorithm pa in this.HigherNeighbours) {
				this.SendMessage(new VectorMessage(this, pa, calculateResult(pa.Id)));
			}
			yield return 0;
		}

		private double[] calculateResult (int id) {
			IEnumerable<VectorMessage> peek = this.PeekMessagesGeneric<VectorMessage>(this.NeighbourIds.Where(x => x != id)).Select(x => x.Item2);
			IEnumerable<double[]> matrix = peek.Select(x => (x != null) ? x.Data : null);
			int n = matrix.Select(x => ((x == null) ? 0 : x.Length)).FirstOrDefault(x => x > 0);
			if(n <= 0) {
				n = this.possibleValues.Length;
			}
			double[] res = new double[n];
			for(int i = 0; i < n; i++) {
				res[i] = 1.0d;
			}
			foreach(double[] row in matrix) {
				if(row != null) {
					for(int i = 0; i < n; i++) {
						res[i] *= row[i];
					}
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
			if(n <= 0) {
				n = this.possibleValues.Length;
			}
			double[] seed = new double[n];
			for(int i = 0; i < n; i++) {
				seed[i] = 1.0d;
			}
			double[][] marginal = new double[][] {peek.Where(x => x.Item2 != null).Select(x => x.Item2.Data).Aggregate((IEnumerable<double>)seed, (x,y) => Utils.Elementwise((a,b) => a*b, x, y)).ToArray()};
			object[] nullrow = new object[0];
			for(int i = 0; i < matrix.Length; i++) {
				if(matrix[i] == null) {
					matrix[i] = nullrow;
				}
			}
			GraphicsUtils.PaintVectorSpace(ctx, matrix, NeighbourIds.ToArray(), this.possibleValues, 0, new Color(0.7d, 0.0d, 0.0d));
			ctx.Translate(0.0d, 50.0d);
			GraphicsUtils.PaintVectorSpace(ctx, marginal, new string[] {"marginal"}, this.possibleValues, 1, new Color(0.35d, 0.0d, 0.0d));
		}
		#endregion
		
		
	}
}


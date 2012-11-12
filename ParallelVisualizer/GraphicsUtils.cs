//
//  GraphicsUtils.cs
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
using Cairo;

namespace ParallelVisualizer {

	public static class GraphicsUtils {

		public static void PaintVectorSpace<X,Y,Z> (Context ctx, Z[][] matrix, X[] xlabels, Y[] ylabels, int markedrow, Color markColor) {
			TextExtents te;
			double colw = 0.0d, colh = 0.0d;
			double[] ciws = new double[xlabels.Length];
			int i = 0;
			foreach(Z[] vec in matrix) {
				te = ctx.TextExtents(xlabels[i].ToString());
				ciws[i++] = te.Width;
				colw = Math.Max(colw, te.Width);
				colh = Math.Max(colh, te.Height);
				foreach(Z x in vec) {
					te = ctx.TextExtents(x.ToString());
					colw = Math.Max(colw, te.Width);
					colh = Math.Max(colh, te.Height);
				}
			}
			colw = Math.Max(10.0d, colw);
			colh += 3.0d;
			double colx = 5.0d;
			double coly = 2*colh+5.0d;
			i = 0;
			double coll = 0.0d;
			foreach(Y l in ylabels) {
				coll = Math.Max(coll, ctx.TextExtents(l.ToString()).XAdvance);
			}
			if(markedrow >= 0) {
				ctx.Save();
				ctx.Color = markColor;
				ctx.Rectangle(colx, 5.0d+colh+markedrow*colh, coll+(10.0d+colw)*xlabels.Length+colx, colh+5.0d);
				ctx.Fill();
				ctx.Restore();
			}
			foreach(Y l in ylabels) {
				ctx.MoveTo(colx, coly);
				ctx.ShowText(l.ToString());
				coly += colh;
			}
			colx += 10.0d+coll;
			i = 0;
			foreach(Z[] vec in matrix) {
				coly = colh+5.0d;
				ctx.MoveTo(colx+0.5d*(colw-ciws[i]), coly);
				ctx.ShowText(xlabels[i].ToString());
				coly += colh;
				foreach(Z x in vec) {
					te = ctx.TextExtents(x.ToString());
					ctx.MoveTo(colx+colw-te.Width, coly);
					ctx.ShowText(x.ToString());
					coly += colh;
				}
				i++;
				colx += colw+10.0d;
			}
		}

	}
}


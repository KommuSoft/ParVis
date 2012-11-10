//  
//  Utils.cs
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
	
	public static class Utils {
		
		public static void CutTowardsCenter (PointD pa, PointD pb, double dr, out PointD pc, out PointD pd)
		{
			double cx = 0.5d * (pa.X + pb.X);
			double cy = 0.5d * (pa.Y + pb.Y);
			double dx = pb.X - cx;
			double dy = pb.Y - cy;
			double drr = Math.Sqrt (dx * dx + dy * dy);
			drr = (drr - dr) / drr;
			dx *= drr;
			dy *= drr;
			pc = new PointD (cx - dx, cy - dy);
			pd = new PointD(cx + dx,cy + dy);
		}
		public static Rectangle CreateConvexRectangle (params PointD[] points)
		{
			if (points == null || points.Length < 1) {
				return new Rectangle(0.0d,0.0d,0.0d,0.0d);
			}
			else {
				double xm = points[0].X;
				double xM = xm;
				double ym = points[0].Y;
				double yM = ym;
				double x, y;
				for (int i = 1; i < points.Length; i++) {
					x = points[i].X;
					y = points[i].Y;
					xm = Math.Min (xm, x);
					ym = Math.Min (ym, y);
					xM = Math.Max (xM, x);
					yM = Math.Max (yM, y);
				}
				return new Rectangle (xm, ym, xM - xm, yM - ym);
			}
		}
		public static Rectangle CreateConvexRectangle (IEnumerable<PointD> points)
		{
			if (points == null) {
				return new Rectangle (0.0d, 0.0d, 0.0d, 0.0d);
			} else {
				double xm = double.PositiveInfinity;
				double xM = double.NegativeInfinity;
				double ym = double.PositiveInfinity;
				double yM = double.NegativeInfinity;
				double x, y;
				foreach (PointD p in points) {
					x = p.X;
					y = p.Y;
					xm = Math.Min (xm, x);
					ym = Math.Min (ym, y);
					xM = Math.Max (xM, x);
					yM = Math.Max (yM, y);
				}
				return new Rectangle (xm, ym, xM - xm, yM - ym);
			}
		}
		public static Rectangle InflateRectangle (Rectangle r, double width, double height)
		{
			return new Rectangle(r.X-0.5d*width,r.Y-0.5d*height,r.Width+width,r.Height+height);
		}
		
	}
}


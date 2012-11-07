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
		
	}
}


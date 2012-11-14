//  
//  BlueprintStyle.cs
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
	
	public static class BlueprintStyle {
		
		public static readonly Color BluePrint = new Color(0.0d, 0.3647d, 0.5882d);
		public static readonly Color BluePrintShadow = new Color(0.0d, 0.273525d, 0.44115d);
		public static readonly Color SoftWhite = new Color(0.5d, 0.6824d, 0.7961d);
		public static readonly Color HardWhite = new Color(0.96d, 0.96d, 0.96d);
		public const double Thickness = 5.0d;
		public const int SkewDelta = 5;
		private static Pattern fillPattern = null;
		private static Pattern shadowPatternDown = null;
		private static Pattern shadowPatternRight = null;
		private static Pattern shadowPatternDownRight = null;
		public static Pattern FillPattern {
			get {
				if(fillPattern == null) {
					using(ImageSurface imsu = new ImageSurface (Format.ARGB32, SkewDelta, SkewDelta)) {
						using(Context ctx = new Context (imsu)) {
							ctx.MoveTo(2.0d*SkewDelta, -SkewDelta);
							ctx.LineTo(-SkewDelta, 2.0d*SkewDelta);
							ctx.Color = HardWhite;
							ctx.LineWidth = 1.0d;
							ctx.Stroke();
							((IDisposable)ctx).Dispose();
						}
						fillPattern = new SurfacePattern(imsu);
						fillPattern.Extend = Extend.Repeat;
					}
				}
				return fillPattern;
			}
		}
		public static Pattern ShadowPatternDown {
			get {
				if(shadowPatternDown == null) {
					LinearGradient p = new LinearGradient(0.0d, 0.0d, 0.0d, 10.0d);
					double fctr = 0.5d;
					for(double y = 0.0d; y <= 1.0d; y += 0.1d) {
						p.AddColorStop(y, new Color(0.0d, 0.0d, 0.0d, fctr));
						fctr -= 0.05d;
					}
					p.Extend = Extend.Pad;
					shadowPatternDown = p;
				}
				return shadowPatternDown;
			}
		}
		public static Pattern ShadowPatternRight {
			get {
				if(shadowPatternRight == null) {
					LinearGradient p = new LinearGradient(0.0d, 0.0d, 10.0d, 0.0d);
					double fctr = 0.5d;
					for(double y = 0.0d; y <= 1.0d; y += 0.1d) {
						p.AddColorStop(y, new Color(0.0d, 0.0d, 0.0d, fctr));
						fctr -= 0.05d;
					}
					p.Extend = Extend.Pad;
					shadowPatternRight = p;
				}
				return shadowPatternRight;
			}
		}
		public static Pattern ShadowPatternDownRight {
			get {
				if(shadowPatternRight == null) {
					RadialGradient p = new RadialGradient(0.0d, 0.0d, 0.0d, 0.0d, 0.0d, 10.0d);
					double fctr = 0.5d;
					for(double y = 0.0d; y <= 1.0d; y += 0.1d) {
						p.AddColorStop(y, new Color(0.0d, 0.0d, 0.0d, fctr));
						fctr -= 0.05d;
					}
					p.Extend = Extend.Pad;
					shadowPatternDownRight = p;
				}
				return shadowPatternDownRight;
			}
		}
		
	}
}


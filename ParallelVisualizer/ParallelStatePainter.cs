//  
//  ParallelStatePainter.cs
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
using Gtk;
using Cairo;

namespace ParallelVisualizer {

	[System.ComponentModel.ToolboxItem(true)]
	public class ParallelStatePainter : DrawingArea {
		
		public readonly Color BluePrint = new Color (0.0d, 0.3647d, 0.5882d);
		public readonly Color SoftWhite = new Color (0.5d, 0.6824d, 0.7961d);
		public readonly Color HardWhite = new Color (0.96d, 0.96d, 0.96d);
		private Pattern fillPattern = null;
		public const double Offset = 10.0d;
		public const double Thickness = 5.0d;
		public const double Offset2 = Offset+Thickness;
		public const double LineDelta = 10.0d;
		public const double AlgorithmRadius = 37.0d;
		public const int SkewDelta = 5;
		private readonly Dictionary<ParallelAlgorithm,PointD> positions = new Dictionary<ParallelAlgorithm,PointD>();
		private readonly ParallelSimulation ps;
		
		public Pattern FillPattern {
			get {
				if (this.fillPattern == null) {
					using (ImageSurface imsu = new ImageSurface (Format.ARGB32, SkewDelta, SkewDelta)) {
						using (Context ctx = new Context (imsu)) {
							ctx.MoveTo (2.0d * SkewDelta, -SkewDelta);
							ctx.LineTo (-SkewDelta, 2.0d * SkewDelta);
							ctx.Color = HardWhite;
							ctx.LineWidth = 1.0d;
							ctx.Stroke ();
							((IDisposable)ctx).Dispose ();
						}
						this.fillPattern = new SurfacePattern (imsu);
						this.fillPattern.Extend = Extend.Repeat;
					}
				}
				return this.fillPattern;
			}
		}
		
		public ParallelStatePainter (ParallelSimulation ps)
		{
			// Insert initialization code here.
			this.ps = ps;
			double gamma = 2.0d * Math.PI / ps.Algorithms.Count;
			int i = 0;
			foreach (ParallelAlgorithm pa in this.ps.Algorithms) {
				this.positions.Add (pa, new PointD (0.5d + 0.375d * Math.Cos (i * gamma), 0.5d + 0.375d * Math.Sin (i * gamma)));
				i++;
			}
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev) {
			// Insert button press handling code here.
			return base.OnButtonPressEvent (ev);
		}

		protected override bool OnExposeEvent (Gdk.EventExpose ev)
		{
			base.OnExposeEvent (ev);
			// Insert drawing code here.
			Gdk.Rectangle reg = ev.Area;
			int w = reg.Width;
			int h = reg.Height;
			Context ctx = Gdk.CairoHelper.Create (this.GdkWindow);
			ctx.FillRule = FillRule.EvenOdd;
			ctx.Color = BluePrint;
			ctx.Rectangle (0.0d, 0.0d, w, h);
			ctx.Fill ();
			ctx.Color = SoftWhite;
			ctx.Rectangle (Offset, Offset, w - 2 * Offset, h - 2 * Offset);
			ctx.ClosePath ();
			ctx.Rectangle (Offset2, Offset2, w - 2 * Offset2, h - 2 * Offset2);
			ctx.ClosePath ();
			ctx.Fill ();
			double W = w - 2 * Offset2;
			int n = (int)Math.Round ((double)W / LineDelta);
			double dx = (double)W / n;
			ctx.LineWidth = 0.5d;
			for (int i = 1; i < n; i++) {
				ctx.MoveTo (dx * i + Offset2, Offset2);
				ctx.RelLineTo (0.0d, h - 2 * Offset2);
				ctx.Stroke ();
			}
			double H = h - 2 * Offset2;
			n = (int)Math.Round ((double)H / LineDelta);
			double dy = (double)H / n;
			for (int i = 1; i < n; i++) {
				ctx.MoveTo (Offset2, dy * i + Offset2);
				ctx.RelLineTo (w - 2.0d * Offset2, 0.0d);
				ctx.Stroke ();
			}
			ctx.LineWidth = 2.0d;
			foreach (KeyValuePair<ParallelAlgorithm, PointD> kvp in this.positions) {
				PointD abs = new PointD (kvp.Value.X * w, kvp.Value.Y * h);
				ctx.Arc (abs.X, abs.Y, AlgorithmRadius, 0.0d, 2.0d * Math.PI);
				ctx.ClosePath ();
				ctx.NewSubPath ();
				ctx.Arc (abs.X, abs.Y, AlgorithmRadius - Thickness, 0.0d, 2.0d * Math.PI);
				ctx.ClosePath ();
				ctx.NewSubPath();
			}
			ctx.Pattern = this.FillPattern;
			ctx.FillPreserve ();
			ctx.Color = HardWhite;
			ctx.Stroke();
			((IDisposable)ctx.Target).Dispose ();
			((IDisposable)ctx).Dispose ();
			return true;
		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation) {
			base.OnSizeAllocated (allocation);
			// Insert layout code here.
		}

		protected override void OnSizeRequested (ref Requisition requisition) {
			// Calculate desired size here.
			requisition.Height = 50;
			requisition.Width = 50;
		}
	}
}


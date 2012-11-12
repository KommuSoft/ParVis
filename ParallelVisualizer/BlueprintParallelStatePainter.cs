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
	public class BlueprintParallelStatePainter : DrawingArea {
		
		public const double Offset = 10.0d;
		public const double Offset2 = Offset+BlueprintStyle.Thickness;
		public const double LineDelta = 10.0d;
		public const double AlgorithmRadius = 37.0d;
		private readonly Dictionary<ParallelAlgorithm,PointD> positions = new Dictionary<ParallelAlgorithm,PointD>();
		private readonly ParallelSimulation ps;
		private ParallelAlgorithm hoverAlgo = null;
		private double time;
		private readonly Rectangle NoteRectangle = new Rectangle(1.5d*Offset2, 1.5d*Offset2, 300.0d, 450.0d);

		internal double Time {
			set {
				this.time = value;
			}
		}
		public ParallelAlgorithm HoverAlgo {
			get {
				return this.hoverAlgo;
			}
			set {
				if(value != this.hoverAlgo) {
					this.hoverAlgo = value;
					this.QueueDrawArea((int)Math.Floor(NoteRectangle.X), (int)Math.Floor(NoteRectangle.Y), (int)Math.Ceiling(NoteRectangle.Width)+1, (int)Math.Ceiling(NoteRectangle.Height)+1);
				}
			}
		}
		
		public BlueprintParallelStatePainter (ParallelSimulation ps) {
			// Insert initialization code here.
			this.ps = ps;
			this.Reload();
			this.AddEvents((int)Gdk.EventMask.PointerMotionMask);
		}
		
		public void Reload () {
			double gamma = 2.0d*Math.PI/ps.Algorithms.Count;
			int i = 0;
			foreach(ParallelAlgorithm pa in this.ps.Algorithms) {
				if(!this.positions.ContainsKey(pa)) {
					RelativePosition rp = ps.GetRelativePosition(pa);
					if(rp != null) {
						this.positions.Add(pa, new PointD(rp.X, rp.Y));
					}
					else {
						this.positions.Add(pa, new PointD(0.5d+0.375d*Math.Sin(i*gamma), 0.5d+0.375d*Math.Cos(i*gamma)));
					}
				}
				i++;
			}
			this.Repaint(null, null);
		}
		public void Repaint (object s, EventArgs e) {
			this.QueueDraw();
		}
		public void RepaintEdges (object s, EventArgs e) {
			this.QueueDraw();
		}

		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt) {
			double x = evnt.X, y = evnt.Y;
			int w, h;
			this.GdkWindow.GetSize(out w, out h);
			foreach(KeyValuePair<ParallelAlgorithm,PointD> kvp in positions) {
				double dx = kvp.Value.X*w-x;
				double dy = kvp.Value.Y*h-y;
				if(dx*dx+dy*dy <= AlgorithmRadius*AlgorithmRadius) {
					this.HoverAlgo = kvp.Key;
					return base.OnMotionNotifyEvent(evnt);
				}
			}
			this.HoverAlgo = null;
			return base.OnMotionNotifyEvent(evnt);
		}

		protected override bool OnButtonPressEvent (Gdk.EventButton ev) {
			// Insert button press handling code here.
			return base.OnButtonPressEvent(ev);
		}

		protected override bool OnExposeEvent (Gdk.EventExpose ev) {
			base.OnExposeEvent(ev);
			// Insert drawing code here.
			int w, h;
			this.GdkWindow.GetSize(out w, out h);
			Context ctx = Gdk.CairoHelper.Create(this.GdkWindow);
			ctx.FillRule = FillRule.EvenOdd;
			ctx.Color = BlueprintStyle.BluePrint;
			ctx.Rectangle(0.0d, 0.0d, w, h);
			ctx.Fill();
			ctx.Color = BlueprintStyle.SoftWhite;
			ctx.Rectangle(Offset, Offset, w-2*Offset, h-2*Offset);
			ctx.ClosePath();
			ctx.Rectangle(Offset2, Offset2, w-2*Offset2, h-2*Offset2);
			ctx.ClosePath();
			ctx.Fill();
			double W = w-2*Offset2;
			int n = (int)Math.Round((double)W/LineDelta);
			double dx = (double)W/n;
			ctx.LineWidth = 0.5d;
			for(int i = 1; i < n; i++) {
				ctx.MoveTo(dx*i+Offset2, Offset2);
				ctx.RelLineTo(0.0d, h-2*Offset2);
				ctx.Stroke();
			}
			double H = h-2*Offset2;
			n = (int)Math.Round((double)H/LineDelta);
			double dy = (double)H/n;
			for(int i = 1; i < n; i++) {
				ctx.MoveTo(Offset2, dy*i+Offset2);
				ctx.RelLineTo(w-2.0d*Offset2, 0.0d);
				ctx.Stroke();
			}
			ctx.LineWidth = 2.0d;
			foreach(KeyValuePair<ParallelAlgorithm, PointD> kvp in this.positions) {
				PointD abs = new PointD(kvp.Value.X*w, kvp.Value.Y*h);
				ctx.Arc(abs.X, abs.Y, AlgorithmRadius, 0.0d, 2.0d*Math.PI);
				ctx.ClosePath();
				ctx.NewSubPath();
				ctx.Arc(abs.X, abs.Y, AlgorithmRadius-BlueprintStyle.Thickness, 0.0d, 2.0d*Math.PI);
				ctx.ClosePath();
				ctx.NewSubPath();
			}
			ctx.Pattern = BlueprintStyle.FillPattern;
			ctx.FillPreserve();
			ctx.Color = BlueprintStyle.HardWhite;
			ctx.Stroke();
			foreach(KeyValuePair<ParallelAlgorithm, PointD> kvp in this.positions) {
				PointD abs = new PointD(kvp.Value.X*w, kvp.Value.Y*h);
				TextExtents te = ctx.TextExtents(kvp.Key.Name);
				ctx.MoveTo(abs.X-0.5d*te.Width, abs.Y+0.5d*te.Height);
				ctx.ShowText(kvp.Key.Name);
			}
			PointD pc, pd;
			double tm = this.time%1.0d;
			foreach(Edge e in this.ps.Edges) {
				PointD pa = new PointD(this.positions[e.Node1].X, this.positions[e.Node1].Y);
				pa.X *= w;
				pa.Y *= h;
				PointD pb = new PointD(this.positions[e.Node2].X, this.positions[e.Node2].Y);
				pb.X *= w;
				pb.Y *= h;
				Utils.CutTowardsCenter(pa, pb, AlgorithmRadius, out pc, out pd);
				double dxr = pc.X-pd.X;
				double dyr = pc.Y-pd.Y;
				double r = Math.Sqrt(dxr*dxr+dyr*dyr);
				ctx.MoveTo(pc);
				ctx.LineTo(pd);
				ctx.Stroke();
				ctx.Save();
				ctx.Translate(pc.X, pc.Y);
				ctx.Rotate(Math.Atan2(pb.Y-pa.Y, pb.X-pa.X));
				double v = tm/e.Delay;
				foreach(Tuple<double,string> msg in e.GetUpwardsMessages()) {
					TextExtents te = ctx.TextExtents(msg.Item2);
					ctx.MoveTo((msg.Item1+v)*(r-te.XAdvance), te.Height+2.0d);
					ctx.ShowText(msg.Item2);
				}
				ctx.Restore();
				ctx.Save();
				ctx.Translate(pd.X, pd.Y);
				ctx.Rotate(Math.Atan2(pa.Y-pb.Y, pa.X-pb.X));
				foreach(Tuple<double, string> msg in e.GetDownwardsMessages()) {
					TextExtents te = ctx.TextExtents(msg.Item2);
					ctx.MoveTo((msg.Item1+v)*(r-te.XAdvance), te.Height+2.0d);
					ctx.ShowText(msg.Item2);
				}
				ctx.Restore();
			}
			if(this.hoverAlgo != null) {
				ctx.Rectangle(this.NoteRectangle);
				ctx.Color = new Color(1.0d, 1.0d, 1.0d);
				ctx.Fill();
				ctx.Save();
				ctx.Translate(this.NoteRectangle.X, this.NoteRectangle.Y);
				ctx.Color = new Color(0.0d, 0.0d, 0.0d);
				this.hoverAlgo.PaintState(ctx);
				ctx.Restore();
			}
			/*ctx.MoveTo(1.5d*Offset2+40, Offset2);
			ctx.RelLineTo(0.0d, 30.0d);
			ctx.Arc(1.5d*Offset2+25, 1.5d*Offset2+20.0d, 15.0d, 0.0d, Math.PI);
			ctx.RelLineTo(0.0d, -1.5d*Offset2-20.0d);
			ctx.Color = new Color(0.0d, 0.0d, 0.0d);
			ctx.LineWidth = 5.0d;
			ctx.Stroke();*/
			((IDisposable)ctx.Target).Dispose();
			((IDisposable)ctx).Dispose();
			return true;
		}

		protected override void OnSizeAllocated (Gdk.Rectangle allocation) {
			base.OnSizeAllocated(allocation);
			// Insert layout code here.
			this.QueueDraw();
		}

		protected override void OnSizeRequested (ref Requisition requisition) {
			// Calculate desired size here.
			requisition.Height = 50;
			requisition.Width = 50;
		}
	}
}


//  
//  BlueprintSlider.cs
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

namespace ParallelVisualizer
{
	[System.ComponentModel.ToolboxItem(true)]
	public class BlueprintSlider : Gtk.DrawingArea {
		
		public const double Offset = 10;
		private int min = 1;
		private int max = 15;
		private int current = 10;
		private const double shaft = 0.25d;
		private double flapWidth = 1.0d;
		
		public BlueprintSlider ()
		{
			// Insert initialization code here.
			this.AddEvents((int)(Gdk.EventMask.PointerMotionMask|Gdk.EventMask.ButtonPressMask));
		}
		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			// Insert button press handling code here.
			int index = getIndexFromX (ev.X);
			if (index != -1) {
				int w, h;
				this.GdkWindow.GetSize (out w, out h);
				int old = this.current;
				this.current = index;
				double xm, xM;
				getBoundsFromIndex (old, out xm, out xM);
				this.QueueDrawArea ((int)Math.Floor (xm), 0x00, (int)Math.Ceiling (xM), h);
				getBoundsFromIndex (this.current, out xm, out xM);
				this.QueueDrawArea ((int)Math.Floor (xm), 0x00, (int)Math.Ceiling (xM), h);
			}
			return base.OnButtonPressEvent (ev);
		}
		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt) {
			if(getIndexFromX(evnt.X) != -1) {
				this.GdkWindow.Cursor = new Gdk.Cursor(Gdk.CursorType.Hand1);
			}
			else {
				this.GdkWindow.Cursor = new Gdk.Cursor (Gdk.CursorType.Arrow);
			}
			return base.OnMotionNotifyEvent (evnt);
		}
		private int getIndexFromX (double x) {
			if(x < Offset || x > (max-min+1)*(1.0d+shaft)*flapWidth-shaft*flapWidth+Offset) {
				return -1;
			}
			else {
				double d = ((x - Offset) / ((1.0d + shaft) * flapWidth));
				double dm = d%1.0d;
				if(dm <= 1.0d / (1.0d + shaft)) {
					return min+(int) d;
				}
				else {
					return -1;
				}
			}
		}
		private void getBoundsFromIndex (int index, out double xm, out double xM)
		{
			xm = Offset + (index-min) * (1.0d + shaft) * flapWidth;
			xM = xm+flapWidth;
		}
		protected override bool OnExposeEvent (Gdk.EventExpose ev)
		{
			base.OnExposeEvent (ev);
			// Insert drawing code here.
			Context ctx = Gdk.CairoHelper.Create (this.GdkWindow);
			ctx.FillRule = FillRule.EvenOdd;
			int w, h;
			this.GdkWindow.GetSize(out w, out h);
			ctx.Color = new Color (0.0d, 0.0d, 0.0d);
			ctx.Rectangle (0.0d, Offset, w, h - Offset);
			ctx.Fill ();
			ctx.Color = BlueprintStyle.BluePrint;
			ctx.Rectangle (0.0d, 0.0d, w, Offset);
			ctx.Fill ();
			double wr = w - 2.0d * Offset;
			double ew = wr / (max - min + 1 + shaft * (max - min));
			this.flapWidth = ew;
			double xc = Offset;
			for (int i = min; i <= max; i++) {
				if (i != current) {
					ctx.Rectangle (xc, Offset, ew, h - 2.0d * Offset);
				}
				xc += (1.0d+shaft) * ew;
			}
			ctx.FillPreserve ();
			LinearGradient p = new LinearGradient (0.0d, Offset, 0.0d, Offset + 10.0d);
			double fctr = 0.5d;
			for (double y = 0.0d; y <= 1.0d; y += 0.1d) {
				p.AddColorStopRgb (y, new Color (fctr * BlueprintStyle.BluePrintShadow.R, fctr * BlueprintStyle.BluePrintShadow.G, fctr * BlueprintStyle.BluePrintShadow.B));
				fctr += 0.05d;
			}
			ctx.Pattern = p;
			ctx.Fill ();
			ctx.Rectangle (Offset + (1.0d+shaft) * ew * (current - min), Offset, ew, h - 2.0d * Offset);
			ctx.Color = BlueprintStyle.BluePrint;
			ctx.Fill ();
			xc = Offset;
			ctx.Color = BlueprintStyle.HardWhite;
			ctx.MoveTo (0.0d, Offset);
			ctx.LineTo (Offset, Offset);
			for (int i = min; i <= max; i++) {
				ctx.LineTo (xc, h - Offset);
				ctx.LineTo (xc + ew, h - Offset);
				ctx.LineTo (xc + ew, Offset);
				xc += (1.0d+shaft) * ew;
				if (i < max) {
					ctx.LineTo (xc, Offset);
				}
			}
			ctx.LineTo (w - Offset, Offset);
			ctx.LineTo (w, Offset);
			ctx.MoveTo (Offset, Offset);
			ctx.LineTo (Offset + (1.0d+shaft) * ew * (current - min), Offset);
			ctx.MoveTo (w - Offset, Offset);
			ctx.LineTo (w - Offset + (1.0d + shaft) * ew * (current - max), Offset);
			ctx.Stroke ();
			xc = Offset + 0.5d * ew;
			for (int i = min; i <= max; i++) {
				TextExtents te = ctx.TextExtents (i.ToString ());
				ctx.MoveTo(xc-0.5d*te.Width,0.5d*(h+te.Height));
				ctx.ShowText(i.ToString());
				xc += (1.0d + shaft)*ew;
			}
			((IDisposable)ctx.Target).Dispose ();
			((IDisposable)ctx).Dispose ();
			return true;
		}
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			// Insert layout code here.
			this.QueueDraw();
		}
		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			// Calculate desired size here.
			requisition.Height = 50;
			requisition.Width = 35;
		}
	}
}


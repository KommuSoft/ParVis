//  
//  BlueprintMediabar.cs
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
	public class BlueprintMediabar : Gtk.DrawingArea
	{
		
		public const double Offset = 10.0d;
		private double min = 0.0d;
		private double max = 20.0d;
		private double current = 0.0d;
		private double speed = 1.0d;
		private MediaMode mode = MediaMode.Pause;
		
		public double Min {
			get {
				return this.min;
			}
			set {
				this.min = value;
			}
		}
		public double Max {
			get {
				return this.max;
			}
			set {
				this.max = value;
			}
		}
		public double Current {
			get {
				return this.current;
			}
			set {
				this.current = value;
			}
		}
		public double Speed {
			get {
				return this.speed;
			}
			set {
				this.speed = value;
			}
		}
		public MediaMode Mode {
			get {
				return this.mode;
			}
			set {
				this.mode = value;
				this.checkMode ();
				this.QueueDrawArea ((int)Math.Floor (Offset) - 1, 0, 35, 34);
			}
		}
		
		public BlueprintMediabar ()
		{
			// Insert initialization code here.
			this.AddEvents ((int) (Gdk.EventMask.PointerMotionMask|Gdk.EventMask.ButtonPressMask));
		}
		protected override bool OnMotionNotifyEvent (Gdk.EventMotion evnt)
		{
			double x = evnt.X, y = evnt.Y;
			if (x >= Offset && x <= Offset+32 && y >= 1 && y <= 33) {
				this.GdkWindow.Cursor = new Gdk.Cursor (Gdk.CursorType.Hand1);
			} else {
				this.GdkWindow.Cursor = new Gdk.Cursor (Gdk.CursorType.Arrow);
			}
			return base.OnMotionNotifyEvent (evnt);
		}
		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			// Insert button press handling code here.
			double x = ev.X, y = ev.Y;
			if (x >= Offset && x <= Offset + 32 && y >= 1 && y <= 33) {
				this.Mode = (MediaMode)(0x01 - this.Mode);
			}
			return base.OnButtonPressEvent (ev);
		}
		private void checkMode ()
		{
			if (this.mode == MediaMode.Play) {
				Gdk.Threads.AddTimeout (0, 40, this.updateCurrent);
			}
		}
		private bool updateCurrent ()
		{
			if (this.mode == MediaMode.Pause) {
				return false;
			}
			else {
				this.current += 0.04d * this.speed;
				if (this.current >= this.max) {
					this.current = this.max;
					this.Mode = MediaMode.Pause;
				}
				this.QueueDraw();
				return true;
			}
		}
		protected override bool OnExposeEvent (Gdk.EventExpose ev)
		{
			base.OnExposeEvent (ev);
			// Insert drawing code here.
			Context ctx = Gdk.CairoHelper.Create (this.GdkWindow);
			ctx.FillRule = FillRule.EvenOdd;
			int w, h;
			this.GdkWindow.GetSize (out w, out h);
			ctx.Rectangle (0.0d, 0.0d, w, h);
			ctx.Color = BlueprintStyle.BluePrint;
			double xb = 2 * Offset + 36.0d;
			double wb = w - Offset - xb-4.0d;
			double xbt = wb * (current - min) / (max - min) - 4.0d;
			ctx.Fill ();
			switch (this.mode) {
			case MediaMode.Pause:
				ctx.MoveTo (Offset, 1.0d);
				ctx.RelLineTo (32.0d, 16.0d);
				ctx.RelLineTo (-32.0d, 16.0d);
				ctx.ClosePath ();
				break;
			case MediaMode.Play:
				ctx.Rectangle (Offset + 2, 1.0d, 8.0d, 32.0d);
				ctx.Rectangle (Offset + 18, 1.0d, 8.0d, 32.0d);
				ctx.ClosePath ();
				break;
			}
			ctx.Rectangle (xb + xbt, 1.0d, 8.0d, 32.0d);
			ctx.Pattern = BlueprintStyle.FillPattern;
			ctx.FillPreserve ();
			ctx.Color = BlueprintStyle.HardWhite;
			if (xbt > 0.0d) {
				ctx.MoveTo (xb, 16.0d);
				ctx.RelLineTo (xbt, 0.0d);
			}
			if(xb+xbt+8 < w-Offset) {
			ctx.MoveTo (xb+xbt+8.0d, 16.0d);
			ctx.LineTo (w-Offset-4.0d, 16.0d);
			}
			ctx.Stroke();
			((IDisposable)ctx.Target).Dispose ();
			((IDisposable)ctx).Dispose ();
			return true;
		}
		protected override void OnSizeAllocated (Gdk.Rectangle allocation)
		{
			base.OnSizeAllocated (allocation);
			// Insert layout code here.
		}
		protected override void OnSizeRequested (ref Gtk.Requisition requisition)
		{
			// Calculate desired size here.
			requisition.Height = 34;
			requisition.Width = 100;
		}
	}
}


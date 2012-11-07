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
		private int min = 0;
		private int max = 25;
		private int current = 10;
		
		public BlueprintSlider ()
		{
			// Insert initialization code here.
		}
		protected override bool OnButtonPressEvent (Gdk.EventButton ev)
		{
			// Insert button press handling code here.
			return base.OnButtonPressEvent (ev);
		}
		protected override bool OnExposeEvent (Gdk.EventExpose ev)
		{
			base.OnExposeEvent (ev);
			// Insert drawing code here.
			Context ctx = Gdk.CairoHelper.Create (this.GdkWindow);
			ctx.FillRule = FillRule.EvenOdd;
			Gdk.Rectangle reg = ev.Area;
			int w = reg.Width;
			int h = reg.Height;
			ctx.Color = new Color (0.0d, 0.0d, 0.0d);
			ctx.Rectangle (0.0d, Offset, w, h - Offset);
			ctx.Fill();
			ctx.Color = BlueprintStyle.BluePrint;
			ctx.Rectangle (0.0d, 0.0d, w, Offset);
			double wr = w - 2.0d * Offset;
			double ew = wr / (max - min + 1 + 0.5d * (max - min));
			double xc = Offset;
			for (int i = min; i <= max; i++) {
				ctx.Rectangle (xc, Offset, ew, h-2.0d*Offset);
				xc += 1.5d*ew;
			}
			ctx.Fill ();
			xc = Offset;
			ctx.Color = BlueprintStyle.HardWhite;
			ctx.MoveTo (0.0d, Offset);
			ctx.LineTo (Offset, Offset);
			for (int i = min; i <= max; i++) {
				ctx.LineTo (xc, h - Offset);
				ctx.LineTo (xc + ew, h - Offset);
				ctx.LineTo (xc + ew, Offset);
				xc += 1.5d * ew;
				if (i < max) {
					ctx.LineTo (xc, Offset);
				}
			}
			ctx.LineTo (w - Offset, Offset);
			ctx.LineTo (w, Offset);
			ctx.MoveTo (Offset, Offset);
			ctx.LineTo (Offset + 1.5d*ew * (current - min), Offset);
			ctx.MoveTo (w-Offset, Offset);
			ctx.LineTo (w-Offset + 1.5d *ew * (current-max), Offset);
			ctx.Stroke ();
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


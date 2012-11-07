//  
//  DisplayWindow.cs
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
using Gtk;

namespace ParallelVisualizer {

	public class DisplayWindow : Window {
		
		private ParallelStatePainter psp;
		private BlueprintSlider slider;
		private VBox vb = new VBox(false,0);
		
		public DisplayWindow () : base(WindowType.Toplevel)
		{
			SampleParallelAlgorithm spa0 = new SampleParallelAlgorithm ();
			SampleParallelAlgorithm spa1 = new SampleParallelAlgorithm ();
			SampleParallelAlgorithm spa2 = new SampleParallelAlgorithm ();
			ParallelSimulation ps = new ParallelSimulation (spa0, spa1, spa2);
			this.slider = new BlueprintSlider ();
			ps.AddEdge (spa0, spa1);
			ps.AddEdge (spa0, spa2);
			this.psp = new ParallelStatePainter (ps);
			vb.PackStart (this.psp, true, true, 0x00);
			vb.PackStart(this.slider,false,false,0x00);
			this.Title = "Parallel Visualizer";
			this.Resize (640, 480);
			this.Add(vb);
			this.ShowAll ();
		}
		
		public static void Main (string[] args) {
			Application.Init ();
			using (DisplayWindow dw = new DisplayWindow()) {
				Application.Run ();
			}
		}
	}
}


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
		public DisplayWindow () : base(WindowType.Toplevel) {
			this.Title = "Parallel Visualizer";
			this.Resize (640, 480);
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


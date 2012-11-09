//  
//  RelativePosition.cs
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
using System.Xml.Serialization;
namespace ParallelVisualizer {
	
	[XmlType("RelativePosition")]
	public class RelativePosition {
		
		private double x = 0.5d;
		private double y = 0.5d;
		
		[XmlAttribute("X")]
		public double X {
			get {
				return this.x;
			}
			set {
				this.x = Math.Max (0.0d, Math.Min (1.0d, value));
			}
		}
		[XmlAttribute("Y")]
		public double Y {
			get {
				return this.y;
			}
			set {
				this.y = Math.Max (0.0d, Math.Min (1.0d, value));
			}
		}
		
		public RelativePosition ()
		{
		}
		public RelativePosition (double x, double y)
		{
			this.X = x;
			this.Y = y;
		}
	}
}


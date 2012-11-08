//  
//  EdgeSpecification.cs
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
	
	[XmlType("Edge")]
	public class EdgeSpecification {
		
		private string node1;
		private string node2;
		private int delay;
		
		[XmlAttribute("Node1")]
		public string Node1 {
			get {
				return this.node1;
			}
			set {
				this.node1 = value;
			}
		}
		[XmlAttribute("Node2")]
		public string Node2 {
			get {
				return this.node2; 
			}
			set {
				this.node2 = value;
			}
		}
		[XmlAttribute("Delay")]
		public int Delay {
			get {
				return this.delay;
			}
			set {
				this.delay = value;
			}
		}
		
		public EdgeSpecification ()
		{
		}
	}
}


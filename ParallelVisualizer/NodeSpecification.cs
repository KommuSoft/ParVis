//  
//  NodeSpecification.cs
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

namespace ParallelVisualizer.Specification {
	
	[XmlType("Node")]
	public class NodeSpecification {
		
		private string algorithmName;
		private string nodeName;
		private RelativePosition relPos;
		private string[] args;
		
		[XmlAttribute("AlgorithmName")]
		public string AlgorithmName {
			get {
				return this.algorithmName;
			}
			set {
				this.algorithmName = value;
			}
		}
		[XmlAttribute("NodeName")]
		public string NodeName {
			get {
				return this.nodeName;
			}
			set {
				this.nodeName = value;
			}
		}
		[XmlElement("RelativePosition")]
		public RelativePosition RelativePosition {
			get {
				return this.relPos;
			}
			set {
				this.relPos = value;
			}
		}
		[XmlArray("Arguments")]
		[XmlArrayItem("Argument")]
		public string[] InitializationArguments {
			get {
				return this.args;
			}
			set {
				this.args = value;
			}
		}
		
		public NodeSpecification () {}
		public NodeSpecification (string nodeName, string algorithmName, RelativePosition relPos, params string[] arguments)
		{
			this.nodeName = nodeName;
			this.algorithmName = algorithmName;
			this.relPos = relPos;
			this.args = args;
		}
	}
}
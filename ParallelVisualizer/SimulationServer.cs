//  
//  SimulationServer.cs
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
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using ParallelVisualizer.Specification;

namespace ParallelVisualizer {
	
	public class SimulationServer {
		
		private readonly DllLoader loader = new DllLoader();
		private readonly ParallelSimulation simulator = new ParallelSimulation();
		
		public ParallelSimulation Simulator {
			get {
				return this.simulator;
			}
		}
		
		public SimulationServer ()
		{
		}
		
		public void ConnectAlgorithmLibrary (string filename)
		{
			Assembly a = Assembly.LoadFile (filename);
			loader.AnalyzeAssembly (a);
		}
		public void ReadConfigFile (string filename)
		{
			FileStream fs = File.Open (filename, FileMode.Open, FileAccess.Read);
			XmlSerializer xs = new XmlSerializer (typeof(SimulationSpecification));
			SimulationSpecification ss;
			try {
				ss = (SimulationSpecification)xs.Deserialize (fs);
			}
			catch {
				fs.Close();
				throw new ArgumentException("The file has no correct format: cannot interpret the node structure!");
			}
			ss.AddToSimulator(this.simulator,this.loader);
			fs.Close();
		}
		
	}
}


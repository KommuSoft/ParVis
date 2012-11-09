//  
//  DllLoader.cs
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
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace ParallelVisualizer
{
	public class DllLoader {
		
		private readonly Dictionary<string,ConstructorInfo> loaded = new Dictionary<string, ConstructorInfo>();
		
		public DllLoader () {
		}
		
		public ParallelAlgorithm CreateAlgorithm (string algorithmName)
		{
			ConstructorInfo ci;
			if (!this.loaded.TryGetValue (algorithmName, out ci)) {
				throw new ArgumentException (string.Format ("Cannot find the algorithm \"{0}\". Did you forget to load the proper algorithm library?", algorithmName));
			}
			return (ParallelAlgorithm) ci.Invoke(new object[0x00]);
		}
		
		public void AnalyzeAssembly (Assembly assembly)
		{
			foreach (Type t in assembly.GetTypes ()) {
				if (t.IsSubclassOf (typeof(ParallelAlgorithm)) && !t.IsAbstract && t.IsPublic) {
					ConstructorInfo dc = t.GetConstructor (Type.EmptyTypes);
					if (dc != null) {
						foreach (AlgorithmNameAttribute ana in t.GetCustomAttributes (typeof(AlgorithmNameAttribute), false).Cast<AlgorithmNameAttribute> ()) {
							if (!loaded.ContainsKey (ana.AlgorithmName)) {
								loaded.Add(ana.AlgorithmName,dc);
							}
						}
					}
				}
			}
		}
		
	}
}


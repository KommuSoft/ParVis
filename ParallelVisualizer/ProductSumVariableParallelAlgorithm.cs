//  
//  ProductSumVariableParallelAlgorithm.cs
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
using System.Collections.Generic;
using Cairo;
namespace ParallelVisualizer {
	
	[AlgorithmName("ProductSumVariable")]
	public class ProductSumVariableParallelAlgorithm : ParallelAlgorithm {
		#region implemented abstract members of ParallelVisualizer.ParallelAlgorithm
		public override string SourceCode {
			get {
return @"WaitForAllNeighbors(id > this_id);
res = (1,1);
\for id \in larger_neighbour_ids
res .*= v_id
\end
SendMessage(lowest_neighbour,res);
WaitForAllNeighbors(id < this_id);
res = (1,1);
\for id \in smaller_neighbour_ids
res .*= v_id
\end
SendMessage(id > this_id,res);";
			}
		}
		
		public override void Setup (params string[] args) {
			
		}
		
		
		public override IEnumerable<int> Steps ()
		{
			return null;
		}
		
		
		public override void PaintState (Context ctx)
		{
			
		}
		
		
		protected internal override void ReciveMessage (Message message)
		{
			
		}
		
		#endregion
		
		
	}
}


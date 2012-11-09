using System;
using System.Collections.Generic;

namespace ParallelVisualizer {
	
	[AlgorithmName("BroadcastAlgorithm")]
	public class BroadcastParallelAlgorithm : ParallelAlgorithm {
		
		
		private ParallelAlgorithm other;
		
		public BroadcastParallelAlgorithm ()
		{
			this.other = null;
		}
		
		public override void PaintState (Cairo.Context ctx)
		{
		
		}
		
		protected override internal void ReciveMessage (Message message)
		{
			
		}
		
		public override void Setup (string[] args)
		{
		
		}
		public override IEnumerable<int> Steps ()
		{
			int i = 0;
			while (true) {
				i++;
				if ((i % 5) == 0) {
					foreach (ParallelAlgorithm n in this.Neighbours) {
						this.SendMessage (new TextMessage (this, n, "Test"));
					}
				}
				yield return 0;
			}
		}
		
	}
}


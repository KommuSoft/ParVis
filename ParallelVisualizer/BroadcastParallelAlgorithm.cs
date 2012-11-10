using System;
using System.Collections.Generic;

namespace ParallelVisualizer {
	
	[AlgorithmName("BroadcastAlgorithm")]
	public class BroadcastParallelAlgorithm : ParallelAlgorithm {
		
		
		public override string SourceCode {
			get {
				return "while true\nfor i=1:5\nend\nsendMessageToAll(\"Test\");\nend";
			}
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


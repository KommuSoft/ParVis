//  
//  MailboxParallelAlgorithm.cs
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

namespace ParallelVisualizer {

	public abstract class MailboxParallelAlgorithm : ParallelAlgorithm {
		
		private readonly Dictionary<int,Queue<Message>> mailbox = new Dictionary<int, Queue<Message>> ();
		
		internal protected override void ReciveMessage (Message message) {
			if (message == null) {
				return;
			}
			Queue<Message> msgs;
			if (!this.mailbox.TryGetValue (message.Sender, out  msgs)) {
				msgs = new Queue<Message> ();
				this.mailbox.Add (message.Sender, msgs);
			}
			msgs.Enqueue (message);
		}

		protected bool HasReceivedMessages (Type messageType, IEnumerable<ParallelAlgorithm> ids) {
			return false;
		}
		
	}
}


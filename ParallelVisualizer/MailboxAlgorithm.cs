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
using System.Linq;
using System.Collections.Generic;

namespace ParallelVisualizer {

	public abstract class MailboxAlgorithm : OrderedIdParallelAlgorithm {

		private readonly Dictionary<int,LinkedList<Message>> mailbox = new Dictionary<int, LinkedList<Message>>();

		internal protected override void ReciveMessage (Message message) {
			if(message == null) {
				return;
			}
			LinkedList<Message> msgs;
			if(!this.mailbox.TryGetValue(message.Sender.Id, out  msgs)) {
				msgs = new LinkedList<Message>();
				this.mailbox.Add(message.Sender.Id, msgs);
			}
			msgs.AddLast(message);
		}

		protected bool HasReceivedMessagesGeneric<T> (IEnumerable<int> ids) {
			LinkedList<Message> mq;
			foreach(int id in ids) {
				if(!this.mailbox.TryGetValue(id, out mq) || !mq.Any(x => typeof(T).IsAssignableFrom(x.GetType()))) {
					return false;
				}
			}
			return true;
		}
		protected bool HasReceivedMessages (IEnumerable<int> ids) {
			return HasReceivedMessagesGeneric<Message>(ids);
		}
		protected IEnumerable<Tuple<int,T>> PeekMessagesGeneric<T> (IEnumerable<int> ids) where T : Message {
			LinkedList<Message> mq;
			foreach(int id in ids) {
				if(this.mailbox.TryGetValue(id, out mq)) {
					yield return new Tuple<int,T>(id, (T)mq.FirstOrDefault(x => typeof(T).IsAssignableFrom(x.GetType())));
				}
				else {
					yield return new Tuple<int,T>(id, (T)null);
				}
			}
		}
		protected List<Tuple<int,T>> DequeueMessages<T> (IEnumerable<int> ids) where T : Message {
			List<Tuple<int,T>> res = new List<Tuple<int,T>>();
			Message msg;
			LinkedList<Message> mq;
			LinkedListNode<Message> lmq;
			foreach(int id in ids) {
				if(this.mailbox.TryGetValue(id, out mq)) {
					lmq = mq.First;
					while(lmq != null && !typeof(T).IsAssignableFrom(lmq.Value.GetType())) {
						lmq = lmq.Next;
					}
					if(lmq == null) {
						msg = null;
					}
					else {
						msg = lmq.Value;
						mq.Remove(lmq);
					}
					res.Add(new Tuple<int,T>(id, (T)msg));
				}
				else {
					res.Add(new Tuple<int,T>(id, null));
				}
			}
			return res;
		}

	}
}
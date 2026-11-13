using ClubPenguin.Net.Client.Event;
using System;
using System.Collections.Generic;

namespace ClubPenguin.Net.Locomotion
{
	[Serializable]
	public class PositionTimeline : List<LocomotionActionEvent>
	{
		private readonly long maxQueueTimeMS;

		private readonly long warningQueueTimeMS;

		public PositionTimeline(long maxQueueTimeMS, long warningQueueTimeMS)
		{
			this.maxQueueTimeMS = maxQueueTimeMS;
			this.warningQueueTimeMS = warningQueueTimeMS;
		}

		public LocomotionActionEvent Peek()
		{
			return base[0];
		}

		public LocomotionActionEvent PeekLast()
		{
			return base[base.Count - 1];
		}

		public LocomotionActionEvent Dequeue()
		{
			LocomotionActionEvent result = base[0];
			RemoveAt(0);
			return result;
		}

		public int Enqueue(LocomotionActionEvent position, out bool queueTooLong)
		{
			queueTooLong = false;
			int num = base.Count;
			if (num > 0)
			{
				long num2 = position.Timestamp - base[0].Timestamp;
				if (num2 > warningQueueTimeMS)
				{
					queueTooLong = true;
					if (num2 > maxQueueTimeMS)
					{
						do
						{
							Dequeue();
							num--;
						}
						while (num > 0 && position.Timestamp - base[0].Timestamp > maxQueueTimeMS);
					}
				}
				else
				{
					num2 = base[num - 1].Timestamp - position.Timestamp;
					if (num2 > maxQueueTimeMS)
					{
						return -1;
					}
				}
			}
			int i;
			for (i = 0; i < num; i++)
			{
				if (base[i].Timestamp > position.Timestamp)
				{
					Insert(i, position);
					return i;
				}
			}
			Add(position);
			return i;
		}

		private void debugPrint()
		{
			string str = "Position Timeline: [";
			using (Enumerator enumerator = GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					str = str + "[" + enumerator.Current.ToString() + "], ";
				}
			}
			str += "]";
		}
	}
}

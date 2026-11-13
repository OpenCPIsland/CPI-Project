using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct TaskProgress
	{
		public string taskId;

		public int counter;

		public long day;

		public bool claimed;
	}
}

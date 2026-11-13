using ClubPenguin.Net.Domain;
using System.Collections.Generic;

namespace ClubPenguin.Net.Offline
{
	// Counters and claim flags of the daily challenges. Online this lives on the
	// server; offline it has to survive a restart, so it gets its own table.
	//
	// Day is the UTC midnight the set belongs to: challenges are picked per day,
	// so yesterday's counters must not carry into today's list.
	public struct DailyTaskProgress : IOfflineData
	{
		public long Day;

		public List<TaskProgress> Tasks;

		public void Init()
		{
			Day = 0L;
			Tasks = new List<TaskProgress>();
		}
	}
}

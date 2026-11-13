using System;

namespace Disney.Manimal.Common.Util
{
	public interface IScheduler
	{
		int Schedule(TimeSpan delay, bool repeating, ScheduleCallback callback, object state);

		void Cancel(int id);

		bool IsPending(int id);

		TimeSpan GetTimeLeft(int id);

		TimeSpan GetTimeSinceStart(int id);

		TimeSpan GetDuration(int id);
	}
}

using System;

namespace Disney.Manimal.Common.Util
{
	public class TickDrivenScheduler : BasicScheduler
	{
		private readonly long DefaultPollEveryTicks = 10000000L;

		private long _pollEveryTicks;

		private DateTime _lastCall;

		public long PollEveryTicks
		{
			get
			{
				return (_pollEveryTicks == 0) ? DefaultPollEveryTicks : _pollEveryTicks;
			}
			set
			{
				_pollEveryTicks = value;
			}
		}

		public void Tick()
		{
			DateTime utcNow = DateTime.UtcNow;
			EnsureLastCall(utcNow);
			TimeSpan delta = utcNow - _lastCall;
			if (delta.Ticks >= PollEveryTicks)
			{
				AdvanceTime(delta);
				_lastCall = utcNow;
			}
		}

		private void EnsureLastCall(DateTime now)
		{
			if (_lastCall == default(DateTime))
			{
				_lastCall = now;
			}
		}
	}
}

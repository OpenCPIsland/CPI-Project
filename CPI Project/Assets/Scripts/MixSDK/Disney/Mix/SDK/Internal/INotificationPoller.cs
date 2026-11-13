using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public interface INotificationPoller : IDisposable
	{
		IEnumerable<int> PollIntervals
		{
			get;
			set;
		}

		IEnumerable<int> PokeIntervals
		{
			get;
			set;
		}

		bool UsePollIntervals
		{
			get;
			set;
		}

		int MaximumMissingNotificationTime
		{
			set;
		}

		int Jitter
		{
			get;
			set;
		}

		event EventHandler<AbstractNotificationsPolledEventArgs> OnNotificationsPolled;

		event EventHandler<AbstractNotificationPollerSynchronizationErrorEventArgs> OnSynchronizationError;

		void Pause();

		void Resume();

		void Update();

		void RequestPoll();
	}
}

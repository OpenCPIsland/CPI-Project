using System;

namespace Disney.Kelowna.Common
{
	public interface ICoroutine
	{
		TimeSpan Duration
		{
			get;
		}

		bool Cancelled
		{
			get;
		}

		bool Completed
		{
			get;
		}

		bool Paused
		{
			get;
		}

		bool Disposed
		{
			get;
		}

		event Action ECancelled;

		event Action ECompleted;

		event Action EPaused;

		event Action EResumed;

		event Action EDisposed;

		void Cancel();

		void Pause();

		void Resume();

		void Stop();
	}
}

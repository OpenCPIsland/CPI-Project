using System;
using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class Timer
	{
		private bool repeat;

		private bool stop;

		private float duration;

		private Action callback;

		public bool IsRunning
		{
			get
			{
				return !stop;
			}
		}

		public bool Repeat
		{
			get
			{
				return repeat;
			}
		}

		public Timer(float duration, bool repeat, Action callback)
		{
			stop = false;
			this.repeat = repeat;
			this.duration = duration;
			this.callback = callback;
		}

		public IEnumerator Start()
		{
			do
			{
				if (!stop)
				{
					yield return new WaitForSeconds(duration);
					if (callback != null)
					{
						callback();
					}
				}
			}
			while (repeat && !stop);
		}

		public void Stop()
		{
			stop = true;
		}

		public static IEnumerator Start(float duration, Action callback)
		{
			return Start(duration, false, callback);
		}

		public static IEnumerator Start(float duration, bool repeat, Action callback)
		{
			Timer timer = new Timer(duration, repeat, callback);
			return timer.Start();
		}
	}
}

#if !UNITY_WEBGL
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ClubPenguin.Net.Lan
{
	public sealed class LanTimer
	{
		internal long Due;

		internal long Seq;

		internal Action Callback;

		public bool Cancelled
		{
			get;
			private set;
		}

		public void Cancel()
		{
			Cancelled = true;
		}
	}

	public sealed class LanLoop
	{
		private sealed class TimerComparer : IComparer<LanTimer>
		{
			public int Compare(LanTimer a, LanTimer b)
			{
				int c = a.Due.CompareTo(b.Due);
				return (c != 0) ? c : a.Seq.CompareTo(b.Seq);
			}
		}

		private readonly BlockingCollection<Action> queue = new BlockingCollection<Action>();

		private readonly SortedSet<LanTimer> timers = new SortedSet<LanTimer>(new TimerComparer());

		private readonly Stopwatch clock = Stopwatch.StartNew();

		private long seq;

		private Thread thread;

		private volatile bool running;

		public bool IsLoopThread
		{
			get
			{
				return Thread.CurrentThread == thread;
			}
		}

		public long NowMs
		{
			get
			{
				return clock.ElapsedMilliseconds;
			}
		}

		public void Start()
		{
			running = true;
			thread = new Thread(Run);
			thread.IsBackground = true;
			thread.Name = "CPI LAN Server";
			thread.Start();
		}

		public void Stop()
		{
			running = false;
			queue.Add(delegate
			{
			});
		}

		public void Post(Action action)
		{
			if (running)
			{
				try
				{
					queue.Add(action);
				}
				catch (InvalidOperationException)
				{
				}
			}
		}

		public LanTimer After(double ms, Action action)
		{
			LanTimer t = new LanTimer();
			t.Due = NowMs + (long)Math.Max(0.0, ms);
			t.Seq = Interlocked.Increment(ref seq);
			t.Callback = action;
			if (IsLoopThread)
			{
				timers.Add(t);
			}
			else
			{
				Post(delegate
				{
					timers.Add(t);
				});
			}
			return t;
		}

		private void Run()
		{
			while (running)
			{
				int wait = -1;
				if (timers.Count > 0)
				{
					long delta = timers.Min.Due - NowMs;
					wait = (int)Math.Max(0L, Math.Min(delta, int.MaxValue));
				}
				Action action;
				if (queue.TryTake(out action, wait))
				{
					Invoke(action);
				}
				long now = NowMs;
				while (timers.Count > 0 && timers.Min.Due <= now)
				{
					LanTimer t = timers.Min;
					timers.Remove(t);
					if (!t.Cancelled)
					{
						Invoke(t.Callback);
					}
				}
			}
			timers.Clear();
			Action leftover;
			while (queue.TryTake(out leftover))
			{
			}
		}

		private static void Invoke(Action action)
		{
			try
			{
				action();
			}
			catch (Exception)
			{
			}
		}
	}
}
#endif

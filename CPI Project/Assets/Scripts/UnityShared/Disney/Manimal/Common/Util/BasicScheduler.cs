using Disney.Manimal.Collections;
using Disney.Manimal.Common.Memory;
using System;
using System.Collections.Generic;

namespace Disney.Manimal.Common.Util
{
	public class BasicScheduler : IScheduler
	{
		private class ScheduleEvent : IComparable<ScheduleEvent>
		{
			public int Id
			{
				get;
				set;
			}

			public TimeSpan Delay
			{
				get;
				set;
			}

			public long StartTime
			{
				get;
				set;
			}

			public long EndTime
			{
				get
				{
					return StartTime + Delay.Ticks;
				}
			}

			public bool Repeat
			{
				get;
				set;
			}

			public bool Cancelled
			{
				get;
				set;
			}

			public ScheduleCallback Callback
			{
				get;
				set;
			}

			public object State
			{
				get;
				set;
			}

			public int CompareTo(ScheduleEvent other)
			{
				int num = EndTime.CompareTo(other.EndTime);
				if (num == 0)
				{
					num = Id.CompareTo(other.Id);
				}
				return num;
			}
		}

		private readonly TimeSpan MinDelay = TimeSpan.FromMilliseconds(1.0);

		private readonly TimeSpan MaxDelay = TimeSpan.FromHours(2.0);

		private readonly MinHeap<ScheduleEvent> _heap;

		private readonly Dictionary<int, ScheduleEvent> _lookup;

		private readonly List<ScheduleEvent> _repeat;

		private readonly BasicPool<ScheduleEvent> _pool;

		private int _eventSequence;

		private long _timeNow;

		public BasicScheduler()
		{
			_heap = new MinHeap<ScheduleEvent>();
			_lookup = new Dictionary<int, ScheduleEvent>();
			_repeat = new List<ScheduleEvent>();
			_pool = new BasicPool<ScheduleEvent>(() => new ScheduleEvent(), delegate(ScheduleEvent item)
			{
				item.Callback = null;
				item.State = null;
			});
			_eventSequence = 1;
			_timeNow = 0L;
		}

		public int Schedule(TimeSpan delay, bool repeating, ScheduleCallback callback, object state)
		{
			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}
			delay = TimeSpan.FromTicks(Math.Max(MinDelay.Ticks, Math.Min(MaxDelay.Ticks, delay.Ticks)));
			ScheduleEvent scheduleEvent = _pool.Acquire();
			scheduleEvent.Id = _eventSequence++;
			scheduleEvent.Delay = delay;
			scheduleEvent.StartTime = _timeNow;
			scheduleEvent.Repeat = repeating;
			scheduleEvent.Cancelled = false;
			scheduleEvent.Callback = callback;
			scheduleEvent.State = state;
			_heap.Add(scheduleEvent);
			_lookup.Add(scheduleEvent.Id, scheduleEvent);
			return scheduleEvent.Id;
		}

		public void Cancel(int id)
		{
			ScheduleEvent value;
			if (_lookup.TryGetValue(id, out value))
			{
				value.Cancelled = true;
			}
		}

		public void CancelAll()
		{
			_heap.ForEach(delegate(ScheduleEvent s)
			{
				s.Cancelled = true;
			});
		}

		public bool IsPending(int id)
		{
			ScheduleEvent value;
			if (_lookup.TryGetValue(id, out value))
			{
				return !value.Cancelled;
			}
			return false;
		}

		public TimeSpan GetTimeLeft(int id)
		{
			ScheduleEvent value;
			if (_lookup.TryGetValue(id, out value))
			{
				return value.Cancelled ? TimeSpan.Zero : TimeSpan.FromTicks(value.EndTime - _timeNow);
			}
			return TimeSpan.Zero;
		}

		public TimeSpan GetTimeSinceStart(int id)
		{
			ScheduleEvent value;
			if (_lookup.TryGetValue(id, out value))
			{
				return value.Cancelled ? TimeSpan.Zero : TimeSpan.FromTicks(_timeNow - value.StartTime);
			}
			return TimeSpan.Zero;
		}

		public TimeSpan GetDuration(int id)
		{
			ScheduleEvent value;
			if (_lookup.TryGetValue(id, out value))
			{
				return value.Cancelled ? TimeSpan.Zero : value.Delay;
			}
			return TimeSpan.Zero;
		}

		public void AdvanceTime(TimeSpan delta)
		{
			_timeNow += delta.Ticks;
			while (_heap.Count > 0 && _heap.Peek().EndTime <= _timeNow)
			{
				ScheduleEvent scheduleEvent = _heap.ExtractMin();
				_lookup.Remove(scheduleEvent.Id);
				if (!scheduleEvent.Cancelled)
				{
					scheduleEvent.Callback(scheduleEvent.Id, scheduleEvent.State);
					if (!scheduleEvent.Cancelled)
					{
						scheduleEvent.Cancelled = !scheduleEvent.Repeat;
						if (scheduleEvent.Repeat)
						{
							_repeat.Add(scheduleEvent);
						}
					}
				}
				if (scheduleEvent.Cancelled)
				{
					_pool.Release(scheduleEvent);
				}
			}
			_repeat.ForEach(delegate(ScheduleEvent s)
			{
				s.StartTime = _timeNow;
				_heap.Add(s);
				_lookup.Add(s.Id, s);
			});
			_repeat.Clear();
		}
	}
}

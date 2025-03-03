using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class NotificationQueue : INotificationQueue
	{
		private readonly List<BaseNotification> queue;

		private readonly List<BaseNotification> prePauseQueue;

		private readonly INotificationDispatcher dispatcher;

		private bool isPaused;

		public bool IsPaused
		{
			get
			{
				return isPaused;
			}
			set
			{
				isPaused = value;
				if (isPaused)
				{
					InsertAllSorted(queue, prePauseQueue);
					queue.Clear();
				}
				else
				{
					ClearAndCallBackPrePauseQueue();
					DispatchQueue();
				}
			}
		}

		public long LatestSequenceNumber
		{
			get;
			set;
		}

		public IEnumerable<long> SequenceNumbers
		{
			get
			{
				return queue.Select((BaseNotification n) => n.SequenceNumber.Value);
			}
		}

		private event Action OnCleared = delegate
		{
		};

		public event Action<long> OnQueued = delegate
		{
		};

		public event Action<long> OnDispatched = delegate
		{
		};

		public NotificationQueue(INotificationDispatcher dispatcher)
		{
			this.dispatcher = dispatcher;
			queue = new List<BaseNotification>();
			prePauseQueue = new List<BaseNotification>();
		}

		public void Dispatch(BaseNotification notification, Action successCallback, Action failureCallback)
		{
			if (notification.SequenceNumber <= LatestSequenceNumber)
			{
				successCallback();
			}
			else if (ShouldDispatchNotification(notification))
			{
				this.OnQueued(notification.SequenceNumber.Value);
				DispatchNotification(notification);
				DispatchQueue();
				successCallback();
			}
			else
			{
				QueueNotification(notification);
				WaitForDispatchOrClear(notification.SequenceNumber.Value, successCallback, failureCallback);
			}
		}

		public void Dispatch(IEnumerable<BaseNotification> notifications, Action successCallback, Action failureCallback)
		{
			BaseNotification[] array = notifications.OrderBy((BaseNotification x) => x.SequenceNumber).ToArray();
			int count = array.Length;
			if (count == 0)
			{
				successCallback();
				return;
			}
			BaseNotification[] array2 = array;
			foreach (BaseNotification notification in array2)
			{
				Dispatch(notification, delegate
				{
					count--;
					if (count == 0)
					{
						successCallback();
					}
				}, failureCallback);
			}
		}

		public void Clear()
		{
			queue.Clear();
			prePauseQueue.Clear();
			this.OnCleared();
		}

		public bool IsQueued(long sequenceNumber)
		{
			return queue.Any((BaseNotification q) => q.SequenceNumber == sequenceNumber);
		}

		private void ClearAndCallBackPrePauseQueue()
		{
			while (prePauseQueue.Count > 0)
			{
				BaseNotification baseNotification = prePauseQueue[0];
				prePauseQueue.RemoveAt(0);
				this.OnDispatched(baseNotification.SequenceNumber.Value);
			}
		}

		private void DispatchQueue()
		{
			while (queue.Count > 0 && ShouldDispatchNotification(queue[0]))
			{
				BaseNotification notification = queue[0];
				queue.RemoveAt(0);
				DispatchNotification(notification);
			}
		}

		private static int CompareNotificationsBySequenceNumber(BaseNotification a, BaseNotification b)
		{
			return (b.SequenceNumber > a.SequenceNumber) ? (-1) : ((b.SequenceNumber < a.SequenceNumber) ? 1 : 0);
		}

		private bool ShouldDispatchNotification(BaseNotification notification)
		{
			long? sequenceNumber = notification.SequenceNumber;
			return !IsPaused && sequenceNumber.HasValue && sequenceNumber.Value == LatestSequenceNumber + 1;
		}

		private void QueueNotification(BaseNotification notification)
		{
			queue.InsertIntoSortedList(notification, CompareNotificationsBySequenceNumber);
			this.OnQueued(notification.SequenceNumber.Value);
		}

		private void InsertAllSorted(IEnumerable<BaseNotification> from, IList<BaseNotification> to)
		{
			foreach (BaseNotification item in from)
			{
				to.InsertIntoSortedList(item, CompareNotificationsBySequenceNumber);
			}
		}

		private void DispatchNotification(BaseNotification notification)
		{
			LatestSequenceNumber = notification.SequenceNumber.Value;
			dispatcher.Dispatch(notification);
			this.OnDispatched(LatestSequenceNumber);
		}

		private void WaitForDispatchOrClear(long sequenceNumber, Action successCallback, Action failureCallback)
		{
			Action handleCleared = null;
			Action<long> handleDispatched = null;
			handleCleared = delegate
			{
				OnDispatched -= handleDispatched;
				OnCleared -= handleCleared;
				failureCallback();
			};
			handleDispatched = delegate(long n)
			{
				if (n == sequenceNumber)
				{
					OnDispatched -= handleDispatched;
					OnCleared -= handleCleared;
					successCallback();
				}
			};
			OnDispatched += handleDispatched;
			OnCleared += handleCleared;
		}
	}
}

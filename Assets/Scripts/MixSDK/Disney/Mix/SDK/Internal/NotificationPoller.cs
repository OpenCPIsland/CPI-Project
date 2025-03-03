using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class NotificationPoller : INotificationPoller, IDisposable
	{
		private readonly AbstractLogger logger;

		private readonly IMixWebCallFactory webCallFactory;

		private readonly IStopwatch pollCountdownStopwatch;

		private readonly IGetStateResponseParser getStateResponseParser;

		private readonly IEpochTime epochTime;

		private readonly IRandom random;

		private readonly IDatabase database;

		private readonly string localUserSwid;

		private readonly INotificationQueue queue;

		private readonly Dictionary<long, long> missedNotificationTimes;

		private int[] pollIntervals;

		private int[] pokeIntervals;

		private bool usePollIntervals;

		private int intervalIndex;

		private int intervalTime;

		private bool isPaused;

		private bool isDisposed;

		private bool isPolling;

		private bool needPollAgain;

		private int jitter;

		public IEnumerable<int> PollIntervals
		{
			get
			{
				return pollIntervals;
			}
			set
			{
				pollIntervals = value.ToArray();
			}
		}

		public IEnumerable<int> PokeIntervals
		{
			get
			{
				return pokeIntervals;
			}
			set
			{
				pokeIntervals = value.ToArray();
			}
		}

		public int MaximumMissingNotificationTime
		{
			private get;
			set;
		}

		public int Jitter
		{
			get
			{
				return jitter;
			}
			set
			{
				jitter = value;
				IntervalIndex = intervalIndex;
			}
		}

		public bool UsePollIntervals
		{
			get
			{
				return usePollIntervals;
			}
			set
			{
				usePollIntervals = value;
				IntervalIndex = 0;
			}
		}

		private int[] CurIntervals
		{
			get
			{
				return UsePollIntervals ? pollIntervals : pokeIntervals;
			}
		}

		private int IntervalIndex
		{
			get
			{
				return intervalIndex;
			}
			set
			{
				intervalIndex = value;
				intervalTime = ApplyJitter(CurIntervals[intervalIndex], jitter, random);
			}
		}

		private bool IsNotificationMissingTooLong
		{
			get
			{
				Dictionary<long, long>.ValueCollection values = missedNotificationTimes.Values;
				return values.Any() && epochTime.Milliseconds - values.Max() > MaximumMissingNotificationTime;
			}
		}

		private bool IsPollCountdownComplete
		{
			get
			{
				return pollCountdownStopwatch.ElapsedMilliseconds > intervalTime;
			}
		}

		public event EventHandler<AbstractNotificationsPolledEventArgs> OnNotificationsPolled = delegate
		{
		};

		public event EventHandler<AbstractNotificationPollerSynchronizationErrorEventArgs> OnSynchronizationError = delegate
		{
		};

		public NotificationPoller(AbstractLogger logger, IMixWebCallFactory webCallFactory, INotificationQueue queue, IStopwatch pollCountdownStopwatch, IGetStateResponseParser getStateResponseParser, IEpochTime epochTime, IRandom random, IDatabase database, string localUserSwid)
		{
			usePollIntervals = true;
			missedNotificationTimes = new Dictionary<long, long>();
			isPaused = true;
			this.logger = logger;
			this.webCallFactory = webCallFactory;
			this.queue = queue;
			this.pollCountdownStopwatch = pollCountdownStopwatch;
			this.getStateResponseParser = getStateResponseParser;
			this.epochTime = epochTime;
			this.random = random;
			this.database = database;
			this.localUserSwid = localUserSwid;
			PollIntervals = new int[1]
			{
				2147483647
			};
			PokeIntervals = new int[1]
			{
				2147483647
			};
			MaximumMissingNotificationTime = int.MaxValue;
			Jitter = 0;
			IntervalIndex = 0;
			queue.OnQueued += HandleQueued;
			queue.OnDispatched += HandleQueueDispatched;
		}

		private void HandleQueued(long seqNum)
		{
			missedNotificationTimes.Remove(seqNum);
		}

		private void HandleQueueDispatched(long seqNum)
		{
			database.UpdateSessionDocument(localUserSwid, delegate(SessionDocument doc)
			{
				doc.LatestNotificationSequenceNumber = seqNum;
			});
		}

		public void Pause()
		{
			if (!isPaused)
			{
				isPaused = true;
				pollCountdownStopwatch.Stop();
			}
		}

		public void Resume()
		{
			if (isPaused)
			{
				isPaused = false;
				pollCountdownStopwatch.Start();
				PollAgain();
			}
		}

		public void Update()
		{
			if (!isPolling && !isPaused && !isDisposed && IsPollCountdownComplete)
			{
				Poll();
			}
		}

		public void RequestPoll()
		{
			if (!isPaused && !isDisposed)
			{
				if (isPolling)
				{
					needPollAgain = true;
				}
				else
				{
					Poll();
				}
			}
		}

		public void Dispose()
		{
			isDisposed = true;
			this.OnNotificationsPolled = null;
		}

		private void PollAgain()
		{
			if (needPollAgain)
			{
				needPollAgain = false;
				RequestPoll();
			}
		}

		private void Poll()
		{
			isPolling = true;
			pollCountdownStopwatch.Reset();
			pollCountdownStopwatch.Start();
			long value = queue.LatestSequenceNumber + 1;
			List<long> source = queue.SequenceNumbers.ToList();
			List<long?> excludeNotificationSequenceNumbers = source.Cast<long?>().ToList();
			GetNotificationsSinceSequenceRequest getNotificationsSinceSequenceRequest = new GetNotificationsSinceSequenceRequest();
			getNotificationsSinceSequenceRequest.SequenceNumber = value;
			getNotificationsSinceSequenceRequest.ExcludeNotificationSequenceNumbers = excludeNotificationSequenceNumbers;
			GetNotificationsSinceSequenceRequest request = getNotificationsSinceSequenceRequest;
			IWebCall<GetNotificationsSinceSequenceRequest, GetNotificationsResponse> webCall = webCallFactory.NotificationsSinceSequencePost(request);
			webCall.OnResponse += delegate(object sender, WebCallEventArgs<GetNotificationsResponse> e)
			{
				HandleGetNotificationsResponse(e.Response);
			};
			webCall.OnError += HandleGetNotificationsError;
			webCall.Execute();
		}

		private void HandleGetNotificationsResponse(GetNotificationsResponse response)
		{
			isPolling = false;
			if (isDisposed)
			{
				return;
			}
			if (!Validate(response))
			{
				logger.Critical("Invalid notifications response:\n" + JsonParser.ToJson(response));
				return;
			}
			long? lastNotificationTimestamp = response.LastNotificationTimestamp;
			List<BaseNotification> list = getStateResponseParser.CollectNotifications(response);
			RemoveMissingNotificationTimes(list, missedNotificationTimes);
			RemoveInvalidNotifications(list, queue);
			SortNotificationsBySequenceNumber(list);
			long value = response.NotificationSequenceCounter.Value;
			long requestedSince = queue.LatestSequenceNumber + 1;
			List<long> queued = queue.SequenceNumbers.ToList();
			IEnumerable<long> missingSequenceNumbers = GetMissingSequenceNumbers(requestedSince, queued, list, value);
			long milliseconds = epochTime.Milliseconds;
			AddTimes(missingSequenceNumbers, missedNotificationTimes, milliseconds);
			bool anyMissing = missedNotificationTimes.Count > 0;
			bool anyReceived = list.Count > 0;
			int numIntervals = CurIntervals.Length;
			IntervalIndex = GetNextIntervalIndex(IntervalIndex, numIntervals, anyMissing, anyReceived);
			queue.Dispatch(list, delegate
			{
			}, delegate
			{
			});
			DispatchOnNotificationsPolledIfNotNull(lastNotificationTimestamp);
			PollAgain();
			if (IsNotificationMissingTooLong)
			{
				Pause();
				missedNotificationTimes.Clear();
				DispatchOnSynchronizationError();
			}
		}

		private void HandleGetNotificationsError(object sender, WebCallErrorEventArgs e)
		{
			isPolling = false;
			IntervalIndex = 0;
		}

		private void DispatchOnSynchronizationError()
		{
			this.OnSynchronizationError(this, new NotificationPollerSynchronizationErrorEventArgs());
		}

		private void DispatchOnNotificationsPolledIfNotNull(long? timestamp)
		{
			if (timestamp.HasValue)
			{
				this.OnNotificationsPolled(this, new NotificationsPolledEventArgs(timestamp.Value));
			}
		}

		private static void AddTimes(IEnumerable<long> missing, IDictionary<long, long> times, long now)
		{
			foreach (long item in missing)
			{
				if (!times.ContainsKey(item))
				{
					times.Add(item, now);
				}
			}
		}

		private static IEnumerable<long> EnumerableRangeLong(long from, long to)
		{
			for (long i = from; i <= to; i++)
			{
				yield return i;
			}
		}

		private static IEnumerable<long> GetMissingSequenceNumbers(long requestedSince, IEnumerable<long> queued, IEnumerable<BaseNotification> returned, long serverSequenceCounter)
		{
			return EnumerableRangeLong(requestedSince, serverSequenceCounter).Except(queued).Except(returned.Select((BaseNotification n) => n.SequenceNumber.Value));
		}

		private static bool Validate(GetNotificationsResponse response)
		{
			return response.NotificationSequenceCounter.HasValue;
		}

		private static int GetNextIntervalIndex(int intervalIndex, int numIntervals, bool anyMissing, bool anyReceived)
		{
			return (!anyMissing && !anyReceived) ? Math.Min(numIntervals - 1, intervalIndex + 1) : 0;
		}

		private static void RemoveMissingNotificationTimes(IEnumerable<BaseNotification> notifications, IDictionary<long, long> missedNotificationTimes)
		{
			foreach (BaseNotification notification in notifications)
			{
				missedNotificationTimes.Remove(notification.SequenceNumber.Value);
			}
		}

		private static void RemoveInvalidNotifications(List<BaseNotification> notifications, INotificationQueue queue)
		{
			notifications.RemoveAll((BaseNotification n) => n.SequenceNumber <= queue.LatestSequenceNumber || queue.IsQueued(n.SequenceNumber.Value));
		}

		private static void SortNotificationsBySequenceNumber(List<BaseNotification> notifications)
		{
			notifications.Sort((BaseNotification a, BaseNotification b) => (b.SequenceNumber > a.SequenceNumber) ? (-1) : ((b.SequenceNumber < a.SequenceNumber) ? 1 : 0));
		}

		private static int ApplyJitter(int val, int jitter, IRandom random)
		{
			return val + random.Next(jitter) - jitter / 2;
		}
	}
}

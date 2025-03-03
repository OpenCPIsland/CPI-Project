using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public interface INotificationQueue
	{
		bool IsPaused
		{
			get;
			set;
		}

		long LatestSequenceNumber
		{
			get;
			set;
		}

		IEnumerable<long> SequenceNumbers
		{
			get;
		}

		event Action<long> OnQueued;

		event Action<long> OnDispatched;

		void Dispatch(BaseNotification notification, Action successCallback, Action failureCallback);

		void Dispatch(IEnumerable<BaseNotification> notifications, Action successCallback, Action failureCallback);

		void Clear();

		bool IsQueued(long sequenceNumber);
	}
}

using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class CoinsForChangeTracker : MonoBehaviour
	{
		[Range(5f, 60f)]
		public int UpdateIntervalInSeconds = 30;

		private long currentCoinCount = 0L;

		public Action<long> OnCoinCountUpdated;

		private EventDispatcher dispatcher;

		private IScheduledEventService scheduledEventService;

		private Timer updateTimer;

		public long CurrentCoinCount
		{
			get
			{
				return currentCoinCount;
			}
		}

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			scheduledEventService = Service.Get<INetworkServicesManager>().ScheduledEventService;
			dispatcher.AddListener<ScheduledEventServiceEvents.CFCDonationPosted>(onDonationPosted);
			dispatcher.AddListener<ScheduledEventServiceEvents.CFCDonationsLoaded>(onDonationLoaded);
			updateTimer = new Timer(UpdateIntervalInSeconds, true, delegate
			{
				onTimerTick();
			});
			CoroutineRunner.Start(updateTimer.Start(), this, "CFCCounterUpdate");
			scheduledEventService.GetCFCDonations();
		}

		private void OnDestroy()
		{
			dispatcher.RemoveListener<ScheduledEventServiceEvents.CFCDonationPosted>(onDonationPosted);
			dispatcher.RemoveListener<ScheduledEventServiceEvents.CFCDonationsLoaded>(onDonationLoaded);
			CoroutineRunner.StopAllForOwner(this);
		}

		private void onTimerTick()
		{
			scheduledEventService.GetCFCDonations();
		}

		private bool onDonationPosted(ScheduledEventServiceEvents.CFCDonationPosted evt)
		{
			currentCoinCount = evt.DonationResult.cfcTotal;
			if (OnCoinCountUpdated != null)
			{
				OnCoinCountUpdated(currentCoinCount);
			}
			return false;
		}

		private bool onDonationLoaded(ScheduledEventServiceEvents.CFCDonationsLoaded evt)
		{
			currentCoinCount = evt.CFCDonations.cfcTotal;
			if (OnCoinCountUpdated != null)
			{
				OnCoinCountUpdated(currentCoinCount);
			}
			return false;
		}
	}
}

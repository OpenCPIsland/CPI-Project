using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.SledRacer
{
	[DisallowMultipleComponent]
	public class RaceGameController : MonoBehaviour
	{
		public long CompletionTime = 0L;

		public RaceResults.RaceResultsCategory RaceResultsCategory = RaceResults.RaceResultsCategory.Incomplete;

		private EventDispatcher dispatcher;

		private CPDataEntityCollection dataEntityCollection;

		private RaceData localPlayerRaceData;

		private RaceResults raceResults;

		private Timer updateTimer;

		private long launchTime;

		private GameObject raceGate;

		private GameObject lite1a;

		private GameObject lite2a;

		private GameObject lite3a;

		private GameObject lite1b;

		private GameObject lite2b;

		private GameObject lite3b;

		private long silverTimeMS;

		private long goldTimeMS;

		private long legendaryTimeMS;

		private string trackId;

		private int tolerance = 100;

		public string TrackId
		{
			get
			{
				return trackId;
			}
		}

		private void Start()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			dispatcher.DispatchEvent(default(RaceGameEvents.Start));
			DataEntityHandle localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
			if (dataEntityCollection.TryGetComponent(localPlayerHandle, out localPlayerRaceData))
			{
				CompletionTime = localPlayerRaceData.RaceResults.CompletionTime;
				RaceResultsCategory = localPlayerRaceData.RaceResults.raceResultsCategory;
				return;
			}
			localPlayerRaceData = dataEntityCollection.AddComponent<RaceData>(localPlayerHandle);
			RaceResults raceResults = new RaceResults();
			raceResults.CompletionTime = 0L;
			raceResults.raceResultsCategory = RaceResults.RaceResultsCategory.Incomplete;
			localPlayerRaceData.RaceResults = raceResults;
		}

		private void OnDestroy()
		{
			CoroutineRunner.StopAllForOwner(this);
		}

		public void InitializeRace(GameObject raceGate, GameObject lite1a, GameObject lite2a, GameObject lite3a, GameObject lite1b, GameObject lite2b, GameObject lite3b, long silverTimeMS, long goldTimeMS, long legendaryTimeMS, string trackId)
		{
			this.raceGate = raceGate;
			this.lite1a = lite1a;
			this.lite2a = lite2a;
			this.lite3a = lite3a;
			this.lite1b = lite1b;
			this.lite2b = lite2b;
			this.lite3b = lite3b;
			this.silverTimeMS = silverTimeMS;
			this.goldTimeMS = goldTimeMS;
			this.legendaryTimeMS = legendaryTimeMS;
			this.trackId = trackId;
		}

		public void StartRace()
		{
			updateTimerLights(3000L);
			launchTime = Service.Get<INetworkServicesManager>().GameTimeMilliseconds + 3000;
			updateTimer = new Timer(1f, true, delegate
			{
				onTimerTick();
			});
			CoroutineRunner.Start(updateTimer.Start(), this, "RaceStartTimer");
			Service.Get<ICPSwrveService>().Action("tube_race_start", trackId);
		}

		public void FinishRace()
		{
			long num = Service.Get<INetworkServicesManager>().GameTimeMilliseconds;
			RaceResults.RaceResultsCategory raceResultsCategory = RaceResults.RaceResultsCategory.Bronze;
			if (localPlayerRaceData.RaceResults.raceResultsCategory == RaceResults.RaceResultsCategory.Incomplete)
			{
				localPlayerRaceData.RaceResults.CompletionTime = num;
				num -= localPlayerRaceData.RaceResults.StartTime;
				raceResultsCategory = ((num <= legendaryTimeMS) ? RaceResults.RaceResultsCategory.Legendary : ((num <= goldTimeMS) ? RaceResults.RaceResultsCategory.Gold : ((num > silverTimeMS) ? RaceResults.RaceResultsCategory.Bronze : RaceResults.RaceResultsCategory.Silver)));
				localPlayerRaceData.RaceResults.trackId = trackId;
			}
			localPlayerRaceData.RaceResults.raceResultsCategory = raceResultsCategory;
			long[] rankTimes = new long[3]
			{
				silverTimeMS,
				goldTimeMS,
				legendaryTimeMS
			};
			dispatcher.DispatchEvent(new RaceGameEvents.RaceFinished(localPlayerRaceData.RaceResults, rankTimes));
			Service.Get<ICPSwrveService>().Action("tube_race_finish", localPlayerRaceData.RaceResults.trackId, num.ToString());
		}

		public void ActivateGate()
		{
			raceGate.SetActive(true);
		}

		public void RemoveLocalPlayerRaceData()
		{
			DataEntityHandle localPlayerHandle = dataEntityCollection.LocalPlayerHandle;
			dataEntityCollection.RemoveComponent<RaceData>(localPlayerHandle);
		}

		private void onTimerTick()
		{
			long gameTimeMilliseconds = Service.Get<INetworkServicesManager>().GameTimeMilliseconds;
			long num = launchTime - gameTimeMilliseconds;
			updateTimerLights(num);
			if (num < 0)
			{
				raceGate.SetActive(false);
				updateTimer.Stop();
				localPlayerRaceData.RaceResults.StartTime = Service.Get<INetworkServicesManager>().GameTimeMilliseconds;
				dispatcher.DispatchEvent(default(RaceGameEvents.Launch));
				CoroutineRunner.StopAllForOwner(this);
			}
		}

		private void updateTimerLights(long timeLeft)
		{
			if (lite1a != null && lite2a != null && lite3a != null && lite1b != null && lite2b != null && lite3b != null)
			{
				if (timeLeft == 3000)
				{
					lite1a.SetActive(false);
					lite1b.SetActive(false);
					lite2a.SetActive(false);
					lite2b.SetActive(false);
					lite3a.SetActive(false);
					lite3b.SetActive(false);
				}
				else if (Math.Abs(timeLeft - 2000) < tolerance)
				{
					lite1a.SetActive(true);
					lite1b.SetActive(true);
					lite2a.SetActive(false);
					lite2b.SetActive(false);
					lite3a.SetActive(false);
					lite3b.SetActive(false);
				}
				else if (Math.Abs(timeLeft - 1000) < tolerance)
				{
					lite1a.SetActive(true);
					lite1b.SetActive(true);
					lite2a.SetActive(true);
					lite2b.SetActive(true);
					lite3a.SetActive(false);
					lite3b.SetActive(false);
				}
				else
				{
					lite1a.SetActive(true);
					lite1b.SetActive(true);
					lite2a.SetActive(true);
					lite2b.SetActive(true);
					lite3a.SetActive(true);
					lite3b.SetActive(true);
				}
			}
			else
			{
				Log.LogError(this, "Race starting timer lite is null");
			}
		}
	}
}

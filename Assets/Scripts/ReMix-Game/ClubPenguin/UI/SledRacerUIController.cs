using ClubPenguin.SledRacer;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class SledRacerUIController : MonoBehaviour
	{
		private PrefabContentKey raceFinishPopupPrefabKey = new PrefabContentKey("Prefabs/TubeRaceScreen");

		public SledRaceLeaderboard BlueLeaderboard;

		public SledRaceLeaderboard RedLeaderboard;

		private RaceResults lastRaceResults;

		private long[] rankTimes;

		private void Start()
		{
		}

		private void OnDestroy()
		{
		}

		private bool onRaceFinished(RaceGameEvents.RaceFinished evt)
		{
			lastRaceResults = evt.RaceResults;
			rankTimes = evt.RankTimes;
			showEndRaceScreen();
			if (evt.RaceResults.trackId == "blue")
			{
				BlueLeaderboard.ShowFinishScreen(evt.RaceResults);
			}
			else
			{
				RedLeaderboard.ShowFinishScreen(evt.RaceResults);
			}
			return false;
		}

		private void showEndRaceScreen()
		{
			Content.LoadAsync(onRaceFinishPopupLoaded, raceFinishPopupPrefabKey);
		}

		private void onRaceFinishPopupLoaded(string path, GameObject popup)
		{
			GameObject gameObject = Object.Instantiate(popup);
			gameObject.GetComponent<SledRacerFinishPopup>().Initialize(lastRaceResults, rankTimes);
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(gameObject));
		}
	}
}

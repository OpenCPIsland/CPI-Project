using ClubPenguin.PartyGames;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class TubeRaceEventsMediator : MonoBehaviour
	{
		public GameObject RaceTrigger;

		public PartyGameDefinition.GameTypes RaceType;

		public TubeRaceScoreModifierLayout[] ScoreModifierLayouts;

		private EventChannel eventChannel;

		private void Start()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<TubeRaceEvents.SetModifierLayout>(onSetModifierLayout);
			eventChannel.AddListener<TubeRaceEvents.RaceStart>(onRaceStart);
			eventChannel.AddListener<TubeRaceEvents.RaceEnd>(onRaceEnd);
			RaceTrigger.SetActive(false);
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			CoroutineRunner.StopAllForOwner(this);
		}

		private bool onRaceStart(TubeRaceEvents.RaceStart evt)
		{
			if (evt.RaceType == RaceType)
			{
				CoroutineRunner.Start(startRaceCoroutine(RaceTrigger), this, "startRaceCoroutine");
			}
			return false;
		}

		private bool onRaceEnd(TubeRaceEvents.RaceEnd evt)
		{
			if (evt.RaceType == RaceType)
			{
				deactivateScoreModifierLayouts();
			}
			return false;
		}

		private bool onSetModifierLayout(TubeRaceEvents.SetModifierLayout evt)
		{
			if (evt.RaceType == RaceType)
			{
				activateScoreModifierLayout(evt.ScoreModifierLayoutId);
			}
			return false;
		}

		private IEnumerator startRaceCoroutine(GameObject raceTrigger)
		{
			raceTrigger.SetActive(true);
			yield return new WaitForSeconds(1f);
			raceTrigger.SetActive(false);
		}

		private void activateScoreModifierLayout(int scoreModifierLayoutId)
		{
			int num = 0;
			while (true)
			{
				if (num < ScoreModifierLayouts.Length)
				{
					if (ScoreModifierLayouts[num].LayoutId == scoreModifierLayoutId)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			ScoreModifierLayouts[num].Activate();
		}

		private void deactivateScoreModifierLayouts()
		{
			for (int i = 0; i < ScoreModifierLayouts.Length; i++)
			{
				ScoreModifierLayouts[i].Deactivate();
			}
		}
	}
}

using ClubPenguin.PartyGames;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class TubeRaceLeaderBoard : MonoBehaviour
	{
		public float TimeToShowFinishScreenInSeconds;

		public PartyGameDefinition GameDefinition;

		public TubeRaceLeaderboardItem[] LeaderBoardItems;

		public GameObject DefaultScreen;

		public GameObject FinishScreen;

		private void Start()
		{
			Service.Get<EventDispatcher>().AddListener<TubeRaceEvents.EndGameResultsReceived>(onEndGameResultsReceived);
			showDefaultScreen();
		}

		private bool onEndGameResultsReceived(TubeRaceEvents.EndGameResultsReceived evt)
		{
			if (evt.PartyGameId == GameDefinition.Id && evt.PlayerResults.Count > 1)
			{
				CoroutineRunner.StopAllForOwner(this);
				CoroutineRunner.Start(showRaceResults(evt), this, "showFinishScreen");
			}
			return false;
		}

		private IEnumerator showRaceResults(TubeRaceEvents.EndGameResultsReceived results)
		{
			FinishScreen.SetActive(true);
			DefaultScreen.SetActive(false);
			List<long> sortedSessionIds = new List<long>(results.PlayerIdToPlacement.Keys);
			sortedSessionIds.Sort((long o1, long o2) => results.PlayerIdToPlacement[o1].CompareTo(results.PlayerIdToPlacement[o2]));
			for (int i = 0; i < LeaderBoardItems.Length; i++)
			{
				if (i < sortedSessionIds.Count)
				{
					PartyGameSessionMessages.TubeRacePlayerResult resultBySessionId = getResultBySessionId(results.PlayerResults, sortedSessionIds[i]);
					if (resultBySessionId != null)
					{
						LeaderBoardItems[i].SetData(sortedSessionIds[i], (int)resultBySessionId.OverallScore);
						LeaderBoardItems[i].gameObject.SetActive(true);
					}
					else
					{
						LeaderBoardItems[i].gameObject.SetActive(false);
					}
				}
				else
				{
					LeaderBoardItems[i].gameObject.SetActive(false);
				}
			}
			yield return new WaitForSeconds(TimeToShowFinishScreenInSeconds);
			showDefaultScreen();
		}

		private void showDefaultScreen()
		{
			FinishScreen.SetActive(false);
			DefaultScreen.SetActive(true);
		}

		private PartyGameSessionMessages.TubeRacePlayerResult getResultBySessionId(List<PartyGameSessionMessages.TubeRacePlayerResult> results, long sessionId)
		{
			PartyGameSessionMessages.TubeRacePlayerResult result = null;
			for (int i = 0; i < results.Count; i++)
			{
				if (results[i].PlayerId == sessionId)
				{
					result = results[i];
					break;
				}
			}
			return result;
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<TubeRaceEvents.EndGameResultsReceived>(onEndGameResultsReceived);
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}

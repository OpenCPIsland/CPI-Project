using ClubPenguin.Adventure;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Game.PartyGames;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using ClubPenguin.SledRacer;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.PartyGames
{
	public class TubeRaceEndGamePopup : MonoBehaviour
	{
		private enum PopupState
		{
			Uninitialized,
			AwaitingResults,
			AwaitingClaim,
			Complete
		}

		private const string QUEST_EVENT_END_RACE = "Finish{0}Race";

		public Transform rowParentTransform;

		public TubeRaceEndGamePopupReward Reward;

		public GameObject CalculatingResultsPanel;

		public GameObject RewardPanel;

		public GameObject ClaimButtonPanel;

		public GameObject EndGameButtonPanel;

		public Animator ResultsAnimator;

		public string ResultsAnimatorTrigger;

		public List<TintSelector> TintSelectors;

		public List<SpriteSelector> SpriteSelectors;

		private readonly PrefabContentKey otherPlayerRowKey = new PrefabContentKey("Prefabs/RankPanel2");

		private readonly PrefabContentKey localPlayerRowKey = new PrefabContentKey("Prefabs/RankPanelSelected2");

		private readonly PrefabContentKey unfinishedRowKey = new PrefabContentKey("Prefabs/RankPanelUnfinished");

		private GameObject otherPlayerRowPrefab;

		private GameObject localPlayerRowPrefab;

		private GameObject unfinishedRowPrefab;

		private GameObject unfinishedRow;

		private TubeRaceDefinition tubeRaceDefinition;

		private PartyGameDefinition partyGameDefinition;

		private EventChannel eventChannel;

		private List<PartyGameSessionMessages.TubeRacePlayerResult> playerResults;

		private int gameSessionId;

		private PopupState currentState;

		private void Awake()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<TubeRaceEvents.PlayerResultReceived>(onPlayerResultReceived);
			eventChannel.AddListener<TubeRaceEvents.EndGameResultsReceived>(onEndGameResultsReceived);
		}

		private void Start()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("XButton"));
			Service.Get<EventDispatcher>().DispatchEvent(new RaceGameEvents.RaceFinishPopupOpened(string.Format("Finish{0}Race", tubeRaceDefinition.QuestEventIdentifier)));
		}

		public void SetInitialData(int gameSessionId, List<PartyGameSessionMessages.TubeRacePlayerResult> results, PartyGameDefinition partyGameDefinition, TubeRaceDefinition tubeRaceDefinition)
		{
			playerResults = new List<PartyGameSessionMessages.TubeRacePlayerResult>(results);
			this.gameSessionId = gameSessionId;
			this.partyGameDefinition = partyGameDefinition;
			this.tubeRaceDefinition = tubeRaceDefinition;
			skinForDefinition(partyGameDefinition);
			changeState(PopupState.Uninitialized);
			CoroutineRunner.Start(loadRowPrefabs(), this, "loadRowPrefabs");
		}

		public void OnClaimButtonClicked()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new RewardServiceEvents.ClaimDelayedReward(RewardSource.MINI_GAME, gameSessionId.ToString()));
			changeState(PopupState.Complete);
		}

		public void OnReplayButtonClicked()
		{
			jumpToRestartLocation();
		}

		private void jumpToRestartLocation()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			Vector3 raceRestartPosition = tubeRaceDefinition.RaceRestartPosition;
			RaceController component = localPlayerGameObject.GetComponent<RaceController>();
			if (component != null)
			{
				component.setControlsEnabled(true);
			}
			LocomotionTracker component2 = localPlayerGameObject.GetComponent<LocomotionTracker>();
			if (!component2.IsCurrentControllerOfType<RunController>())
			{
				component2.SetCurrentController<RunController>();
			}
			localPlayerGameObject.transform.position = raceRestartPosition;
			RaceGameController component3 = localPlayerGameObject.GetComponent<RaceGameController>();
			if (component3 != null)
			{
				component3.RemoveLocalPlayerRaceData();
				Object.Destroy(component3);
			}
			Service.Get<EventDispatcher>().DispatchEvent(new CinematographyEvents.CameraSnapLockEvent(false, true));
			CoroutineRunner.Start(LocomotionUtils.nudgePlayer(component2), component2.gameObject, "MoveAfterJump");
		}

		private IEnumerator loadRowPrefabs()
		{
			AssetRequest<GameObject> otherPlayerRowRequest = Content.LoadAsync(otherPlayerRowKey);
			yield return otherPlayerRowRequest;
			AssetRequest<GameObject> localPlayerRowRequest = Content.LoadAsync(localPlayerRowKey);
			yield return localPlayerRowRequest;
			AssetRequest<GameObject> unfinishedRowRequest = Content.LoadAsync(unfinishedRowKey);
			yield return unfinishedRowRequest;
			otherPlayerRowPrefab = otherPlayerRowRequest.Asset;
			localPlayerRowPrefab = localPlayerRowRequest.Asset;
			unfinishedRowPrefab = unfinishedRowRequest.Asset;
			showInitialData();
		}

		private void showInitialData()
		{
			playerResults.Sort((PartyGameSessionMessages.TubeRacePlayerResult o1, PartyGameSessionMessages.TubeRacePlayerResult o2) => o2.OverallScore.CompareTo(o1.OverallScore));
			for (int num = playerResults.Count - 1; num >= 0; num--)
			{
				loadPlayerRow(playerResults[num]);
			}
			unfinishedRow = Object.Instantiate(unfinishedRowPrefab, rowParentTransform, false);
			refreshPlacementText();
			if (currentState == PopupState.Uninitialized)
			{
				changeState(PopupState.AwaitingResults);
			}
		}

		internal void EndGame(Dictionary<long, int> playerIdToPlacement)
		{
			onEndGameResultsReceived(new TubeRaceEvents.EndGameResultsReceived(0, playerIdToPlacement, null));
		}

		private void loadPlayerRow(PartyGameSessionMessages.TubeRacePlayerResult result, int index = 0)
		{
			GameObject gameObject = (result.PlayerId != Service.Get<CPDataEntityCollection>().LocalPlayerSessionId) ? Object.Instantiate(otherPlayerRowPrefab, rowParentTransform, false) : Object.Instantiate(localPlayerRowPrefab, rowParentTransform, false);
			gameObject.transform.SetSiblingIndex(index);
			gameObject.GetComponent<TubeRaceEndGamePopupRow>().SetData(result, partyGameDefinition);
		}

		private bool onPlayerResultReceived(TubeRaceEvents.PlayerResultReceived evt)
		{
			if (currentState != 0)
			{
				int resultIndex = getResultIndex(evt.PlayerResult);
				playerResults.Insert(resultIndex, evt.PlayerResult);
				loadPlayerRow(evt.PlayerResult, resultIndex);
				refreshPlacementText();
			}
			else
			{
				playerResults.Add(evt.PlayerResult);
			}
			return false;
		}

		private int getResultIndex(PartyGameSessionMessages.TubeRacePlayerResult result)
		{
			int result2 = playerResults.Count;
			for (int i = 0; i < playerResults.Count; i++)
			{
				if (result.OverallScore > playerResults[i].OverallScore)
				{
					result2 = i;
					break;
				}
			}
			return result2;
		}

		private void refreshPlacementText()
		{
			for (int i = 0; i < rowParentTransform.childCount; i++)
			{
				TubeRaceEndGamePopupRow component = rowParentTransform.GetChild(i).GetComponent<TubeRaceEndGamePopupRow>();
				if (component != null)
				{
					component.ShowPlacementText(i + 1);
				}
			}
		}

		private bool onEndGameResultsReceived(TubeRaceEvents.EndGameResultsReceived evt)
		{
			Reward.ShowReward(evt.PlayerIdToPlacement, partyGameDefinition);
			Object.Destroy(unfinishedRow);
			changeState(PopupState.AwaitingClaim);
			return false;
		}

		private void changeState(PopupState newState)
		{
			currentState = newState;
			switch (newState)
			{
			case PopupState.Uninitialized:
			case PopupState.AwaitingResults:
				RewardPanel.SetActive(false);
				EndGameButtonPanel.SetActive(false);
				ClaimButtonPanel.SetActive(false);
				CalculatingResultsPanel.SetActive(true);
				break;
			case PopupState.AwaitingClaim:
				ResultsAnimator.SetTrigger(ResultsAnimatorTrigger);
				RewardPanel.SetActive(true);
				EndGameButtonPanel.SetActive(false);
				ClaimButtonPanel.SetActive(true);
				CalculatingResultsPanel.SetActive(false);
				break;
			case PopupState.Complete:
				RewardPanel.SetActive(true);
				EndGameButtonPanel.SetActive(true);
				ClaimButtonPanel.SetActive(false);
				CalculatingResultsPanel.SetActive(false);
				break;
			}
		}

		private void OnDestroy()
		{
			eventChannel.RemoveAllListeners();
			CoroutineRunner.StopAllForOwner(this);
			Service.Get<QuestService>().SendEvent(string.Format("Finish{0}Race", tubeRaceDefinition.QuestEventIdentifier));
		}

		private void skinForDefinition(PartyGameDefinition definition)
		{
			int index;
			switch (definition.Id)
			{
			case 4:
				index = 1;
				break;
			case 5:
				index = 0;
				break;
			default:
				index = 0;
				break;
			}
			for (int i = 0; i < TintSelectors.Count; i++)
			{
				TintSelectors[i].SelectColor(index);
			}
			for (int i = 0; i < SpriteSelectors.Count; i++)
			{
				SpriteSelectors[i].SelectSprite(index);
			}
		}
	}
}

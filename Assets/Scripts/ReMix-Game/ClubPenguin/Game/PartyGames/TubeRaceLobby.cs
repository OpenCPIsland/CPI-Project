using ClubPenguin.Adventure;
using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net;
using ClubPenguin.PartyGames;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class TubeRaceLobby : MonoBehaviour
	{
		private const string QUEST_EVENT_JOIN_LOBBY = "Join{0}Race";

		private const string QUEST_EVENT_EXIT_LOBBY = "Cancel{0}Race";

		public TubeRaceLobbyMmoItemObserver MmoItemObserver;

		public NetworkObjectController LobbyInteractionObject;

		public Transform LobbyPosition;

		public Transform LobbyExitPosition;

		public PartyGameDefinition GameDefinition;

		public TubeRaceDefinition RaceDefinition;

		private bool isLocalPlayerInLobby;

		private void Awake()
		{
			TubeRaceLobbyMmoItemObserver mmoItemObserver = MmoItemObserver;
			mmoItemObserver.LobbyStartedAction = (Action<long>)Delegate.Combine(mmoItemObserver.LobbyStartedAction, new Action<long>(onLobbyStarted));
			TubeRaceLobbyMmoItemObserver mmoItemObserver2 = MmoItemObserver;
			mmoItemObserver2.LobbyPlayersUpdatedAction = (Action<PartyGamePlayerCollection>)Delegate.Combine(mmoItemObserver2.LobbyPlayersUpdatedAction, new Action<PartyGamePlayerCollection>(onLobbyPlayersUpdated));
			TubeRaceLobbyMmoItemObserver mmoItemObserver3 = MmoItemObserver;
			mmoItemObserver3.LobbyEndedAction = (System.Action)Delegate.Combine(mmoItemObserver3.LobbyEndedAction, new System.Action(onLobbyEnded));
			Service.Get<EventDispatcher>().AddListener<SessionEvents.SessionPausingEvent>(onSessionPausing);
			Service.Get<EventDispatcher>().AddListener<TubeRaceEvents.CloseLobby>(onCloseLobby);
			onLobbyEnded();
		}

		private bool onSessionPausing(SessionEvents.SessionPausingEvent evt)
		{
			if (isLocalPlayerInLobby)
			{
				PausedStateData component;
				if (Service.Get<CPDataEntityCollection>().TryGetComponent(Service.Get<CPDataEntityCollection>().LocalPlayerHandle, out component))
				{
					component.Position = LobbyExitPosition.position;
				}
				isLocalPlayerInLobby = false;
			}
			return false;
		}

		private bool onCloseLobby(TubeRaceEvents.CloseLobby evt)
		{
			if (isLocalPlayerInLobby)
			{
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("XButton"));
			}
			closeLobby();
			return false;
		}

		private void onLobbyStarted(long startTimestampInSeconds)
		{
			LobbyInteractionObject.gameObject.SetActive(true);
			LobbyInteractionObject.ItemId = MmoItemObserver.MmoItemId;
		}

		private void onLobbyPlayersUpdated(PartyGamePlayerCollection players)
		{
			PartyGamePlayer partyGamePlayer = null;
			foreach (PartyGamePlayer player in players.Players)
			{
				if (player.UserSessionId == Service.Get<CPDataEntityCollection>().LocalPlayerSessionId)
				{
					partyGamePlayer = player;
					break;
				}
			}
			bool flag = partyGamePlayer != null;
			if (isLocalPlayerInLobby && !flag)
			{
				handleLocalPlayerLeftLobby();
			}
			if (!isLocalPlayerInLobby && flag)
			{
				handleLocalPlayerJoinedLobby();
			}
		}

		private void handleLocalPlayerJoinedLobby()
		{
			isLocalPlayerInLobby = true;
			Service.Get<EventDispatcher>().DispatchEvent(default(TubeRaceEvents.LocalPlayerJoinedLobby));
			moveLocalPlayerToPosition(LobbyPosition.position);
			LocomotionHelper.SetCurrentController<SlideController>(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject);
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton2"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ActionButton"));
			PartyGameUtils.DisableMainNavigation();
			PartyGameUtils.DisableCellPhoneButton(true);
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerCardEvents.DismissPlayerCard));
			Service.Get<QuestService>().SendEvent(string.Format("Join{0}Race", RaceDefinition.QuestEventIdentifier));
			Service.Get<ICPSwrveService>().Action("tube_race", "lobby");
		}

		private void handleLocalPlayerLeftLobby()
		{
			isLocalPlayerInLobby = false;
			Service.Get<EventDispatcher>().DispatchEvent(default(TubeRaceEvents.LocalPlayerLeftLobby));
			moveLocalPlayerToPosition(LobbyExitPosition.position);
			LocomotionHelper.SetCurrentController<RunController>(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject);
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton2"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ActionButton"));
			PartyGameUtils.EnableMainNavigation();
			PartyGameUtils.EnableCellPhoneButton();
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
			Service.Get<QuestService>().SendEvent(string.Format("Cancel{0}Race", RaceDefinition.QuestEventIdentifier));
		}

		private void onLobbyEnded()
		{
			closeLobby();
		}

		private void closeLobby()
		{
			isLocalPlayerInLobby = false;
			LobbyInteractionObject.gameObject.SetActive(false);
		}

		private void OnDestroy()
		{
			TubeRaceLobbyMmoItemObserver mmoItemObserver = MmoItemObserver;
			mmoItemObserver.LobbyStartedAction = (Action<long>)Delegate.Remove(mmoItemObserver.LobbyStartedAction, new Action<long>(onLobbyStarted));
			TubeRaceLobbyMmoItemObserver mmoItemObserver2 = MmoItemObserver;
			mmoItemObserver2.LobbyPlayersUpdatedAction = (Action<PartyGamePlayerCollection>)Delegate.Remove(mmoItemObserver2.LobbyPlayersUpdatedAction, new Action<PartyGamePlayerCollection>(onLobbyPlayersUpdated));
			TubeRaceLobbyMmoItemObserver mmoItemObserver3 = MmoItemObserver;
			mmoItemObserver3.LobbyEndedAction = (System.Action)Delegate.Remove(mmoItemObserver3.LobbyEndedAction, new System.Action(onLobbyEnded));
			Service.Get<EventDispatcher>().RemoveListener<SessionEvents.SessionPausingEvent>(onSessionPausing);
			Service.Get<EventDispatcher>().RemoveListener<TubeRaceEvents.CloseLobby>(onCloseLobby);
		}

		private void moveLocalPlayerToPosition(Vector3 position)
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			localPlayerGameObject.transform.position = position + new Vector3(0f, 0.5f, 0f);
			LocomotionTracker component = localPlayerGameObject.GetComponent<LocomotionTracker>();
			CoroutineRunner.Start(LocomotionUtils.nudgePlayer(component), component.gameObject, "MoveAfterJump");
		}
	}
}

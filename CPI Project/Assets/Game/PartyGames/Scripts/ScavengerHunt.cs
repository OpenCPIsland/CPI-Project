using ClubPenguin.Analytics;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Participation;
using ClubPenguin.PartyGames;
using ClubPenguin.Props;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public class ScavengerHunt : AbstractPartyGameSession
	{
		public enum ScavengerHuntRoles
		{
			Hider,
			Finder
		}

		private enum ScavengerHuntState
		{
			AwaitingRoles,
			Hiding,
			Finding
		}

		private enum RewardIndex
		{
			Win,
			Lose
		}

		private const int DEFINITION_ID = 0;

		private readonly PrefabContentKey HIDDEN_ITEM_PREFAB_KEY = new PrefabContentKey("Prefabs/ScavengerHunt/ScavengerHuntItem");

		private readonly PrefabContentKey HUD_UI_PREFAB_KEY = new PrefabContentKey("Prefabs/ScavengerHunt/ScavengerHuntHud");

		private readonly PrefabContentKey ROLE_INDICATOR_PREFAB_KEY = new PrefabContentKey("Prefabs/ScavengerHunt/ScavengerHuntRoleIndicator");

		private readonly PrefabContentKey END_GAME_PREFAB_KEY = new PrefabContentKey("Prefabs/ScavengerHunt/ScavengerHuntEndGame");

		private readonly ParticipationController localPlayerParticipationController;

		private EventChannel eventChannel;

		private JsonService jsonService;

		private ScavengerHuntData scavengerHuntData;

		private Dictionary<long, GameObject> playerIdToRoleIndicators;

		private Dictionary<int, GameObject> itemIdToItemGameObject;

		private ScavengerHuntRoles role;

		private ScavengerHuntState currentState;

		private Transform parentTransform;

		private long localPlayerSessionId;

		private long otherPlayerSessionId;

		private ScavengerHuntHud hudUI;

		private int totalItemsHidden;

		private int totalItemsFound;

		private PartyGameDefinition partyGameDefinition;

		private ScavengerHuntDefinition scavengerHuntDefinition;

		private bool didWin;

		public ScavengerHunt()
		{
			playerIdToRoleIndicators = new Dictionary<long, GameObject>();
			itemIdToItemGameObject = new Dictionary<int, GameObject>();
			jsonService = Service.Get<JsonService>();
			localPlayerParticipationController = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<ParticipationController>();
			localPlayerSessionId = Service.Get<CPDataEntityCollection>().LocalPlayerSessionId;
			parentTransform = new GameObject("ScavengerHunt_" + base.sessionId).transform;
			getDefinitions();
			addListeners();
		}

		protected override void startGame()
		{
			loadAudioPrefab(partyGameDefinition);
			totalItemsHidden = 0;
			totalItemsFound = 0;
			otherPlayerSessionId = ((base.players[0].UserSessionId == localPlayerSessionId) ? base.players[1].UserSessionId : base.players[0].UserSessionId);
			scavengerHuntData = new ScavengerHuntData(base.sessionId, localPlayerSessionId, otherPlayerSessionId, scavengerHuntDefinition.MaxHiddenItems, partyGameDefinition.Rewards[0].Reward, partyGameDefinition.Rewards[1].Reward);
			changeState(ScavengerHuntState.AwaitingRoles);
			PartyGameUtils.AddParticipationFilter(localPlayerParticipationController);
			PartyGameUtils.AddActionConfirmationFilter(partyGameDefinition);
			disableMainNavigation();
			if (base.players[0].UserSessionId == localPlayerSessionId)
			{
				PartyGameUtils.LogGameStartBi(partyGameDefinition.name, base.players.Count);
				PartyGameUtils.StartBiTimer(partyGameDefinition.name, base.sessionId);
			}
			Service.Get<EventDispatcher>().DispatchEvent(new PartyGameEvents.PartyGameStarted(partyGameDefinition));
		}

		protected override void audioPrefabLoaded()
		{
			EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/RolesLoading", EventAction.PlaySound);
		}

		protected override void endGame(Dictionary<long, int> playerSessionIdToPlacement)
		{
			if (playerSessionIdToPlacement.ContainsKey(localPlayerSessionId))
			{
				didWin = false;
				if (playerSessionIdToPlacement[localPlayerSessionId] == 0)
				{
					didWin = true;
				}
				showEndGameUI();
				if (didWin)
				{
					logWinBi();
				}
				if (currentState != ScavengerHuntState.Hiding)
				{
					PartyGameUtils.LogBalkBi(partyGameDefinition.name);
				}
				if (base.players[0].UserSessionId == localPlayerSessionId)
				{
					PartyGameUtils.StopBiTimer(base.sessionId);
					PartyGameUtils.LogGameEndBi(partyGameDefinition.name, playerSessionIdToPlacement.Count);
				}
				EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/TimeRunningOut", EventAction.StopSound);
				EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/ClockLoop", EventAction.StopSound);
			}
		}

		protected override void destroy()
		{
			itemIdToItemGameObject.Clear();
			removeListeners();
			PartyGameUtils.RemoveParticipationFilter(localPlayerParticipationController);
			enableLocomotionControls();
			changeToDefaultCamera();
			PartyGameUtils.RemoveActionConfirmationFilter();
			enableMainNavigation();
			resetPropControls();
			if (playerIdToRoleIndicators.Count > 0)
			{
				foreach (KeyValuePair<long, GameObject> playerIdToRoleIndicator in playerIdToRoleIndicators)
				{
					Service.Get<EventDispatcher>().DispatchEvent(new PlayerIndicatorEvents.RemovePlayerIndicator(playerIdToRoleIndicator.Key, false));
				}
				playerIdToRoleIndicators.Clear();
			}
			if (hudUI != null)
			{
				Object.Destroy(hudUI.gameObject);
			}
			Object.Destroy(parentTransform.gameObject);
			CoroutineRunner.StopAllForOwner(this);
			Service.Get<EventDispatcher>().DispatchEvent(new PartyGameEvents.PartyGameEnded(partyGameDefinition));
		}

		protected override void handleSessionMessage(PartyGameSessionMessageTypes type, string data)
		{
			switch (type)
			{
			case PartyGameSessionMessageTypes.SetRole:
			{
				PartyGameSessionMessages.SetRole setRole = jsonService.Deserialize<PartyGameSessionMessages.SetRole>(data);
				role = (ScavengerHuntRoles)setRole.RoleId;
				scavengerHuntData.LocalPlayerRole = role;
				scavengerHuntData.OtherPlayerRole = ((role == ScavengerHuntRoles.Hider) ? ScavengerHuntRoles.Finder : ScavengerHuntRoles.Hider);
				break;
			}
			case PartyGameSessionMessageTypes.SetGameState:
			{
				PartyGameSessionMessages.SetGameState setGameState = jsonService.Deserialize<PartyGameSessionMessages.SetGameState>(data);
				ScavengerHuntState gameStateId = (ScavengerHuntState)setGameState.GameStateId;
				changeState(gameStateId);
				break;
			}
			case PartyGameSessionMessageTypes.RemoveItem:
				handleRemoveItem(jsonService.Deserialize<PartyGameSessionMessages.RemoveItem>(data));
				break;
			case PartyGameSessionMessageTypes.AddItem:
				handleAddItem(jsonService.Deserialize<PartyGameSessionMessages.AddItem>(data));
				break;
			}
		}

		private void handleAddItem(PartyGameSessionMessages.AddItem addItemData)
		{
			if (!itemIdToItemGameObject.ContainsKey(addItemData.ItemId))
			{
				hudUI.HideItem();
				totalItemsHidden++;
				EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/MarbleDropped", EventAction.PlaySound);
				CoroutineRunner.Start(addItem(addItemData), this, "addItem_marble");
			}
		}

		private IEnumerator addItem(PartyGameSessionMessages.AddItem addItemData)
		{
			AssetRequest<GameObject> request = Content.LoadAsync(HIDDEN_ITEM_PREFAB_KEY);
			yield return request;
			if (role == ScavengerHuntRoles.Hider && scavengerHuntData.LocalPlayerAnimator != null)
			{
				scavengerHuntData.LocalPlayerAnimator.ResetTrigger("TorsoAction1");
				scavengerHuntData.LocalPlayerAnimator.SetTrigger("TorsoAction1");
			}
			else if (scavengerHuntData.OtherPlayerAnimator != null)
			{
				scavengerHuntData.OtherPlayerAnimator.ResetTrigger("TorsoAction1");
				scavengerHuntData.OtherPlayerAnimator.SetTrigger("TorsoAction1");
			}
			GameObject hiddenObjectGO = Object.Instantiate<GameObject>(position: addItemData.Pos, rotation: Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 0f)), original: request.Asset, parent: parentTransform);
			hiddenObjectGO.GetComponentInChildren<PartyGameInteractibleProperties>().ItemId = addItemData.ItemId;
			itemIdToItemGameObject.Add(addItemData.ItemId, hiddenObjectGO);
			if (role == ScavengerHuntRoles.Hider)
			{
				logItemHiddenBi();
			}
		}

		private void handleRemoveItem(PartyGameSessionMessages.RemoveItem removeItemData)
		{
			if (itemIdToItemGameObject.ContainsKey(removeItemData.ItemId))
			{
				hudUI.FoundItem();
				Service.Get<EventDispatcher>().DispatchEvent(new ScavengerHuntEvents.RemoveMarble(itemIdToItemGameObject[removeItemData.ItemId].transform));
				if (role == ScavengerHuntRoles.Finder)
				{
					scavengerHuntData.LocalPlayerAnimator.ResetTrigger("TorsoAction1");
					scavengerHuntData.LocalPlayerAnimator.SetTrigger("TorsoAction1");
				}
				else
				{
					scavengerHuntData.OtherPlayerAnimator.ResetTrigger("TorsoAction1");
					scavengerHuntData.OtherPlayerAnimator.SetTrigger("TorsoAction1");
				}
				Object.Destroy(itemIdToItemGameObject[removeItemData.ItemId]);
				itemIdToItemGameObject.Remove(removeItemData.ItemId);
				totalItemsFound++;
				if (role == ScavengerHuntRoles.Finder)
				{
					logItemFoundBi();
				}
			}
		}

		private void changeState(ScavengerHuntState newState)
		{
			currentState = newState;
			switch (newState)
			{
			case ScavengerHuntState.AwaitingRoles:
				startAwaitingRoles();
				break;
			case ScavengerHuntState.Hiding:
				startHiding();
				EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/RoleAccepted", EventAction.PlaySound);
				break;
			case ScavengerHuntState.Finding:
				startFinding();
				EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/RoleAccepted", EventAction.PlaySound);
				break;
			}
		}

		private void startAwaitingRoles()
		{
			disableLocomotionControls();
			Content.LoadAsync(onHudPrefabLoaded, HUD_UI_PREFAB_KEY);
			Content.LoadAsync(onRoleIndicatorLoaded, ROLE_INDICATOR_PREFAB_KEY);
		}

		private void onHudPrefabLoaded(string path, GameObject prefab)
		{
			GameObject gameObject = GameObject.Find("HudLeftPanel");
			GameObject gameObject2 = Object.Instantiate(prefab, gameObject.transform, false);
			hudUI = gameObject2.GetComponent<ScavengerHuntHud>();
			hudUI.Init(scavengerHuntData);
			hudUI.SetToSelectingRoleState();
		}

		private void onRoleIndicatorLoaded(string path, GameObject prefab)
		{
			createNewRoleIndicator(prefab, localPlayerSessionId);
			createNewRoleIndicator(prefab, otherPlayerSessionId);
		}

		private void createNewRoleIndicator(GameObject prefab, long playerId)
		{
			GameObject gameObject = Object.Instantiate(prefab);
			playerIdToRoleIndicators.Add(playerId, gameObject);
			gameObject.GetComponent<ScavengerHuntRoleIndicator>().Init(playerId, scavengerHuntDefinition.IntroTimeInSeconds, scavengerHuntData);
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerIndicatorEvents.ShowPlayerIndicator(gameObject, playerId));
		}

		private void startHiding()
		{
			hudUI.SetToHidingState(scavengerHuntDefinition.HideBufferTimeInSeconds, scavengerHuntDefinition.HideTimeInSeconds);
			if (role == ScavengerHuntRoles.Finder)
			{
				changeToZoomedCamera();
			}
			else if (role == ScavengerHuntRoles.Hider)
			{
				CoroutineRunner.Start(delayedStartHiding(), this, "delayedStartHiding");
			}
			EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/ClockLoop", EventAction.PlaySound);
			EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/GameStart", EventAction.PlaySound);
		}

		private IEnumerator delayedStartHiding()
		{
			yield return new WaitForSeconds(scavengerHuntDefinition.FindBufferTimeInSeconds);
			enableLocomotionControls();
		}

		private void startFinding()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerIndicatorEvents.RemovePlayerIndicator(localPlayerSessionId, false));
			if (role == ScavengerHuntRoles.Finder)
			{
				if (scavengerHuntData.OtherPlayerAnimator != null)
				{
					scavengerHuntData.OtherPlayerAnimator.SetInteger("PropMode", 3);
				}
				enableLocomotionControls();
				changeToDefaultCamera();
			}
			else if (role == ScavengerHuntRoles.Hider)
			{
				if (scavengerHuntData.LocalPlayerAnimator != null)
				{
					scavengerHuntData.LocalPlayerAnimator.SetInteger("PropMode", 3);
				}
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ActionButton"));
			}
			Service.Get<EventDispatcher>().DispatchEvent(new ScavengerHuntEvents.StartFinderBulb(localPlayerSessionId, otherPlayerSessionId, itemIdToItemGameObject));
			Service.Get<EventDispatcher>().DispatchEvent(default(ScavengerHuntEvents.ShowHiderClockProp));
			hudUI.SetToFindingState(scavengerHuntDefinition.FindBufferTimeInSeconds, scavengerHuntDefinition.FindTimeInSeconds);
		}

		private void showEndGameUI()
		{
			Content.LoadAsync(onEndGameUILoaded, END_GAME_PREFAB_KEY);
		}

		private void onEndGameUILoaded(string path, GameObject prefab)
		{
			GameObject gameObject = Object.Instantiate(prefab);
			Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowCameraSpacePopup(gameObject));
			gameObject.GetComponent<ScavengerHuntEndGame>().Init(scavengerHuntData, totalItemsHidden, totalItemsFound, didWin);
			if (didWin)
			{
				EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/MenuWinner", EventAction.PlaySound);
			}
			else
			{
				EventManager.Instance.PostEvent("SFX/AO/MarbleHunt/MenuLoser", EventAction.PlaySound);
			}
		}

		private void addListeners()
		{
			eventChannel = new EventChannel(Service.Get<EventDispatcher>());
			eventChannel.AddListener<PenguinInteraction.InteractionStartedEvent>(onItemInteractionStarted);
		}

		private void removeListeners()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
		}

		private bool onItemInteractionStarted(PenguinInteraction.InteractionStartedEvent evt)
		{
			if (evt.InteractingPlayerId != localPlayerSessionId || role != ScavengerHuntRoles.Finder || currentState != ScavengerHuntState.Finding)
			{
				return false;
			}
			PartyGameInteractibleProperties componentInChildren = evt.ObjectInteractedWith.GetComponentInChildren<PartyGameInteractibleProperties>();
			if (componentInChildren != null && itemIdToItemGameObject.ContainsKey(componentInChildren.ItemId))
			{
				PartyGameSessionMessages.RemoveItem data = new PartyGameSessionMessages.RemoveItem(componentInChildren.ItemId);
				sendSessionMessage(PartyGameSessionMessageTypes.RemoveItem, data);
			}
			return false;
		}

		private void disableLocomotionControls()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton2"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ControlsButton1"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ActionButton"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("JumpButton"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("Joystick"));
		}

		private void enableLocomotionControls()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("ControlsButtons"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("Joystick"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton2"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton1"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ActionButton"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("JumpButton"));
		}

		private void changeToZoomedCamera()
		{
			float num = 0.5f;
			float heightOffset = 1f;
			float minDist = 1.5f;
			CinematographyEvents.ZoomCameraEvent evt = new CinematographyEvents.ZoomCameraEvent(true, num, num, 0f, heightOffset, minDist);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void changeToDefaultCamera()
		{
			CinematographyEvents.ZoomCameraEvent evt = new CinematographyEvents.ZoomCameraEvent(false);
			Service.Get<EventDispatcher>().DispatchEvent(evt);
		}

		private void disableMainNavigation()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
		}

		private void enableMainNavigation()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
		}

		private void getDefinitions()
		{
			partyGameDefinition = getPartyGameDefinition(0);
			scavengerHuntDefinition = (ScavengerHuntDefinition)partyGameDefinition.GameData;
		}

		private void logItemFoundBi()
		{
			Service.Get<ICPSwrveService>().Action("scavenger_hunt", "find", totalItemsFound.ToString());
		}

		private void logItemHiddenBi()
		{
			Service.Get<ICPSwrveService>().Action("scavenger_hunt", "hide", totalItemsHidden.ToString());
		}

		private void logWinBi()
		{
			Service.Get<ICPSwrveService>().Action("scavenger_hunt", "win", role.ToString());
		}

		private void resetPropControls()
		{
			Service.Get<CPDataEntityCollection>().GetComponent<HeldObjectsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).HeldObject = null;
			Service.Get<PropService>().LocalPlayerPropUser.ResetPropControls(true);
		}
	}
}

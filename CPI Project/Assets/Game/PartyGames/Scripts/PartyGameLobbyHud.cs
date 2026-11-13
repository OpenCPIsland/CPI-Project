using ClubPenguin.Interactables;
using ClubPenguin.PartyGames;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Fabric;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Game.PartyGames
{
	public class PartyGameLobbyHud : MonoBehaviour
	{
		public enum PartyGameLobbyHudState
		{
			Lobby,
			HostLobby
		}

		private const int LOBBY_PLAYER_ICON_ON_INDEX = 0;

		private const int LOBBY_PLAYER_ICON_OFF_INDEX = 1;

		private const string WAITING_FOR_PLAYERS_TOKEN = "Activity.PartyGame.WaitingText";

		public Text HeaderText;

		public GameObject PlayersPanel;

		public GameObject LoadingSpinner;

		public TintSelector[] PlayerJoinedIcons;

		public Button StartGameButton;

		[Space(10f)]
		public string PlayerJoinedSFXTrigger;

		private Localizer localizer;

		private PartyGameLobbyHudState hudState;

		private int totalInvitationItems;

		private SpriteSelector startButtonSpriteSelector;

		private int prevPlayers = 1;

		private bool partyGameStarted;

		public PartyGameLobbyHudState HudState
		{
			get
			{
				return hudState;
			}
		}

		private void Awake()
		{
			Service.Get<EventDispatcher>().AddListener<PartyGameEvents.PartyGameStarted>(onPartyGameStarted);
			localizer = Service.Get<Localizer>();
			startButtonSpriteSelector = StartGameButton.GetComponent<SpriteSelector>();
			SetState(PartyGameLobbyHudState.Lobby);
			setLobbyPlayersIcons(1);
		}

		private void Start()
		{
			PartyGameUtils.DisableCellPhoneButton();
		}

		private void OnDestroy()
		{
			if (!partyGameStarted)
			{
				PartyGameUtils.EnableCellPhoneButton();
			}
			Service.Get<EventDispatcher>().RemoveListener<PartyGameEvents.PartyGameStarted>(onPartyGameStarted);
		}

		public void SetState(PartyGameLobbyHudState newState)
		{
			switch (newState)
			{
			case PartyGameLobbyHudState.Lobby:
				StartGameButton.gameObject.SetActive(false);
				HeaderText.text = localizer.GetTokenTranslation("Activity.PartyGame.WaitingText");
				PlayersPanel.SetActive(false);
				LoadingSpinner.SetActive(true);
				break;
			case PartyGameLobbyHudState.HostLobby:
				StartGameButton.gameObject.SetActive(true);
				HeaderText.text = localizer.GetTokenTranslation("Activity.PartyGame.WaitingText");
				PlayersPanel.SetActive(true);
				LoadingSpinner.SetActive(false);
				break;
			}
			hudState = newState;
		}

		public void SetInvitationalItem(InvitationalItemExperience invitationalItem)
		{
			totalInvitationItems = invitationalItem.TotalItemQuantity;
			int lobbyPlayersIcons = totalInvitationItems - invitationalItem.AvailableItemQuantity + 1;
			setLobbyPlayersIcons(lobbyPlayersIcons);
			invitationalItem.AvailableItemQuantityChangedAction = (Action<int>)Delegate.Combine(invitationalItem.AvailableItemQuantityChangedAction, new Action<int>(onInvitationalItemQuantityChanged));
		}

		private void onInvitationalItemQuantityChanged(int quantity)
		{
			int num = totalInvitationItems - quantity + 1;
			if (num > prevPlayers)
			{
				EventManager.Instance.PostEvent(PlayerJoinedSFXTrigger, EventAction.PlaySound);
				prevPlayers = num;
			}
			setLobbyPlayersIcons(num);
		}

		private void setLobbyPlayersIcons(int numPlayers)
		{
			for (int i = 0; i < PlayerJoinedIcons.Length; i++)
			{
				if (i < numPlayers)
				{
					PlayerJoinedIcons[i].SelectColor(0);
				}
				else
				{
					PlayerJoinedIcons[i].SelectColor(1);
				}
			}
			bool flag = numPlayers > 1;
			StartGameButton.interactable = flag;
			if (flag)
			{
				startButtonSpriteSelector.SelectSprite(0);
			}
			else
			{
				startButtonSpriteSelector.SelectSprite(1);
			}
		}

		private bool onPartyGameStarted(PartyGameEvents.PartyGameStarted evt)
		{
			partyGameStarted = true;
			return false;
		}
	}
}

using ClubPenguin.Core;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Game.UI.PlayerCard.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class PlayerDetailsController : MonoBehaviour
	{
		public GameObject DetailsPreloader;

		public GameObject ContentPanel;

		public GameObject CoinsPanel;

		public Text NameText;

		public Text AgeText;

		public Text CoinsText;

		public GameObject ActionButtonsPanel;

		public GameObject SendFriendRequestButton;

		public GameObject FriendRequestSentText;

		public GameObject FriendsListFullButton;

		public GameObject AcceptFriendRequestButton;

		public GameObject JumpToFriendButton;

		public GameObject InactiveJumpToFriendButton;

		private PlayerCardController playerCardController;

		private string playerWorld;

		public void Awake()
		{
			SetPreloaderActive(true);
		}

		public void SetDisplayName(string displayName)
		{
			NameText.text = displayName;
		}

		public void SetPenguinAge(int penguinAge)
		{
			AgeText.text = penguinAge.ToString();
		}

		public void SetCoins(int coins)
		{
			CoinsText.text = coins.ToString();
		}

		public void SetPlayerCardController(PlayerCardController playerCardController)
		{
			this.playerCardController = playerCardController;
		}

		public void SetFriendStatus(FriendStatus friendStatus)
		{
			SetPreloaderActive(false);
			bool flag = FriendsDataModelService.FriendsList.Count >= FriendsDataModelService.MaxFriendsCount;
			switch (friendStatus)
			{
			case FriendStatus.Self:
				CoinsPanel.SetActive(true);
				break;
			case FriendStatus.Friend:
				ActionButtonsPanel.SetActive(true);
				JumpToFriendButton.SetActive(true);
				break;
			case FriendStatus.IncomingInvite:
				ActionButtonsPanel.SetActive(true);
				AcceptFriendRequestButton.SetActive(!flag);
				FriendsListFullButton.SetActive(flag);
				break;
			case FriendStatus.OutgoingInvite:
				ActionButtonsPanel.SetActive(true);
				FriendRequestSentText.SetActive(true);
				break;
			case FriendStatus.None:
				ActionButtonsPanel.SetActive(true);
				SendFriendRequestButton.SetActive(!flag);
				FriendsListFullButton.SetActive(flag);
				break;
			}
			if (flag && FriendsListFullButton.activeSelf)
			{
				FriendsListFullButton.GetComponent<FriendsListFullTooltipButton>().SetIsLocalPlayer(true);
			}
		}

		public void PresenceDataUpdated(FriendStatus friendStatus, PresenceData presenceData)
		{
			playerWorld = presenceData.World;
			if (friendStatus == FriendStatus.Friend)
			{
				bool flag = isPresenceDataValidForJump(presenceData);
				JumpToFriendButton.SetActive(flag);
				InactiveJumpToFriendButton.SetActive(!flag);
			}
		}

		public void SetPreloaderActive(bool isActive)
		{
			deactivateAllElements();
			DetailsPreloader.SetActive(isActive);
			ContentPanel.SetActive(!isActive);
		}

		public void OnSendFriendRequestButtonClicked()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerCardEvents.SendFriendInvitation));
		}

		public void OnAcceptFriendRequestButtonClicked()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerCardEvents.AcceptFriendInvitation));
		}

		public void OnJumpToFriendButtonClicked()
		{
			if (!playerCardController.IsShowingJumpPrompt)
			{
				playerCardController.IsShowingJumpPrompt = true;
				CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
				PresenceData component = cPDataEntityCollection.GetComponent<PresenceData>(cPDataEntityCollection.LocalPlayerHandle);
				if (component.World == playerWorld)
				{
					Service.Get<PromptManager>().ShowPrompt("JumpToFriendPrompt", onJumpToFriendPromptButtonClicked);
					return;
				}
				PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("JumpToFriendServerPrompt");
				PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, showJumpToFriendServerPrompt);
				promptLoaderCMD.Execute();
			}
		}

		private void showJumpToFriendServerPrompt(PromptLoaderCMD promptLoader)
		{
			string i18nText = "-";
			WorldDefinition world = Service.Get<ZoneTransitionService>().GetWorld(playerWorld);
			if (world != null)
			{
				i18nText = Service.Get<Localizer>().GetTokenTranslation(LocalizationLanguage.GetLanguageToken(world.Language));
			}
			promptLoader.PromptData.SetText("Prompt.Text.World", playerWorld, true);
			promptLoader.PromptData.SetText("Prompt.Text.Language", i18nText, true);
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, onJumpToFriendPromptButtonClicked, promptLoader.Prefab);
		}

		private void deactivateAllElements()
		{
			CoinsPanel.SetActive(false);
			ActionButtonsPanel.SetActive(false);
			SendFriendRequestButton.SetActive(false);
			FriendRequestSentText.SetActive(false);
			AcceptFriendRequestButton.SetActive(false);
			JumpToFriendButton.SetActive(false);
			InactiveJumpToFriendButton.SetActive(false);
		}

		private bool isPresenceDataValidForJump(PresenceData remotePresenceData)
		{
			if (!string.IsNullOrEmpty(remotePresenceData.World) && !string.IsNullOrEmpty(remotePresenceData.Room))
			{
				ZoneDefinition zone = Service.Get<ZoneTransitionService>().GetZone(remotePresenceData.Room);
				if (zone != null)
				{
					CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
					PresenceData component;
					if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component) && component.ContentIdentifier == remotePresenceData.ContentIdentifier)
					{
						return !zone.IsQuestOnly && (zone.Type != ZoneDefinition.ZoneType.Igloo || remotePresenceData.IsInInstancedRoom);
					}
				}
			}
			return false;
		}

		private void onJumpToFriendPromptButtonClicked(DPrompt.ButtonFlags pressed)
		{
			if (pressed == DPrompt.ButtonFlags.YES)
			{
				Service.Get<ActionConfirmationService>().ConfirmAction(typeof(PlayerCardEvents.JoinPlayer), null, onJumpToFriendConfirmationSuccess);
			}
			playerCardController.IsShowingJumpPrompt = false;
		}

		private void onJumpToFriendConfirmationSuccess()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerCardEvents.JoinPlayer));
		}
	}
}

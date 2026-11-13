using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class PlayerCardActionListController : MonoBehaviour
	{
		public GameObject IgnorePlayerButton;

		public GameObject ReportPlayerButton;

		public GameObject UnfriendPlayerButton;

		private string ignoreConfirmationTitleToken;

		private string ignoreConfirmationToken;

		private string unfriendConfirmationTitle;

		private string unfriendConfirmation;

		private string displayName;

		private void Awake()
		{
			ignoreConfirmationTitleToken = "PlayerCard.Prompt.IgnoreConfirmationTitle";
			ignoreConfirmationToken = "PlayerCard.Prompt.IgnoreConfirmation";
		}

		public void SetName(string displayName)
		{
			this.displayName = displayName;
		}

		public void SetFriendStatus(FriendStatus friendStatus)
		{
			setDefaults();
			switch (friendStatus)
			{
			case FriendStatus.None:
				break;
			case FriendStatus.OutgoingInvite:
				break;
			case FriendStatus.IncomingInvite:
				break;
			case FriendStatus.Self:
				ReportPlayerButton.SetActive(false);
				break;
			case FriendStatus.Friend:
				UnfriendPlayerButton.SetActive(true);
				break;
			}
		}

		private void setDefaults()
		{
			UnfriendPlayerButton.SetActive(false);
			IgnorePlayerButton.SetActive(false);
		}

		public void OnUnfriendPlayerButtonClicked()
		{
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("UnfriendPrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, showUnfriendPrompt);
			promptLoaderCMD.Execute();
		}

		private void showUnfriendPrompt(PromptLoaderCMD promptLoader)
		{
			string i18nText = string.Format(Service.Get<Localizer>().GetTokenTranslation(promptLoader.PromptData.TextFields[DPrompt.PROMPT_TEXT_BODY].I18nText), displayName);
			promptLoader.PromptData.SetText(DPrompt.PROMPT_TEXT_BODY, i18nText, true);
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, onUnfriendPlayerPromptButtonClicked, promptLoader.Prefab);
		}

		public void OnIgnorePlayerButtonClicked()
		{
			createConfirmationPrompt(ignoreConfirmationTitleToken, ignoreConfirmationToken, onPlaceholderPromptButtonClicked);
		}

		public void OnReportPlayerButtonClicked()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(PlayerCardEvents.ReportPlayer));
		}

		private void createConfirmationPrompt(string titleText, string bodyText, Action<DPrompt.ButtonFlags> callback)
		{
			DPrompt data = new DPrompt(titleText, bodyText, DPrompt.ButtonFlags.NO | DPrompt.ButtonFlags.YES);
			Service.Get<PromptManager>().ShowPrompt(data, callback);
		}

		private void onUnfriendPlayerPromptButtonClicked(DPrompt.ButtonFlags pressed)
		{
			if (pressed == DPrompt.ButtonFlags.YES)
			{
				Service.Get<EventDispatcher>().DispatchEvent(default(PlayerCardEvents.UnfriendPlayer));
			}
		}

		private void onPlaceholderPromptButtonClicked(DPrompt.ButtonFlags pressed)
		{
		}
	}
}

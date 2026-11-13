using ClubPenguin.Core;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ReportPlayerController : MonoBehaviour
	{
		private const string NOTIFICATION_TOKEN = "GlobalUI.ReportingBans.SuccessNotification";

		private const string REASON_TEXT_TOKEN = "GlobalUI.ReportingBans.ReasonText";

		private const string CONFIRMATION_TOKEN = "GlobalUI.ReportingBans.ConfirmDescriptionText";

		private const string INAPPROPRIATE_TALK_TOKEN = "GlobalUI.ReportingBans.Category3";

		private const string REVEALING_PII_TOKEN = "GlobalUI.ReportingBans.Category2";

		private const string OBTAINING_PII_TOKEN = "GlobalUI.ReportingBans.Category1";

		private const string DISRESPECTING_OTHERS_TOKEN = "GlobalUI.ReportingBans.Category4";

		private const string BAD_DISPLAY_NAME_TOKEN = "GlobalUI.ReportingBans.Category5";

		private const string INAPPROPRIATE_IGLOO_TOKEN = "GlobalUI.ReportingBans.Category.Igloo";

		public Text DescriptionText;

		public Text NameText;

		public Text ReasonText;

		public Text ConfirmationText;

		public AvatarRenderTextureComponent AvatarRenderTextureComponent;

		public ReportingBansTweener Tweener;

		public DisableableReportPlayerCategoryButton reportIglooButton;

		private ReportUserReason reason;

		private Dictionary<int, string> reasonsToTokens;

		private string displayName;

		private ProfileData profileData;

		public void Initialize(DataEntityHandle handle)
		{
			mapCategoriesStrings();
			DataEntityCollection dataEntityCollection = Service.Get<CPDataEntityCollection>();
			DisplayNameData component = dataEntityCollection.GetComponent<DisplayNameData>(handle);
			if (component != null && !string.IsNullOrEmpty(component.DisplayName))
			{
				displayName = component.DisplayName;
				NameText.text = displayName;
			}
			else
			{
				Log.LogError(this, "Could not find display name data on this handle");
				destroy();
			}
			AvatarDetailsData component2;
			if (dataEntityCollection.TryGetComponent(handle, out component2))
			{
				AvatarRenderTextureComponent.RenderAvatar(component2);
			}
			else
			{
				Log.LogError(this, "AvatarDetailsData was not found");
			}
			if (dataEntityCollection.TryGetComponent(handle, out profileData))
			{
				bool enabled = profileData != null && profileData.HasPublicIgloo;
				reportIglooButton.ToggleButton(enabled);
			}
			else
			{
				Log.LogError(this, "Could not find ProfileData for this handle.");
				reportIglooButton.ToggleButton(false);
			}
		}

		public string GetTextForReason(ReportUserReason reason)
		{
			string tokenForReason = getTokenForReason(reason);
			if (!string.IsNullOrEmpty(tokenForReason))
			{
				return Service.Get<Localizer>().GetTokenTranslation(tokenForReason);
			}
			return null;
		}

		private string getTokenForReason(ReportUserReason reason)
		{
			if (reasonsToTokens.ContainsKey((int)reason))
			{
				return reasonsToTokens[(int)reason];
			}
			Log.LogErrorFormatted(this, "Did not find a token for reason {0}", reason);
			return null;
		}

		public void OnCategoryButtonClicked(ReportUserReason reason)
		{
			this.reason = reason;
			Tweener.Open();
			Localizer localizer = Service.Get<Localizer>();
			string tokenForReason = getTokenForReason(reason);
			ReasonText.text = localizer.GetTokenTranslationFormatted("GlobalUI.ReportingBans.ReasonText", tokenForReason);
			ConfirmationText.text = localizer.GetTokenTranslation("GlobalUI.ReportingBans.ConfirmDescriptionText");
		}

		public void OnCancelButtonClicked()
		{
			Tweener.Close();
		}

		public void OnReportButtonClicked()
		{
			showNotification();
			Service.Get<INetworkServicesManager>().ModerationService.ReportPlayer(displayName, reason.ToString());
			destroy();
		}

		public void OnCloseButtonClicked()
		{
			destroy();
		}

		private void destroy()
		{
			Object.Destroy(base.gameObject);
		}

		private void mapCategoriesStrings()
		{
			reasonsToTokens = new Dictionary<int, string>();
			reasonsToTokens.Add(5, "GlobalUI.ReportingBans.Category3");
			reasonsToTokens.Add(7, "GlobalUI.ReportingBans.Category2");
			reasonsToTokens.Add(10, "GlobalUI.ReportingBans.Category1");
			reasonsToTokens.Add(6, "GlobalUI.ReportingBans.Category4");
			reasonsToTokens.Add(11, "GlobalUI.ReportingBans.Category5");
			reasonsToTokens.Add(30, "GlobalUI.ReportingBans.Category.Igloo");
		}

		private void showNotification()
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("GlobalUI.ReportingBans.SuccessNotification");
			DNotification dNotification = new DNotification();
			dNotification.PopUpDelayTime = 6f;
			dNotification.Message = tokenTranslation;
			Service.Get<TrayNotificationManager>().ShowNotification(dNotification);
		}
	}
}

using ClubPenguin.Breadcrumbs;
using ClubPenguin.CellPhone;
using ClubPenguin.Configuration;
using ClubPenguin.Net;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Newsfeed
{
	public class NewsfeedViewController : MonoBehaviour
	{
		public Button ScrollUp;

		public Button ScrollDown;

		public GameObject LoadingPanel;

		public GameObject WebViewerPanel;

		public GameObject WebViewFailedPanel;

		public StaticBreadcrumbDefinitionKey Breadcrumb;

		private NewsfeedController newsfeedController;

		private void OnDestroy()
		{
			if (newsfeedController != null)
			{
				newsfeedController.NewsfeedClosed -= newsfeedClosed;
				newsfeedController.NewsfeedLoaded -= newsfeedLoaded;
				newsfeedController.NewsfeedFailed -= newsfeedFailed;
				newsfeedController.NewsfeedLoginSucceeded -= newsfeedLoginSucceeded;
				newsfeedController.Close();
			}
		}

		private IEnumerator Start()
		{
			yield return new WaitForEndOfFrame();
			if (Service.Get<ConnectionManager>().ConnectionState == ConnectionManager.NetworkConnectionState.NoConnection)
			{
				Service.Get<PromptManager>().ShowError(Service.Get<Localizer>().GetTokenTranslation("GlobalUI.ErrorMessages.NetworkConnectionError"), Service.Get<Localizer>().GetTokenTranslation("GlobalUI.ErrorMessages.CheckNetworkConnection"));
				yield break;
			}
			string accessToken = Service.Get<INetworkServicesManager>().PlayerStateService.GetAccessToken();
			string javaScriptLoginFunction = "CLUB_PENGUIN.app.login('" + accessToken + "')";
			string newsfeedUrlToken = getNewsfeedUrlToken();
			bool isDownsampled = Service.Get<ConditionalConfiguration>().Get("UniWebView.Downsampled.property", false);
			newsfeedController = Service.Get<NewsfeedController>();
			newsfeedController.NewsfeedClosed += newsfeedClosed;
			newsfeedController.NewsfeedLoaded += newsfeedLoaded;
			newsfeedController.NewsfeedFailed += newsfeedFailed;
			newsfeedController.NewsfeedLoginSucceeded += newsfeedLoginSucceeded;
			newsfeedController.ShowOnPanel(newsfeedUrlToken, ScrollUp, ScrollDown, WebViewerPanel, isDownsampled, javaScriptLoginFunction);
		}

		public void OnBackButtonPressed()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.ReturnToHomeScreen));
		}

		private string getNewsfeedUrlToken()
		{
			string text = "Activity.Newsfeed.URL.Apple";
			text = "Activity.Newsfeed.URL.Windows";
			text = Service.Get<Localizer>().GetTokenForCurrentEnvironment(text);
			GameSettings gameSettings = Service.Get<GameSettings>();
			if (gameSettings.OfflineMode && !string.IsNullOrEmpty(gameSettings.CPWebsiteAPIServicehost))
			{
				text = gameSettings.CPWebsiteAPIServicehost;
			}
			return text;
		}

		private void newsfeedClosed()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.ReturnToHomeScreen));
		}

		private void newsfeedLoaded()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.HideLoadingScreen));
		}

		private void newsfeedFailed()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(CellPhoneEvents.HideLoadingScreen));
			if (WebViewFailedPanel != null)
			{
				WebViewFailedPanel.SetActive(true);
			}
		}

		private void newsfeedLoginSucceeded()
		{
			if (LoadingPanel != null)
			{
				LoadingPanel.SetActive(false);
			}
			Service.Get<NotificationBreadcrumbController>().ResetBreadcrumbs(Breadcrumb);
		}
	}
}

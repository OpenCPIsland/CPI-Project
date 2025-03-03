using ClubPenguin.Configuration;
using ClubPenguin.ContentGates;
using ClubPenguin.NativeWebViewer;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common.SEDFSM;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class SettingsWebView : MonoBehaviour
	{
		public Button ScrollUp;

		public Button ScrollDown;

		public GameObject WebViewerPanel;

		public Text TitleText;

		public GameObject LoadingPanel;

		public GameObject WebViewFailedPanel;

		public string UrlToken;

		public string TitleToken;

		public bool IsParentGated;

		private WebViewController webView;

		private void Start()
		{
			setWebPageSubPanelTitle(TitleToken);
			OpenCloseTweener componentInParent = GetComponentInParent<OpenCloseTweener>();
			if (componentInParent.IsOpen && !componentInParent.IsTransitioning)
			{
				StartCoroutine(waitForEndOfFrame());
			}
			else
			{
				componentInParent.OnComplete += onTweenComplete;
			}
		}

		private IEnumerator waitForEndOfFrame()
		{
			yield return new WaitForEndOfFrame();
			showURL(UrlToken, IsParentGated);
		}

		private void onTweenComplete()
		{
			GetComponentInParent<OpenCloseTweener>().OnComplete -= onTweenComplete;
			showURL(UrlToken, IsParentGated);
		}

		private void showURL(string urlToken, bool isParentGate)
		{
			if (Service.Get<ConnectionManager>().ConnectionState == ConnectionManager.NetworkConnectionState.NoConnection)
			{
				Service.Get<PromptManager>().ShowError("GlobalUI.ErrorMessages.NetworkConnectionError", "GlobalUI.ErrorMessages.CheckNetworkConnection");
				return;
			}
			webView = new WebViewController(base.gameObject);
			addWebViewListeners();
			if (isParentGate)
			{
				showWebPageViaParentGate(urlToken);
			}
			else
			{
				showWebPageSubPanel(urlToken);
			}
		}

		private void setWebPageSubPanelTitle(string titleToken)
		{
			TitleText.text = Service.Get<Localizer>().GetTokenTranslation(titleToken);
		}

		private void showWebPageSubPanel(string urlToken)
		{
			bool isDownsampled = Service.Get<ConditionalConfiguration>().Get("UniWebView.Downsampled.property", false);
			webView.Show(urlToken, null, WebViewerPanel, ScrollUp, ScrollDown, isDownsampled);
		}

		private void showWebPageViaParentGate(string urlToken)
		{
			bool isDownsampled = Service.Get<ConditionalConfiguration>().Get("UniWebView.Downsampled.property", false);
			webView.Show(urlToken, new ParentGate(), WebViewerPanel, ScrollUp, ScrollDown, isDownsampled);
		}

		private void onWebViewLoaded()
		{
			if (LoadingPanel != null)
			{
				LoadingPanel.SetActive(false);
			}
		}

		private void onWebViewClosed()
		{
			closeSubScreen();
		}

		private void onWebViewFailed()
		{
			if (WebViewFailedPanel != null)
			{
				WebViewFailedPanel.SetActive(true);
				if (LoadingPanel != null)
				{
					LoadingPanel.SetActive(false);
				}
			}
			else
			{
				closeSubScreen();
			}
		}

		public void OnBackClicked()
		{
			closeSubScreen();
		}

		private void closeSubScreen()
		{
			GetComponentInParent<StateMachineContext>().SendEvent(new ExternalEvent("Settings", "back"));
		}

		private void OnDestroy()
		{
			if (webView != null)
			{
				removeWebViewListeners();
				webView.Close();
			}
		}

		private void addWebViewListeners()
		{
			webView.OnClosed += onWebViewClosed;
			webView.OnFailed += onWebViewFailed;
			webView.OnLoaded += onWebViewLoaded;
		}

		private void removeWebViewListeners()
		{
			webView.OnClosed -= onWebViewClosed;
			webView.OnFailed -= onWebViewFailed;
			webView.OnLoaded -= onWebViewLoaded;
		}
	}
}

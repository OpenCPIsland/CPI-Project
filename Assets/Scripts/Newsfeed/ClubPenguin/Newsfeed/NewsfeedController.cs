using ClubPenguin.Core;
using ClubPenguin.NativeWebViewer;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Newsfeed
{
	public class NewsfeedController : MonoBehaviour
	{
		private const string NEWSFEED_LOGIN_TIMESTAMP_PLAYERPREFS_KEY = "newsfeed_login_timestamp";

		private WebViewController webView;

		private string jsLoginFunction;

		private bool receivedInitialLoginFunctionResponse;

		private JsonService jsonService;

		private CPDataEntityCollection dataEntityCollection;

		private Localizer localizer;

		[SerializeField]
		private bool waitForReadyToShow = false;

		public event System.Action NewsfeedClosed;

		public event System.Action NewsfeedLoaded;

		public event System.Action NewsfeedFailed;

		public event System.Action NewsfeedLoginSucceeded;

		private void Start()
		{
			dataEntityCollection = Service.Get<CPDataEntityCollection>();
			jsonService = Service.Get<JsonService>();
			localizer = Service.Get<Localizer>();
			Service.Get<EventDispatcher>().AddListener<NewsfeedServiceEvents.LatestPostTime>(onLatestPostTime);
			newPostCheck();
		}

		private void OnDestroy()
		{
			Service.Get<EventDispatcher>().RemoveListener<NewsfeedServiceEvents.LatestPostTime>(onLatestPostTime);
		}

		private void OnApplicationPause(bool isPaused)
		{
			if (!isPaused)
			{
				newPostCheck();
			}
		}

		public void ShowOnPanel(string urlToken, Button scrollUp, Button scrollDown, GameObject viewerPanel, bool isDownsampled, string javaScriptLoginFunction = "")
		{
			jsLoginFunction = javaScriptLoginFunction;
			webView = new WebViewController(base.gameObject);
			addWebViewListeners();
			webView.WaitForReadyToShow = waitForReadyToShow;
			webView.IsReadyToShow = receivedInitialLoginFunctionResponse;
			webView.Show(urlToken, null, viewerPanel, scrollUp, scrollDown, isDownsampled);
		}

		public void Close()
		{
			closeWebView();
		}

		private void onWebViewLoaded()
		{
			if (this.NewsfeedLoaded != null)
			{
				this.NewsfeedLoaded();
				this.NewsfeedLoaded = null;
			}
			if (!string.IsNullOrEmpty(jsLoginFunction))
			{
				webView.SendJavaScript(jsLoginFunction);
			}
		}

		private void onWebViewClosed()
		{
			closeWebView();
		}

		private void onWebViewFailed()
		{
			if (this.NewsfeedFailed != null)
			{
				this.NewsfeedFailed();
				this.NewsfeedFailed = null;
			}
		}

		private void closeWebView()
		{
			if (webView != null)
			{
				removeWebViewListeners();
				webView.CleanCache();
				webView.Close();
				if (this.NewsfeedClosed != null)
				{
					this.NewsfeedClosed();
					this.NewsfeedClosed = null;
				}
			}
		}

		private void onEvalJavaScriptFinished(string result)
		{
			if (string.IsNullOrEmpty(result) || result.Equals("undefined"))
			{
				return;
			}
			Dictionary<string, string> dictionary = jsonService.Deserialize<Dictionary<string, string>>(result);
			if (dictionary != null && dictionary.ContainsKey("type") && dictionary.ContainsKey("status") && dictionary.ContainsKey("timestamp") && dictionary["type"].Equals("tokenLogin") && !receivedInitialLoginFunctionResponse && dictionary["status"] == "success")
			{
				webView.IsReadyToShow = true;
				receivedInitialLoginFunctionResponse = true;
				PlayerPrefs.SetInt("newsfeed_login_timestamp", Convert.ToInt32(dictionary["timestamp"]));
				dataEntityCollection.RemoveComponent<NewPostData>(dataEntityCollection.LocalPlayerHandle);
				if (this.NewsfeedLoginSucceeded != null)
				{
					this.NewsfeedLoginSucceeded();
					this.NewsfeedLoginSucceeded = null;
				}
			}
		}

		private void newPostCheck()
		{
			NewPostData component;
			if (!dataEntityCollection.TryGetComponent(dataEntityCollection.LocalPlayerHandle, out component))
			{
				if (PlayerPrefs.GetInt("newsfeed_login_timestamp") <= 0)
				{
					dataEntityCollection.AddComponent<NewPostData>(dataEntityCollection.LocalPlayerHandle);
				}
				else
				{
					Service.Get<INetworkServicesManager>().NewsfeedService.GetLatestPostTime(localizer.Language.ToString());
				}
			}
		}

		private bool onLatestPostTime(NewsfeedServiceEvents.LatestPostTime evt)
		{
			if (evt.Timestamp > PlayerPrefs.GetInt("newsfeed_login_timestamp"))
			{
				dataEntityCollection.AddComponent<NewPostData>(dataEntityCollection.LocalPlayerHandle);
			}
			return false;
		}

		private void addWebViewListeners()
		{
			webView.OnClosed += onWebViewClosed;
			webView.OnFailed += onWebViewFailed;
			webView.OnLoaded += onWebViewLoaded;
			webView.OnEvalJavaScriptFinished += onEvalJavaScriptFinished;
		}

		private void removeWebViewListeners()
		{
			webView.OnClosed -= onWebViewClosed;
			webView.OnFailed -= onWebViewFailed;
			webView.OnLoaded -= onWebViewLoaded;
			webView.OnEvalJavaScriptFinished -= onEvalJavaScriptFinished;
		}
	}
}

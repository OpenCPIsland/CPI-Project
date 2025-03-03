#if UNITY_ANDROID// || UNITY_IOS || UNITY_IPHONE
using ClubPenguin.Core;
using ClubPenguin.NativeWebViewer;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using LitJson;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.NativeWebViewer
{
public class WebViewController
{
	public delegate string FormatURLDelegate(string url);

	private GameObject instantiatedPrefab;

	private PrefabContentKey webViewerPopupKey = new PrefabContentKey("AccountSystemPrefabs/WebviewerPopupPrefab");

	private Button backButton;

	private Text title;

	private Button scrollUp;

	private Button scrollDown;

	private int scrollAmount = 50;

	private GameObject loadingPanel;

	private GameObject webViewPopupPanel;

	private AbstractWebViewComponentController webViewComponentController;

	private IContentInterruption openContentInterruption;

	private bool blockLinks;

	private Transform parentTransform;

	private const string WHITELIST_TOKEN = "GlobalUI.Settings.SettingsURLs.whitelist";

	private bool isLoaded;

	public FormatURLDelegate FormatURL;

	public bool WaitForReadyToShow;

	private bool isReadyToShow;

	public bool IsReadyToShow
	{
		get
		{
			return isReadyToShow;
		}
		set
		{
			isReadyToShow = value;
			if (isLoaded)
			{
				webViewComponentController.Show();
			}
		}
	}

	public event System.Action OnClosed;

	public event System.Action OnLoaded;

	public event System.Action OnFailed;

	public event System.Action OnSetupComplete;

	public event Action<string> OnEvalJavaScriptFinished;

	public event Action<JsonData> OnReceivedMessage;

	public WebViewController(GameObject gameObject)
	{
		webViewComponentController = new UniWebViewComponentController(gameObject);
	}

	public void Show(string URLToken, IContentInterruption gate, GameObject webViewerPanel, Button ScrollUpButton, Button ScrollDownButton, bool isDownsampled, bool AllowPopups = false, bool openPopupInNewBrowser = false, float zoomLevel = 0f, float newBrowserZoomLevel = 0f)
	{
		if (ScrollUpButton != null)
		{
			scrollUp = ScrollUpButton;
			scrollUp.gameObject.SetActive(false);
		}
		if (ScrollDownButton != null)
		{
			scrollDown = ScrollDownButton;
			scrollDown.gameObject.SetActive(false);
		}
		parentTransform = webViewerPanel.transform;
		webViewComponentController.Url = getLocalizedURL(URLToken);
		webViewComponentController.SetUp(webViewerPanel, isDownsampled, AllowPopups, openPopupInNewBrowser, zoomLevel, newBrowserZoomLevel);
		initializeGate(gate);
	}

	public void Show(string URLToken, IContentInterruption gate, string popupTitleToken, bool AllowPopups = false, bool openPopupInNewBrowser = false, float zoomLevel = 0f, float newBrowserZoomLevel = 0f)
	{
		initializePopup(webViewerPopupKey, URLToken, gate, popupTitleToken, AllowPopups, openPopupInNewBrowser, zoomLevel, newBrowserZoomLevel);
	}

	public void Show(string URLToken, IContentInterruption gate, string popupTitleToken, PrefabContentKey webViewerPrefabKey, bool AllowPopups = false, bool openPopupInNewBrowser = false, float zoomLevel = 0f, float newBrowserZoomLevel = 0f)
	{
		initializePopup(webViewerPrefabKey, URLToken, gate, popupTitleToken, AllowPopups, openPopupInNewBrowser, zoomLevel, newBrowserZoomLevel);
	}

	public void CleanCache()
	{
		webViewComponentController.CleanCache();
	}

	private string getLocalizedURL(string Token)
	{
		string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(Token);
		tokenTranslation = tokenTranslation.Replace("##", "");
		tokenTranslation = tokenTranslation.Replace("**", "");
		if (FormatURL != null)
		{
			tokenTranslation = FormatURL(tokenTranslation);
		}
		return tokenTranslation;
	}

	private void initializeGate(IContentInterruption gate)
	{
		if (gate != null)
		{
			blockLinks = false;
			showGate(gate);
		}
		else
		{
			blockLinks = true;
			loadWebPage();
		}
	}

	private void loadWebPage()
	{
		webViewComponentController.OnSetupComplete += onSetupComplete;
		webViewComponentController.OnLoadComplete += onLoadComplete;
		webViewComponentController.OnWebViewShouldClose += onWebViewShouldClose;
		webViewComponentController.OnEvalJavaScriptFinished += onEvalJavaScriptFinished;
		webViewComponentController.OnReceivedMessage += onReceivedMessage;
		webViewComponentController.Load();
	}

	private void showGate(IContentInterruption gate)
	{
		openContentInterruption = gate;
		gate.OnReturn += HandleOnGateFailed;
		gate.OnContinue += onGatePassed;
		gate.Show(parentTransform);
	}

	private void onGatePassed()
	{
		openContentInterruption.OnReturn -= HandleOnGateFailed;
		openContentInterruption.OnContinue -= onGatePassed;
		loadWebPage();
	}

	private void HandleOnGateFailed()
	{
		openContentInterruption.OnReturn -= HandleOnGateFailed;
		openContentInterruption.OnContinue -= onGatePassed;
		this.OnFailed.InvokeSafe();
	}

	private void onEvalJavaScriptFinished(string result)
	{
		this.OnEvalJavaScriptFinished.InvokeSafe(result);
	}

	private void onReceivedMessage(JsonData message)
	{
		this.OnReceivedMessage.InvokeSafe(message);
	}

	private void onSetupComplete()
	{
		this.OnSetupComplete.InvokeSafe();
	}

	private void onLoadComplete(AbstractWebViewComponentController webViewComponentController, bool success, string errorMessage)
	{
		if (success)
		{
			if (MonoSingleton<NativeAccessibilityManager>.Instance.IsEnabled)
			{
				string str = "Enabling Accessible Scroll buttons: ";
				if (scrollUp != null)
				{
					scrollUp.onClick.AddListener(delegate
					{
						handleScrollUp(webViewComponentController);
					});
					scrollUp.gameObject.SetActive(true);
					str += "scrollUp ";
				}
				if (scrollDown != null)
				{
					scrollDown.onClick.AddListener(delegate
					{
						handleScrollDown(webViewComponentController);
					});
					scrollDown.gameObject.SetActive(true);
					str += "scrollDown ";
				}
			}
			if (blockLinks)
			{
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Settings.SettingsURLs.whitelist");
				Uri uri = new Uri(webViewComponentController.Url);
				tokenTranslation = tokenTranslation + "," + uri.Host;
				string javaScript = "\n\t\t\t\t\t\tconsole.log('[DisableLinks] define CPIdisableLinks');\n                        function CPIdisableLinks() {\n                            aTags = document.getElementsByTagName('a');\n                            var whitelist = '" + tokenTranslation + "';\n                            var hosts = whitelist.split(',');\n                            console.log('[DisableLinks] whitelist='+whitelist);\n                            for (i = 0; i < aTags.length; i++) {\n                                if (aTags[i].hasAttribute('href')) {\n                                    var href = aTags[i].getAttribute('href');\n                                    var whitelisted = false;\n                                    for (var j = 0; j < hosts.length; j++) {\n                                        if (href.indexOf(hosts[j]) > -1) {\n                                            whitelisted = true;\n                                            break;\n                                        }\n                                    }\n                                    console.log('[DisableLinks] checking '+href);\n                                    if (!whitelisted && href && href.indexOf('#') != 0 && (href.indexOf('/') != 0 || href.indexOf('//') == 0)) {\n                                        aTags[i].removeAttribute('href');\n                                        console.log('[DisableLinks] blocking '+href);\n                                    }\n                                }\n                            }\n                        }";
				string javaScript2 = "\n\t\t\t\t\t\tconsole.log('[DisableLinks] assign CPItarget');\n                        var CPItarget = document.getElementsByTagName('body')[0];\n\t\t\t\t\t\tconsole.log('[DisableLinks] assign CPIobserver');\n                        var CPIobserver = new MutationObserver(function(mutations) {\n                          console.log('[DisableLinks] disable links for dynamically added content');\n                          CPIdisableLinks();\n                        });\n\t\t\t\t\t\tconsole.log('[DisableLinks] assign CPIconfig');\n                        var CPIconfig = { attributes: true, childList: true, characterData: true };";
				if (webViewComponentController.SupportsAddJavascript)
				{
					webViewComponentController.AddJavaScript(javaScript);
					webViewComponentController.AddJavaScript(javaScript2);
				}
				else
				{
					webViewComponentController.EvaluateJavaScript(javaScript);
					webViewComponentController.EvaluateJavaScript(javaScript2);
				}
				string javaScript3 = "setTimeout(CPIdisableLinks, 300);";
				string javaScript4 = "setTimeout(function(){ CPIobserver.observe(CPItarget, CPIconfig);}, 500); ";
				webViewComponentController.EvaluateJavaScript(javaScript3);
				webViewComponentController.EvaluateJavaScript(javaScript4);
			}
			this.OnLoaded.InvokeSafe();
			isLoaded = true;
			if (!WaitForReadyToShow || IsReadyToShow)
			{
				webViewComponentController.Show();
			}
			if (loadingPanel != null)
			{
				loadingPanel.SetActive(false);
			}
		}
		else
		{
			Log.LogError(this, "Something wrong in web view loading: " + errorMessage);
			this.OnFailed.InvokeSafe();
		}
	}

	private void handleScrollUp(AbstractWebViewComponentController webViewComponentController)
	{
		string javaScript = "window.scrollBy(0, -" + scrollAmount + ");";
		webViewComponentController.EvaluateJavaScript(javaScript);
	}

	private void handleScrollDown(AbstractWebViewComponentController webViewComponentController)
	{
		string javaScript = "window.scrollBy(0, " + scrollAmount + ");";
		webViewComponentController.EvaluateJavaScript(javaScript);
	}

	private void onWebViewShouldClose(AbstractWebViewComponentController webViewComponentController)
	{
		webViewComponentController.OnWebViewShouldClose -= onWebViewShouldClose;
		webViewComponentController.OnLoadComplete -= onLoadComplete;
		webViewComponentController.OnEvalJavaScriptFinished -= onEvalJavaScriptFinished;
		webViewComponentController.OnReceivedMessage -= onReceivedMessage;
		Close();
	}

	public void Close()
	{
		if (webViewComponentController != null)
		{
			webViewComponentController.OnWebViewShouldClose -= onWebViewShouldClose;
			webViewComponentController.OnLoadComplete -= onLoadComplete;
			webViewComponentController.OnEvalJavaScriptFinished -= onEvalJavaScriptFinished;
			webViewComponentController.OnReceivedMessage -= onReceivedMessage;
			webViewComponentController.Close();
		}
		this.OnClosed.InvokeSafe();
		if (instantiatedPrefab != null)
		{
			UnityEngine.Object.Destroy(instantiatedPrefab);
		}
	}

	private void handlePopupClose()
	{
		Close();
	}

	public void initializePopup(PrefabContentKey popupKey, string URLToken, IContentInterruption gate, string popupTitleToken, bool allowPopups = false, bool openPopupInNewBrowser = false, float zoomLevel = 0f, float newBrowserZoomLevel = 0f)
	{
		CoroutineRunner.Start(loadPopupFromPrefab(popupKey, URLToken, gate, popupTitleToken, allowPopups, openPopupInNewBrowser, zoomLevel, newBrowserZoomLevel), this, "loadPopupFromPrefab");
	}

	private IEnumerator loadPopupFromPrefab(PrefabContentKey popupKey, string URLToken, IContentInterruption gate, string popupTitleToken, bool allowPopups = false, bool openPopupInNewBrowser = false, float zoomLevel = 0f, float newBrowserZoomLevel = 0f)
	{
		AssetRequest<GameObject> assetRequest = Content.LoadAsync(popupKey);
		yield return assetRequest;
		instantiatedPrefab = UnityEngine.Object.Instantiate(assetRequest.Asset);
		WebViewPrefabController webViewPrefabController = instantiatedPrefab.GetComponent<WebViewPrefabController>();
		title = webViewPrefabController.TitleText;
		Button scrollUpButton = webViewPrefabController.ScrollUp;
		Button scrollDownButton = webViewPrefabController.ScrollDown;
		backButton = webViewPrefabController.CloseButton;
		webViewPopupPanel = webViewPrefabController.WebViewerPanel;
		loadingPanel = webViewPrefabController.LoadingPanel;
		if (title != null && backButton != null && webViewPopupPanel != null)
		{
			title.text = getLocalizedURL(popupTitleToken);
			backButton.onClick.AddListener(handlePopupClose);
			if ((bool)GameObject.Find("TopCanvas"))
			{
				Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowTopPopup(instantiatedPrefab));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(instantiatedPrefab));
			}
			yield return new WaitForEndOfFrame();
			if (loadingPanel != null)
			{
				loadingPanel.SetActive(true);
			}
			Show(URLToken, gate, webViewPopupPanel, scrollUpButton, scrollDownButton, false, allowPopups, openPopupInNewBrowser, zoomLevel, newBrowserZoomLevel);
		}
		else
		{
			Log.LogError(this, "Missing key elements from prefab for " + popupKey);
			this.OnFailed.InvokeSafe();
		}
		yield return null;
	}

	public void SendJavaScript(string javaScript)
	{
		if (webViewComponentController != null)
		{
			webViewComponentController.EvaluateJavaScript(javaScript);
		}
	}

	public void RegisterJSFunction(string jsFunctionName)
	{
		if (webViewComponentController != null)
		{
			webViewComponentController.RegisterJSFunction(jsFunctionName);
		}
	}

	public void CallFunction(string jsFunctionName, params JsonData[] arguments)
	{
		if (webViewComponentController != null)
		{
			webViewComponentController.CallFunction(jsFunctionName, arguments);
		}
	}
}
}
#else
using ClubPenguin.Core;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using Disney.Native;
using LitJson;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX
using ZenFulcrum.EmbeddedBrowser;
#endif

namespace ClubPenguin.NativeWebViewer
{
	public class WebViewController
	{
		public delegate string FormatURLDelegate(string url);

		private const string WHITELIST_TOKEN = "GlobalUI.Settings.SettingsURLs.whitelist";

		private GameObject instantiatedPrefab;

		private PrefabContentKey webViewerPopupKey = new PrefabContentKey("AccountSystemPrefabs/WebviewerPopupPrefab");

		private Button backButton;

		private Text title;

		private Button scrollUp;

		private Button scrollDown;

		private int scrollAmount = 50;

		private GameObject loadingPanel;

		private GameObject webViewPopupPanel;

		private AbstractWebViewComponentController webViewComponentController;

		private IContentInterruption openContentInterruption;

		private bool blockLinks;

		private Transform parentTransform;

		private bool isLoaded;

		public FormatURLDelegate FormatURL;

		public bool WaitForReadyToShow;

		private bool isReadyToShow;

		public bool IsReadyToShow
		{
			get
			{
				return isReadyToShow;
			}
			set
			{
				isReadyToShow = value;
				if (isLoaded)
				{
					webViewComponentController.Show();
				}
			}
		}

		public event System.Action OnClosed;

		public event System.Action OnLoaded;

		public event System.Action OnFailed;

		public event System.Action OnSetupComplete;

		public event Action<string> OnEvalJavaScriptFinished;

		public event Action<JsonData> OnReceivedMessage;

		public WebViewController(GameObject gameObject)
		{
#if UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX

			webViewComponentController = new ZFBrowserComponentController(gameObject);
#endif
        }

        public void Show(string URLToken, IContentInterruption gate, GameObject webViewerPanel, Button ScrollUpButton, Button ScrollDownButton, bool isDownsampled, bool AllowPopups = false, bool openPopupInNewBrowser = false, float zoomLevel = 0f, float newBrowserZoomLevel = 0f)
		{
			if (ScrollUpButton != null)
			{
				scrollUp = ScrollUpButton;
				scrollUp.gameObject.SetActive(false);
			}
			if (ScrollDownButton != null)
			{
				scrollDown = ScrollDownButton;
				scrollDown.gameObject.SetActive(false);
			}
			parentTransform = webViewerPanel.transform;
			webViewComponentController.Url = getLocalizedURL(URLToken);
			webViewComponentController.SetUp(webViewerPanel, isDownsampled, AllowPopups, openPopupInNewBrowser, zoomLevel, newBrowserZoomLevel);
			initializeGate(gate);
		}

		public void Show(string URLToken, IContentInterruption gate, string popupTitleToken, bool AllowPopups = false, bool openPopupInNewBrowser = false, float zoomLevel = 0f, float newBrowserZoomLevel = 0f)
		{
			initializePopup(webViewerPopupKey, URLToken, gate, popupTitleToken, AllowPopups, openPopupInNewBrowser, zoomLevel, newBrowserZoomLevel);
		}

		public void Show(string URLToken, IContentInterruption gate, string popupTitleToken, PrefabContentKey webViewerPrefabKey, bool AllowPopups = false, bool openPopupInNewBrowser = false, float zoomLevel = 0f, float newBrowserZoomLevel = 0f)
		{
			initializePopup(webViewerPrefabKey, URLToken, gate, popupTitleToken, AllowPopups, openPopupInNewBrowser, zoomLevel, newBrowserZoomLevel);
		}

		public void CleanCache()
		{
			webViewComponentController.CleanCache();
		}

		private string getLocalizedURL(string Token)
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(Token);
			tokenTranslation = tokenTranslation.Replace("##", "");
			tokenTranslation = tokenTranslation.Replace("**", "");
			if (FormatURL != null)
			{
				tokenTranslation = FormatURL(tokenTranslation);
			}
			return tokenTranslation;
		}

		private void initializeGate(IContentInterruption gate)
		{
			if (gate != null)
			{
				blockLinks = false;
				showGate(gate);
			}
			else
			{
				blockLinks = true;
				loadWebPage();
			}
		}

		private void loadWebPage()
		{
			webViewComponentController.OnSetupComplete += onSetupComplete;
			webViewComponentController.OnLoadComplete += onLoadComplete;
			webViewComponentController.OnWebViewShouldClose += onWebViewShouldClose;
			webViewComponentController.OnEvalJavaScriptFinished += onEvalJavaScriptFinished;
			webViewComponentController.OnReceivedMessage += onReceivedMessage;
			webViewComponentController.Load();
		}

		private void showGate(IContentInterruption gate)
		{
			openContentInterruption = gate;
			gate.OnReturn += HandleOnGateFailed;
			gate.OnContinue += onGatePassed;
			gate.Show(parentTransform);
		}

		private void onGatePassed()
		{
			openContentInterruption.OnReturn -= HandleOnGateFailed;
			openContentInterruption.OnContinue -= onGatePassed;
			loadWebPage();
		}

		private void HandleOnGateFailed()
		{
			openContentInterruption.OnReturn -= HandleOnGateFailed;
			openContentInterruption.OnContinue -= onGatePassed;
			this.OnFailed.InvokeSafe();
		}

		private void onEvalJavaScriptFinished(string result)
		{
			this.OnEvalJavaScriptFinished.InvokeSafe(result);
		}

		private void onReceivedMessage(JsonData message)
		{
			this.OnReceivedMessage.InvokeSafe(message);
		}

		private void onSetupComplete()
		{
			this.OnSetupComplete.InvokeSafe();
		}

		private void onLoadComplete(AbstractWebViewComponentController webViewComponentController, bool success, string errorMessage)
		{
			if (success)
			{
				if (MonoSingleton<NativeAccessibilityManager>.Instance.IsEnabled)
				{
					string str = "Enabling Accessible Scroll buttons: ";
					if (scrollUp != null)
					{
						scrollUp.onClick.AddListener(delegate
						{
							handleScrollUp(webViewComponentController);
						});
						scrollUp.gameObject.SetActive(true);
						str += "scrollUp ";
					}
					if (scrollDown != null)
					{
						scrollDown.onClick.AddListener(delegate
						{
							handleScrollDown(webViewComponentController);
						});
						scrollDown.gameObject.SetActive(true);
						str += "scrollDown ";
					}
				}
				if (blockLinks)
				{
					string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Settings.SettingsURLs.whitelist");
					Uri uri = new Uri(webViewComponentController.Url);
					tokenTranslation = tokenTranslation + "," + uri.Host;
					string javaScript = "\r\n\t\t\t\t\t\tconsole.log('[DisableLinks] define CPIdisableLinks');\r\n                        function CPIdisableLinks() {\r\n                            aTags = document.getElementsByTagName('a');\r\n                            var whitelist = '" + tokenTranslation + "';\r\n                            var hosts = whitelist.split(',');\r\n                            console.log('[DisableLinks] whitelist='+whitelist);\r\n                            for (i = 0; i < aTags.length; i++) {\r\n                                if (aTags[i].hasAttribute('href')) {\r\n                                    var href = aTags[i].getAttribute('href');\r\n                                    var whitelisted = false;\r\n                                    for (var j = 0; j < hosts.length; j++) {\r\n                                        if (href.indexOf(hosts[j]) > -1) {\r\n                                            whitelisted = true;\r\n                                            break;\r\n                                        }\r\n                                    }\r\n                                    console.log('[DisableLinks] checking '+href);\r\n                                    if (!whitelisted && href && href.indexOf('#') != 0 && (href.indexOf('/') != 0 || href.indexOf('//') == 0)) {\r\n                                        aTags[i].removeAttribute('href');\r\n                                        console.log('[DisableLinks] blocking '+href);\r\n                                    }\r\n                                }\r\n                            }\r\n                        }";
					string javaScript2 = "\r\n\t\t\t\t\t\tconsole.log('[DisableLinks] assign CPItarget');\r\n                        var CPItarget = document.getElementsByTagName('body')[0];\r\n\t\t\t\t\t\tconsole.log('[DisableLinks] assign CPIobserver');\r\n                        var CPIobserver = new MutationObserver(function(mutations) {\r\n                          console.log('[DisableLinks] disable links for dynamically added content');\r\n                          CPIdisableLinks();\r\n                        });\r\n\t\t\t\t\t\tconsole.log('[DisableLinks] assign CPIconfig');\r\n                        var CPIconfig = { attributes: true, childList: true, characterData: true };";
					if (webViewComponentController.SupportsAddJavascript)
					{
						webViewComponentController.AddJavaScript(javaScript);
						webViewComponentController.AddJavaScript(javaScript2);
					}
					else
					{
						webViewComponentController.EvaluateJavaScript(javaScript);
						webViewComponentController.EvaluateJavaScript(javaScript2);
					}
					string javaScript3 = "setTimeout(CPIdisableLinks, 300);";
					string javaScript4 = "setTimeout(function(){ CPIobserver.observe(CPItarget, CPIconfig);}, 500); ";
					webViewComponentController.EvaluateJavaScript(javaScript3);
					webViewComponentController.EvaluateJavaScript(javaScript4);
				}
				this.OnLoaded.InvokeSafe();
				isLoaded = true;
				if (!WaitForReadyToShow || IsReadyToShow)
				{
					webViewComponentController.Show();
				}
				if (loadingPanel != null)
				{
					loadingPanel.SetActive(false);
				}
			}
			else
			{
				Log.LogError(this, "Something wrong in web view loading: " + errorMessage);
				this.OnFailed.InvokeSafe();
			}
		}

		private void handleScrollUp(AbstractWebViewComponentController webViewComponentController)
		{
			string javaScript = "window.scrollBy(0, -" + scrollAmount + ");";
			webViewComponentController.EvaluateJavaScript(javaScript);
		}

		private void handleScrollDown(AbstractWebViewComponentController webViewComponentController)
		{
			string javaScript = "window.scrollBy(0, " + scrollAmount + ");";
			webViewComponentController.EvaluateJavaScript(javaScript);
		}

		private void onWebViewShouldClose(AbstractWebViewComponentController webViewComponentController)
		{
			webViewComponentController.OnWebViewShouldClose -= onWebViewShouldClose;
			webViewComponentController.OnLoadComplete -= onLoadComplete;
			webViewComponentController.OnEvalJavaScriptFinished -= onEvalJavaScriptFinished;
			webViewComponentController.OnReceivedMessage -= onReceivedMessage;
			Close();
		}

		public void Close()
		{
			if (webViewComponentController != null)
			{
				webViewComponentController.OnWebViewShouldClose -= onWebViewShouldClose;
				webViewComponentController.OnLoadComplete -= onLoadComplete;
				webViewComponentController.OnEvalJavaScriptFinished -= onEvalJavaScriptFinished;
				webViewComponentController.OnReceivedMessage -= onReceivedMessage;
                webViewComponentController.Close();
#if UNITY_ANDROID || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_STANDALONE_OSX
                BrowserNative.zfb_destroyAllBrowsers();
#endif
           //     BrowserNative.DestroyAllBrowsers();
			}
			this.OnClosed.InvokeSafe();
			if (instantiatedPrefab != null)
			{
				UnityEngine.Object.Destroy(instantiatedPrefab);
			}
		}

		private void handlePopupClose()
		{
			Close();
		}

		public void initializePopup(PrefabContentKey popupKey, string URLToken, IContentInterruption gate, string popupTitleToken, bool allowPopups = false, bool openPopupInNewBrowser = false, float zoomLevel = 0f, float newBrowserZoomLevel = 0f)
		{
			CoroutineRunner.Start(loadPopupFromPrefab(popupKey, URLToken, gate, popupTitleToken, allowPopups, openPopupInNewBrowser, zoomLevel, newBrowserZoomLevel), this, "loadPopupFromPrefab");
		}

		private IEnumerator loadPopupFromPrefab(PrefabContentKey popupKey, string URLToken, IContentInterruption gate, string popupTitleToken, bool allowPopups = false, bool openPopupInNewBrowser = false, float zoomLevel = 0f, float newBrowserZoomLevel = 0f)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(popupKey);
			yield return assetRequest;
			instantiatedPrefab = UnityEngine.Object.Instantiate(assetRequest.Asset);
			WebViewPrefabController webViewPrefabController = instantiatedPrefab.GetComponent<WebViewPrefabController>();
			title = webViewPrefabController.TitleText;
			Button scrollUpButton = webViewPrefabController.ScrollUp;
			Button scrollDownButton = webViewPrefabController.ScrollDown;
			backButton = webViewPrefabController.CloseButton;
			webViewPopupPanel = webViewPrefabController.WebViewerPanel;
			loadingPanel = webViewPrefabController.LoadingPanel;
			if (title != null && backButton != null && webViewPopupPanel != null)
			{
				title.text = getLocalizedURL(popupTitleToken);
				backButton.onClick.AddListener(handlePopupClose);
				if ((bool)GameObject.Find("TopCanvas"))
				{
					Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowTopPopup(instantiatedPrefab));
				}
				else
				{
					Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(instantiatedPrefab));
				}
				yield return new WaitForEndOfFrame();
				if (loadingPanel != null)
				{
					loadingPanel.SetActive(true);
				}
				Show(URLToken, gate, webViewPopupPanel, scrollUpButton, scrollDownButton, false, allowPopups, openPopupInNewBrowser, zoomLevel, newBrowserZoomLevel);
			}
			else
			{
				Log.LogError(this, "Missing key elements from prefab for " + popupKey);
				this.OnFailed.InvokeSafe();
			}
			yield return null;
		}

		public void SendJavaScript(string javaScript)
		{
			if (webViewComponentController != null)
			{
				webViewComponentController.EvaluateJavaScript(javaScript);
			}
		}

		public void RegisterJSFunction(string jsFunctionName)
		{
			if (webViewComponentController != null)
			{
				webViewComponentController.RegisterJSFunction(jsFunctionName);
			}
		}

		public void CallFunction(string jsFunctionName, params JsonData[] arguments)
		{
			if (webViewComponentController != null)
			{
				webViewComponentController.CallFunction(jsFunctionName, arguments);
			}
		}
	}
}
#endif
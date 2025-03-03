#if UNITY_ANDROID// || UNITY_IOS || UNITY_IPHONE
using ClubPenguin.NativeWebViewer;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using Disney.Native;
using LitJson;
using System;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.NativeWebViewer
{
public class UniWebViewComponentController : AbstractWebViewComponentController
{
	[Tweakable("Debug.WebView.UseWebKit", Description = "If the current platform is iOS, control whether or not WKWebView is used instead of UIWebView")]
	public static bool UseWebKitIfAvailable = true;

	private GameObject gameObject;

	private UniWebView uniWebView;

	private const string LOADING_TEXT_TOKEN = "GlobalUI.Settings.Loading";

	public override bool SupportsAddJavascript
	{
		get
		{
			return true;
		}
	}

	public UniWebViewComponentController(GameObject gameObject)
	{
		this.gameObject = gameObject;
	}

	public override void SetUp(GameObject webViewPanel, bool isDownsampled, bool allowPopups, bool openPopupInNewBrowser, float zoomLevel, float newBrowserZoomLevel)
	{
		UniWebView.UseWebKitIfAvailable = UseWebKitIfAvailable;
		uniWebView = gameObject.AddComponent<UniWebView>();
		uniWebView.HideToolBar(false);
		uniWebView.immersiveMode = false;
		uniWebView.insets = getWebViewEdgeInsets(webViewPanel, isDownsampled);
		uniWebView.SetSpinnerLabelText(Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Settings.Loading"));
		raiseOnSetupComplete();
	}

	public override void Load()
	{
		if (base.Url.StartsWith("localGame://"))
		{
			base.Url = UniWebViewHelper.streamingAssetURLForPath("uniwebview/" + base.Url.Substring("localGame://".Length));
		}
		uniWebView.url = base.Url;
		uniWebView.SetShowSpinnerWhenLoading(true);
		uniWebView.autoShowWhenLoadComplete = false;
		uniWebView.OnLoadComplete += onLoadComplete;
		uniWebView.OnWebViewShouldClose += onWebViewShouldClose;
		uniWebView.OnEvalJavaScriptFinished += onEvalJavaScriptFinished;
		uniWebView.OnReceivedMessage += onReceivedMessage;
		uniWebView.Load();
	}

	public override void Show()
	{
		uniWebView.Show();
	}

	public override void CleanCache()
	{
		uniWebView.CleanCache();
	}

	public override void Close()
	{
		uniWebView.Hide(false, UniWebViewTransitionEdge.None, 0.4f, delegate
		{
			UnityEngine.Object.Destroy(uniWebView);
		});
	}

	public override void EvaluateJavaScript(string javaScript)
	{
		uniWebView.EvaluatingJavaScript(javaScript);
	}

	public override void AddJavaScript(string javaScript)
	{
		uniWebView.AddJavaScript(javaScript);
	}

	public override void CallFunction(string name, params JsonData[] arguments)
	{
		string str = name + "(";
		string str2 = "";
		foreach (JsonData jsonData in arguments)
		{
			str = str + str2 + jsonData.ToJson();
			str2 = ", ";
		}
		str += ");";
		uniWebView.EvaluatingJavaScript(str);
	}

	private UniWebViewEdgeInsets getWebViewEdgeInsets(GameObject webViewerPanel, bool isDownsampled)
	{
		Canvas componentInParent = webViewerPanel.gameObject.GetComponentInParent<Canvas>();
		if (componentInParent == null)
		{
			return new UniWebViewEdgeInsets(0, 0, 0, 0);
		}
		RectTransform component = webViewerPanel.GetComponent<RectTransform>();
		Vector3[] array = new Vector3[4];
		component.GetWorldCorners(array);
		Camera worldCamera = componentInParent.worldCamera;
		Vector2 vector = RectTransformUtility.WorldToScreenPoint(worldCamera, array[0]);
		Vector2 vector2 = RectTransformUtility.WorldToScreenPoint(worldCamera, array[2]);
		double num = UniWebViewHelper.screenScale;
		if (isDownsampled)
		{
			num = (double)UniWebViewHelper.screenScale / 1.15;
		}
		if (MonoSingleton<NativeAccessibilityManager>.Instance.Native.IsDisplayZoomEnabled())
		{
			num = ((UniWebViewHelper.screenScale != 3) ? ((double)UniWebViewHelper.screenScale * 1.1742957746478873) : ((double)UniWebViewHelper.screenScale / 1.15 * 1.103448275862069));
		}
		int aTop = (int)((double)UniWebViewHelper.screenHeight - (double)vector2.y / num);
		int aLeft = (int)((double)vector.x / num);
		int aBottom = (int)((double)vector.y / num);
		int aRight = (int)((double)UniWebViewHelper.screenWidth - (double)vector2.x / num);
		return new UniWebViewEdgeInsets(aTop, aLeft, aBottom, aRight);
	}

	private void onLoadComplete(UniWebView webView, bool success, string errorMessage)
	{
		raiseOnLoadComplete(success, errorMessage);
	}

	private bool onWebViewShouldClose(UniWebView webView)
	{
		raiseOnWebViewShouldClose();
		return true;
	}

	private void onEvalJavaScriptFinished(UniWebView webView, string result)
	{
		raiseOnEvalJavaScriptFinished(result);
	}

	private void onReceivedMessage(UniWebView webView, UniWebViewMessage message)
	{
		raiseOnReceivedMessage(JsonMapper.ToObject(JsonMapper.ToJson(message.args)));
	}

	public override void RegisterJSFunction(string javaScript)
	{
		throw new NotImplementedException();
	}
}
}
#endif
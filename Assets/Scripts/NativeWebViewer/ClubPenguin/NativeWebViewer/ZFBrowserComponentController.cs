using LitJson;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

namespace ClubPenguin.NativeWebViewer
{
	public class ZFBrowserComponentController : AbstractWebViewComponentController
	{
		private Browser browser;

		private RawImage rawImage;

		private GameObject webViewContainer;

		private bool hadError = false;

		public override bool SupportsAddJavascript
		{
			get
			{
				return false;
			}
		}

		public ZFBrowserComponentController(GameObject gameObject)
		{
		}

		public static void SetupBrowser(GameObject webViewPanel, GameObject webViewContainer)
		{
			Canvas componentInParent = webViewPanel.GetComponentInParent<Canvas>();
			RectTransform component = webViewContainer.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.anchoredPosition = Vector2.zero;
			float scaleFactor = componentInParent.scaleFactor;
			float num = (float)Screen.currentResolution.height / (float)Screen.height;
			scaleFactor *= num;
			float num2 = 1f / scaleFactor;
			component.localScale *= num2;
			RectTransform rectTransform = webViewPanel.transform as RectTransform;
			Vector2 b = rectTransform.rect.size * scaleFactor;
			Vector2 a = rectTransform.rect.size - b;
			component.sizeDelta = -a;
			LayoutElement component2 = webViewContainer.GetComponent<LayoutElement>();
			component2.ignoreLayout = true;
			webViewContainer.transform.SetParent(webViewPanel.transform, false);
			webViewContainer.AddComponent<GUIBrowserUI>();
		}

		public override void SetUp(GameObject webViewPanel, bool isDownsampled, bool allowPopups, bool openPopupInNewBrowser, float zoomLevel, float newBrowserZoomLevel)
		{
			webViewContainer = new GameObject("WebViewContainer", typeof(RectTransform), typeof(LayoutElement));
			SetupBrowser(webViewPanel, webViewContainer);
			browser = webViewContainer.GetComponent<Browser>();
			browser.Zoom = zoomLevel;
			if (allowPopups)
			{
				if (openPopupInNewBrowser)
				{
					browser.newWindowAction = Browser.NewWindowAction.NewBrowser;
					browser.NewWindowHandler = new ZFNewWindowHandler(newBrowserZoomLevel);
				}
				else
				{
					browser.newWindowAction = Browser.NewWindowAction.NewWindow;
				}
			}
			browser.onLoad += onLoad;
			browser.onFetchError += onFetchError;
			rawImage = webViewContainer.GetComponent<RawImage>();
			rawImage.enabled = false;
			raiseOnSetupComplete();
		}

		public override void Load()
		{
			browser.Url = base.Url;
		}

		private void onLoad(JSONNode obj)
		{
			int num = obj["status"];
			bool success = num >= 200 && num <= 399;
			if (!hadError)
			{
				raiseOnLoadComplete(success, null);
			}
		}

		private void onFetchError(JSONNode obj)
		{
			hadError = true;
			int num = obj["status"];
			bool success = num >= 200 && num <= 399;
			raiseOnLoadComplete(success, null);
		}

		public override void Show()
		{
			rawImage.enabled = true;
		}

		public override void CleanCache()
		{
		}

		public override void Close()
		{
			UnityEngine.Object.Destroy(webViewContainer);
		}

		public override void EvaluateJavaScript(string javaScript)
		{
			browser.EvalJS(javaScript).Then((Action<JSONNode>)onResolved);
		}

		public override void CallFunction(string name, params JsonData[] arguments)
		{
			JSONNode[] array = new JSONNode[arguments.Length];
			for (int i = 0; i < arguments.Length; i++)
			{
				array[i] = JSONNode.Parse(arguments[i].ToJson());
			}
			browser.CallFunction(name, array).Then((Action<JSONNode>)onResolved);
		}

		public override void RegisterJSFunction(string jsFunctionName)
		{
			browser.RegisterFunction(jsFunctionName, onRegisteredJSFunctionCalled);
		}

		private void onRegisteredJSFunctionCalled(JSONNode args)
		{
			raiseOnReceivedMessage(JsonMapper.ToObject(args.AsJSON));
		}

		public override void AddJavaScript(string javaScript)
		{
			throw new NotImplementedException();
		}

		private void onResolved(JSONNode jsonNode)
		{
			raiseOnEvalJavaScriptFinished((string)jsonNode.Value);
		}
	}
}

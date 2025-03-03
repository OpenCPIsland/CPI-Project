using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;
using ZenFulcrum.EmbeddedBrowser;

namespace ClubPenguin.NativeWebViewer
{
	public class ZFNewWindowHandler : INewWindowHandler
	{
		private PrefabContentKey webViewPopupKey = new PrefabContentKey("AccountSystemPrefabs/WebviewerPopupPrefab");

		private GameObject webViewPopup;

		private GameObject webViewPanel;

		private GameObject loadingPanel;

		private RawImage rawImage;

		private float newBrowserZoomLevel;

		public ZFNewWindowHandler(float newBrowserZoomLevel)
		{
			this.newBrowserZoomLevel = newBrowserZoomLevel;
		}

		public Browser CreateBrowser(Browser parent)
		{
			Browser browser = null;
			GameObject gameObject = new GameObject("WebViewContainer", typeof(RectTransform), typeof(LayoutElement));
			onWebViewPopupPrefabLoaded(Content.LoadImmediate(webViewPopupKey));
			if (webViewPanel != null)
			{
				ZFBrowserComponentController.SetupBrowser(webViewPanel, gameObject);
				rawImage = gameObject.GetComponent<RawImage>();
				rawImage.enabled = false;
				browser = gameObject.GetComponent<Browser>();
				browser.Zoom = newBrowserZoomLevel;
				browser.onLoad += onLoad;
			}
			else
			{
				Log.LogError(this, "webViewPanel is null");
				Destroy();
			}
			return browser;
		}

		public void Destroy()
		{
			if (webViewPopup != null)
			{
				Object.Destroy(webViewPopup);
			}
		}

		private void onLoad(JSONNode obj)
		{
			rawImage.enabled = true;
			if (loadingPanel != null)
			{
				loadingPanel.SetActive(false);
			}
		}

		private void onWebViewPopupPrefabLoaded(GameObject prefab)
		{
			webViewPopup = Object.Instantiate(prefab);
			WebViewPrefabController component = webViewPopup.GetComponent<WebViewPrefabController>();
			if (component != null)
			{
				if (component.WebViewerPanel != null)
				{
					webViewPanel = component.WebViewerPanel;
				}
				if (component.CloseButton != null)
				{
					component.CloseButton.onClick.AddListener(Destroy);
				}
				if (component.LoadingPanel != null)
				{
					loadingPanel = component.LoadingPanel;
					loadingPanel.SetActive(true);
				}
			}
			if ((bool)GameObject.Find("TopCanvas"))
			{
				Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowTopPopup(webViewPopup));
			}
			else
			{
				Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(webViewPopup));
			}
		}
	}
}

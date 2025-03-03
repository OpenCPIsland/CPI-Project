using ClubPenguin.NativeWebViewer;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Button))]
	public class WebViewButton : MonoBehaviour
	{
		public string URLToken;

		public string TitleToken;

		private WebViewController webView;

		private void Start()
		{
			webView = new WebViewController(base.gameObject);
			Button component = GetComponent<Button>();
			component.onClick.AddListener(onClicked);
		}

		private void onClicked()
		{
			webView.Show(URLToken, null, TitleToken);
		}
	}
}

using ClubPenguin.NativeWebViewer;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class LegalLinkController : MonoBehaviour
	{
		private WebViewController webView;

		private string linkURL;

		private string linkText;

		public Button LinkButton;

		public Text LinkText;

		private void Start()
		{
			webView = new WebViewController(base.gameObject);
		}

		public void SetLink(string inlinkText, string inlinkURL)
		{
			linkURL = inlinkURL;
			linkText = inlinkText;
			LinkText.text = linkText;
		}

		public void OnLinkClicked()
		{
			webView.Show(linkURL, null, linkText);
		}
	}
}

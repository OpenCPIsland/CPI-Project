using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Button))]
	public class SetMainNavTitleTextOnClick : MonoBehaviour
	{
		private const string MAIN_NAV_GO_NAME = "MainNavBar";

		public string TitleTextToken;

		public void Start()
		{
			GetComponent<Button>().onClick.AddListener(onButtonClick);
		}

		public void OnDestroy()
		{
			GetComponent<Button>().onClick.RemoveListener(onButtonClick);
		}

		private void onButtonClick()
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(TitleTextToken);
			GameObject gameObject = GameObject.Find("MainNavBar");
			if (gameObject != null)
			{
				gameObject.GetComponent<MainNavStateHandler>().SetTitleText(tokenTranslation);
			}
		}
	}
}

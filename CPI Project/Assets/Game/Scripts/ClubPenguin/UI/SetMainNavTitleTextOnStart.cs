using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class SetMainNavTitleTextOnStart : MonoBehaviour
	{
		private const string MAIN_NAV_GO_NAME = "MainNavBar";

		public string TitleTextToken;

		public void Start()
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(TitleTextToken);
			MainNavStateHandler componentInChildren = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root).GetComponentInChildren<MainNavStateHandler>();
			componentInChildren.SetTitleText(tokenTranslation);
		}
	}
}

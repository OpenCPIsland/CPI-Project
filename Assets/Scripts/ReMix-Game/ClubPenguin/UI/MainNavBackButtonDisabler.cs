using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(MainNavBarBackButton))]
	public class MainNavBackButtonDisabler : UIElementDisabler
	{
		public override void DisableElement(bool hide)
		{
			GetComponent<MainNavBarBackButton>().SetButtonActive(false);
		}

		public override void EnableElement()
		{
			GetComponent<MainNavBarBackButton>().SetButtonActive(true);
		}
	}
}

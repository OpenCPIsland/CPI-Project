using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(MainNavButton))]
	public class MainNavButtonDisabler : UIElementDisabler
	{
		public override void DisableElement(bool hide)
		{
			changeVisibility(!hide);
			GetComponent<MainNavButton>().SetButtonEnabled(false);
			if (Breadcrumb != null)
			{
				Breadcrumb.SetActive(false);
			}
			QuestButtonSpriteSelector component = base.gameObject.GetComponent<QuestButtonSpriteSelector>();
			if (component != null)
			{
				component.OnButtonDisabled();
			}
		}

		public override void EnableElement()
		{
			changeVisibility(true);
			GetComponent<MainNavButton>().SetButtonEnabled(true);
			if (Breadcrumb != null)
			{
				Breadcrumb.SetActive(true);
			}
			QuestButtonSpriteSelector component = base.gameObject.GetComponent<QuestButtonSpriteSelector>();
			if (component != null)
			{
				component.OnButtonEnabled();
			}
		}
	}
}

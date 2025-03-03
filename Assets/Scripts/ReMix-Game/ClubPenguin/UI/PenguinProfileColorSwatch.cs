using ClubPenguin.Breadcrumbs;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using Disney.Native;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class PenguinProfileColorSwatch : MonoBehaviour
	{
		public Action<int> OnClicked;

		public Image ColorImage;

		public GameObject SelectedPanel;

		public TutorialBreadcrumb TutorialBreadcrumb;

		private int colorId;

		private AvatarDetailsData avatarDetailsData;

		public void SetColor(int colorId, Color color, string colorName)
		{
			this.colorId = colorId;
			ColorImage.color = color;
			AccessibilitySettings component = base.gameObject.GetComponent<AccessibilitySettings>();
			if (component != null)
			{
				component.DynamicText = colorName + " " + Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Navigation.button");
			}
			TutorialBreadcrumb.SetBreadcrumbId(string.Format("Color_{0}", colorName));
			getAvatarDetailsData();
			avatarDetailsData.PlayerColorChanged += onColorChanged;
			onColorChanged(avatarDetailsData.BodyColor);
		}

		public void OnButtonClick()
		{
			if (OnClicked != null)
			{
				OnClicked(colorId);
			}
			Service.Get<TutorialBreadcrumbController>().RemoveBreadcrumb(TutorialBreadcrumb.BreadcrumbId);
		}

		private void getAvatarDetailsData()
		{
			avatarDetailsData = Service.Get<CPDataEntityCollection>().GetComponent<AvatarDetailsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle);
		}

		private void onColorChanged(Color color)
		{
			SelectedPanel.SetActive(color == ColorImage.color);
		}

		private void OnDestroy()
		{
			if (avatarDetailsData != null)
			{
				avatarDetailsData.PlayerColorChanged -= onColorChanged;
			}
			OnClicked = null;
		}
	}
}

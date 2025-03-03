using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class ChatButtonDisabler : UIElementDisabler
	{
		private bool isHidden;

		private Button[] childButtons;

		private Image[] childImages;

		public override void DisableElement(bool hide)
		{
			isEnabled = false;
			if (hide)
			{
				isHidden = true;
				CanvasGroup canvasGroup = base.gameObject.AddComponent<CanvasGroup>();
				canvasGroup.blocksRaycasts = false;
			}
			setButtonsEnabled(false);
			setImagesEnabled(false);
		}

		public override void EnableElement()
		{
			isEnabled = true;
			changeVisibility(true);
			if (isHidden)
			{
				isHidden = false;
				if (base.gameObject.GetComponent<CanvasGroup>() != null)
				{
					Object.Destroy(base.gameObject.GetComponent<CanvasGroup>());
				}
			}
			setButtonsEnabled(true);
			setImagesEnabled(true);
		}

		private void setImagesEnabled(bool isEnabled)
		{
			if (childImages == null)
			{
				childImages = GetComponentsInChildren<Image>();
			}
			for (int i = 0; i < childImages.Length; i++)
			{
				childImages[i].color = (isEnabled ? Color.white : DisabledColor);
			}
			Text[] componentsInChildren = GetComponentsInChildren<Text>();
			Text[] array = componentsInChildren;
			foreach (Text text in array)
			{
				float a = isEnabled ? 1f : 0.4f;
				text.color = new Color(text.color.r, text.color.g, text.color.b, a);
			}
		}

		private void setButtonsEnabled(bool isEnabled)
		{
			if (childButtons == null)
			{
				childButtons = GetComponentsInChildren<Button>();
			}
			for (int i = 0; i < childButtons.Length; i++)
			{
				childButtons[i].interactable = isEnabled;
			}
		}
	}
}

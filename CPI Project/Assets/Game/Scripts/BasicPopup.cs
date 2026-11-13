using ClubPenguin.UI;
using UnityEngine;
using UnityEngine.UI;

public class BasicPopup : AnimatedPopup
{
	public Button CloseButton;

	public Button FullScreenButton;

	public RectTransform Panel;

	public bool ShowCloseButton = false;

	protected override void start()
	{
		if (CloseButton != null)
		{
			CloseButton.gameObject.AddComponent<BackButton>();
			CloseButton.enabled = ShowCloseButton;
		}
	}

	public void SetData(DBasicPopup data)
	{
		Panel.anchoredPosition = data.PopupOffset;
		Panel.localScale = new Vector3(data.PopupScale.x, data.PopupScale.y, 1f);
		ShowBackground = data.ShowBackground;
	}

	public void EnableCloseButtons(bool closeButtonEnabled, bool fullScreenButtonEnabled)
	{
		if (CloseButton != null)
		{
			CloseButton.gameObject.SetActive(closeButtonEnabled);
		}
		if (FullScreenButton != null)
		{
			FullScreenButton.gameObject.SetActive(fullScreenButtonEnabled);
		}
	}
}

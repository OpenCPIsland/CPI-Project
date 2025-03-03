using ClubPenguin.Input;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Button))]
	public class MainNavBarCloseButton : MainNavButton
	{
		public const string BUTTON_CLICK_ID = "MainNavControl";

		public override void setState(MainNavButtonState newState)
		{
			switch (newState)
			{
			case MainNavButtonState.NORMAL:
				SetButtonEnabled(true);
				break;
			case MainNavButtonState.SELECTED:
			{
				Image.sprite = SelectedSprite;
				SetButtonEnabled(false);
				for (int i = 0; i < mainNavButtons.Length; i++)
				{
					if (mainNavButtons[i] != this && mainNavButtons[i].State == MainNavButtonState.SELECTED)
					{
						if (!mainNavButtons[i].GetComponent<Button>().IsInteractable())
						{
							mainNavButtons[i].setState(MainNavButtonState.DISABLED);
						}
						else
						{
							mainNavButtons[i].setState(MainNavButtonState.NORMAL);
						}
					}
				}
				break;
			}
			case MainNavButtonState.DISABLED:
			case MainNavButtonState.UNAVAILABLE:
				SetButtonEnabled(false);
				break;
			}
			if (ChangeAlphaOnDisable && newState != MainNavButtonState.DISABLED)
			{
				Image.color = Color.white;
			}
			state = newState;
		}

		public override void OnClick(ButtonClickListener.ClickType clickType)
		{
			DButton dButton = new DButton();
			dButton.Id = "MainNavControl";
			Service.Get<EventDispatcher>().DispatchEvent(new ButtonEvents.ClickEvent(dButton));
			setState(MainNavButtonState.SELECTED);
		}

		public override void SetButtonEnabled(bool enabled, bool changeState = true)
		{
			Image.enabled = enabled;
			buttonClickListener.Button.enabled = enabled;
		}
	}
}

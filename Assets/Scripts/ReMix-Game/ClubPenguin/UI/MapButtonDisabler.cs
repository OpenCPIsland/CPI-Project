using ClubPenguin.Input;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using Disney.Native;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MapButtonDisabler : UIElementDisabler
	{
		private static readonly DPrompt exitPromptData = new DPrompt("GlobalUI.Prompts.exitGameTitle", "GlobalUI.Prompts.exitGameBody", DPrompt.ButtonFlags.CANCEL | DPrompt.ButtonFlags.OK);

		public Image ButtonIcon;

		public Text ButtonText;

		[LocalizationToken]
		public string MapToken = "GlobalUI.Navigation.Map";

		[LocalizationToken]
		public string ExitToken = "GlobalUI.Navigation.ScreenMoreOptionsLayout.Button1";

		private bool onPressedEventAdded = false;

		protected override void start()
		{
			base.start();
			ButtonText.text = Service.Get<Localizer>().GetTokenTranslation(MapToken);
		}

		public override void DisableElement(bool hide)
		{
			MainNavMapButton component = GetComponent<MainNavMapButton>();
			if (component != null)
			{
				component.enabled = false;
			}
			AccessibilitySettings component2 = GetComponent<AccessibilitySettings>();
			if (component2 != null)
			{
				component2.CustomToken = AccessibilityDisabledToken;
			}
			GetComponent<MainNavButton>().enabled = false;
			ButtonIcon.sprite = GetComponent<MainNavButton>().DisabledSprite;
			if (!onPressedEventAdded)
			{
				onPressedEventAdded = true;
				GetComponent<ButtonClickListener>().OnClick.AddListener(onPressed);
			}
			ButtonText.text = Service.Get<Localizer>().GetTokenTranslation(ExitToken);
		}

		public override void EnableElement()
		{
			MainNavMapButton component = GetComponent<MainNavMapButton>();
			if (component != null)
			{
				component.enabled = true;
			}
			AccessibilitySettings component2 = GetComponent<AccessibilitySettings>();
			if (component2 != null)
			{
				component2.CustomToken = AccessibilityEnabledToken;
			}
			GetComponent<MainNavButton>().enabled = true;
			ButtonIcon.sprite = GetComponent<MainNavButton>().NormalSprite;
			onPressedEventAdded = false;
			GetComponent<ButtonClickListener>().OnClick.RemoveListener(onPressed);
			ButtonText.text = Service.Get<Localizer>().GetTokenTranslation(MapToken);
		}

		protected override void onDestroy()
		{
			onPressedEventAdded = false;
			GetComponent<ButtonClickListener>().OnClick.RemoveListener(onPressed);
			base.onDestroy();
		}

		private void onPressed(ButtonClickListener.ClickType clickType)
		{
			if (base.enabled)
			{
				Service.Get<PromptManager>().ShowPrompt(exitPromptData, onExitConfirmPromptComplete);
			}
		}

		private void onExitConfirmPromptComplete(DPrompt.ButtonFlags button)
		{
			if (button == DPrompt.ButtonFlags.OK)
			{
				Service.Get<GameStateController>().ExitWorld();
			}
		}
	}
}

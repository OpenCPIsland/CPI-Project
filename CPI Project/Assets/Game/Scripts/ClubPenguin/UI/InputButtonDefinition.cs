using Disney.LaunchPadFramework;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class InputButtonDefinition : ScriptableObject
	{
		public GameObject ButtonPrefab;

		public bool IsBackgroundVisible = true;

		public TrayInputButton.ButtonState DefaultState = TrayInputButton.ButtonState.Default;

		public Sprite[] ButtonIcons;

		public Color[] IconTints;

		public void SetUpButton(TrayInputButton trayInputButton)
		{
			GameObject gameObject = Object.Instantiate(ButtonPrefab);
			gameObject.transform.SetParent(trayInputButton.Behaviour.transform, false);
			if (trayInputButton.IconSprite != null && trayInputButton.IconTint != null)
			{
				trayInputButton.Icon.enabled = true;
				trayInputButton.IconSprite.Sprites = new Sprite[ButtonIcons.Length];
				trayInputButton.IconTint.Colors = new Color[ButtonIcons.Length];
				for (int i = 0; i < ButtonIcons.Length; i++)
				{
					trayInputButton.IconSprite.Sprites[i] = ButtonIcons[i];
					trayInputButton.IconTint.Colors[i] = IconTints[i];
				}
				trayInputButton.IconSprite.SelectSprite((int)DefaultState);
				trayInputButton.IconTint.SelectColor((int)DefaultState);
			}
			else
			{
				trayInputButton.Icon.enabled = false;
				Log.LogError(this, "Button object did not have an icon sprite or tint selector");
			}
			trayInputButton.DefaultState = DefaultState;
			trayInputButton.IsBackgroundVisible = IsBackgroundVisible;
			if (!IsBackgroundVisible)
			{
				trayInputButton.GetComponent<Image>().enabled = false;
			}
			TrayInputButton.ButtonState buttonVisualState = DefaultState;
			TrayInputButton.ButtonState buttonState = DefaultState;
			if (trayInputButton.IsLocked)
			{
				buttonVisualState = trayInputButton.CurrentViewState;
				buttonState = TrayInputButton.ButtonState.Disabled;
			}
			trayInputButton.InitializeView(DefaultState, buttonVisualState);
			trayInputButton.GetComponent<Button>().interactable = (buttonState != TrayInputButton.ButtonState.Disabled);
			trayInputButton.gameObject.SetActive(true);
		}
	}
}

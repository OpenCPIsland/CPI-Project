using Disney.LaunchPadFramework;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class InputButtonGroupDefinition : ScriptableObject
	{
		public TrayInputButtonGroupContentKey TemplateContentKey;

		public InputButtonDefinition[] InputButtonDefinitions;

		public TrayInputButton.ButtonState[] ButtonStateOverrides;

		public TrayInputButtonGroup Create(TrayInputButtonGroup trayInputButtonGroupAsset, Transform parent)
		{
			TrayInputButtonGroup trayInputButtonGroup = Object.Instantiate(trayInputButtonGroupAsset, parent);
			if (trayInputButtonGroup.Buttons.Length != InputButtonDefinitions.Length)
			{
				Log.LogError(this, "Number of button containers does not match the number of button definitions");
			}
			else
			{
				for (int i = 0; i < InputButtonDefinitions.Length; i++)
				{
					trayInputButtonGroup.Buttons[i].Index = i;
					if (InputButtonDefinitions[i] != null)
					{
						InputButtonDefinitions[i].SetUpButton(trayInputButtonGroup.Buttons[i]);
						if (ButtonStateOverrides != null && ButtonStateOverrides.Length > i && ButtonStateOverrides[i] != TrayInputButton.ButtonState.None)
						{
							trayInputButtonGroup.Buttons[i].InitializeView(ButtonStateOverrides[i]);
						}
					}
					else
					{
						trayInputButtonGroup.Buttons[i].InitializeView(TrayInputButton.ButtonState.Disabled);
						trayInputButtonGroup.Buttons[i].gameObject.SetActive(false);
					}
				}
			}
			return trayInputButtonGroup;
		}
	}
}

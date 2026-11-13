using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.UI
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(InputFieldFormSubmitButton))]
	public class InputFieldFormSubmitButtonSettingsComponent : AspectRatioSpecificSettingsComponent<InputFieldFormSubmitButton, InputFieldFormSubmitButtonSettings>
	{
		protected override void applySettings(InputFieldFormSubmitButton component, InputFieldFormSubmitButtonSettings settings)
		{
			component.HideSubmitButton = settings.HideSubmitButton;
		}
	}
}

using ClubPenguin.Core;
using TMPro;
using UnityEngine;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	[DisallowMultipleComponent]
	public class TextMeshProSettingsComponent : AspectRatioSpecificSettingsComponent<TextMeshProUGUI, TextSettings>
	{
		protected override void applySettings(TextMeshProUGUI component, TextSettings settings)
		{
			component.fontSize = settings.FontSize;
		}
	}
}

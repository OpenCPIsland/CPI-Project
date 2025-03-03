using ClubPenguin.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Text))]
	[DisallowMultipleComponent]
	public class TextSettingsComponent : AspectRatioSpecificSettingsComponent<Text, TextSettings>
	{
		protected override void applySettings(Text component, TextSettings settings)
		{
			component.fontSize = settings.FontSize;
		}
	}
}

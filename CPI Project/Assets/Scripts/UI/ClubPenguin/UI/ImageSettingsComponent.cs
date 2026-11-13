using ClubPenguin.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Image))]
	public class ImageSettingsComponent : AspectRatioSpecificSettingsComponent<Image, ImageSettings>
	{
		protected override void applySettings(Image component, ImageSettings settings)
		{
			component.sprite = settings.Sprite;
		}
	}
}

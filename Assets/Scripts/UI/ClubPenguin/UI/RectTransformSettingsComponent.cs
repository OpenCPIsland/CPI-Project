using ClubPenguin.Core;
using UnityEngine;

namespace ClubPenguin.UI
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class RectTransformSettingsComponent : AspectRatioSpecificSettingsComponent<RectTransform, RectTransformSettings>
	{
		public static void ApplySettings(RectTransform component, RectTransformSettings settings)
		{
			component.anchorMin = getVector2Override(settings.AnchorMin, component.anchorMin, settings.AnchorMinXOption, settings.AnchorMinYOption);
			component.anchorMax = getVector2Override(settings.AnchorMax, component.anchorMax, settings.AnchorMaxXOption, settings.AnchorMaxYOption);
			component.sizeDelta = getVector2Override(settings.SizeDelta, component.sizeDelta, settings.SizeDeltaXOption, settings.SizeDeltaYOption);
			if (settings.PositionZOption)
			{
				component.localPosition = new Vector3(component.localPosition.x, component.localPosition.y, settings.PositionZ);
			}
		}

		protected override void applySettings(RectTransform component, RectTransformSettings settings)
		{
			ApplySettings(component, settings);
		}

		private static Vector2 getVector2Override(Vector2 settings, Vector2 component, bool xEnabled, bool yEnabled)
		{
			float x = xEnabled ? settings.x : component.x;
			float y = yEnabled ? settings.y : component.y;
			return new Vector2(x, y);
		}
	}
}

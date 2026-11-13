using ClubPenguin.Core;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(HorizontalOrVerticalLayoutGroup))]
	public class HorizontalOrVerticalLayoutGroupSettingsComponent : AspectRatioSpecificSettingsComponent<HorizontalOrVerticalLayoutGroup, HorizontalOrVerticalLayoutGroupSettings>
	{
		protected override void applySettings(HorizontalOrVerticalLayoutGroup component, HorizontalOrVerticalLayoutGroupSettings settings)
		{
			component.childControlWidth = (settings.ChildControlWidthOption ? settings.ChildControlWidth : component.childControlWidth);
			component.childControlHeight = (settings.ChildControlHeightOption ? settings.ChildControlHeight : component.childControlHeight);
			component.childForceExpandWidth = (settings.ChildForceExpandWidthOption ? settings.ChildForceExpandWidth : component.childForceExpandWidth);
			component.childForceExpandHeight = (settings.ChildForceExpandHeightOption ? settings.ChildForceExpandHeight : component.childForceExpandHeight);
		}
	}
}

using UnityEngine;
using UnityEngine.UI;

namespace Disney.Native
{
	public class ParseToggle : ParseBase<Toggle>
	{
		public ParseToggle(IAccessibilityLocalization aLocalization)
			: base(aLocalization)
		{
		}

		public void Click(int aId)
		{
			if (items.ContainsKey(aId) && items[aId] != null)
			{
				ToggleAccessibilitySettings component = items[aId].GetComponent<ToggleAccessibilitySettings>();
				if (component.ReferenceToggleGroup != null)
				{
					items[aId].isOn = true;
				}
				else
				{
					items[aId].isOn = !items[aId].isOn;
				}
			}
			CheckCustomOnClickHandler(aId);
		}

		protected override GameObject GetGameobject(Toggle aItem)
		{
			return aItem.gameObject;
		}

		protected override string GetControlDescriptionForLabel(AccessibilitySettings aSettings)
		{
			if (aSettings is ToggleAccessibilitySettings)
			{
				ToggleAccessibilitySettings toggleAccessibilitySettings = aSettings as ToggleAccessibilitySettings;
				Toggle componentInChildren = toggleAccessibilitySettings.gameObject.GetComponentInChildren<Toggle>(false);
				if (toggleAccessibilitySettings != null)
				{
					string @string;
					if (toggleAccessibilitySettings.ReferenceToggleGroup != null)
					{
						string labelFromReferenceToken = GetLabelFromReferenceToken(toggleAccessibilitySettings.ToggleGroupName.gameObject);
						@string = localization.GetString("GlobalUI.Navigation.radio_button");
						if (!string.IsNullOrEmpty(labelFromReferenceToken) && !string.IsNullOrEmpty(@string))
						{
							return labelFromReferenceToken + " " + @string;
						}
						return "";
					}
					@string = localization.GetString("GlobalUI.Navigation.toggle");
					string text = "";
					if (componentInChildren != null)
					{
						text = (componentInChildren.isOn ? ((!(toggleAccessibilitySettings.OnText != null)) ? localization.GetString("GlobalUI.Navigation.enabled") : GetLabelFromReferenceToken(toggleAccessibilitySettings.OnText.gameObject)) : ((!(toggleAccessibilitySettings.OffText != null)) ? localization.GetString("GlobalUI.Navigation.disabled") : GetLabelFromReferenceToken(toggleAccessibilitySettings.OffText.gameObject)));
					}
					if (!string.IsNullOrEmpty(@string) && !string.IsNullOrEmpty(text))
					{
						return @string + " " + text;
					}
					return "";
				}
				return "";
			}
			return "";
		}
	}
}

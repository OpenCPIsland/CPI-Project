using UnityEngine;
using UnityEngine.UI;

namespace Disney.Native
{
	public class ParseSlider : ParseBase<Slider>
	{
		private float SLIDER_BRACKET_1 = 0.25f;

		private float SLIDER_BRACKET_2 = 0.5f;

		private float SLIDER_BRACKET_3 = 0.75f;

		private float SLIDER_BRACKET_4 = 1f;

		public ParseSlider(IAccessibilityLocalization aLocalization)
			: base(aLocalization)
		{
		}

		public void Click(int aId)
		{
			if (items.ContainsKey(aId))
			{
				if (items[aId] != null)
				{
					float normalizedValue = items[aId].normalizedValue;
					if (normalizedValue < SLIDER_BRACKET_1)
					{
						items[aId].normalizedValue = SLIDER_BRACKET_1;
					}
					else if (normalizedValue < SLIDER_BRACKET_2)
					{
						items[aId].normalizedValue = SLIDER_BRACKET_2;
					}
					else if (normalizedValue < SLIDER_BRACKET_3)
					{
						items[aId].normalizedValue = SLIDER_BRACKET_3;
					}
					else if (normalizedValue < SLIDER_BRACKET_4)
					{
						items[aId].normalizedValue = SLIDER_BRACKET_4;
					}
					else
					{
						items[aId].normalizedValue = 0f;
					}
				}
				Update(items[aId]);
			}
			CheckCustomOnClickHandler(aId);
		}

		protected override GameObject GetGameobject(Slider aItem)
		{
			return aItem.gameObject;
		}

		protected override string GetControlDescriptionForLabel(AccessibilitySettings aSettings)
		{
			string @string = localization.GetString("GlobalUI.Navigation.slider");
			string text = "";
			Slider componentInChildren = aSettings.gameObject.GetComponentInChildren<Slider>(false);
			if (componentInChildren != null)
			{
				text = ((int)(componentInChildren.normalizedValue * 100f)).ToString();
			}
			if (!string.IsNullOrEmpty(@string) && !string.IsNullOrEmpty(text))
			{
				return @string + " " + text + " " + localization.GetString("GlobalUI.Navigation.percent");
			}
			return "";
		}
	}
}

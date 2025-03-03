using UnityEngine;
using UnityEngine.UI;

namespace Disney.Native
{
	public class ParseImage : ParseBase<Image>
	{
		public ParseImage(IAccessibilityLocalization aLocalization)
			: base(aLocalization)
		{
		}

		public void Click(int aId)
		{
			CheckCustomOnClickHandler(aId);
		}

		protected override GameObject GetGameobject(Image aItem)
		{
			return aItem.gameObject;
		}

		protected override string GetControlDescriptionForLabel(AccessibilitySettings aSettings)
		{
			string @string = localization.GetString("GlobalUI.Navigation.image");
			if (!string.IsNullOrEmpty(@string))
			{
				return @string;
			}
			return "";
		}
	}
}

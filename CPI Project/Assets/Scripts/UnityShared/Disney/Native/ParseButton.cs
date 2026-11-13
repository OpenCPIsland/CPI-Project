using UnityEngine;
using UnityEngine.UI;

namespace Disney.Native
{
	public class ParseButton : ParseBase<Button>
	{
		public ParseButton(IAccessibilityLocalization aLocalization)
			: base(aLocalization)
		{
		}

		public void Click(int aId)
		{
			if (items.ContainsKey(aId) && items[aId] != null)
			{
				items[aId].onClick.Invoke();
			}
			CheckCustomOnClickHandler(aId);
		}

		protected override GameObject GetGameobject(Button aItem)
		{
			return aItem.gameObject;
		}

		protected override string GetControlDescriptionForLabel(AccessibilitySettings aSettings)
		{
			string @string = localization.GetString("GlobalUI.Navigation.button");
			if (!string.IsNullOrEmpty(@string))
			{
				return @string;
			}
			return "";
		}
	}
}

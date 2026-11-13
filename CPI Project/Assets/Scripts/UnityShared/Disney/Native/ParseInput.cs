using UnityEngine;

namespace Disney.Native
{
	public class ParseInput : ParseBase<InputFieldEx>
	{
		public ParseInput(IAccessibilityLocalization aLocalization)
			: base(aLocalization)
		{
		}

		public void Click(int aId)
		{
			if (items.ContainsKey(aId) && items[aId] != null)
			{
				items[aId].ActivateInputField();
			}
			CheckCustomOnClickHandler(aId);
		}

		protected override GameObject GetGameobject(InputFieldEx aItem)
		{
			return aItem.gameObject;
		}

		protected override string GetControlDescriptionForLabel(AccessibilitySettings aSettings)
		{
			string @string = localization.GetString("GlobalUI.Navigation.text_input");
			if (!string.IsNullOrEmpty(@string))
			{
				return @string;
			}
			return "";
		}
	}
}

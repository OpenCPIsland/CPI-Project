using UnityEngine;

namespace Disney.Native
{
	public class ParseChatText : ParseBase<AccessibilitySettings>
	{
		public ParseChatText(IAccessibilityLocalization aLocalization)
			: base(aLocalization)
		{
		}

		public void Click(int aId)
		{
			CheckCustomOnClickHandler(aId);
		}

		protected override GameObject GetGameobject(AccessibilitySettings aItem)
		{
			return aItem.gameObject;
		}
	}
}

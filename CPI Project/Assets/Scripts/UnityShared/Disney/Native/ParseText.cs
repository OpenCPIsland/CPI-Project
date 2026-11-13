using UnityEngine;
using UnityEngine.UI;

namespace Disney.Native
{
	public class ParseText : ParseBase<Text>
	{
		public ParseText(IAccessibilityLocalization aLocalization)
			: base(aLocalization)
		{
		}

		public void Click(int aId)
		{
			CheckCustomOnClickHandler(aId);
		}

		protected override GameObject GetGameobject(Text aItem)
		{
			return aItem.gameObject;
		}
	}
}

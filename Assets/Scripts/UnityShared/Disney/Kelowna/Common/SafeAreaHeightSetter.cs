using UnityEngine;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	public class SafeAreaHeightSetter : AbstractSafeAreaComponent
	{
		public SafeArea SafeArea;

		private void Start()
		{
			float verticalOffset = getVerticalOffset(SafeArea);
			RectTransform component = GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(component.sizeDelta.x, verticalOffset);
		}
	}
}

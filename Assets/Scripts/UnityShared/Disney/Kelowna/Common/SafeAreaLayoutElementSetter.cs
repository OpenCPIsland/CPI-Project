using UnityEngine;
using UnityEngine.UI;

namespace Disney.Kelowna.Common
{
	[RequireComponent(typeof(LayoutElement))]
	[DisallowMultipleComponent]
	public class SafeAreaLayoutElementSetter : AbstractSafeAreaComponent
	{
		public SafeArea SafeArea;

		private LayoutElement layoutElement;

		private RectTransform canvasTransform;

		private float normalizedOffset;

		private void Start()
		{
			layoutElement = GetComponent<LayoutElement>();
			canvasTransform = (GetComponentInParent<Canvas>().transform as RectTransform);
			float verticalOffset = getVerticalOffset(SafeArea);
			normalizedOffset = safeAreaService.GetNormalizedVerticalOffset(verticalOffset);
		}

		private void Update()
		{
			float minHeight = canvasTransform.rect.height * normalizedOffset;
			layoutElement.minHeight = minHeight;
		}
	}
}

using UnityEngine.UI;

namespace Disney.Native
{
	public class ScrollContentAccessibilitySettings : AccessibilitySettings
	{
		public ScrollRect ReferenceScrollRect;

		private void Start()
		{
			DontRender = true;
			Setup();
		}

		public override void Setup()
		{
			base.Setup();
		}
	}
}

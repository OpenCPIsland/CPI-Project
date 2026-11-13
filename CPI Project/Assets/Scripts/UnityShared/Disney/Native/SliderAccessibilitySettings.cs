using UnityEngine.UI;

namespace Disney.Native
{
	public class SliderAccessibilitySettings : AccessibilitySettings
	{
		public Slider ReferenceSlider;

		private void Start()
		{
			Setup();
		}

		public override void Setup()
		{
			base.Setup();
		}

		public void Slide(float aDistance)
		{
			ReferenceSlider.value += aDistance;
		}
	}
}

using UnityEngine.UI;

namespace Disney.Native
{
	public class ToggleAccessibilitySettings : AccessibilitySettings
	{
		public ToggleGroup ReferenceToggleGroup;

		public Text ToggleGroupName;

		public Text OnText;

		public Text OffText;

		private void Start()
		{
			Setup();
		}

		public override void Setup()
		{
			base.Setup();
		}
	}
}

using UnityEngine;

namespace Disney.Native
{
	public class SwipeAccessibilitySettings : AccessibilitySettings
	{
		public GameObject ReferenceSwipeButton;

		private void Start()
		{
			Setup();
		}

		public override void Setup()
		{
			base.Setup();
		}

		public void SetSwipeButton(GameObject aButton)
		{
			ReferenceSwipeButton = aButton;
		}

		public void OnClick()
		{
		}

		public void OnDeleteClicked()
		{
		}

		public void OnDeleteConfirmClicked()
		{
		}

		public void OnDeleteRejectClicked()
		{
		}

		public void OnEditClicked()
		{
		}
	}
}

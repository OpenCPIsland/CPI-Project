using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class DisableableReportPlayerCategoryButton : ReportPlayerCategoryButton
	{
		[Header("Disabled State Setup")]
		[SerializeField]
		private Image backgroundEnabled;

		[SerializeField]
		private Image backgroundDisabled;

		[SerializeField]
		private Color disabledTextColor;

		private Color? originalTextColor;

		private bool buttonEnabled = true;

		public void ToggleButton(bool enabled)
		{
			if (!originalTextColor.HasValue)
			{
				originalTextColor = ButtonText.color;
			}
			buttonEnabled = enabled;
			backgroundEnabled.enabled = enabled;
			backgroundDisabled.enabled = !enabled;
			ButtonText.color = (buttonEnabled ? originalTextColor.Value : disabledTextColor);
		}

		protected override void onClick()
		{
			if (buttonEnabled)
			{
				reportPlayerController.OnCategoryButtonClicked(Reason);
			}
		}
	}
}

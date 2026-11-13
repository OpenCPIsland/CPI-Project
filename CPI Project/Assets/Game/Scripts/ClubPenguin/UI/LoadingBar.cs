using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class LoadingBar : MonoBehaviour
	{
		public Image FillImage;

		public Text PercentageLabel;

		public void SetCompletion(float completion)
		{
			if (FillImage != null)
			{
				FillImage.fillAmount = completion;
			}
			if (PercentageLabel != null)
			{
				PercentageLabel.text = (int)(completion * 100f) + "%";
			}
		}
	}
}

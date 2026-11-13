using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	[RequireComponent(typeof(Text))]
	public class RewardPopupTitleText : MonoBehaviour
	{
		private void Start()
		{
			setTitleText();
		}

		private void setTitleText()
		{
			string popupTitle = GetComponentInParent<RewardPopupController>().PopupTitle;
			if (!string.IsNullOrEmpty(popupTitle))
			{
				GetComponent<Text>().text = popupTitle;
			}
		}
	}
}

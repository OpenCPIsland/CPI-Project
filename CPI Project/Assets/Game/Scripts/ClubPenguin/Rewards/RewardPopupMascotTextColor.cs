using ClubPenguin.Adventure;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	[RequireComponent(typeof(Text))]
	public class RewardPopupMascotTextColor : MonoBehaviour
	{
		private void Start()
		{
			DRewardPopup popupData = GetComponentInParent<RewardPopupController>().PopupData;
			if (!string.IsNullOrEmpty(popupData.MascotName))
			{
				tintTextForMascot(popupData.MascotName);
			}
		}

		private void tintTextForMascot(string mascotName)
		{
			Mascot mascot = Service.Get<MascotService>().GetMascot(mascotName);
			if (mascot != null)
			{
				GetComponent<Text>().color = mascot.Definition.BannerTextColor;
			}
		}
	}
}

using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	[RequireComponent(typeof(Image))]
	public class RewardPopupMascotButtonImage : MonoBehaviour
	{
		private const string DEFAULT_IMAGE_POSTFIX = "CP";

		private SpriteContentKey bannerKey = new SpriteContentKey("Rewards/Sprites/Quest_MascotBTN_*");

		private void Start()
		{
			DRewardPopup popupData = GetComponentInParent<RewardPopupController>().PopupData;
			string text = "CP";
			if (!string.IsNullOrEmpty(popupData.MascotName) && popupData.PopupType != DRewardPopup.RewardPopupType.levelUp)
			{
				text = Service.Get<MascotService>().GetMascot(popupData.MascotName).AbbreviatedName;
			}
			Content.LoadAsync(onImageLoaded, bannerKey, text);
		}

		private void onImageLoaded(string key, Sprite image)
		{
			GetComponent<Image>().sprite = image;
		}
	}
}

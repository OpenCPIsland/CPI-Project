using ClubPenguin.Adventure;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.Rewards
{
	[RequireComponent(typeof(Image))]
	public class RewardPopupBanner : MonoBehaviour
	{
		private const string DEFAULT_IMAGE_POSTFIX = "Text";

		public bool IsTitle = true;

		private SpriteContentKey bannerKey = new SpriteContentKey("Rewards/Sprites/Quests_Banners_Title_*");

		private SpriteContentKey bannerKeyTitle = new SpriteContentKey("Rewards/Sprites/Quests_Banners_Title_*");

		private void Start()
		{
			GetComponent<Image>().enabled = false;
			DRewardPopup popupData = GetComponentInParent<RewardPopupController>().PopupData;
			string text = "Text";
			if (!string.IsNullOrEmpty(popupData.MascotName) && popupData.PopupType != DRewardPopup.RewardPopupType.levelUp)
			{
				text = Service.Get<MascotService>().GetMascot(popupData.MascotName).AbbreviatedName;
			}
			if (IsTitle)
			{
				Content.LoadAsync(onBannerLoader, bannerKeyTitle, text);
			}
			else
			{
				Content.LoadAsync(onBannerLoader, bannerKey, text);
			}
		}

		private void onBannerLoader(string key, Sprite banner)
		{
			GetComponent<Image>().enabled = true;
			GetComponent<Image>().sprite = banner;
		}
	}
}

using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ClubPenguin.UI
{
	public class MarketplaceAdPopup : MonoBehaviour
	{
		private const string OFFER_TOKEN = "Marketplace.Offer.Text";

		public Image ItemIcon;

		public Image BackgroundImage;

		public TMP_Text PriceText;

		public TMP_Text HeaderText;

		public TMP_Text DescriptionText;

		public TMP_Text ListHeaderText;

		public TMP_Text ListLevelText;

		public GameObject ListLevelItem;

		public GameObject ListMemberItem;

		private Localizer localizer;

		public void SetData(MarketplaceScreenController.MarketplaceItemData itemData, string adDescription, Sprite itemIcon, Sprite backgroundImage)
		{
			localizer = Service.Get<Localizer>();
			ItemIcon.sprite = itemIcon;
			PriceText.text = itemData.PropDefn.Cost.ToString();
			HeaderText.text = localizer.GetTokenTranslation("Marketplace.Offer.Text");
			DescriptionText.text = localizer.GetTokenTranslation(adDescription);
			ListHeaderText.text = localizer.GetTokenTranslation(itemData.PropDefn.Name);
			if (itemData.UnlockLevel > 0)
			{
				ListLevelText.text = itemData.UnlockLevel.ToString();
			}
			else
			{
				ListLevelItem.SetActive(false);
			}
			BackgroundImage.sprite = backgroundImage;
		}

		public void OnDisable()
		{
			ListLevelItem.SetActive(true);
		}
	}
}

using ClubPenguin.Marketplace;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class MarketplaceAd : MonoBehaviour
	{
		public Image ItemIcon;

		public Image BackgroundImage;

		public Text DescriptionText;

		public Button LearnMoreButton;

		private Localizer localizer;

		private static SpriteContentKey buttonContentKey = new SpriteContentKey("Images/Marketplace_Btn");

		public void SetData(MarketplaceScreenController.MarketplaceItemData itemData, MarketplaceDefinition marketplaceDefinition, Sprite itemIcon, Sprite backgroundImage)
		{
			localizer = Service.Get<Localizer>();
			ItemIcon.sprite = itemIcon;
			string tokenTranslation = localizer.GetTokenTranslation(marketplaceDefinition.AdText);
			DescriptionText.text = tokenTranslation;
			DescriptionText.color = ColorUtils.HexToColor(marketplaceDefinition.AdMarket.TextColorHex);
			Content.LoadAsync(onBtnLoaded, buttonContentKey);
			BackgroundImage.sprite = backgroundImage;
		}

		private void onBtnLoaded(string path, Sprite image)
		{
			LearnMoreButton.GetComponent<Image>().sprite = image;
		}
	}
}

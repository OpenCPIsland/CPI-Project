using ClubPenguin.Analytics;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.ClothingDesigner.ItemCustomizer
{
	public class TemplateChosenController : MonoBehaviour
	{
		private const int TEXT_TINT_CAN_BUY = 0;

		private const int TEXT_TINT_CANT_AFFORD = 1;

		[SerializeField]
		private Image templateImage;

		[SerializeField]
		private Text templateTitle;

		[SerializeField]
		private Text templateDescription;

		[SerializeField]
		private Text templateCost;

		[SerializeField]
		private GameObject selectButton;

		[SerializeField]
		private GameObject notEnoughCoins;

		[SerializeField]
		private TintSelector tintSelector;

		private DItemCustomization itemModel;

		private string translatedTitleString;

		public void OnSelect()
		{
			CustomizationContext.EventBus.DispatchEvent(default(CustomizerUIEvents.TemplateConfirmed));
			Service.Get<ICPSwrveService>().Action("game.item_preview", itemModel.TemplateDefinition.name);
		}

		public void OnCancel()
		{
			CustomizationContext.EventBus.DispatchEvent(new CustomizerUIEvents.BackButtonClickedEvent(false));
		}

		public void SetModel(DItemCustomization itemModel)
		{
			this.itemModel = itemModel;
		}

		private void Awake()
		{
			translatedTitleString = Service.Get<Localizer>().GetTokenTranslation("ClothingDesigner.Customizer.DesignTemplateTitle");
		}

		private void OnEnable()
		{
			if (itemModel != null)
			{
				setTemplateChosenData();
			}
		}

		private void setTemplateChosenData()
		{
			string text = string.Format(translatedTitleString, Service.Get<Localizer>().GetTokenTranslation(itemModel.TemplateDefinition.Name));
			templateDescription.text = Service.Get<Localizer>().GetTokenTranslation(itemModel.TemplateDefinition.Description);
			templateImage.sprite = itemModel.TemplateSprite;
			templateTitle.text = text;
			templateCost.text = itemModel.TemplateDefinition.Cost.ToString();
			bool flag = getMyCoinCount() >= itemModel.TemplateDefinition.Cost;
			if (Service.Get<CatalogServiceProxy>().IsCatalogThemeActive())
			{
				templateCost.transform.parent.gameObject.SetActive(false);
				flag = true;
			}
			int index = (!flag) ? 1 : 0;
			tintSelector.SelectColor(index);
			selectButton.SetActive(flag);
			notEnoughCoins.SetActive(!flag);
		}

		private int getMyCoinCount()
		{
			int result = 0;
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			CoinsData component;
			if (!cPDataEntityCollection.LocalPlayerHandle.IsNull && cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
			{
				result = component.Coins;
			}
			return result;
		}
	}
}

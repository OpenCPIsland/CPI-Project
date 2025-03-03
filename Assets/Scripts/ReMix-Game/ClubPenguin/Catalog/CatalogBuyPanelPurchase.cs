using UnityEngine.UI;

namespace ClubPenguin.Catalog
{
	public class CatalogBuyPanelPurchase : CatalogBuyPanel
	{
		public Button ItemCostButton;

		public Button NotEnoughCoinsButton;

		public Text ItemCostText;

		public CatalogNotEnoughCoinsToolTip ToolTip;

		public void SetPurchaseButtonInteractableState(bool isInteractable)
		{
			ItemCostButton.interactable = isInteractable;
		}

		public void SetVisibleButton(bool hasEnoughCoins)
		{
			if (hasEnoughCoins)
			{
				ItemCostButton.gameObject.SetActive(true);
				NotEnoughCoinsButton.gameObject.SetActive(false);
			}
			else
			{
				ItemCostButton.gameObject.SetActive(false);
				NotEnoughCoinsButton.gameObject.SetActive(true);
			}
		}

		public void ShowNotEnoughCoinsToolTip()
		{
			ToolTip.Show();
		}

		private void OnDisable()
		{
			ToolTip.Hide();
		}
	}
}

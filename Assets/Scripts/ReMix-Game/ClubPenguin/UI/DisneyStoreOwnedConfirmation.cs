using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class DisneyStoreOwnedConfirmation : AbstractDisneyStoreConfirmation
	{
		public Image IconImage;

		private DisneyStoreFranchise storeFranchise;

		public override void SetItem(DisneyStoreItemData item, Sprite icon, DisneyStoreFranchise storeFranchise, IDisneyStoreController storeController, DisneyStoreFranchiseItem shopItem, RectTransform scrollRectTransform)
		{
			IconImage.sprite = icon;
			this.storeFranchise = storeFranchise;
			GetComponent<StoreItemConfirmationPlacement>().PositionConfirmation((RectTransform)shopItem.transform, scrollRectTransform);
			selectTrayIcons(item);
			logItemViewed(item);
		}

		public void OnOkayClicked()
		{
			storeFranchise.HideConfirmation();
		}
	}
}
